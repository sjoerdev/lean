using System;
using System.IO;
using System.Timers;

using Hexa.NET.OpenAL;

namespace Lean;

public unsafe class AudioClipWav
{
    // wav file data
    int dataPosition;
    int sampleRate;
    bool stereo;
    int bitsPerSample;

    // openal
    uint[] buffers;
    uint source;

    // streaming
    Timer timer;
    FileStream stream;
    int bufferAmount = 4;
    int secondsPerBuffer = 1;
    int bufferSize => sampleRate * (stereo ? 2 : 1) * (bitsPerSample / 8) * secondsPerBuffer;

    public AudioClipWav(string path)
    {
        // start audio file stream
        stream = new FileStream(path, FileMode.Open, FileAccess.Read);

        // read wav header
        byte[] header = new byte[44];
        stream.ReadExactly(header, 0, 44);
        sampleRate = BitConverter.ToInt32(header, 24);
        stereo = BitConverter.ToInt16(header, 22) > 1;
        bitsPerSample = BitConverter.ToInt16(header, 34);

        // find and skip to actual sound data position
        dataPosition = FindDataChunkPosition();
        stream.Position = dataPosition;

        // setup openal buffers
        buffers = new uint[bufferAmount];
        fixed (uint* ptr = &buffers[0]) OpenAL.GenBuffers(bufferAmount, ptr);

        // setup openal source
        fixed (uint* ptr = &source) OpenAL.GenSources(1, ptr);
        OpenAL.SetSourceProperty(source, ALEnum.SourceType, (int)ALEnum.Streaming);

        // Setup timer
        timer = new Timer(50);
        timer.Elapsed += (sender, args) => RecycleUsedBuffers();
    }

    public void Start()
    {
        // cannot start if already playing or paused
        if (GetState() == ALEnum.Playing || GetState() == ALEnum.Paused) return;
        
        // que first buffers
        for (int i = 0; i < bufferAmount; i++)
        {
            FillBuffer(buffers[i]);
            fixed (uint* ptr = &buffers[i]) OpenAL.SourceQueueBuffers(source, 1, ptr);
        }

        // start source and timer
        OpenAL.SourcePlay(source);
        timer.Start();
    }

    public void PauseOrContinue()
    {
        RecycleUsedBuffers();
        if (GetState() == ALEnum.Playing) OpenAL.SourcePause(source);
        else if (GetState() == ALEnum.Paused) OpenAL.SourcePlay(source);
    }

    public void Stop()
    {
        // already stopped
        if (GetState() == ALEnum.Stopped) return;

        // stop timer and source
        timer.Stop();
        OpenAL.SourceStop(source);

        // unque buffers
        OpenAL.GetSourceProperty(source, ALEnum.BuffersProcessed, out int processed);
        if (processed > 0)
        {
            uint[] buffersToUnqueue = new uint[processed];
            fixed (uint* ptr = &buffersToUnqueue[0]) OpenAL.SourceUnqueueBuffers(source, processed, ptr);
        }

        // reset filestream position
        stream.Position = dataPosition;
    }

    // finds the data chunk position
    public int FindDataChunkPosition()
    {
        using var tempStream = new FileStream(stream.Name, FileMode.Open, FileAccess.Read);
        
        // skip the 12 byte long riff header ("riff", filesize, "wave")
        tempStream.Position = 12;

        while (tempStream.Position < tempStream.Length)
        {
            // read chunk name and size
            byte[] buffer = new byte[8];
            tempStream.ReadExactly(buffer, 0, 8);
            string chunkName = System.Text.Encoding.ASCII.GetString(buffer, 0, 4);
            int chunkSize = BitConverter.ToInt32(buffer, 4);

            // if chunk name is data then we found the position
            if (chunkName == "data") return (int)tempStream.Position;

            // skip chunk
            tempStream.Position += chunkSize;
        }

        return 0;
    }

    // fills a buffer at the current filestream position and returns the amount of actual audio bytes that where put in the buffer
    private int FillBuffer(uint buffer)
    {
        byte[] audioData = new byte[bufferSize];
        int bytesRead = stream.Read(audioData, 0, audioData.Length);
        fixed (void* data = &audioData[0]) OpenAL.BufferData(buffer, GetFormat(), data, bytesRead, sampleRate);
        return bytesRead;
    }

    // if a buffer is used it gets recycled and requed
    private void RecycleUsedBuffers()
    {
        OpenAL.GetSourceProperty(source, ALEnum.BuffersProcessed, out int processed);
        for (uint i = 0; i < processed; i++)
        {
            // unque
            uint buffer;
            OpenAL.SourceUnqueueBuffers(source, 1, &buffer);

            // read bytes from stream
            int bytesRead = FillBuffer(buffer);

            // stop if no more audio data
            if (bytesRead == 0)
            {
                Stop();
                return;
            }

            // que new buffer
            OpenAL.SourceQueueBuffers(source, 1, &buffer);
        }
    }

    private ALEnum GetFormat()
    {
        ALEnum format;
        format = stereo ? (bitsPerSample == 16 ? ALEnum.FormatStereo16 : ALEnum.FormatStereo8) : (bitsPerSample == 16 ? ALEnum.FormatMono16 : ALEnum.FormatMono8);
        return format;
    }

    private ALEnum GetState()
    {
        OpenAL.GetSourceProperty(source, ALEnum.SourceState, out int state);
        return (ALEnum)state;
    }
}

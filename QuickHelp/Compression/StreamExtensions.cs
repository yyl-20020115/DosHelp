﻿using System;
using System.IO;

namespace QuickHelp.Compression;

static class StreamExtensions
{
    public static int ReadBytes(
        Stream stream, byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || offset > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || count > buffer.Length - offset)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = 0; i < count; i++)
        {
            int value = stream.ReadByte();
            if (value < 0)
                return i;
            buffer[offset + i] = (byte)value;
        }
        return count;
    }

    public static void WriteBytes(
        Stream stream, byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || offset > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || count > buffer.Length - offset)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = 0; i < count; i++)
        {
            stream.WriteByte(buffer[offset + i]);
        }
    }
}

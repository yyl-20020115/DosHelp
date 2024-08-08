using System;
using System.IO;

namespace QuickHelp.Compression;

/// <summary>
/// Represents a stream that reads and writes bits (from MSB to LSB) to
/// and from a base stream as bytes of value zero and one.
/// </summary>
/// <remarks>
/// This class only accesses the base stream when necessary and does not
/// prefetch reads or buffer writes.
/// </remarks>
public class BitStream(Stream stream) : Stream
{
    readonly Stream stream = stream ?? throw new ArgumentNullException(nameof(stream));
    int current_byte = 0;
    int available_bits = 0;

    public override bool CanRead => stream.CanRead;

    public override bool CanSeek => stream.CanSeek;

    public override bool CanWrite => stream.CanWrite;

    public override long Length => stream.Length * 8;

    public override long Position
    {
        get => stream.Position * 8 - available_bits;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            long bitPosition = value;
            long bytePosition = bitPosition / 8;
            stream.Position = bytePosition;
            available_bits = (int)(bytePosition * 8 - bitPosition);
            // numBitsAvailable may be negative and this is intended.
        }
    }

    public override void Flush() => stream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => StreamExtensions.ReadBytes(this, buffer, offset, count);

    /// <summary>
    /// Reads the next bit from the base stream as a byte of value 0 or 1.
    /// </summary>
    /// <returns>
    /// 1 if the bit is set; 0 if the bit is reset; -1 if EOF.
    /// </returns>
    public override int ReadByte()
    {
        if (available_bits <= 0)
        {
            current_byte = stream.ReadByte();
            if (current_byte < 0)
                return -1;
            available_bits += 8;
        }
        return (current_byte >> --available_bits) & 1;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                this.Position = offset;
                break;
            case SeekOrigin.Current:
                this.Position += offset;
                break;
            case SeekOrigin.End:
                this.Position = this.Length + offset;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin));
        }
        return this.Position;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException(
            "BitStream does not support SetLength().");
    }

    public override void Write(byte[] buffer, int offset, int count) => StreamExtensions.WriteBytes(this, buffer, offset, count);

    public override void WriteByte(byte value)
    {
        if (value != 0 && value != 1)
            throw new ArgumentOutOfRangeException(nameof(value));

        int newByte = current_byte;
        int numBitsRemaining = available_bits;
        int seekOffset = -1;

        if (numBitsRemaining <= 0)
        {
            newByte = stream.ReadByte();
            if (newByte < 0)
            {
                // Writing past the end of the stream is allowed in case
                // the base stream supports extending the stream on write.
                seekOffset = 0;
                newByte = 0;
            }
            numBitsRemaining += 8;
        }

        --numBitsRemaining;
        newByte = newByte
            & ~(1 << numBitsRemaining)
            | (value << numBitsRemaining);

        if (seekOffset != 0)
            stream.Seek(seekOffset, SeekOrigin.Current);

        try
        {
            stream.WriteByte((byte)newByte);
        }
        finally
        {
            if (seekOffset != 0)
                stream.Seek(-seekOffset, SeekOrigin.Current);
        }

        current_byte = newByte;
        available_bits = numBitsRemaining;
    }
}

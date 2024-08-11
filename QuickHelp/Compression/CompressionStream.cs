using System;
using System.IO;

namespace QuickHelp.Compression;

/// <summary>
/// Represents a stream with dictionary substitution and run-length
/// compression. This stream is used by compressed QuickHelp databases.
/// </summary>
/// <remarks>
/// This stream class is not buffered; that is, it does not read more
/// bytes from the base stream than requested. Therefore it is suitable
/// for detecting errors in the base stream.
/// </remarks>
/// TODO: dispose the base stream
class CompressionStream : Stream
{
    readonly Stream stream;
    readonly byte[][] dictionary;
    readonly byte[] buffer = new byte[512];
    int bufferStart = 0;
    int bufferEnd = 0;

    public CompressionStream(Stream baseStream, byte[][] dictionary)
    {
        ArgumentNullException.ThrowIfNull(baseStream);
        dictionary ??= [];

        this.stream = baseStream;
        this.dictionary = dictionary;
    }

    private byte ReadArgumentByte()
    {
        int b = stream.ReadByte();
        return b < 0 ? throw new EndOfStreamException("Missing argument for control byte.") : (byte)b;
    }

    private void FillBuffer(int maxBytes)
    {
        System.Diagnostics.Debug.Assert(bufferStart >= bufferEnd);

        bufferStart = 0;
        bufferEnd = 0;

        // Fill buffer until its size is greater than 256. Since a
        // control byte may at most encode 256 bytes, this ensures
        // we can always store the decoded data in the buffer.
        while (bufferEnd <= 256 && bufferEnd < maxBytes)
        {
            int b = stream.ReadByte();
            if (b < 0) // EOF
                break;

            if (b < 0x10 || b > 0x1A) // not a control byte
            {
                buffer[bufferEnd++] = (byte)b;
            }
            else if (b == 0x1A) // 0x1A, ESCAPE-BYTE
            {
                var escapeByte = ReadArgumentByte();
                buffer[bufferEnd++] = escapeByte;
            }
            else if (b == 0x19) // 0x19, REPEAT-BYTE, REPEAT-COUNT
            {
                var repeatByte = ReadArgumentByte();
                var repeatCount = ReadArgumentByte();
                for (int i = 0; i < repeatCount; i++)
                    buffer[bufferEnd++] = repeatByte;
            }
            else if (b == 0x18) // 0x18, SPACE-COUNT
            {
                var spaceCount = ReadArgumentByte();
                for (int i = 0; i < spaceCount; i++)
                    buffer[bufferEnd++] = 0x20;
            }
            else // dictionary entry index
            {
                var dictIndexLowByte = ReadArgumentByte();
                int dictIndex = ((b & 3) << 8) | dictIndexLowByte;
                if (dictIndex >= dictionary.Length)
                {
                    throw new InvalidDataException("Dictionary index is out of range.");
                }

                var dictEntry = dictionary[dictIndex];
                Array.Copy(dictEntry, 0, buffer, bufferEnd, dictEntry.Length);
                bufferEnd += dictEntry.Length;

                // Append space if bit 2 is set.
                if ((b & 4) != 0)
                    buffer[bufferEnd++] = 0x20;
            }
        }
    }

    // TODO: do not implement ReadFull logic here.
    public override int Read(byte[] buffer, int offset, int count)
    {
        int actual = 0;
        while (count > 0)
        {
            if (bufferStart >= bufferEnd)
            {
                FillBuffer(count); // do not fill unrestrictedly, because
                                   // the underlying huffman stream has
                                   // no way to tell EOF, and may return
                                   // invalid control bytes.
                if (bufferStart >= bufferEnd) // EOF
                    break;
            }

            int n = Math.Min(count, bufferEnd - bufferStart);
            Array.Copy(this.buffer, bufferStart, buffer, offset, n);
            bufferStart += n;
            offset += n;
            count -= n;
            actual += n;
        }
        return actual;
    }

    #region Stream Members

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanSeek => false;

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override long Length => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Flush() => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    #endregion
}

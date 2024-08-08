using System;
using System.IO;

namespace QuickHelp.Serialization;

/// <summary>
/// Represents a view on an underlying base stream with different position
/// and length.
/// </summary>
class StreamView : Stream
{
    private readonly Stream stream;
    private readonly long length;
    private long position;

    public StreamView(Stream baseStream, long length)
        : this(baseStream, length, 0)
    {
    }

    /// <summary>
    /// Creates a view on a stream.
    /// </summary>
    public StreamView(Stream baseStream, long length, long position)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
        if (!(position >= 0 && position <= length))
            throw new ArgumentOutOfRangeException(nameof(position));

        stream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        this.length = length;
        this.position = position;
    }

    public override long Length => length;

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (!(offset >= 0 && offset <= buffer.Length))
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (!(count >= 0 && offset + count <= buffer.Length))
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count > length - position)
            count = (int)(length - position);
        if (count == 0)
            return 0;

        // TODO: read full or throw exception
        int actual = stream.Read(buffer, offset, count);
        position += actual;
        return actual;
    }

    public override bool CanRead => stream.CanRead;

    public override bool CanWrite => false;

    public override bool CanSeek => false;

    public override void Flush() => throw new NotSupportedException();

    public override long Position
    {
        get => position;
        set => throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotSupportedException();
}

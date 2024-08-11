using System;
using System.IO;

namespace QuickHelp.Compression;

/// <summary>
/// Represents a huffman-encoded stream. Only supports decoding.
/// </summary>
/// <remarks>
/// This stream class is unbuffered; that is, it does not pre-read any
/// byte that is not necessary from the base stream. Therefore it is 
/// suitable for detecting errors in the base stream.
/// </remarks>
sealed class HuffmanStream : Stream
{
    readonly HuffmanTree huffmanTree;
    readonly BitStream bitStream;

    public HuffmanStream(Stream baseStream, HuffmanTree huffmanTree)
    {
        ArgumentNullException.ThrowIfNull(baseStream);
        this.bitStream = new BitStream(baseStream);
        this.huffmanTree = huffmanTree ?? throw new ArgumentNullException(nameof(huffmanTree));
    }

    public override int Read(byte[] buffer, int offset, int count) => StreamExtensions.ReadBytes(this, buffer, offset, count);

    public override int ReadByte()
    {
        var decoder = new HuffmanDecoder(this.huffmanTree);
        while (!decoder.HasValue)
        {
            int bit = bitStream.ReadByte();
            if (bit < 0) // EOF
                return -1;
            decoder.Push(bit != 0);
        }
        return decoder.Value;
    }

    #region Stream Members

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanSeek => false;

    public override void Flush() => throw new NotSupportedException();

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    #endregion
}

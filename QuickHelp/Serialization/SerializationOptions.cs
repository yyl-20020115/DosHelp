﻿using System;
using QuickHelp.Compression;

namespace QuickHelp.Serialization;

/// <summary>
/// Contains options to control the serialization process.  `
/// </summary>
public class SerializationOptions
{
    //protected readonly List<byte[]> m_keywords = [];

    /// <summary>
    /// Gets or sets the serialized format.
    /// </summary>
    public SerializationFormat Format { get; set; }

    public char ControlCharacter { get; set; }

    /// <summary>
    /// Gets or sets the compression level.
    /// </summary>
    /// <remarks>
    /// On serialization, if Format is Automatic or Binary, this value
    /// controls the compression level in the serialized .HLP file. If
    /// Format is Markup, this value is ignored.
    /// 
    /// On deserialization, this property is set to the actual compression
    /// level used in the input.
    /// </remarks>
    public CompressionFlags Compression { get; set; }

    /// <summary>
    /// Gets or sets a list of keywords used for keyword compression.
    /// </summary>
    /// <remarks>
    /// On serialization, if keyword compression is enabled, the
    /// serializer uses the dictionary specified by this property if it
    /// is not <c>null</c>, or computes the dictionary on the fly and 
    /// updates this property if it is null.
    /// 
    /// On deserialization, the serializer sets this property to the
    /// actual dictionary used in the input or <c>null</c> if the source
    /// does not use keyword compression.
    /// </remarks>
    public byte[][] Keywords { get; set; }

    /// <summary>
    /// Gets or sets the Huffman tree used for Huffman compression.
    /// </summary>
    /// <remarks>
    /// On serialization, if Huffman compression is enabled, the
    /// serializer uses the Huffman tree specified by this property if it
    /// is not <c>null</c>, or computes a Huffman tree on-the-fly and
    /// updates this property if it is <c>null</c>.
    /// 
    /// On deserialization, the serializer sets this property to the
    /// actual Huffman tree used in the input, or <c>null</c> if the
    /// source does not use Huffman compression.
    /// </remarks>
    public HuffmanTree HuffmanTree { get; set; }
}

/// <summary>
/// Specifies the serialized format of a help database.
/// </summary>
public enum SerializationFormat : int
{
    /// <summary>
    /// On deserialization, automatically detect the input format. On
    /// serialization, use Binary format.
    /// </summary>
    Automatic = 0,

    /// <summary>
    /// Specifies the binary format (with .HLP extension).
    /// </summary>
    Binary = 1,

    /// <summary>
    /// Specifies the markup format (with .SRC extension).
    /// </summary>
    Markup = 2,
}

[Flags]
public enum CompressionFlags : int
{
    None = 0,
    RunLength = 1,
    Keyword = 2,
    ExtendedKeyword = 4,
    Huffman = 8,
    All = RunLength | Keyword | ExtendedKeyword | Huffman
}

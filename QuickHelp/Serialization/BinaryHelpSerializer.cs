﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuickHelp.Compression;

namespace QuickHelp.Serialization;

/// <summary>
/// Provides methods to deserialize help databases from a .HLP file.
/// </summary>
public class BinaryHelpDeserializer
{
    public delegate void InvalidTopicDataEventHandler(
        object sender, InvalidTopicDataEventArgs e);

    /// <summary>
    /// Raised when malformed input is encountered during deserialization
    /// of a topic.
    /// </summary>
    public event InvalidTopicDataEventHandler InvalidTopicData;

    public IEnumerable<HelpDatabase> LoadDatabases(string fileName)
    {
        using var stream = File.OpenRead(fileName);
        while (stream.Position < stream.Length)
        {
            var database = DeserializeDatabase(stream);
            yield return database;
        }
    }

    public HelpDatabase DeserializeDatabase(Stream stream) => DeserializeDatabase(stream, new SerializationOptions());

    /// <summary>
    /// Deserializes the next help database from a binary reader.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if it encounters an irrecoverable
    /// error in the input, such as an IO error or malformed input in the
    /// meta data. It raises an <c>InvalidTopicData</c> event for each
    /// format error it encounters during topic deserialization.
    /// </remarks>
    public HelpDatabase DeserializeDatabase(
        Stream stream, SerializationOptions options)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (options == null)
            options = new SerializationOptions();

        CheckSignature(stream);

        var header = ReadFileHeader(stream);
        bool isCaseSensitive = (header.Attributes & HelpFileAttributes.CaseSensitive) != 0;
        var database = new HelpDatabase(header.DatabaseName, isCaseSensitive);
        options.ControlCharacter = Graphic437.GetChars([header.ControlCharacter])[0];

        using (var streamView = new StreamView(stream, header.DatabaseSize, 0x46))
        using (var reader = new BinaryReader(streamView))
        {
            int[] topicOffsets = ReadTopicIndex(reader, header);

            // Read Context Strings and Context Map sections.
            if (true)
            {
                string[] contextStrings = ReadContextStrings(reader, header);
                UInt16[] contextMap = ReadContextMap(reader, header);
                for (int i = 0; i < header.ContextCount; i++)
                {
                    database.AddContext(contextStrings[i], contextMap[i]);
                }
            }

            // Read Keywords section.
            if (header.KeywordsOffset > 0)
            {
                options.Keywords = ReadKeywords(reader, header);
                options.Compression |= CompressionFlags.Keyword;
                options.Compression |= CompressionFlags.ExtendedKeyword;
            }
            else
            {
                options.Keywords = null;
            }

            // Read Huffman Tree section.
            if (header.HuffmanTreeOffset > 0)
            {
                options.HuffmanTree = ReadHuffmanTree(reader, header);
                // file.HuffmanTree.Dump();
                options.Compression |= CompressionFlags.Huffman;
            }
            else
            {
                options.HuffmanTree = null;
            }

            // Read topic data.
            if (reader.BaseStream.Position != header.TopicTextOffset)
            {
                throw new InvalidDataException("Incorrect topic position.");
            }
            for (int i = 0; i < header.TopicCount; i++)
            {
                if (reader.BaseStream.Position != topicOffsets[i])
                    throw new InvalidDataException("Incorrect topic position.");
                int inputLength = topicOffsets[i + 1] - topicOffsets[i];

                byte[] inputData = reader.ReadBytes(inputLength);
                HelpTopic topic = DeserializeTopic(inputData, options);
                database.Topics.Add(topic);
            }

            // TODO: check position
            if (reader.BaseStream.Position != topicOffsets[header.TopicCount])
                throw new InvalidDataException("Incorrect topic end position.");
            if (reader.BaseStream.Position != header.DatabaseSize)
                throw new InvalidDataException("Incorrect database size.");
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format(
                "Decoded database {0} of {1} bytes.",
                database.Name, header.DatabaseSize));
#endif
        }
        return database;
    }

    private static void CheckSignature(Stream stream)
    {
        int byte1 = stream.ReadByte();
        int byte2 = stream.ReadByte();
        if (!(byte1 == 0x4C && byte2 == 0x4E))
        {
            throw new InvalidDataException("File signature mismatch.");
        }
    }

    private static BinaryHelpFileHeader ReadFileHeader(Stream stream)
    {
        using var reader = new BinaryReader(new StreamView(stream, 0x46, 2));
        var header = new BinaryHelpFileHeader
        {
            Version = reader.ReadUInt16(),
            Attributes = (HelpFileAttributes)reader.ReadUInt16(),
            ControlCharacter = reader.ReadByte(),
            Padding1 = reader.ReadByte(),
            TopicCount = reader.ReadUInt16(),
            ContextCount = reader.ReadUInt16(),
            DisplayWidth = reader.ReadByte(),
            Padding2 = reader.ReadByte(),
            Padding3 = reader.ReadUInt16()
        };

        byte[] stringData = reader.ReadBytes(14);
        header.DatabaseName = Encoding.ASCII.GetString(stringData);
        int k = header.DatabaseName.IndexOf('\0');
        if (k >= 0)
            header.DatabaseName = header.DatabaseName.Substring(0, k);

        header.Reserved1 = reader.ReadInt32();
        header.TopicIndexOffset = reader.ReadInt32();
        header.ContextStringsOffset = reader.ReadInt32();
        header.ContextMapOffset = reader.ReadInt32();
        header.KeywordsOffset = reader.ReadInt32();
        header.HuffmanTreeOffset = reader.ReadInt32();
        header.TopicTextOffset = reader.ReadInt32();
        header.Reserved2 = reader.ReadInt32();
        header.Reserved3 = reader.ReadInt32();
        header.DatabaseSize = reader.ReadInt32();

        // TODO: make them warnings instead of errors.
        // COBOL.HLP contains "JCK\r" in Reserved1.
#if false
            // Validate unknown fields.
            if (header.Version != 2)
                throw new NotSupportedException("Unexpected Version field.");
            if (header.Padding1 != 0 || header.Padding2 != 0 || header.Padding3 != 0)
                throw new NotSupportedException("Unexpected Padding field.");
            if (header.Reserved1 != 0 || header.Reserved2 != 0 || header.Reserved3 != 0)
                throw new NotSupportedException("Unexpected Reserved field.");
#endif
        return header;
    }

    private static int[] ReadTopicIndex(BinaryReader reader, BinaryHelpFileHeader header)
    {
        if (reader.BaseStream.Position != header.TopicIndexOffset)
            throw new InvalidDataException("Incorrect Topic Index section position.");

        int[] topicOffsets = new int[header.TopicCount + 1];
        for (int i = 0; i <= header.TopicCount; i++)
        {
            topicOffsets[i] = reader.ReadInt32();
        }
        return topicOffsets;
    }

    private static string[] ReadContextStrings(BinaryReader reader, BinaryHelpFileHeader header)
    {
        if (reader.BaseStream.Position != header.ContextStringsOffset)
            throw new InvalidDataException("Incorrect Context Strings section position.");

        // TODO: the NULL at the very end produces an extra, empty context string.
        // TODO: check exact number of context strings.
        int size = header.ContextMapOffset - header.ContextStringsOffset;
        string all = Encoding.ASCII.GetString(reader.ReadBytes(size));
        return all.Split('\0');
    }

    private static UInt16[] ReadContextMap(BinaryReader reader, BinaryHelpFileHeader header)
    {
        if (reader.BaseStream.Position != header.ContextMapOffset)
            throw new InvalidDataException("Incorrect Context Map section position.");

        UInt16[] contextMap = new UInt16[header.ContextCount];
        for (int i = 0; i < header.ContextCount; i++)
        {
            contextMap[i] = reader.ReadUInt16();
        }
        return contextMap;
    }

    // TODO: this section doesn't terminate by itself?!
    private static byte[][] ReadKeywords(
        BinaryReader reader, BinaryHelpFileHeader header)
    {
        if (reader.BaseStream.Position != header.KeywordsOffset)
            throw new InvalidDataException("Incorrect Keywords section position.");

        int sectionSize = (header.HuffmanTreeOffset > 0 ? header.HuffmanTreeOffset : header.TopicTextOffset)
            - header.KeywordsOffset;
        byte[] section = reader.ReadBytes(sectionSize);
        if (section.Length != sectionSize)
            throw new InvalidDataException("Cannot fully read dictionary section.");

        return KeywordListSerializer.Deserialize(section).ToArray();
    }

    private static HuffmanTree ReadHuffmanTree(BinaryReader reader, BinaryHelpFileHeader header)
    {
        if (reader.BaseStream.Position != header.HuffmanTreeOffset)
        {
            throw new InvalidDataException("Incorrect Huffman Tree section position.");
        }

        //int sectionSize = file.Header.TopicTextOffset - file.Header.HuffmanTreeOffset;
        HuffmanTree tree = HuffmanTree.Deserialize(reader);
        if (tree.IsEmpty || tree.IsSingular)
            throw new InvalidDataException("Invalid huffman tree.");
        return tree;
    }

    public HelpTopic DeserializeTopic(byte[] input, SerializationOptions options)
    {
        // Read decompressed length.
        if (input.Length < 2)
        {
            //var e = new InvalidTopicDataEventArgs(topic, input,
            //    "Not enough bytes for DecodedLength field.");
            //this.InvalidTopicData?.Invoke(this, e);
            return null;
        }
        int outputLength = BitConverter.ToUInt16(input, 0);

        byte[] encodedData = new byte[input.Length - 2];
        Array.Copy(input, 2, encodedData, 0, encodedData.Length);

        // Step 3. Huffman decoding pass.
        byte[] compactData;
        if (options.HuffmanTree != null)
            compactData = HuffmanDecode(encodedData, options.HuffmanTree);
        else
            compactData = encodedData;

        // Step 2. Decompression pass.
        byte[] binaryData = Decompress(compactData, outputLength, options.Keywords);

        // Step 1. Decompile topic.
        HelpTopic topic = DecompileTopic(binaryData, options.ControlCharacter);
        topic.Source = binaryData;

        return topic;
    }

    static byte[] HuffmanDecode(byte[] input, HuffmanTree huffmanTree)
    {
        using (var inputStream = new MemoryStream(input))
        using (var huffmanStream = new HuffmanStream(inputStream, huffmanTree))
        using (var reader = new BinaryReader(huffmanStream))
        {
            return reader.ReadBytes(1024 * 1024);
        }
    }

    static byte[] Decompress(byte[] input, int outputLength, byte[][] keywords)
    {
        using var inputStream = new MemoryStream(input);
        using var compressionStream = new CompressionStream(inputStream, keywords);
        using var reader = new BinaryReader(compressionStream);
        return reader.ReadBytes(outputLength);
    }

    private static readonly Graphic437Encoding Graphic437 =
        new Graphic437Encoding();

    private byte[] DecompressTopicData(byte[] input, HelpTopic topic,
        SerializationOptions options)
    {
        // The first two bytes indicates decompressed data size.
        if (input.Length < 2)
        {
            var e = new InvalidTopicDataEventArgs(topic, input,
                "Not enough bytes for DecodedLength field.");
            this.InvalidTopicData?.Invoke(this, e);
            return null;
        }
        int decompressedLength = BitConverter.ToUInt16(input, 0);

        // The rest of the buffer is a huffman stream wrapping a
        // compression stream wrapping binary-encoded topic data.
        byte[] output;
        using (var memoryStream = new MemoryStream(input, 2, input.Length - 2))
        using (var huffmanStream = new HuffmanStream(memoryStream, options.HuffmanTree))
        using (var compressionStream = new CompressionStream(huffmanStream, options.Keywords))
        using (var compressionReader = new BinaryReader(compressionStream))
        {
            output = compressionReader.ReadBytes(decompressedLength);
        }

        if (output.Length != decompressedLength)
        {
            var e = new InvalidTopicDataEventArgs(topic, input,
                string.Format("Decompressed topic size mismatch: " +
                "expecting {0} bytes, got {1} bytes.",
                decompressedLength, output.Length));
            this.InvalidTopicData?.Invoke(this, e);
        }
        return output;
    }

    // TODO: compression is topic/database independent, so move them
    // into a separate namespace/class.

    static HelpTopic DecompileTopic(byte[] buffer, char controlCharacter)
    {
        HelpTopic topic = new HelpTopic();
        BufferReader reader = new BufferReader(buffer, Graphic437);
        while (!reader.IsEOF)
        {
            HelpLine line = null;
            try
            {
                DecodeLine(reader, out line);
            }
            catch (Exception)
            {
                if (line != null)
                    topic.Lines.Add(line);
                throw;
            }

            bool isCommand = true;
            try
            {
                isCommand = HelpCommandConverter.ProcessCommand(
                    line.Text, controlCharacter, topic);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(
                    "Unable to process command '{0}': {1}",
                    line.Text, ex.Message));
            }

            if (!isCommand)
            {
                topic.Lines.Add(line);
            }
        }
        return topic;
    }

    internal static void DecodeTopic(
        byte[] buffer, HelpTopic topic, char controlCharacter)
    {
        BufferReader reader = new BufferReader(buffer, Graphic437);

        while (!reader.IsEOF)
        {
            HelpLine line = null;
            try
            {
                DecodeLine(reader, out line);
            }
            catch (Exception)
            {
                if (line != null)
                    topic.Lines.Add(line);
                throw;
            }

            bool isCommand = true;
            try
            {
                isCommand = HelpCommandConverter.ProcessCommand(
                    line.Text, controlCharacter, topic);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(
                    "Unable to process command '{0}': {1}",
                    line.Text, ex.Message));
            }

            if (!isCommand)
            {
                topic.Lines.Add(line);
            }
        }
    }

    internal static void DecodeLine(BufferReader reader, out HelpLine line)
    {
        //line = null;

        // Read text length in bytes.
        int textLength = reader.ReadByte();
        string text = reader.ReadFixedLengthString(textLength - 1);
        line = new HelpLine(text);

        // Read byte count of attributes.
        int attrLength = reader.ReadByte();
        BufferReader attrReader = reader.ReadBuffer(attrLength - 1);
        DecodeLineAttributes(line, attrReader);

        // Read hyperlinks.
        while (!attrReader.IsEOF)
        {
            DecodeLineHyperlink(line, attrReader);
        }
    }

    //private static void DecodeLine(Stream stream, out HelpLine line)
    //{
    //    line = null;

    //    // Read text length in bytes.
    //    int textLength = stream.ReadByte();
    //    if (textLength < 0)
    //        throw new EndOfStreamException("Cannot read text length byte.");
    //    byte[] textBytes = new byte[textLength];
    //    stream.ReadFull(textBytes, 0, textBytes.Length);
    //    string text = Graphic437.GetString(textBytes);
    //    line = new HelpLine(text);

    //    // Read byte count of attributes.
    //    int attrLength = reader.ReadByte();
    //    BufferReader attrReader = reader.ReadBuffer(attrLength - 1);
    //    DecodeLineAttributes(line, attrReader);

    //    // Read hyperlinks.
    //    while (!attrReader.IsEOF)
    //    {
    //        DecodeLineHyperlink(line, attrReader);
    //    }
    //}

    private static void DecodeLineAttributes(HelpLine line, BufferReader reader)
    {
        int charIndex = 0;
        for (int chunkIndex = 0; !reader.IsEOF; chunkIndex++)
        {
            TextStyle textStyle = TextStyle.None;

            // Read attribute byte except for the first chunk (for which
            // default attributes are applied).
            if (chunkIndex > 0)
            {
                byte a = reader.ReadByte();
                if (a == 0xFF) // marks the beginning of hyperlinks
                    break;

                textStyle = TextStyle.None;
                if ((a & 1) != 0)
                    textStyle |= TextStyle.Bold;
                if ((a & 2) != 0)
                    textStyle |= TextStyle.Italic;
                if ((a & 4) != 0)
                    textStyle |= TextStyle.Underline;
                if ((a & 0xF8) != 0)
                {
                    // should exit
                    //System.Diagnostics.Debug.WriteLine(string.Format(
                    //    "Text attribute bits {0:X2} is not recognized and is ignored.",
                    //    a & 0xF8));
                    throw new InvalidDataException("Invalid text attribute");
                }
            }

            // Read chunk length to apply this attribute to.
            int charCount = reader.ReadByte();
            if (charCount > line.Length - charIndex)
            {
                // TODO: issue warning
                charCount = line.Length - charIndex;
            }
            if (textStyle != TextStyle.None)
            {
                for (int j = 0; j < charCount; j++)
                    line.Attributes[charIndex + j] = new TextAttribute(textStyle, null);
            }
            charIndex += charCount;
        }
    }

    private static void DecodeLineHyperlink(HelpLine line, BufferReader reader)
    {
        // Read link location.
        int linkStartIndex = reader.ReadByte(); // one-base, inclusive
        int linkEndIndex = reader.ReadByte(); // one-base, inclusive

        if (linkStartIndex == 0 || linkStartIndex > linkEndIndex)
        {
            throw new InvalidDataException("Invalid link location.");
        }
        if (linkEndIndex > line.Length)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(
                "WARNING: Link end {0} is past line end {1}.",
                linkEndIndex, line.Length));
            linkEndIndex = line.Length;
        }
        //if (linkStartIndex 

        // Read NULL-terminated context string.
        var context = reader.ReadNullTerminatedString();
        if (context == "") // link is WORD topic index
        {
            int numContext = reader.ReadUInt16(); // 0x8000 | topicIndex
            context = "@L" + numContext.ToString("X4");
        }

        // Add hyperlink to the line.
        var link = new HelpUri(context);
        for (int j = linkStartIndex; j <= linkEndIndex; j++)
        {
            line.Attributes[j - 1] = new TextAttribute(line.Attributes[j - 1].Style, link);
        }
    }
}

/// <summary>
/// Contains information about invalid data encountered during topic
/// deserialization.
/// </summary>
public class InvalidTopicDataEventArgs(HelpTopic topic, byte[] input, string message) : EventArgs
{
    private readonly HelpTopic topic = topic;
    private readonly byte[] input = input;
    private readonly string message = message;

    /// <summary>
    /// Gets the topic being deserialized.
    /// </summary>
    public HelpTopic Topic => topic;

    public byte[] Input => input;

    public string Message => message;
}
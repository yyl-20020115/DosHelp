using System;
using System.Text;

namespace QuickHelp.Serialization;

public class Graphic437Encoding : Encoding
{
    public static readonly Encoding CP437 = Encoding.GetEncoding(437);
    public const string GraphicCharacters = "\0☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼";

    public static bool IsControlCharacter(char c) => (c < 32) || (c == 127);

    public static bool ContainsControlCharacter(string s)
    {
        ArgumentNullException.ThrowIfNull(s);

        for (int i = 0; i < s.Length; i++)
            if (IsControlCharacter(s[i]))
                return true;
        return false;
    }

    public static char[] SubstituteControlCharacters(char[] chars)
    {
        ArgumentNullException.ThrowIfNull(chars);

        return SubstituteControlCharacters(chars, 0, chars.Length);
    }

    public static char[] SubstituteControlCharacters(char[] chars, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(chars);
        if (index < 0 || index > chars.Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (count < 0 || count > chars.Length - index)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = index; i < index + count; i++)
        {
            chars[i] = chars[i] < 32 
                ? GraphicCharacters[chars[i]] 
                : chars[i] == 127 
                ? '⌂' 
                : chars[i]
                ;
        }
        return chars;
    }

    public static string SubstituteControlCharacters(string text) =>
        text == null 
        ? null : !ContainsControlCharacter(text) 
        ? text 
        : new string(SubstituteControlCharacters(text.ToCharArray()))
        ;

    public override int GetByteCount(char[] chars, int index, int count) => CP437.GetByteCount(chars, index, count);

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => CP437.GetBytes(chars, charIndex, charCount, bytes, byteIndex);

    public override int GetCharCount(byte[] bytes, int index, int count) => CP437.GetCharCount(bytes, index, count);

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        int charCount = CP437.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        SubstituteControlCharacters(chars, charIndex, charCount);
        return charCount;
    }

    public override int GetMaxByteCount(int charCount) => CP437.GetMaxByteCount(charCount);

    public override int GetMaxCharCount(int byteCount) => CP437.GetMaxCharCount(byteCount);
}

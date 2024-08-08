using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp;

/// <summary>
/// Represents a line of text in a help topic, along with formatting and
/// hyperlink information.
/// </summary>
public class HelpLine
{
    readonly string text;
    readonly TextAttribute[] attributes;

    public HelpLine(string text, TextAttribute[] attributes)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));
        if (attributes == null)
            throw new ArgumentNullException(nameof(attributes));
        if (text.Length != attributes.Length)
            throw new ArgumentException("text and attributes must have the same length.");

        this.text = text;
        this.attributes = attributes;
    }

    public HelpLine(string text)
    {
        this.text = text ?? throw new ArgumentNullException(nameof(text));
        this.attributes = new TextAttribute[text.Length];
    }

    public int Length => text.Length;

    /// <summary>
    /// Gets the text in this line without any formatting information.
    /// This is the text displayed on the screen.
    /// </summary>
    public string Text => text;

    /// <summary>
    /// Gets the attribute of each character in the line.
    /// </summary>
    public TextAttribute[] Attributes => attributes;

    public IEnumerable<HelpUri> Links
    {
        get
        {
            HelpUri lastLink = null;
            foreach (var a in attributes)
            {
                if (a.Link != lastLink)
                {
                    if (a.Link != null)
                        yield return a.Link;
                    lastLink = a.Link;
                }
            }
        }
    }

    public override string ToString() => this.text;
}

public readonly struct TextAttribute(TextStyle style, HelpUri link)
{
    readonly TextStyle style = style;
    readonly HelpUri link = link;

    public TextStyle Style => style;

    public HelpUri Link => link;

    public override readonly string ToString() => link == null ? style.ToString() : style.ToString() + "; " + link.ToString();

    public static readonly TextAttribute Default = new();
}

[Flags]
public enum TextStyle : int
{
    None = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4,
}

public class HelpLineBuilder(int capacity)
{
    readonly StringBuilder textBuilder = new(capacity);
    readonly List<TextAttribute> attrBuilder = new(capacity);

    public int Length => textBuilder.Length;

    public void Append(string s, int index, int count, TextStyle styles)
    {
        textBuilder.Append(s, index, count);
        for (int i = 0; i < count; i++)
            attrBuilder.Add(new TextAttribute(styles, null));
    }

    public void Append(char c, TextStyle styles)
    {
        textBuilder.Append(c);
        attrBuilder.Add(new TextAttribute(styles, null));
    }

    public void AddLink(int index, int count, HelpUri link)
    {
        for (int i = index; i < index + count; i++)
        {
            attrBuilder[i] = new TextAttribute(attrBuilder[i].Style, link);
        }
    }

    public HelpLine ToLine() => new HelpLine(textBuilder.ToString(), attrBuilder.ToArray());
}

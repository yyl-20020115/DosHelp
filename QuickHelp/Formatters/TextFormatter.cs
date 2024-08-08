using System;
using System.Text;

namespace QuickHelp.Formatters;

/// <summary>
/// Provides methods to format a help topic as plain text.
/// </summary>
public class TextFormatter
{
    public static string FormatTopic(HelpTopic topic)
    {
        if (topic == null)
            throw new ArgumentNullException(nameof(topic));

        var builder = new StringBuilder();
        foreach (var line in topic.Lines)
        {
            builder.AppendLine(line.Text);
        }
        return builder.ToString();
    }
}

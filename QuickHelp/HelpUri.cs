using System;
using System.Globalization;

namespace QuickHelp;

/// <summary>
/// Refers to a location within a help system.
/// </summary>
/// <remarks>
/// A help uri may take one of the following formats, and is resolved in
/// that order:
///
/// @LXXXX -- where XXXX is a hexidecimal number with the higest bit set
///     Display the topic with index (XXXX & 0x7FFF) in the local
///     database.
///     
/// @contextstring
///     Display the topic associated with "@contextstring". Only the
///     local database is searched for the context.
/// 
/// !command
///     Execute the command specified after the exclamation point (!).
///     The command is case sensitive. Commands are application-specific.
///
/// filename!
///     Display filename as a single topic. The specified file must be a
///     text file no larger than 64K.
///     
/// helpfile!contextstring
///     Search helpfile for contextstring and display the associated
///     topic. Only the specified Help database or physical Help file is
///     searched for the context.
///     
/// contextstring
///     Display the topic associated with contextstring. The context
///     string is first searched for in the local database; if it is
///     not found, it is searched for in other databases in the help
///     system, and the first match is returned.
/// </remarks>
public class HelpUri
{
    readonly string target;

    /// <summary>
    /// Creates a uri that points to a topic in the local database. This
    /// is called "local context" in QuickHelp terms.
    /// </summary>
    /// <param name="topicIndex">Zero-based topic index.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If topic index is less than 0 or greater than or equal to 0x8000.
    /// </exception>
    public HelpUri(int topicIndex)
    {
        if (topicIndex < 0 || topicIndex >= 0x8000)
            throw new ArgumentOutOfRangeException(nameof(topicIndex));

        this.target = $"@L{topicIndex | 0x8000:X4}";
    }

    /// <summary>
    /// Creates a uri directly.
    /// </summary>
    public HelpUri(string target)
    {
        this.target = target ?? throw new ArgumentNullException(nameof(target));
    }

    /// <summary>
    /// Gets the type of location this uri refers to.
    /// </summary>
    public HelpUriType Type => string.IsNullOrEmpty(target)
                ? HelpUriType.None
                : target.StartsWith('@')
                ? TopicIndex >= 0 
                ? HelpUriType.LocalTopic 
                : HelpUriType.LocalContext
                : target.StartsWith('!')
                ? HelpUriType.Command
                : target.EndsWith('!') 
                ? HelpUriType.File 
                : target.Contains('!') 
                ? HelpUriType.GlobalContext 
                : HelpUriType.Context
                ;

    /// <summary>
    /// Gets the topic index specified by this uri, or -1 if this uri
    /// does not specify a topic index.
    /// </summary>
    public int TopicIndex => target.Length == 6 && target.StartsWith("@L") 
             && int.TryParse(
                target.AsSpan(2),
                NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture,
                out int topicIndexOr8000) &&
                (topicIndexOr8000 & 0x8000) != 0
                ? topicIndexOr8000 & 0x7FFF
                : -1
            ;

    /// <summary>
    /// Gets the database name component of the url.
    /// </summary>
    /// <value>
    /// The text to the left of the first '!' in the uri, or <c>null</c>
    /// if the uri does not contain any '!'.
    /// </value>
    public string DatabaseName
    {
        get
        {
            int k = target.IndexOf('!');
            return k <= 0 ? null : target.Substring(0, k);
        }
    }

    /// <summary>
    /// Gets the context string component of the url.
    /// </summary>
    /// <value>
    /// The text to the right of the first '!', or the entire target if
    /// the uri does not contain any '!'.
    /// </value>
    public string ContextString
    {
        get
        {
            int k = target.IndexOf('!');
            return k < 0 ? target : target[(k + 1)..];
        }
    }

    public string Target => target;

    public override string ToString() => target;
}

/// <summary>
/// Specifies the type of a help uri.
/// </summary>
public enum HelpUriType : int
{
    /// <summary>
    /// The uri is empty.
    /// </summary>
    None = 0,

    /// <summary>
    /// The uri specifies a command to be executed.
    /// </summary>
    Command = 1,

    /// <summary>
    /// The uri contains a topic index to be resolved in the local
    /// database.
    /// </summary>
    LocalTopic = 2,

    /// <summary>
    /// The uri contains a context string to be resolved in the local
    /// database.
    /// </summary>
    LocalContext = 3,

    /// <summary>
    /// The uri contains a database name and a context string; the
    /// context string must be resolved in that database.
    /// </summary>
    GlobalContext = 4,

    /// <summary>
    /// The uri contains a context string that can be resolved in any
    /// database in the help system, with the local database searched
    /// first.
    /// </summary>
    Context = 5,

    /// <summary>
    /// The uri contains a database name to be displayed as a single
    /// help topic.
    /// </summary>
    File = 6,
}

#if false
public class HelpLocation
{
    HelpTopic Topic;
    ushort LineNumber; // zero-based
    byte ColumnNumber; // zero-based
}
#endif

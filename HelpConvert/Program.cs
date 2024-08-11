using System;
using System.IO;
using System.Text;
using QuickHelp;
using QuickHelp.Formatters;
using QuickHelp.Serialization;

namespace HelpConvert;

public class Program
{
    public static int Main(string[] args) 
        => args.Length == 0 ? PrintUsage() : Convert(args, false);

    static int PrintUsage()
    {
        Console.WriteLine("HelpConvert input-file [input-file ...]");
        return 0;
    }

    static int Convert(string[] fileNames, bool isDryRun)
    {
        var system = new HelpSystem();
        var converter = new BatchHtmlFormatter(system);

        foreach (var fileName in fileNames)
        {
            var decoder = new BinaryHelpDeserializer();
            foreach (var database in decoder.LoadDatabases(fileName))
                system.Databases.Add(database);
        }

        // export HTML
        foreach (var database in system.Databases)
        {
            var htmlPath = database.Name.Replace('.', '_');

            if ((htmlPath =PathUtils.EnsureDirectory(htmlPath))!=string.Empty)
            {
                int topicIndex = 0;
                foreach (var topic in database.Topics)
                {
                    var html = converter.FormatTopic(topic);
                    var htmlFileName = Path.Combine(htmlPath, string.Format("T{0:X4}.html", topicIndex));
                    if (!isDryRun)
                    {
                        using var writer = new StreamWriter(htmlFileName, false, Encoding.UTF8);
                        writer.Write(html);
                    }
                    topicIndex++;
                }

                // Create contents.html.
                var topic1 = system.ResolveUri(database, new HelpUri("h.contents"));
                if (topic1 != null && topic1.Database == database)
                {
                    if (!isDryRun)
                    {
                        using var writer = new StreamWriter(Path.Combine(htmlPath, "Contents.html"));
                        writer.WriteLine("<meta http-equiv=\"refresh\" content=\"0; url=T{0:X4}.html\">",
                            topic1.TopicIndex);
                    }
                }
            }
        }
        return 0;
    }
}

class BatchHtmlFormatter : HtmlFormatter
{
    readonly HelpSystem system;

    public BatchHtmlFormatter(HelpSystem system)
    {
        base.FixLinks = true;
        this.system = system;
    }

    protected override string FormatUri(HelpTopic source, HelpUri uri)
    {
        switch (uri.Type)
        {
            case HelpUriType.Context:
            case HelpUriType.GlobalContext:
            case HelpUriType.LocalContext:
                {
                    var target = system.ResolveUri(source.Database, uri);
                    if (target != null)
                    {
                        return target.Database == source.Database
                            ? $"T{target.TopicIndex:X4}.html"
                            : $"../{GetDatabasePath(target.Database)}/T{target.TopicIndex:X4}.html";
                    }
                    else
                    {
                        Console.WriteLine("Warning: cannot resolve context string '{0}'", uri);
                    }
                }
                break;

            case HelpUriType.LocalTopic:
                return $"T{uri.TopicIndex:X4}.html";

            case HelpUriType.Command:
            case HelpUriType.File:
            default:
                // TODO: would be better if we have the source location.
                Console.WriteLine("Warning: cannot convert link: {0}", uri);
                break;
        }
        return "?" + uri.ToString();
    }

    static string GetDatabasePath(HelpDatabase database) 
        => PathUtils.EnsureDirectory(database.Name.Replace('.', '_'));

}

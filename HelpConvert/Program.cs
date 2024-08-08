﻿using System;
using System.IO;
using System.Text;
using QuickHelp;
using QuickHelp.Formatters;
using QuickHelp.Serialization;

namespace HelpConvert;

public class Program
{
    public static void Main(string[] args)
    {
        Convert(args, false);

#if DEBUG
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
#endif
    }

    static void PrintUsage()
    {
        Console.WriteLine("HelpConvert input-file [input-file ...]");
    }

    static void Convert(string[] fileNames, bool isDryRun)
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
            // TODO: check invalid chars in database name
            var htmlPath = database.Name.Replace('.', '_');
            Directory.CreateDirectory(htmlPath);

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

    static string GetDatabasePath(HelpDatabase database)
    {
        string path = database.Name.Replace('.', '_');
        Directory.CreateDirectory(path);
        return path;
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
                        if (target.Database == source.Database)
                        {
                            return string.Format("T{0:X4}.html", target.TopicIndex);
                        }
                        else
                        {
                            return string.Format("../{0}/T{1:X4}.html",
                                GetDatabasePath(target.Database),
                                target.TopicIndex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Warning: cannot resolve context string '{0}'", uri);
                    }
                }
                break;

            case HelpUriType.LocalTopic:
                return string.Format("T{0:X4}.html", uri.TopicIndex);

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
    {
        var path = database.Name.Replace('.', '_');
        Directory.CreateDirectory(path);
        return path;
    }
}

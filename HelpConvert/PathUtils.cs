using System;
using System.IO;
using System.Linq;

namespace HelpConvert;

public static class PathUtils
{
    //”（双引号）、*（星号）、<（小于）、>（大于）、?（问号）、\（反斜杠）、|（竖线）、/ (正斜杠)、 : (冒号)。
    public static readonly char[] InvalidChars = [
        '"','*','<','>','?','\\','|','/',':'
        ];
    public static readonly string[] InvalidNames = [
        "CON",
        "PRN",
        "AUX",
        "NUL",
        "COM1",
        "COM2",
        "COM3",
        "COM4",
        "COM5",
        "COM6",
        "COM7",
        "COM8",
        "COM9",
        "LPT1",
        "LPT2",
        "LPT3",
        "LPT4",
        "LPT5",
        "LPT6",
        "LPT7",
        "LPT8",
        "LPT9"
        ];

    public static string EnsureDirectory(string path)
    {
        if (path == null) return string.Empty;
        var i = InvalidNames.ToList().IndexOf(path);
        if (i >= 0)
        {
            path = new string('_', InvalidNames[i].Length);
        }
        if(path!=string.Empty)
        {
            var dest = EnsurePath(path);
            if (!Directory.Exists(dest))
            {
                try
                {
                    Directory.CreateDirectory(dest);
                    return dest;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"ERROR:{e.Message}");
                }
            }
        }
        return path;
    }
    private static string EnsurePath(string path)
    {
        var chars = path.ToCharArray();
        for(int i = 0; i < path.Length; i++)
            if (InvalidChars.Contains(chars[i]))
                chars[i]= '_';
        return new string(chars);
    }
}

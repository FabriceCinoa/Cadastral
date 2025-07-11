using HtmlAgilityPack;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

using System.Runtime.InteropServices;

namespace Common.Library;

public static class Utils
{
    private const int KF_FLAG_DEFAULT = 0;
    private static class KnownFolder
    {
        public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, int dwFlags, IntPtr hToken, out IntPtr ppszPath);

    public static string StoragePath => DropBoxUserApp.Common.Constants.StorageLocation + "\\storage";
    public static string TemporayPath => System.IO.Path.GetTempPath().Trim('\\') + @"\DropBoxUserApp\";

    public static string DownloadPath = GetDownloadsFolderPath()+ @"\DropBoxAccess\" ;

    public static string CalculateChecksum(string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static string GetDownloadsFolderPath()
    {
        StringBuilder path = new StringBuilder(260);
        SHGetKnownFolderPath(KnownFolder.Downloads, 0, IntPtr.Zero, out IntPtr pPath);
        string downloadsPath = Marshal.PtrToStringUni(pPath);
        Marshal.FreeCoTaskMem(pPath);
        return downloadsPath;
    }

    public static string ExtractTextFromHtml(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        var patternInside = @"\[[^\]]*\]";

        doc.LoadHtml(html);

        var sb = new StringBuilder();
        var nodes = doc.DocumentNode.SelectNodes("//text()");
        if (nodes == null)
        {
            return html;
        }
        foreach (HtmlNode node in nodes.Where(c => c.ParentNode.Name != "style"))
        {
            MatchCollection matchesInside = Regex.Matches(node.InnerText, patternInside);
            var txt = node.InnerText;
            matchesInside.ToList().ForEach(a => txt = txt.Replace(a.Value, " "));


            sb.AppendLine(HttpUtility.HtmlDecode(txt));
        }

        return sb.ToString();
    }

    public static string ToSimplEmail(this string text)
    {
        string pattern = @"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})";

        Match match = Regex.Match(text, pattern);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if ((unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != UnicodeCategory.OtherPunctuation) || c == '.')
            {
                stringBuilder.Append(c);
            }

        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }


    public static string ConvertUtf8ToAscii(this string utf8String)
    {
        // Create an encoder for the ASCII encoding
        Encoding ascii = Encoding.ASCII;

        // Get the byte array from the UTF-8 string
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(utf8String);

        // Get the ASCII byte array from the UTF-8 byte array
        byte[] asciiBytes = Encoding.Convert(Encoding.UTF8, ascii, utf8Bytes);

        // Convert the ASCII byte array back to a string
        string asciiString = ascii.GetString(asciiBytes);

        return asciiString.Replace("?", "  ");
    }
    public static string ConvertToUnicode(this string utf8String)
    {
        // Create an encoder for the ASCII encoding
        Encoding unicode = Encoding.Unicode;

        // Get the byte array from the UTF-8 string
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(utf8String);

        // Get the ASCII byte array from the UTF-8 byte array
        byte[] asciiBytes = Encoding.Convert(Encoding.UTF8, unicode, utf8Bytes);

        // Convert the ASCII byte array back to a string
        string asciiString = unicode.GetString(asciiBytes);

        return asciiString;
    }

    public static string ToStringEx(this Byte[] bytes)
    {
        return BitConverter.ToString(bytes);
    }


    public static T Convert<T>( this object obj) where T : class
    {
        try
        {
            var opt = new JsonSerializerOptions()
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles

            };
            var str = JsonSerializer.Serialize(obj,opt);
           

            var ret = JsonSerializer.Deserialize<T>(str,opt);
            return ret??default(T);
        }
        catch (Exception ex)
        {

        }
        return default(T);
    }

}

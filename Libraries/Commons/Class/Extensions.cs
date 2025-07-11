using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Library.Extensions;

public static class Extensions
{

    public static void TryLogError(this ILogger logger, Exception ex, string Message = "", [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
    {
        if (logger != null)
        {
            logger.LogError(ex, $"Error  : {Message}  > in {memberName}, File {sourceFilePath} ");
        }
    }

    public static void TryLogError(this ILogger logger, string Message = "", [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
    {
        if (logger != null)
        {
            logger.LogError($"Error  : {Message}  > in {memberName}, File {sourceFilePath} ");
        }
    }
    public static void TryLogInfomation(this ILogger logger, string Message = "", [CallerMemberName] string memberName = "")
    {
        if (logger != null)
        {
            logger.LogInformation($"{memberName} - {Message}");
        }
    }

    public static string RemoveIllegalPathCaracters(this string source)
    {
        string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        illegal = r.Replace(source, "");
        return illegal;
    }


    public static string Serialize(this object source)
    {
        try
        {
            return JsonSerializer.Serialize(source);
        }
        catch (Exception)
        {

            return "";
        }
    }

    public static T DeSerialize<T>(this string source)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(source);
        }
        catch (Exception)
        {

            return default(T);
        }
    }

    public static string ToQueryString(this object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
                         where p.GetValue(obj, null) != null
                         select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

        return string.Join("&", properties);
    }


}
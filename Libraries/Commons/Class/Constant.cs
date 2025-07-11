

namespace DropBoxUserApp.Common;

public static class Constants
{
    public static readonly string ApplicationName = "DropBoxUserApp";

    private static string _StorageLocation { get; set; } = "";
    public static string StorageLocation
    {
        get
        {
            if (_StorageLocation == "")
            {
                string usePath = "";

                try
                {
                    usePath = Environment.GetEnvironmentVariables()["LOCALAPPDATA"].ToString();
                }
                catch
                {
                    usePath = new DirectoryInfo(Environment.SpecialFolder.LocalApplicationData.ToString()).FullName;
                }

                usePath += @"\" + ApplicationName;
                if (!Directory.Exists(usePath))
                {
                    Directory.CreateDirectory(usePath);
                }
                _StorageLocation = usePath;
            }
            return _StorageLocation;
        }
    }

 }


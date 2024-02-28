using static FacebookCreator.ProxyDroid.ProxyHelper;

namespace FacebookCreator.Models
{
    public class GlobalModels
    {
        public static List<DeviceInfo> Devices = new List<DeviceInfo>();
        //--------------------------------------------------------//
        public static string PathLastName = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\LastName.txt");
        public static string PathFirstName = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\FirstName.txt");
        public static string PathFolderAvatar = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\FolderAvatar");
        public static string PathProxy = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\Proxy.txt");
        public static string PathUserName = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\UserName.txt");
        public static string PathEmailRecovery = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\EmailRecovery.txt");
        public static string PathEmail = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User\\Email.txt");
        //-------------------------------------------------------------//
        public static string PathDataFirstNameUS = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Us\\FirstName.txt");
        public static string PathDataLastNameUs = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Us\\Lastnames.txt");

        public static string PathDataFirstNameVN = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Vn\\FirstName.txt");
        public static string PathDataLastNameVN = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Vn\\Lastnames.txt");

        public static string PathDataUserName = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\UserName.txt");

        public static string NamePasswrod { get;set; }
        public static int NumberPasswrod { get;set; }
        //--------------------------------------------------------//
        public static List<ProxyInfo> Proxies = new List<ProxyInfo>();
        public static List<UserNameInfo> Usernames = new List<UserNameInfo>();
        public static List<string>UserNameRandom =  new List<string>();
        public static List<string> Firstnames = new List<string>();
        public static List<string> Lastnames = new List<string>();
        //-------------------------------------------------------//
        public static string Service { get;set; }
        public static string Alias { get;set; }
        public static string Contry { get;set; }
        public static string Apikey { get;set; }
        public static string PhoneCode { get;set; }
        //-------------------------------------------------------//
        public static string Passwrod { get; set; }
        public static bool IsRandomPassword { get; set; }
        //-----------------------------------------------------//
        public static bool IsRandomFullnameUs { get;set; } = true;
        public static bool IsRandomFullnameVN { get;set; }
        public static bool IsCustomFullname { get; set; }
        //------------------------------------------------------//
    }
}

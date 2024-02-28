using AdvancedSharpAdbClient;
using static FacebookCreator.FacebookConsoller.FacebookController;

namespace FacebookCreator.FacebookConsoller
{
    public interface IFacebookController
    {
        Task<int> CheckLayoutFacebookAsync(string index, DeviceData data, AdbClient client);
        Task<string> ClickCreaNewAccountFacebookAsync(DeviceData data, AdbClient adbClient);
        Task<string> ImportFullnameFacebookAsync(string index, DeviceData data, AdbClient adbClient, string firtname, string lastname);
        Task<string> SelecBirthDayFacebookAsync(string index, DeviceData data, AdbClient adbClient);
        Task<string> SelecGenderFacebookAsync(DeviceData data, AdbClient adbClient, int? gender);
        Task<string> ImportEmailFacebookAsync(string index, DeviceData data, AdbClient adbClient, string email);
        Task<string> ImportPasswrodFacebookAsync(string index, DeviceData data, AdbClient adbClient, string password);
        Task<bool> ImportCodeFacebookAsync(string index, DeviceData data, AdbClient adbClient, string code);
        Task<int> CheckLoginSuccesFacebookAsync(DeviceData data, AdbClient client);
        Task<string> GetPassword2FAFacebookAsync(string deviceId, string index, DeviceData data, AdbClient client, string password);
        Task<bool> UploadAvatarToFacebookAsync(string accessToken, string imagePath, string? proxyUrl);
        Task<string> ImportPhonenumberFacebookAsync(string index, DeviceData data, AdbClient adbClient, string phonenumber);
        Task<bool> FakeEmailFacebookAsync(string index, DeviceData data, AdbClient adbClient, string email);
        Task<string> GetTokenFacebookAsync(string deviceId, string folderBackup);
    }
}

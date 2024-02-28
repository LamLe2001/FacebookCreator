using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace LDPlayerAndADBController
{
    public class Helpers
    {
        public static void SetValueInJsonFile(string jsonFile, string key, object newValue)
        {
            string json = File.ReadAllText(jsonFile);
            JObject data = JObject.Parse(json);

            // Đặt giá trị mới cho khóa
            data[key] = JToken.FromObject(newValue);

            // Ghi lại tệp JSON với giá trị mới
            string updatedJson = data.ToString(Formatting.Indented);
            File.WriteAllText(jsonFile, updatedJson);
        }
        public static string GenerateRandomString(string charsString,int length)
        {
            //"abcdefghijklmnopqrstuvwxyz0123456789"
            Random random = new Random();  
            string randomString = new string(Enumerable.Repeat(charsString, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}

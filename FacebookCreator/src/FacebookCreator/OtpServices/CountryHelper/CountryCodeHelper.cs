using Newtonsoft.Json;
using Serilog;

namespace FacebookCreator.OtpServices.CountryHelper
{
    public class CountryCodeHelper
    {
        public List<CountryCode> GetCountryCodes()
        {
            try
            {
                string CountryCodes = Path.Combine(Environment.CurrentDirectory, "Data\\Files\\CountryCodes.json");
                string json = File.ReadAllText(CountryCodes);
                List<CountryCode> data = JsonConvert.DeserializeObject<List<CountryCode>>(json);
                return data;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }

            return null;
        }


    }
}

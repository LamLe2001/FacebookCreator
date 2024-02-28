using FacebookCreator.FiveSimApi;
namespace FacebookCreator.FiveSimApi
{
    public class FiveResult
    {
        public decimal id { get; set; }
        public string phone { get; set; }
        public string product { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
        public List<FiveSms> sms { get; set; }
        public string created_at { get; set; }
        public string country { get; set; }
    }
}

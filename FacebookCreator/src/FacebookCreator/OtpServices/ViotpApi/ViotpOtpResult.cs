namespace FacebookCreator.ViotpApi
{
    public class ViotpOtpResult
    {
        public string ID { get; set; }
        public string Phone { get; set; }
        public decimal ServiceID { get; set; }
        public string ServiceName { get; set; }
        public decimal Status { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsSound { get; set; }
        public string SmsContent { get; set; }
        public string Code { get; set; }
        public string PhoneOriginal { get; set; }
        public string CountryISO { get; set; }
        public string CountryCode { get; set; }

    }
}

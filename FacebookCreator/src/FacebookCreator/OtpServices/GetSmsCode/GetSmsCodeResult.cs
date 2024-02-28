namespace FacebookCreator.GetSmsCode
{
    public class GetSmsCodeResult
    {
        public bool Status { get; set; }
        public Data? data { get; set; } 
        public string? sms_code { get; set; }
        public Errors? errors { get; set; }
        public string? error { get;set; }
        public class Data
        {
            public string ActivationId { get; set; }
            public string Phone_number { get; set; }
            public bool CanGetAnotherSms { get; set; }
            public string ActivationTime { get; set; }
            public decimal ActivationCost { get; set; }  
            public string Service_code { get; set;}
            public string Country_id { get; set; }

        }
        public class Errors
        {
            public string error_code { get; set; }
            public string error_mess { get; set; }
        }
    }
}

namespace FacebookCreator.ViotpApi
{
    public class ViotpApiResponse<T> where T : class
    {
        public T data { get; set; }
        public decimal status_code { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}

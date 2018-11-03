namespace Games4Trade.Models
{
    public class OperationResult
    {
        public bool IsSuccessful { get; set; }
        public bool IsClientError { get; set; }
        public string Message { get; set; }
        public object Payload { get; set; }
    }
}

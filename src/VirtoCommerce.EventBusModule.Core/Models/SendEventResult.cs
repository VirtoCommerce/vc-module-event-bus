namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SendEventResult
    {
        public string ResponseResult { get; set; }

        public int? StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}

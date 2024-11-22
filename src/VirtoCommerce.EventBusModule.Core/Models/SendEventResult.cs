using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SendEventResult
    {
        public int Status { get; set; } = StatusCodes.Status200OK;

        public string ErrorMessage { get; set; }

        public string ErrorPayload { get; set; }
    }
}

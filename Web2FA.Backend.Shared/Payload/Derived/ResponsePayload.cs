using Web2FA.Backend.Shared.Payload.Base;

namespace Web2FA.Backend.Shared.Payload.Derived
{
    public class ResponsePayload<TData, TAdditionalData> : PayloadBase
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public TAdditionalData? AdditionalData { get; set; }

        public ResponsePayload(bool isSuccess, string? message = null, TData? data = default, TAdditionalData? additionalData = default)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            AdditionalData = additionalData;
        }

        public ResponsePayload()
        {
            IsSuccess = false;
            Data = default;
            Message = string.Empty;
            AdditionalData = default;
        }
    }
}

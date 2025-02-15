using Ncs.Cqrs.Domain.Constants;

namespace Ncs.Cqrs.Application.Common.DTOs
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? MessageDetail { get; set; }
        public T? Data { get; set; }

        public static ResponseDto<T> SuccessResponse(T data, string message = "Request successful")
        {
            return new ResponseDto<T> { Success = true, Data = data, Message = message, MessageDetail = null };
        }

        public static ResponseDto<T> ErrorResponse(ErrorCodes errorCode, string? messageDetail)
        {
            return new ResponseDto<T>
            {
                Success = false,
                ErrorCode = ErrorMessages.GetCode(errorCode),
                Message = ErrorMessages.GetMessage(errorCode),
                MessageDetail = messageDetail,
                Data = default
            };
        }
    }
}

namespace Ncs.Cqrs.Domain.Constants
{
    public enum ErrorCodes
    {
        Unauthorized,
        AccessDenied,
        NotFound,
        InvalidInput,
        RFIDNotDetected,
        RFIDInvalid,
        PaymentFailed,
        MenuUnavailable,
        DatabaseError,
        CreateFailed,
        UpdateFailed,
        DeleteFailed,
        DuplicateData,
        UnexpectedError
    }
}

namespace Ncs.Cqrs.Domain.Constants
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<ErrorCodes, (string Code, string Message)> Errors = new()
        {
            { ErrorCodes.Unauthorized, ("401-01", "Unauthorized request.") },
            { ErrorCodes.AccessDenied, ("403-01", "Access denied.") },
            { ErrorCodes.NotFound, ("404-01", "Resource not found.") },
            { ErrorCodes.InvalidInput, ("400-V01", "Invalid input parameters.") },
            { ErrorCodes.RFIDNotDetected, ("404-R01", "RFID card not detected. Please scan again.") },
            { ErrorCodes.RFIDInvalid, ("400-R02", "Invalid RFID card. Please contact admin.") },
            { ErrorCodes.PaymentFailed, ("400-P01", "Payment processing failed. Try again.") },
            { ErrorCodes.MenuUnavailable, ("400-M01", "Selected item is out of stock.") },
            { ErrorCodes.CreateFailed, ("400-C01", "Failed to create resource. Please try again.") },
            { ErrorCodes.UpdateFailed, ("400-U01", "Failed to update resource. Please try again.") },
            { ErrorCodes.DeleteFailed, ("400-D01", "Failed to delete resource. Please try again.") },
            { ErrorCodes.DuplicateData, ("400-D02", "The data you are trying to insert already exists.") },
            { ErrorCodes.DatabaseError, ("500-DB01", "Database operation failed. Please try again.") },
            { ErrorCodes.UnexpectedError, ("500-S01", "An unexpected error occurred. Contact support.") }
        };

        public static string GetMessage(ErrorCodes code) =>
            Errors.TryGetValue(code, out var error) ? error.Message : "An unknown error occurred.";
        public static string GetCode(ErrorCodes code) =>
            Errors.TryGetValue(code, out var error) ? error.Code : "999-99";
        public static (string Code, string Message) GetErrorDetails(ErrorCodes code) =>
            Errors.TryGetValue(code, out var error) ? error : ("999-99", "An unknown error occurred.");

        public static ErrorCodes? GetErrorCodeByCode(string code)
        {
            var errorEntry = Errors.FirstOrDefault(e => e.Value.Code == code);
            return errorEntry.Key;
        }
    }
}

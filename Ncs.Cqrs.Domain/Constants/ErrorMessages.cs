namespace Ncs.Cqrs.Domain.Constants
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<ErrorCodes, string> Messages = new()
        {
            { ErrorCodes.Unauthorized, "Unauthorized request." },
            { ErrorCodes.AccessDenied, "Access denied." },
            { ErrorCodes.NotFound, "Resource not found." },
            { ErrorCodes.InvalidInput, "Invalid input parameters." },
            { ErrorCodes.RFIDNotDetected, "RFID card not detected. Please scan again." },
            { ErrorCodes.RFIDInvalid, "Invalid RFID card. Please contact admin." },
            { ErrorCodes.PaymentFailed, "Payment processing failed. Try again." },
            { ErrorCodes.MenuUnavailable, "Selected item is out of stock." },
            { ErrorCodes.CreateFailed, "Failed to create resource. Please try again." },
            { ErrorCodes.UpdateFailed, "Failed to update resource. Please try again." },
            { ErrorCodes.DeleteFailed, "Failed to delete resource. Please try again." },
            { ErrorCodes.DuplicateData, "The data you are trying to insert already exists." },
            { ErrorCodes.DatabaseError, "Database operation failed. Please try again." },
            { ErrorCodes.UnexpectedError, "An unexpected error occurred. Contact support." }
        };

        public static string GetMessage(ErrorCodes code) =>
            Messages.TryGetValue(code, out var message) ? message : "An unknown error occurred.";

        private static readonly Dictionary<ErrorCodes, string> Codes = new()
        {
            { ErrorCodes.RFIDInvalid, "400-02" },
            { ErrorCodes.PaymentFailed, "400-03." },
            { ErrorCodes.MenuUnavailable, "400-04" },
            { ErrorCodes.CreateFailed, "400-05" },
            { ErrorCodes.UpdateFailed, "400-06" },
            { ErrorCodes.DeleteFailed, "400-07" },
            { ErrorCodes.DuplicateData, "400-08" },
            { ErrorCodes.Unauthorized, "401-01" },
            { ErrorCodes.InvalidInput, "400-01" },
            { ErrorCodes.AccessDenied, "403-01." },
            { ErrorCodes.NotFound, "404-01" },
            { ErrorCodes.RFIDNotDetected, "404-02" },
            { ErrorCodes.UnexpectedError, "500-01" },
            { ErrorCodes.DatabaseError, "500-02" },
        };
        public static string GetCode(ErrorCodes code) =>
            Codes.TryGetValue(code, out var messageCode) ? messageCode : "999-99";

    }
}

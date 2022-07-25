namespace SMS_Microservice.Exceptions
{
    public class InvalidSMSCommandException: Exception
    {
        public InvalidSMSCommandException(string message)
            : base(message) { }
    }
}

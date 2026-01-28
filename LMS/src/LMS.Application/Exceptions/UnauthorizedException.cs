namespace LMS.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException() : base("Authentication failed.")
        {
        }
    }
}

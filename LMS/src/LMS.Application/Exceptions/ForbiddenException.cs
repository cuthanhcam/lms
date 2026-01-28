namespace LMS.Application.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException() : base("You do not have permission to perform this action.")
        {
        }
    }
}

namespace LMS.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public List<string> Errors { get; set; } = new();

        public BadRequestException(List<string> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }
}

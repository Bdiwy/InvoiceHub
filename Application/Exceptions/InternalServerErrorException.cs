namespace Application.Exceptions;

public sealed class InternalServerErrorException : Exception
{
    public InternalServerErrorException(string message = "Something went wrong!") : base(message) { }
}
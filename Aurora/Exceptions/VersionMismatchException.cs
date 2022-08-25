using System.Runtime.Serialization;

namespace Aurora.Exceptions;

public class VersionMismatchException : Exception
{
    public VersionMismatchException()
    {
    }

    protected VersionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public VersionMismatchException(string? message) : base(message)
    {
    }

    public VersionMismatchException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
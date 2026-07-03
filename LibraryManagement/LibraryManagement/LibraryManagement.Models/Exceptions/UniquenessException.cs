namespace LibraryManagement.LibraryManagement.Models.Exceptions;

public class UniquenessException : Exception
{
    public UniquenessException()
    { 
    }

    public UniquenessException(string message) 
        : base(message) 
    { 
    }

    public UniquenessException(string message, Exception innerException) 
        : base(message, innerException) 
    { 
    }
}
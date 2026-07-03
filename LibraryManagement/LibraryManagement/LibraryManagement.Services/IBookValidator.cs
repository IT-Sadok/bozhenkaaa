namespace LibraryManagement.LibraryManagement.Services;

public interface IBookValidator
{
    bool DoesBookWithCodeExist(string code);
}
using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Repositories;

namespace LibraryManagement.LibraryManagement.Services;

public class BookValidator : IBookValidator
{
    private readonly IDataRepository _bookRepository;

    public BookValidator(IDataRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public bool DoesBookWithCodeExist(string code)
    {
        return _bookRepository.DoesBookExistByCode(code);
    }
}
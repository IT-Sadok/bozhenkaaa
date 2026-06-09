using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;
using LibraryManagement.LibraryManagement.Models.Exceptions;
using LibraryManagement.LibraryManagement.Repositories;

namespace LibraryManagement.LibraryManagement.Services;

public class BookManagerService : IBookManagerService
{
    private readonly IDataRepository _dataRepository;
    private readonly IBookValidator _bookValidator;

    public BookManagerService(IDataRepository dataRepository, IBookValidator bookValidator)
    {
        _dataRepository = dataRepository;
        _bookValidator = bookValidator;
    }

    public List<Book> GetAllAvailableBooks()
    {
        return _dataRepository.GetAllAvailableBooks();
    }

    public List<Book> GetAllBooksByAuthor(string author)
    {
        return _dataRepository.GetAllBooksByAuthor(author);
    }

    public Book GetBookByTitle(string title)
    {
        var book = _dataRepository.GetBookByTitle(title);
        return book ?? throw new KeyNotFoundException($"The book with title {title} does not exist.");
    }

    public void AddBook(Book book)
    {
        if (_bookValidator.DoesBookWithCodeExist(book.Code))
        {
            throw new UniquenessException($"The book with code {book.Code} already exists.");
        }

        _dataRepository.AddBook(book);
    }

    public void UpdateBookByCode(string code, BookStatus bookStatus)
    {
        if (!_bookValidator.DoesBookWithCodeExist(code))
        {
            throw new KeyNotFoundException($"The book with code {code} does not exist. Start by adding it.");
        }
        
        _dataRepository.UpdateBookByCode(code, bookStatus);
    }

    public void DeleteBookByCode(string code)
    {
        if (!_bookValidator.DoesBookWithCodeExist(code))
        {
            throw new KeyNotFoundException($"The book with code {code} does not exist.");
        }
        
        _dataRepository.DeleteBookByCode(code);
    }
}
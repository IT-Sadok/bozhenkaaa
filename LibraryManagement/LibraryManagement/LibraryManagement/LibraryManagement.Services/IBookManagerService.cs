using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Services;

public interface IBookManagerService
{
    List<Book> GetAllAvailableBooks();
    
    List<Book> GetAllBooksByAuthor(string author);
    
    Book GetBookByTitle(string title);
    
    void AddBook(Book book);
    
    void UpdateBookByCode(string code, BookStatus bookStatus);
    
    void DeleteBookByCode(string code);
}
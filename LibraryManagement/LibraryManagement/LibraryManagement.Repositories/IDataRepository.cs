using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Repositories;

public interface IDataRepository
{
    List<Book> GetAllAvailableBooks();
    
    List<Book> GetAllBooksByAuthor(string author);
    
    Book? GetBookByTitle(string title);
    
    void AddBook(Book book);
    
    void UpdateBookByCode(string code, BookStatus bookStatus);
    
    bool DoesBookExistByCode(string code);
    
    void DeleteBookByCode(string code);
}
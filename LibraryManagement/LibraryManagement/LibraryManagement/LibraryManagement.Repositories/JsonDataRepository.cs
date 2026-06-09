using System.Text.Json;
using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Repositories;

public class JsonDataRepository : IDataRepository
{
    private const string FilePath = "Books.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    
    public List<Book> GetAllAvailableBooks()
    {
        return LoadAll()
            .Where(book => book.BookStatus == BookStatus.Available)
            .ToList();
    }

    public List<Book> GetAllBooksByAuthor(string author)
    {
        return LoadAll()
            .Where(book => book.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Book? GetBookByTitle(string title)
    {
        return LoadAll()
            .FirstOrDefault(book => book.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    public void AddBook(Book book)
    {
        var allBooks = LoadAll();
        allBooks.Add(book);
        SaveAll(allBooks);
    }

    public void UpdateBookByCode(string code, BookStatus bookStatus)
    {
        var books = LoadAll();
        var bookToUpdate = books.FirstOrDefault(book => book.Code == code);
        if (bookToUpdate == null)
        {
            throw new KeyNotFoundException();
        }
        bookToUpdate.BookStatus = bookStatus;
        SaveAll(books);
    }

    public bool DoesBookExistByCode(string code)
    {
        return LoadAll()
            .Any(book => book.Code == code);
    }

    public void DeleteBookByCode(string code)
    {
        var books = LoadAll();
        var bookToDelete = books.FirstOrDefault(book => book.Code == code);
        if (bookToDelete == null)
        {
            return;
        }
        books.Remove(bookToDelete);
        SaveAll(books);
    }
    
    private static List<Book> LoadAll()
    {
        if (!File.Exists(FilePath))
            return [];

        var json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<Book>>(json) ?? [];
    }

    private void SaveAll(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books, _jsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
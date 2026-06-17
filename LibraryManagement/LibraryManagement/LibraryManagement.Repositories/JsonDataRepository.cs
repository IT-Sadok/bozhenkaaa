using System.Text.Json;
using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Repositories;

public class JsonDataRepository : IDataRepository
{
    private const string FilePath = "Books.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    
    private readonly List<Book> _booksCache = LoadAllFromFile();
    
    private readonly object _syncRoot = new();

    public List<Book> GetAllAvailableBooks()
    {
        lock (_syncRoot)
        {
            return _booksCache
                .Where(book => book.BookStatus == BookStatus.Available)
                .ToList();
        }
    }

    public List<Book> GetAllBooksByAuthor(string author)
    {
        lock (_syncRoot)
        {
            return _booksCache
                .Where(book => book.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public Book? GetBookByTitle(string title)
    {
        lock (_syncRoot)
        {
            return _booksCache
                .FirstOrDefault(book => book.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }
    }

    public bool DoesBookExistByCode(string code)
    {
        lock (_syncRoot)
        {
            return _booksCache.Any(book => book.Code == code);
        }
    }

    public void AddBook(Book book)
    {
        lock (_syncRoot)
        {
            _booksCache.Add(book);
            SaveAllToFile();
        }
    }

    public void UpdateBookByCode(string code, BookStatus bookStatus)
    {
        lock (_syncRoot)
        {
            var bookToUpdate = _booksCache.FirstOrDefault(book => book.Code == code);
            if (bookToUpdate == null)
            {
                throw new KeyNotFoundException($"Book with code '{code}' was not found.");
            }
            
            bookToUpdate.BookStatus = bookStatus;
            SaveAllToFile();
        }
    }

    public void DeleteBookByCode(string code)
    {
        lock (_syncRoot)
        {
            var bookToDelete = _booksCache.FirstOrDefault(book => book.Code == code);
            if (bookToDelete == null)
            {
                return;
            }
            
            _booksCache.Remove(bookToDelete);
            SaveAllToFile();
        }
    }
    
    private static List<Book> LoadAllFromFile()
    {
        if (!File.Exists(FilePath))
            return [];

        var json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<Book>>(json) ?? [];
    }

    private void SaveAllToFile()
    {
        var json = JsonSerializer.Serialize(_booksCache, _jsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
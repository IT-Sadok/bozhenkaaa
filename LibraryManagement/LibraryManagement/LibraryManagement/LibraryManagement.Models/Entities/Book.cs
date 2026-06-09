using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Models.Entities;

public class Book
{
    public string Code { get; set; }
    
    public string Title { get; set; }
    
    public string Author { get; set; }
    
    public int Year { get; set; }
    
    public BookStatus BookStatus { get; set; } = BookStatus.Available;
}
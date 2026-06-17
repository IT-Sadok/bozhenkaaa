using LibraryManagement.LibraryManagement.Models.Enums;

namespace LibraryManagement.LibraryManagement.Models.Entities;

public class Book
{
    public required string Code { get; set; }
    
    public required string Title { get; set; }
    
    public required string Author { get; set; }
    
    public required int Year { get; set; }
    
    public BookStatus BookStatus { get; set; } = BookStatus.Available;
}
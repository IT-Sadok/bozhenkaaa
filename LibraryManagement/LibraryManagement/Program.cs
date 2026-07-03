using LibraryManagement.LibraryManagement.Models.Entities;
using LibraryManagement.LibraryManagement.Models.Enums;
using LibraryManagement.LibraryManagement.Models.Exceptions;
using LibraryManagement.LibraryManagement.Repositories;
using LibraryManagement.LibraryManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement
{
    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDataRepository, JsonDataRepository>();
            services.AddSingleton<IBookValidator, BookValidator>();
            services.AddSingleton<IBookManagerService, BookManagerService>();

            var serviceProvider = services.BuildServiceProvider();
            var bookService = serviceProvider.GetRequiredService<IBookManagerService>();
            var bookValidator = serviceProvider.GetRequiredService<IBookValidator>();

            await RunMenuAsync(bookService, bookValidator);
        }

        private static async Task RunMenuAsync(IBookManagerService bookService,  IBookValidator bookValidator)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Library Management");
                Console.WriteLine("1. Add book");
                Console.WriteLine("2. Change book status (Borrow/Return)");
                Console.WriteLine("3. Delete book");
                Console.WriteLine("4. Get books by author");
                Console.WriteLine("5. Get book by title");
                Console.WriteLine("6. Get all available books");
                Console.WriteLine("7. Run Stress Test (100 Threads)");
                Console.WriteLine("0. Exit");
                Console.Write("Choose what you want to do: ");

                var input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case "1": ExecuteAddBook(bookService); break;
                        case "2": ExecuteUpdateStatus(bookService); break;
                        case "3": ExecuteDeleteBook(bookService); break;
                        case "4": ExecuteSearchByAuthor(bookService); break;
                        case "5": ExecuteSearchByTitle(bookService); break;
                        case "6": ExecuteShowAvailable(bookService); break;
                        case "7": await ExecuteStressTestAsync(bookService, bookValidator); break;
                        case "0": return;
                        default: Console.WriteLine("Unknown command."); break;
                    }
                }
                catch (UniquenessException ex)
                {
                    Console.WriteLine($"[Validation error] {ex.Message}");
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"[Search error] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[System error] {ex.Message}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static async Task ExecuteStressTestAsync(IBookManagerService service, IBookValidator bookValidator)
        {
            Console.WriteLine("Starting 101 concurrent tasks...");

            const string testCode = "STRESS-100";

            if (!bookValidator.DoesBookWithCodeExist(testCode))
            {
                service.AddBook(new Book
                {
                    Code = testCode,
                    Title = "Concurrency Testing Book",
                    Author = "Stress Bot",
                    Year = DateTime.Now.Year,
                    BookStatus = BookStatus.Available
                });
            }

            var tasks = new Task[101];

            for (var i = 0; i < 101; i++)
            {
                var taskId = i;

                tasks[i] = Task.Run(() =>
                {
                    var newStatus = taskId % 2 == 0 ? BookStatus.Borrowed : BookStatus.Available;
                    
                    try
                    {
                        service.UpdateBookByCode(testCode, newStatus);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Task {taskId} failed: {ex.Message}");
                    }
                });
            }

            await Task.WhenAll(tasks);

            var finalBook = service.GetBookByTitle("Concurrency Testing Book");
            Console.WriteLine($"Final status of the test book: {finalBook.BookStatus}");
        }

        private static void ExecuteSearchByTitle(IBookManagerService service)
        {
            var title = GetRequiredInput("Enter title: ");

            var book = service.GetBookByTitle(title);

            Console.WriteLine("Found a book:");
            Console.WriteLine($"- [{book.Code}] {book.Title} ({book.Author}, {book.Year}) | Status: {book.BookStatus}");
        }

        private static void ExecuteAddBook(IBookManagerService service)
        {
            var code = GetRequiredInput("Enter code: ");
            var title = GetRequiredInput("Enter title: ");
            var author = GetRequiredInput("Enter author: ");

            Console.Write("Enter year: ");
            if (!int.TryParse(Console.ReadLine(), out var year) || year < 1000 || year > DateTime.Now.Year)
            {
                Console.WriteLine($"Invalid year. Please enter a valid year up to {DateTime.Now.Year}.");
                return;
            }

            var newBook = new Book 
            { 
                Code = code, 
                Title = title, 
                Author = author,
                Year = year,
                BookStatus = BookStatus.Available
            };

            service.AddBook(newBook);
            Console.WriteLine("The book has been added.");
        }

        private static void ExecuteUpdateStatus(IBookManagerService service)
        {
            var code = GetRequiredInput("Enter code: ");

            Console.WriteLine("Choose new state (1 - Return, 2 - Borrow): ");
            var statusInput = Console.ReadLine();

            if (statusInput != "1" && statusInput != "2")
            {
                Console.WriteLine("Invalid choice. Operation cancelled.");
                return;
            }

            var newStatus = statusInput == "2" ? BookStatus.Borrowed : BookStatus.Available;

            service.UpdateBookByCode(code, newStatus);
            Console.WriteLine("Book status was updated.");
        }

        private static void ExecuteDeleteBook(IBookManagerService service)
        {
            var code = GetRequiredInput("Enter code: ");

            service.DeleteBookByCode(code);
            Console.WriteLine("Book was deleted.");
        }

        private static void ExecuteSearchByAuthor(IBookManagerService service)
        {
            var author = GetRequiredInput("Enter author: ");

            var books = service.GetAllBooksByAuthor(author);

            if (books.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            Console.WriteLine($"Books of author '{author}':");
            foreach (var b in books)
            {
                Console.WriteLine($"- [{b.Code}] {b.Title} ({b.Year}) | Status: {b.BookStatus}");
            }
        }

        private static void ExecuteShowAvailable(IBookManagerService service)
        {
            var books = service.GetAllAvailableBooks();

            if (books.Count == 0)
            {
                Console.WriteLine("No available books found.");
                return;
            }

            Console.WriteLine("Available books:");
            foreach (var b in books)
            {
                Console.WriteLine($"- [{b.Code}] {b.Title} ({b.Author}, {b.Year})");
            }
        }
        
        private static string GetRequiredInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrEmpty(input));

            return input;
        }
    }
}
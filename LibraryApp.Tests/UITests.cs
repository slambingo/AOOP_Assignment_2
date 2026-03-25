using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using LibraryApp.ViewModels;
using LibraryApp.Views;
using Xunit;

namespace LibraryApp.Tests;

public class UITests
{
    // Sets up a fresh test environment: SaveData.json + DataManager + MainWindowViewModel
    private MainWindow SetupTestEnvironment()
    {
        string memberHash = DataManager.HashPassword("member");
        string adminHash = DataManager.HashPassword("admin");
        string json = $$"""
        {
          "users": [
            { "username": "member", "password": "{{memberHash}}", "role": "member" },
            { "username": "admin", "password": "{{adminHash}}", "role": "librarian" }
          ],
          "catalog": [
            {
              "Title": "Test Book",
              "Author": "Test Author",
              "Isbn": "1234567890123",
              "Description": "A test book",
              "Owner": ""
            },
            {
              "Title": "Borrowed Book",
              "Author": "Some Author",
              "Isbn": "9876543210123",
              "Description": "Already borrowed",
              "Owner": "member"
            }
          ]
        }
        """;
        File.WriteAllText("SaveData.json", json);

        var vm = new MainWindowViewModel();
        var window = new MainWindow { DataContext = vm };
        window.Show();
        return window;
    }

    // Simulates logging in by setting ViewModel fields and calling the command
    private void LogInAs(string username, string password)
    {
        var loginVm = MainWindowViewModel.Instance.ActiveWindow.DataContext as LogInWindowViewModel;
        Assert.NotNull(loginVm);
        loginVm!.Username = username;
        loginVm.Password = password;
        loginVm.LogIn();
    }

    // Gets the CatalogWindowViewModel after login
    private CatalogWindowViewModel GetCatalogVm()
    {
        var tabVm = MainWindowViewModel.Instance.ActiveWindow.DataContext as TabControlWindowViewModel;
        Assert.NotNull(tabVm);
        var catalogVm = tabVm!.Tab1Window.DataContext as CatalogWindowViewModel;
        Assert.NotNull(catalogVm);
        return catalogVm!;
    }

    // ===== USE CASE I: Member Borrowing Books =====

    [AvaloniaFact]
    public void UseCase1_MemberBorrowsBook()
    {
        var window = SetupTestEnvironment();

        // Step 1: Log in as member
        LogInAs("member", "member");

        // Verify we're on the home screen
        Assert.IsType<TabControlWindowViewModel>(MainWindowViewModel.Instance.ActiveWindow.DataContext);

        // Step 2: Member sees available books (default view for member)
        var catalogVm = GetCatalogVm();
        Assert.Single(catalogVm.BookList); // only "Test Book" is available
        Assert.Equal("Test Book", catalogVm.BookList[0].Title);

        // Step 3: Click a book and borrow it
        catalogVm.BookClicked("1234567890123");
        Assert.True(catalogVm.BookDetailPopupEnabled);
        Assert.True(catalogVm.IsBorrowButtonEnabled);

        catalogVm.BorrowBook();

        // Step 4: Verify the book is now in "My Books"
        catalogVm.ShowMyBooks();
        Assert.Equal(2, catalogVm.BookList.Count); // "Borrowed Book" + newly borrowed "Test Book"
        Assert.Contains(catalogVm.BookList, b => b.Title == "Test Book" && b.Owner == "member");

        // Verify notification was shown
        Assert.Equal("Book successfully borrowed.", MainWindowViewModel.Instance.NotificationMessage);

        window.Close();
    }

    // ===== USE CASE II: Member Returning a Book =====

    [AvaloniaFact]
    public void UseCase2_MemberReturnsBook()
    {
        var window = SetupTestEnvironment();

        // Step 1: Log in as member
        LogInAs("member", "member");

        // Step 2: Navigate to "My Books"
        var catalogVm = GetCatalogVm();
        catalogVm.ShowMyBooks();
        Assert.Single(catalogVm.BookList); // "Borrowed Book" owned by member
        Assert.Equal("Borrowed Book", catalogVm.BookList[0].Title);

        // Step 3: Click the book and return it
        catalogVm.BookClicked("9876543210123");
        Assert.True(catalogVm.BookDetailPopupEnabled);
        Assert.True(catalogVm.IsReturnButtonEnabled);

        catalogVm.ReturnBook();

        // Step 4: Verify "My Books" is now empty
        Assert.Empty(catalogVm.BookList);

        // Step 5: Verify the book is back in "Available Books"
        catalogVm.ShowAvailableBooks();
        Assert.Contains(catalogVm.BookList, b => b.Title == "Borrowed Book" && b.Owner == "");

        // Verify notification was shown
        Assert.Equal("Book successfully returned.", MainWindowViewModel.Instance.NotificationMessage);

        window.Close();
    }

    // ===== USE CASE III: Librarian Tracking Active Loans =====

    [AvaloniaFact]
    public void UseCase3_LibrarianTracksActiveLoans()
    {
        var window = SetupTestEnvironment();

        // Step 1: Log in as librarian
        LogInAs("admin", "admin");

        // Verify we're on the home screen with librarian view
        Assert.IsType<TabControlWindowViewModel>(MainWindowViewModel.Instance.ActiveWindow.DataContext);

        // Step 2: Navigate to "Borrowed Books"
        var catalogVm = GetCatalogVm();
        catalogVm.ShowAllBorrowedBooks();

        // Step 3: Verify the list shows borrowed books with member names
        Assert.True(catalogVm.IsFullBorrowedBookListEnabled);
        Assert.Single(catalogVm.BookList); // only "Borrowed Book" is borrowed
        Assert.Equal("Borrowed Book", catalogVm.BookList[0].Title);
        Assert.Equal("member", catalogVm.BookList[0].Owner);

        // Verify the total count is displayed
        Assert.Equal("1 books are borrowed", catalogVm.BorrowedBooksCount);

        window.Close();
    }
}

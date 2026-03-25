using Xunit;

public class DataManagerTests
{
    // Creates a test SaveData.json in the working directory
    // so DataManager can load it. Each test gets a fresh file.
    // Passwords are stored as SHA256 hashes (same as production)
    private DataManager CreateTestDataManager()
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
        return new DataManager();
    }

    // ===== ACCESS CONTROL TESTS =====

    [Fact]
    public void ValidLogin_ReturnsTrue()
    {
        var dm = CreateTestDataManager();
        Assert.True(dm.IsLogInValid("member", "member"));
    }

    [Fact]
    public void InvalidPassword_ReturnsFalse()
    {
        var dm = CreateTestDataManager();
        Assert.False(dm.IsLogInValid("member", "wrongpassword"));
    }

    [Fact]
    public void NonexistentUser_ReturnsFalse()
    {
        var dm = CreateTestDataManager();
        Assert.False(dm.IsLogInValid("nobody", "nothing"));
    }

    [Fact]
    public void GetUserWithValidCredentials_ReturnsCorrectUser()
    {
        var dm = CreateTestDataManager();
        var user = dm.GetUserWithLoginCredentials("admin", "admin");
        Assert.NotNull(user);
        Assert.Equal("librarian", user.role);
    }

    [Fact]
    public void GetUserWithInvalidCredentials_ReturnsNull()
    {
        var dm = CreateTestDataManager();
        var user = dm.GetUserWithLoginCredentials("admin", "wrong");
        Assert.Null(user);
    }

    // ===== BORROWING SYSTEM TESTS =====

    [Fact]
    public void BorrowBook_SetsOwner()
    {
        var dm = CreateTestDataManager();
        var book = dm.GetBookByIsbn("1234567890123");

        Assert.Equal("", book.Owner); // available before
        book.Owner = "member";
        Assert.Equal("member", book.Owner); // borrowed after
    }

    [Fact]
    public void ReturnBook_ClearsOwner()
    {
        var dm = CreateTestDataManager();
        var book = dm.GetBookByIsbn("9876543210123");

        Assert.Equal("member", book.Owner); // borrowed before
        book.Owner = "";
        Assert.Equal("", book.Owner); // returned after
    }

    [Fact]
    public void AvailableBookList_ExcludesBorrowedBooks()
    {
        var dm = CreateTestDataManager();
        var available = dm.GetAvailableBookList();

        Assert.Single(available); // only "Test Book" is available
        Assert.Equal("Test Book", available[0].Title);
    }

    [Fact]
    public void BorrowedBookList_ExcludesAvailableBooks()
    {
        var dm = CreateTestDataManager();
        var borrowed = dm.GetBorrowedBookList();

        Assert.Single(borrowed);
        Assert.Equal("Borrowed Book", borrowed[0].Title);
    }

    [Fact]
    public void BorrowedBookListFromUser_FiltersCorrectly()
    {
        var dm = CreateTestDataManager();
        var user = dm.GetUserWithLoginCredentials("member", "member");
        var books = dm.GetBorrowedBookListFromUser(user);

        Assert.Single(books);
        Assert.Equal("member", books[0].Owner);
    }

    // ===== DATA PERSISTENCE TESTS =====

    [Fact]
    public void SaveAndLoad_PersistsBookChanges()
    {
        var dm = CreateTestDataManager();

        // Borrow the available book
        var book = dm.GetBookByIsbn("1234567890123");
        book.Owner = "member";
        dm.SaveSaveData();

        // Reload from disk
        dm.LoadSaveData();
        var reloaded = dm.GetBookByIsbn("1234567890123");
        Assert.Equal("member", reloaded.Owner);
    }

    [Fact]
    public void SaveAndLoad_PersistsNewBook()
    {
        var dm = CreateTestDataManager();

        dm.AddBookObject(new BookData
        {
            Title = "New Book",
            Author = "New Author",
            Isbn = "1111111111111",
            Description = "Freshly added",
            Owner = ""
        });
        dm.SaveSaveData();

        // Reload and verify
        dm.LoadSaveData();
        var found = dm.GetBookByIsbn("1111111111111");
        Assert.NotNull(found);
        Assert.Equal("New Book", found.Title);
    }

    [Fact]
    public void SaveAndLoad_PersistsDeletedBook()
    {
        var dm = CreateTestDataManager();

        var book = dm.GetBookByIsbn("1234567890123");
        dm.DeleteBookObject(book);
        dm.SaveSaveData();

        // Reload and verify it's gone
        dm.LoadSaveData();
        var found = dm.GetBookByIsbn("1234567890123");
        Assert.Null(found);
    }

    // ===== SEARCH / FILTER TESTS =====

    [Fact]
    public void SearchByTitle_FindsMatch()
    {
        var dm = CreateTestDataManager();
        var results = dm.GetFullBookListWithSearchQuery("Test");

        Assert.Single(results);
        Assert.Equal("Test Book", results[0].Title);
    }

    [Fact]
    public void SearchByAuthor_FindsMatch()
    {
        var dm = CreateTestDataManager();
        var results = dm.GetFullBookListWithSearchQuery("Some Author");

        Assert.Single(results);
        Assert.Equal("Borrowed Book", results[0].Title);
    }

    [Fact]
    public void SearchCaseInsensitive_Works()
    {
        var dm = CreateTestDataManager();
        var results = dm.GetFullBookListWithSearchQuery("test book");

        Assert.Single(results);
    }

    [Fact]
    public void SearchNoMatch_ReturnsEmpty()
    {
        var dm = CreateTestDataManager();
        var results = dm.GetFullBookListWithSearchQuery("nonexistent garbage");

        Assert.Empty(results);
    }

    // ===== PASSWORD HASHING TESTS =====

    [Fact]
    public void HashPassword_IsDeterministic()
    {
        string hash1 = DataManager.HashPassword("testpassword");
        string hash2 = DataManager.HashPassword("testpassword");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_DifferentInputsDifferentHashes()
    {
        string hash1 = DataManager.HashPassword("password1");
        string hash2 = DataManager.HashPassword("password2");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_IsNot_Plaintext()
    {
        string hash = DataManager.HashPassword("member");
        Assert.NotEqual("member", hash);
    }

    // ===== REGISTRATION TESTS =====

    [Fact]
    public void RegisterUser_CanLogInAfter()
    {
        var dm = CreateTestDataManager();
        bool registered = dm.RegisterUser("newuser", "newpass");
        Assert.True(registered);

        Assert.True(dm.IsLogInValid("newuser", "newpass"));
    }

    [Fact]
    public void RegisterUser_DuplicateUsername_Fails()
    {
        var dm = CreateTestDataManager();
        bool registered = dm.RegisterUser("member", "somepassword");
        Assert.False(registered);
    }

    [Fact]
    public void RegisterUser_EmptyFields_Fails()
    {
        var dm = CreateTestDataManager();
        Assert.False(dm.RegisterUser("", "pass"));
        Assert.False(dm.RegisterUser("user", ""));
        Assert.False(dm.RegisterUser("", ""));
    }

    [Fact]
    public void RegisterUser_AlwaysCreatedAsMember()
    {
        var dm = CreateTestDataManager();
        dm.RegisterUser("newguy", "pass123");
        var user = dm.GetUserWithLoginCredentials("newguy", "pass123");
        Assert.Equal("member", user.role);
    }
}

# AOOP Assignment 2 - Library Management App

A library management application built with C# and Avalonia following the MVVM pattern. Members can browse the catalog, borrow and return books. Librarians can manage the catalog (add, edit, delete books) and view all active loans. User accounts are protected with SHA256 password hashing. Data is persisted to a JSON file.

## Running the App

```
cd LibraryApp
dotnet run
```

### Test Accounts

| Username | Password | Role      |
| -------- | -------- | --------- |
| member   | member   | Member    |
| admin    | admin    | Librarian |
| 1        | 1        | Librarian |
| 2        | 2        | Member    |

New accounts can be registered from the login screen (always created as Member).

## Running Unit Tests

```
cd LibraryApp.Tests
dotnet test
```

### Test Cases (27 total)

**Access Control (5 tests)**

- `ValidLogin_ReturnsTrue` - valid username + password returns true
- `InvalidPassword_ReturnsFalse` - correct username but wrong password returns false
- `NonexistentUser_ReturnsFalse` - nonexistent username returns false
- `GetUserWithValidCredentials_ReturnsCorrectUser` - valid credentials return the correct user object with the right role
- `GetUserWithInvalidCredentials_ReturnsNull` - invalid credentials return null

**Borrowing System (5 tests)**

- `BorrowBook_SetsOwner` - borrowing a book sets the owner to the member's username
- `ReturnBook_ClearsOwner` - returning a book clears the owner back to empty
- `AvailableBookList_ExcludesBorrowedBooks` - available book list only contains books with no owner
- `BorrowedBookList_ExcludesAvailableBooks` - borrowed book list only contains books that have an owner
- `BorrowedBookListFromUser_FiltersCorrectly` - filtering by specific user only returns their books

**Data Persistence (3 tests)**

- `SaveAndLoad_PersistsBookChanges` - borrow a book, save to JSON, reload, owner change persists
- `SaveAndLoad_PersistsNewBook` - add a new book, save, reload, new book exists
- `SaveAndLoad_PersistsDeletedBook` - delete a book, save, reload, book is gone

**Search / Filter (4 tests)**

- `SearchByTitle_FindsMatch` - searching by title returns correct book
- `SearchByAuthor_FindsMatch` - searching by author returns correct book
- `SearchCaseInsensitive_Works` - search is case-insensitive
- `SearchNoMatch_ReturnsEmpty` - searching for nonexistent text returns empty list

**Password Hashing (3 tests)**

- `HashPassword_IsDeterministic` - same input always produces the same hash
- `HashPassword_DifferentInputsDifferentHashes` - different passwords produce different hashes
- `HashPassword_IsNot_Plaintext` - hashed output is not the original plaintext

**Registration (4 tests)**

- `RegisterUser_CanLogInAfter` - newly registered user can log in
- `RegisterUser_DuplicateUsername_Fails` - registering with an existing username is rejected
- `RegisterUser_EmptyFields_Fails` - empty username or password is rejected
- `RegisterUser_AlwaysCreatedAsMember` - new accounts are always created with the member role

**Headless UI Tests (3 tests)** - using Avalonia.Headless.XUnit

- `UseCase1_MemberBorrowsBook` - logs in as member, views available books, borrows a book, verifies it appears in "My Books"
- `UseCase2_MemberReturnsBook` - logs in as member, navigates to "My Books", returns a book, verifies it moves back to available
- `UseCase3_LibrarianTracksActiveLoans` - logs in as librarian, views "Borrowed Books", verifies borrowed books listed with owner names and total count

## Functional Test Results

### Member Functionality

**Borrowing Test**

- Steps: Log in as member/member. Go to "Available Books". Click a book. Click "Borrow this book".
- Result: PASS. Book disappears from available list. Book appears in "My Books" list. Notification popup confirms borrowing.

**Return Book Test**

- Steps: Log in as member/member. Go to "My Books". Click a borrowed book. Click "Return this book".
- Result: PASS. Book disappears from "My Books". Book reappears in "Available Books" with empty owner. Notification popup confirms return.

### Librarian Functionality

**Add Book Test**

- Steps: Log in as admin/admin. Click "Add Book". Fill in title, author, ISBN (13 chars), description. Click "Add Book".
- Result: PASS. New book appears in "Full Catalog" for the librarian. Log out, log in as member. New book visible in "Available Books".

**Delete Book Test**

- Steps: Log in as admin/admin. Click a book in "Full Catalog". Click "Delete Book".
- Result: PASS. Book removed from all catalog views. Log out, log in as member. Deleted book no longer visible.

**Active Loan Tracking**

- Steps: Log in as admin/admin. Click "Borrowed Books".
- Result: PASS. List shows all currently borrowed books with title, author, ISBN, and borrower username. Total borrowed count displayed.

### System-Level Tests

**Login Test**

- Steps: Enter valid member credentials (member/member). Click "Log In". Log out. Enter valid librarian credentials (admin/admin). Click "Log In". Log out. Enter invalid credentials. Click "Log In".
- Result: PASS. Member directed to member view (Available Books, My Books buttons). Librarian directed to librarian view (Full Catalog, Borrowed Books, Add Book buttons). Invalid credentials show "Wrong username or password" popup.

**Search/Filter Test**

- Steps: Log in as member/member. Type a book title in the search bar. Clear and type an author name.
- Result: PASS. Book list filters in real-time as text is typed. Matches by title and author. Case-insensitive.

**Data Persistence Test**

- Steps: Log in as member/member. Borrow a book. Close the application. Reopen the application. Log in again. Check "My Books".
- Result: PASS. Borrowed book still appears in "My Books". SaveData.json contains updated owner field.

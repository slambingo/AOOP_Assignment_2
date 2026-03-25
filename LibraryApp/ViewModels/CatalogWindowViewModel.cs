using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;
using System.Collections.ObjectModel;
using Avalonia.Diagnostics.Screenshots;
using System.Runtime.Versioning;
using Avalonia.Controls;


namespace LibraryApp.ViewModels;
public partial class CatalogWindowViewModel : ViewModelBase
{
    private ObservableCollection<BookData> bookList;

    public ObservableCollection<BookData> BookList
    {
        get => bookList;
        set => SetProperty(ref bookList, value);
    }

    [ObservableProperty]
    public bool isBorrowButtonEnabled;

    [ObservableProperty]
    public bool isReturnButtonEnabled;

    [ObservableProperty]
    public BookData selectedBook;

    [ObservableProperty]
    public bool bookDetailPopupEnabled;


    [ObservableProperty]
    public bool addNewBookPopupEnabled;

    [ObservableProperty]
    public BookData newBook;

    [RelayCommand]
    public void ConfirmAddNewBook()
    {
        //no validation for the book
        if(!DataManager.Instance.IsBookDetailsValid(NewBook)) return;
        NewBook.owner = "";
        DataManager.Instance.AddBookObject(NewBook);
        DataManager.Instance.SaveSaveData();
        CloseAddBookPopup();
        ChangeCatalogView(CatalogView.ALL_BOOKS); //needed to force update book list
    }

    [RelayCommand]
    public void AddNewBook()
    {
        AddNewBookPopupEnabled = false; 
        NewBook = new BookData();
        AddNewBookPopupEnabled = true; 
    }

    public void CloseAddBookPopup()
    {
        AddNewBookPopupEnabled = false; 
    }

    [ObservableProperty]
    public string borrowedBooksCount;

    [ObservableProperty]
    private string searchQueryInput;

    partial void OnSearchQueryInputChanged(string value)
    {
        UpdateCatalogViewBasedOnSearchQuery(value);  // Use 'value' param for safety
    }

    [ObservableProperty]
    private bool isUserRoleMember;

    [ObservableProperty]
    private bool isFullBorrowedBookListEnabled;


    private CatalogView currentCatalogView;

    public CatalogWindowViewModel()
    {
        if(MainWindowViewModel.Instance.IsLoggedInUserMember())
        {
            ChangeCatalogView(CatalogView.AVAILABLE_BOOKS);
            IsUserRoleMember = true;
        }
        else
        {
           ChangeCatalogView(CatalogView.ALL_BOOKS); 
           IsUserRoleMember = false;
        }
        
    }

    [RelayCommand]
    public void BookClicked(string isbn)
    {
        BookData book = DataManager.Instance.GetBookByIsbn(isbn);
        ShowSelectedBookPopup(book);
    }

    [RelayCommand]
    public void BorrowBook()
    {
        SelectedBook.owner = MainWindowViewModel.Instance.GetLoggedInUserProfile().username;
        CloseSelectedBookPopup();
        MainWindowViewModel.Instance.ShowNotificationPopup("Book successfully borrowed.");
        DataManager.Instance.SaveSaveData();
        ChangeCatalogView(CatalogView.AVAILABLE_BOOKS); //needed to force update book list

        
    }

    [RelayCommand]
    public void ReturnBook()
    {
        SelectedBook.owner = "";
        CloseSelectedBookPopup();
        MainWindowViewModel.Instance.ShowNotificationPopup("Book successfully returned.");
        DataManager.Instance.SaveSaveData();
        ChangeCatalogView(CatalogView.MY_BOOKS); //needed to force update book list
        
    }

    [RelayCommand]
    public void ShowAvailableBooks()
    {
        ChangeCatalogView(CatalogView.AVAILABLE_BOOKS);
    }

    [RelayCommand]
    public void ShowMyBooks()
    {
        ChangeCatalogView(CatalogView.MY_BOOKS);
    }

    [RelayCommand]
    public void ShowAllBooks()
    {
        ChangeCatalogView(CatalogView.ALL_BOOKS);
    }


    [RelayCommand]
    public void ShowAllBorrowedBooks()
    {
        BorrowedBooksCount = DataManager.Instance.GetBorrowedBookList().Count + " books are borrowed";
        ChangeCatalogView(CatalogView.BORROWED_BOOKS);
    }

    [RelayCommand]
    public void SaveEditBookChanges()
    {
        if(!DataManager.Instance.IsBookDetailsValid(SelectedBook)) return;
        MainWindowViewModel.Instance.ShowNotificationPopup("Book changes successfully saved.");
        DataManager.Instance.SaveSaveData();
        ChangeCatalogView(CatalogView.ALL_BOOKS); 
    }

    [RelayCommand]
    public void DeleteBook()
    {
        CloseSelectedBookPopup();
        MainWindowViewModel.Instance.ShowNotificationPopup("Book has been deleted.");
        DataManager.Instance.DeleteBookObject(selectedBook);
        DataManager.Instance.SaveSaveData();
        ChangeCatalogView(CatalogView.ALL_BOOKS); //needed to force update book list
    }



    private void UpdateCatalogViewBasedOnSearchQuery(string searchQuery)
    {
        switch(currentCatalogView)
        {
            case CatalogView.AVAILABLE_BOOKS:
                BookList = DataManager.Instance.GetAvailableBookListWithSearchQuery(searchQuery);
                break;
            case CatalogView.MY_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookListFromUserWithSearchQuery(MainWindowViewModel.Instance.GetLoggedInUserProfile(), searchQuery);
                break;
            case CatalogView.ALL_BOOKS:
                BookList = DataManager.Instance.GetFullBookListWithSearchQuery(searchQuery);
                break;
            default:
                break;
        }
    }

    private void ChangeCatalogView(CatalogView catalogView)
    {
        
        IsBorrowButtonEnabled = false;
        IsReturnButtonEnabled = false;
        IsFullBorrowedBookListEnabled = false;

        switch(catalogView)
        {
            case CatalogView.AVAILABLE_BOOKS:
                BookList = DataManager.Instance.GetAvailableBookList();
                IsBorrowButtonEnabled = true;
                break;

            case CatalogView.MY_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookListFromUser(MainWindowViewModel.Instance.GetLoggedInUserProfile());
                IsReturnButtonEnabled = true;
                break;

            case CatalogView.ALL_BOOKS:
                BookList = DataManager.Instance.GetFullBookList();
                break;

            case CatalogView.BORROWED_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookList();
                IsFullBorrowedBookListEnabled = true;
                break;
                
            default:
                break;
        }

        currentCatalogView = catalogView;
        
    }

    private enum CatalogView
    {
        AVAILABLE_BOOKS,
        MY_BOOKS,
        ALL_BOOKS,
        BORROWED_BOOKS
    }

    
    public void CloseSelectedBookPopup()
    {
        BookDetailPopupEnabled = false;
    }

    public void ShowSelectedBookPopup(BookData selectedBookInput)
    {
        BookDetailPopupEnabled = false;
        SelectedBook = selectedBookInput;
        BookDetailPopupEnabled = true;
    }



}

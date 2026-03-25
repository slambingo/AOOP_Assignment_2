using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;
using System.Collections.ObjectModel;
using Avalonia.Diagnostics.Screenshots;
using System.Runtime.Versioning;


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
    private string searchQueryInput;

    [ObservableProperty]
    private bool isMemberTabsEnabled;

    partial void OnSearchQueryInputChanged(string value)
    {
        UpdateCatalogViewBasedOnSearchQuery(value);  // Use 'value' param for safety
    }

    private CatalogView currentCatalogView;

    public CatalogWindowViewModel()
    {
        if(MainWindowViewModel.Instance.IsLoggedInUserMember())
        {
            ChangeCatalogView(CatalogView.AVAILABLE_BOOKS);
            isMemberTabsEnabled = true;
        }
        else
        {
           ChangeCatalogView(CatalogView.ALL_BOOKS); 
           isMemberTabsEnabled = false;
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

            default:
                break;
        }
    }

    private void ChangeCatalogView(CatalogView catalogView)
    {
        

        switch(catalogView)
        {
            case CatalogView.AVAILABLE_BOOKS:
                BookList = DataManager.Instance.GetAvailableBookList();
                IsBorrowButtonEnabled = true;
                IsReturnButtonEnabled = false;
                //UpdateCatalogViewBasedOnSearchQuery(SearchQueryInput);
                break;
            case CatalogView.MY_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookListFromUser(MainWindowViewModel.Instance.GetLoggedInUserProfile());
                IsBorrowButtonEnabled = false;
                IsReturnButtonEnabled = true;
                //UpdateCatalogViewBasedOnSearchQuery(SearchQueryInput);
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
        ALL_BOOKS
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

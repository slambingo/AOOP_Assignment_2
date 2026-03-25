using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;
using System.Collections.ObjectModel;
using Avalonia.Diagnostics.Screenshots;


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

    private CatalogView currentCatalogView;

    public CatalogWindowViewModel()
    {
        ChangeCatalogView(CatalogView.AVAILABLE_BOOKS);
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
    public void SearchInputChanged()
    {
        
    }

    private void UpdateCatalogViewBasedOnSearchQuery(string searchQuery)
    {
        switch(currentCatalogView)
        {
            case CatalogView.AVAILABLE_BOOKS:
                BookList = DataManager.Instance.GetAvailableBookList();
                break;
            case CatalogView.MY_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookListFromUser(MainWindowViewModel.Instance.GetLoggedInUserProfile());
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
                break;
            case CatalogView.MY_BOOKS:
                BookList = DataManager.Instance.GetBorrowedBookListFromUser(MainWindowViewModel.Instance.GetLoggedInUserProfile());
                IsBorrowButtonEnabled = false;
                IsReturnButtonEnabled = true;
                break;

            default:
                break;
        }

        currentCatalogView = catalogView;
    }

    private enum CatalogView
    {
        AVAILABLE_BOOKS,
        MY_BOOKS
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

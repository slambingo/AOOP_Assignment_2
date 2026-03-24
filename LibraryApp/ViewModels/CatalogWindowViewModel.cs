using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;
using System.Collections.ObjectModel;


namespace LibraryApp.ViewModels;
public partial class CatalogWindowViewModel : ViewModelBase
{
    private ObservableCollection<BookData> bookList;

    public ObservableCollection<BookData> BookList
    {
        get => bookList;
        set => SetProperty(ref bookList, value);
    }


    public CatalogWindowViewModel()
    {
        BookList = DataManager.Instance.GetBookList();
    }

}

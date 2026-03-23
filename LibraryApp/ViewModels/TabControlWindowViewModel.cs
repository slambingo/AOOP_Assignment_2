using System;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Views;


namespace LibraryApp.ViewModels;

public partial class TabControlWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public UserControl tab1Window;

    [ObservableProperty]
    public UserControl tab2Window;

    public TabControlWindowViewModel()
    {
        tab1Window = new CatalogWindow() {DataContext = new CatalogWindowViewModel()};
        tab2Window = new ProfileWindow() {DataContext = new ProfileWindowViewModel()};
    }
}

using System;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Views;

namespace LibraryApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //[ObservableProperty]
    //ublic UserControl activeWindow;

    public MainWindowViewModel()
    {
        //ActiveWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};

    }
}

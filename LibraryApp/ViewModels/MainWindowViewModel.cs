using System;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Views;

namespace LibraryApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public UserControl activeWindow;


    [ObservableProperty]
    public bool notificationPopupEnabled;

    [ObservableProperty]
    public string notificationMessage;

    private static MainWindowViewModel instance;
    public static MainWindowViewModel Instance => instance;

    public MainWindowViewModel()
    {
        
        ActiveWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};
       

        instance = this;
    }

    public void ShowNotificationPopup(string msg)
    {
        NotificationPopupEnabled = false;
        NotificationMessage = msg;
        NotificationPopupEnabled = true;
    }
}

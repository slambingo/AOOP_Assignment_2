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
    public UserControl logInWindow, tabControlWindow;


    [ObservableProperty]
    public bool notificationPopupEnabled;

    [ObservableProperty]
    public string notificationMessage;

    private static MainWindowViewModel instance;
    public static MainWindowViewModel Instance => instance;

    

    public MainWindowViewModel()
    {
        logInWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};
        tabControlWindow = new TabControlWindow() {DataContext = new TabControlWindowViewModel()};
        

        ChangeActiveWindow(Window.LOGIN);

        instance = this;
    }



    public void ChangeActiveWindow(Window window)
    {
        switch(window)
        {
            case Window.LOGIN:
                ActiveWindow = logInWindow;
                break;
            case Window.HOME:
                ActiveWindow = tabControlWindow;
                break;
            default:
                ActiveWindow = logInWindow;
                break;
        }
    }

    public void ShowNotificationPopup(string msg)
    {
        NotificationPopupEnabled = false;
        NotificationMessage = msg;
        NotificationPopupEnabled = true;
    }
}

public enum Window
{
    LOGIN,
    HOME
}

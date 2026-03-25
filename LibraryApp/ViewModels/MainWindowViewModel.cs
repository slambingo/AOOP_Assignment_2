using System;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Views;

namespace LibraryApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static MainWindowViewModel instance;
    public static MainWindowViewModel Instance => instance;

    [ObservableProperty]
    public UserControl activeWindow;
    //public UserControl logInWindow, tabControlWindow;
    
    [ObservableProperty]
    public bool notificationPopupEnabled;

    [ObservableProperty]
    public string notificationMessage;


    public UserData loggedInUserProfile;
    

    public MainWindowViewModel()
    {

        //logInWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};
        //tabControlWindow = new TabControlWindow() {DataContext = new TabControlWindowViewModel()};
        
        ChangeActiveWindow(Window.LOGIN);

        instance = this;
    }


    //these shouldn't be here!
    public UserData GetLoggedInUserProfile()
    {
        return loggedInUserProfile;
    }

    public bool IsLoggedInUserMember()
    {
        if(loggedInUserProfile.role == "member")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetLoggedInUserProfile(UserData loggedInUserProfileInput)
    {
        loggedInUserProfile = loggedInUserProfileInput;
    }

    public void ClearLoggedInUserProfile()
    {
        loggedInUserProfile = null;
    }

    public void ChangeActiveWindow(Window window)
    {
        switch(window)
        {
            case Window.LOGIN:
                //ActiveWindow = logInWindow;
                ActiveWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};
                break;
            case Window.HOME:
                //ActiveWindow = tabControlWindow;
                ActiveWindow = new TabControlWindow() {DataContext = new TabControlWindowViewModel()};
                break;
            default:
                //ActiveWindow = logInWindow;
                ActiveWindow = new LogInWindow() {DataContext = new LogInWindowViewModel()};
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

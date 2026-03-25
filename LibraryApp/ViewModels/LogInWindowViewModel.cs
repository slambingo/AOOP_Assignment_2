using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;


namespace LibraryApp.ViewModels;

public partial class LogInWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public string logInFeedback = "hello";

    [ObservableProperty]
    public string username;

    [ObservableProperty]
    public string password;



   


    public LogInWindowViewModel()
    {
        
        new DataManager();

    }
    
    [RelayCommand]
    public void LogIn()
    {

        bool isLogInValid = DataManager.Instance.IsLogInValid(Username, Password);


        if(isLogInValid)
        {
            //commence to next view
            UserData user = DataManager.Instance.GetUserWithLoginCredentials(Username, Password);
            MainWindowViewModel.Instance.SetLoggedInUserProfile(user);
            MainWindowViewModel.Instance.ShowNotificationPopup($"Logged in as {Username}");
            MainWindowViewModel.Instance.ChangeActiveWindow(Window.HOME);

        }
        else
        {
            MainWindowViewModel.Instance.ShowNotificationPopup("Wrong username or password");
        }
    }

    [RelayCommand]
    public void Register()
    {
        if(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            MainWindowViewModel.Instance.ShowNotificationPopup("Username and password cannot be empty");
            return;
        }

        bool success = DataManager.Instance.RegisterUser(Username, Password);
        if(success)
        {
            MainWindowViewModel.Instance.ShowNotificationPopup($"Account '{Username}' registered successfully");
        }
        else
        {
            MainWindowViewModel.Instance.ShowNotificationPopup("Username is already taken");
        }
    }
}

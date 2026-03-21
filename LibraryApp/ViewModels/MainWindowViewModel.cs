using System;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LibraryApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{




    [ObservableProperty]
    public string logInFeedback = "hello";

    [ObservableProperty]
    public string username;

    [ObservableProperty]
    public string password;



    private DataManager dataManager;
    public MainWindowViewModel()
    {
        dataManager = new DataManager();
    }
    
    [RelayCommand]
    public void LogIn()
    {
        Console.WriteLine("Login");

        bool isLogInValid = dataManager.IsLogInValid(Username, Password);

        if(isLogInValid)
        {
            //commence to next view
        }
        else
        {
            LogInFeedback = "Wrong username or password";
        }
    }
}

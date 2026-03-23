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



   

    private DataManager dataManager;
    public LogInWindowViewModel()
    {
        
        dataManager = new DataManager();  // Exception caught here

    }
    
    [RelayCommand]
    public void LogIn()
    {

        bool isLogInValid = dataManager.IsLogInValid(Username, Password);
        

        if(isLogInValid)
        {
            //commence to next view
            MainWindowViewModel.Instance.ShowNotificationPopup("Loggedin");
        }
        else
        {
            MainWindowViewModel.Instance.ShowNotificationPopup("Wrong username or password");
        }
    }
}

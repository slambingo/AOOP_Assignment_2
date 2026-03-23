using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;


namespace LibraryApp.ViewModels;
public partial class ProfileWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string username = "null";
    [ObservableProperty]
    private string role = "null";

    public ProfileWindowViewModel()
    {
        UserData user = MainWindowViewModel.Instance.GetLoggedInUserProfile();
        
        if(user == null) return;

        Username = user.username;
        Role = user.role;
    }

    [RelayCommand]
    public void LogOut()
    {
        MainWindowViewModel.Instance.ShowNotificationPopup("Logged out!");
        MainWindowViewModel.Instance.ChangeActiveWindow(Window.LOGIN);
        MainWindowViewModel.Instance.ClearLoggedInUserProfile();
    }
}

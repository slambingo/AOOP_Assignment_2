using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System;
using Avalonia.Controls;
using LibraryApp;
using System.Collections.ObjectModel;

public class DataManager
{
    private static DataManager instance;
    public static DataManager Instance => instance;
    private SaveData saveData;

    public DataManager() 
    {
        

        LoadSaveData();
        PrintUsers();
        PrintBooks();

        instance = this;
        
    }

    public ObservableCollection<BookData> GetBookList()
    {
        return saveData.catalog;
    }

    public UserData GetUser(string usernameInput, string passwordInput)
    {
        for(int i = 0; i < saveData.users.Count; i++)
        {
            if(saveData.users[i].username == usernameInput)
            {
                if(saveData.users[i].password == passwordInput)
                {
                    return saveData.users[i];
                }
            }
        }

        return null;
    }

    public void LoadSaveData() //written by AI
    {
        string jsonText = File.ReadAllText("SaveData.json");
        
        // Deserialize to ROOT class (contains both arrays)
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //SO THE CAPITAL ex.: Title get/setters can work
        saveData = JsonSerializer.Deserialize<SaveData>(jsonText, options)!;
    }

    public void SaveSaveData()
    {
        
    }

    public bool IsLogInValid(string usernameInput, string passwordInput)
    {
        
        for(int i = 0; i < saveData.users.Count; i++)
        {
            if(saveData.users[i].username == usernameInput)
            {
                if(saveData.users[i].password == passwordInput)
                {
                    return true;
                }
            }
        }
        
        return false;  
    }



    public void PrintUsers()
    {
        // Access users array
        foreach(var user in saveData.users)
        {
            Console.WriteLine(user); 
        }
    }

    public void PrintBooks()
    {
        // Access catalog array  
        foreach(var book in saveData.catalog)
        {
            Console.WriteLine(book);
        }
    }

}


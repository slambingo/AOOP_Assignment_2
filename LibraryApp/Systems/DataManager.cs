using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System;
using Avalonia.Controls;

public class DataManager
{
    private SaveData saveData;

    public DataManager() 
    {
        LoadSaveData();
        PrintUsers();
        PrintBooks();
    }



    public void LoadSaveData() //written by AI
    {
        string jsonText = File.ReadAllText("SaveData.json");
        
        // Deserialize to ROOT class (contains both arrays)
        saveData = JsonSerializer.Deserialize<SaveData>(jsonText)!;
    }

    public void SaveSaveData()
    {
        
    }

    public bool IsLogInValid(string _username, string _password)
    {
        
        for(int i = 0; i < saveData.users.Count; i++)
        {
            if(saveData.users[i].username == _username)
            {
                if(saveData.users[i].password == _password)
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


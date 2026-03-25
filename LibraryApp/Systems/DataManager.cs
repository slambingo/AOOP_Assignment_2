using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System;
using Avalonia.Controls;
using LibraryApp;
using System.Collections.ObjectModel;
using System.Linq;

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

    public ObservableCollection<BookData> GetBookList(bool getBorrowed, bool getAvailable, bool specificOwner, UserData user, bool specificSearchQuery, string searchQuery)
    {
        ObservableCollection<BookData> bookList = new ObservableCollection<BookData>();
        foreach(var book in saveData.catalog)
        {
            
            if(getAvailable && book.owner == string.Empty) 
            {
                bookList.Add(book);
            }
            else if(getBorrowed && book.owner != string.Empty)
            {
                if(specificOwner)
                {
                    if(user.username == book.owner)
                    {
                        bookList.Add(book);
                    }
                }
                else
                {
                    bookList.Add(book);
                }
                
            }   
        }

        return bookList;
        //return saveData.catalog;
    }

    public ObservableCollection<BookData> GetFullBookList()
    {
        return GetBookList(true, true, false, null, false, string.Empty);
    }

    public ObservableCollection<BookData> GetAvailableBookList()
    {
        return GetBookList(false, true, false, null, false, string.Empty);
    }

    public ObservableCollection<BookData> GetBorrowedBookList()
    {
        return GetBookList(true, false, false, null, false, string.Empty);
    }

    public ObservableCollection<BookData> GetFullBookListWithSearchQuery(string searchQuery)
    {
        return GetBookList(true, true, false, null, true, searchQuery);
    }

    public ObservableCollection<BookData> GetAvailableBookListWithSearchQuery(string searchQuery)
    {
        return GetBookList(false, true, false, null, true, searchQuery);
    }

    public ObservableCollection<BookData> GetBorrowedBookListWithSearchQuery(string searchQuery)
    {
        return GetBookList(true, false, false, null, true, searchQuery);
    }

    public ObservableCollection<BookData> GetBorrowedBookListFromUser(UserData user)
    {
        return GetBookList(true, false, true, user, false, string.Empty);
    }

    public BookData GetBookByIsbn(string isbnInput)
    {
        foreach(var book in saveData.catalog)
        {
            if(book.isbn == isbnInput)
            {
                return book;
            }
        }

        return null;
    }

    public UserData GetUserWithLoginCredentials(string usernameInput, string passwordInput)
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
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true,
            WriteIndented = true  // Pretty-print JSON (optional, makes file readable)
        };
        
        string jsonText = JsonSerializer.Serialize(saveData, options);
        File.WriteAllText("SaveData.json", jsonText);
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


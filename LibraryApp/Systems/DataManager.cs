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

    public ObservableCollection<BookData> GetBookList(BookFilter filter)
    {
        ObservableCollection<BookData> bookList = new ObservableCollection<BookData>();
        foreach(var book in saveData.catalog)
        {
            
            if(filter.getAvailable && book.owner == string.Empty) 
            {
                bookList.Add(book);
            }
            else if(filter.getBorrowed && book.owner != string.Empty)
            {
                if(filter.getSpecificOwner)
                {
                    if(filter.owner.username == book.owner)
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


        //remove ones that shouldnt come up from the search query
        if(filter.useSearchQuery && filter.searchQuery != string.Empty)
        {
            var searchQuery = filter.searchQuery.ToLower();
            for (int i = bookList.Count - 1; i >= 0; i--)
            {
                var book = bookList[i];
                if (!book.title.ToLower().Contains(searchQuery) && !book.author.ToLower().Contains(searchQuery))
                {
                    bookList.RemoveAt(i);
                }
            }
        }

        return bookList;
    }


    public ObservableCollection<BookData> GetFullBookList()
    {
        return GetBookList(new BookFilter{ getAvailable=true, getBorrowed=true});
    }

    public ObservableCollection<BookData> GetAvailableBookList()
    {
        return GetBookList(new BookFilter{ getAvailable=true});
    }

    public ObservableCollection<BookData> GetBorrowedBookList()
    {
        return GetBookList(new BookFilter{ getBorrowed=true});
    }

    public ObservableCollection<BookData> GetFullBookListWithSearchQuery(string searchQueryInput)
    {
        return GetBookList(new BookFilter{ getAvailable=true, getBorrowed=true, useSearchQuery=true, searchQuery=searchQueryInput});
    }

    public ObservableCollection<BookData> GetAvailableBookListWithSearchQuery(string searchQueryInput)
    {
        return GetBookList(new BookFilter{ getAvailable=true, useSearchQuery=true, searchQuery=searchQueryInput});
    }

    public ObservableCollection<BookData> GetBorrowedBookListWithSearchQuery(string searchQueryInput)
    {
        return GetBookList(new BookFilter{ getBorrowed=true, useSearchQuery=true, searchQuery=searchQueryInput});
    }

    public ObservableCollection<BookData> GetBorrowedBookListFromUserWithSearchQuery(UserData user, string searchQueryInput)
    {
        return GetBookList(new BookFilter{ getBorrowed=true, getSpecificOwner=true, owner=user, useSearchQuery=true, searchQuery=searchQueryInput});
    }

    public ObservableCollection<BookData> GetBorrowedBookListFromUser(UserData user)
    {
        return GetBookList(new BookFilter{ getBorrowed=true, getSpecificOwner=true, owner=user});
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


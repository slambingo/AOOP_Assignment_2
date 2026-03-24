using CommunityToolkit.Mvvm.ComponentModel;

public class BookData : ObservableObject
{
    public string title;
    public string author;
    public string isbn;
    public string description;
    public string owner;

    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    public string Author
    {
        get => author;
        set => SetProperty(ref author, value);
    }

    public string Isbn
    {
        get => isbn;
        set => SetProperty(ref isbn, value);
    }
    public string Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public string Owner
    {
        get => owner;
        set => SetProperty(ref owner, value);
    }


    public override string ToString()
    {
        return $"{title} by {author} (owner: {owner})";
    }
}
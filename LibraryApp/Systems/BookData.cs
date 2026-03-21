public class BookData  
{
    public string title {get; set;} = string.Empty;
    public string author {get; set;} = string.Empty;
    public string owner {get; set;} = string.Empty;

    public override string ToString()
    {
        return $"{title} by {author} (owner: {owner})";
    }
}
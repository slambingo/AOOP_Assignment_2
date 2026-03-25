public class BookFilter
{
    public bool getAvailable { get; set; } = false;
    public bool getBorrowed { get; set; } = false;
    public bool getSpecificOwner { get; set; } = false;
    public UserData? owner { get; set; }      // Null = all borrowed books
    public bool useSearchQuery { get; set; } = false;
    public string? searchQuery { get; set; } // Null/empty = no search
}
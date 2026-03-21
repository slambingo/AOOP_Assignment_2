using System.Collections.Generic;

public class SaveData  // ROOT class - matches entire JSON
{
    public List<UserData> users { get; set; } = new();
    public List<BookData> catalog { get; set; } = new();
}
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SaveData  // ROOT class - matches entire JSON
{
    public List<UserData> users { get; set; } = new();
    public ObservableCollection<BookData> catalog { get; set; } = new();
}
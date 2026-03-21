public class UserData
{
    public string username {get; set;} = string.Empty;
    public string password {get; set;} = string.Empty;
    public string role {get; set;} = string.Empty;

    public override string ToString()
    {
        return $"{username} {password} {role}";
    }
}
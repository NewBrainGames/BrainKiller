[System.Serializable]
public class LoggedInUser
{

    public string userId;
    public string nickName;
    public string email;
    public LoggedInUser(string userId, string nickName,string email)
    {

        this.userId = userId;
        this.nickName = nickName;
        this.email = email;


    }

}

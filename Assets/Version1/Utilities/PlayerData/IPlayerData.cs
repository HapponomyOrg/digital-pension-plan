namespace Version1.Utilities.PlayerData
{
    public interface IPlayerData
    {
        string PlayerName { get; set; }
        int Age { get; set; }
        int Gender { get; set; }
        int LobbyID { get; set; }
    }
}

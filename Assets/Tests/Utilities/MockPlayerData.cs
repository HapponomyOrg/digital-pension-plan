using Version1.PlayerData;

namespace Tests.Utilities
{
    public class MockPlayerData : IPlayerData
    {
        public string PlayerName { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public int LobbyID { get; set; }
        public string RequestID { get; set; }
    }
}

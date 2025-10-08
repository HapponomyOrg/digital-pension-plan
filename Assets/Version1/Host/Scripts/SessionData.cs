using UnityEngine;

namespace Version1.Host.Scripts
{
    public class SessionData
    {
        private static SessionData _instance;

        public static SessionData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionData();
                }

                return _instance;
            }
        }

        public string HostName { get; set; }
        public int LobbyCode { get; set; }
        public MoneySystems CurrentMoneySystem { get; set; }
        public bool InbalanceMode { get; set; }
        public int Seed { get; set; }
        public int RoundDuration { get; set; }

        private SessionData()
        {
            HostName = "";
            LobbyCode = Random.Range(100000000, 999999999);
            CurrentMoneySystem = MoneySystems.Sustainable;
            InbalanceMode = false;
            Seed = 0;
            RoundDuration = 4;
        }

        /// <summary>
        /// Resets values to default values.
        /// <para>
        /// The following session values are reset to their desfault values:
        /// </para>
        /// </summary>
        /// <param name="HostName"> ""</param>
        /// <param name="LobbyCode"> 0</param>
        /// <param name="InterestMode"> New lobby code</param>
        /// <param name="InbalanceMode"> true</param>
        /// <param name="Seed"> 0</param>
        /// <param name="RoundDuration"> 4 minutes</param>
        /// <param name="hard"></param>
        public void Reset(bool hard)
        {
            if (hard)
            {
                LobbyCode = Random.Range(100000000, 999999999);
                HostName = "";
            }

            CurrentMoneySystem = MoneySystems.Sustainable;
            InbalanceMode = false;
            Seed = 0;
            RoundDuration = 4;
        }
    }
}

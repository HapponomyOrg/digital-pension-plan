using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Version1.Utilities.GameModes
{
    public enum PhaseStatus
    {
        [Description("Not Started")]
        NotStarted,

        [Description("Ongoing")]
        Ongoing,

        [Description("Finished")]
        Finished
    }

    [System.Serializable]
    public class GamePhase
    {
        public string phaseName;
        public float duration;
        public string sceneName;
        public PhaseStatus status;
    }

    [CreateAssetMenu(fileName = "New Game Mode", menuName = "Game/Game Mode")]
    public class GameModeSo : ScriptableObject
    {
        public MoneySystems modeName;
        public bool moneyImbalance;
        public List<GamePhase> phases;
        public string description;

        public int TotalPhases => phases.Count;

        public GamePhase GetPhase(int index)
        {
            if (index >= 0 && index < phases.Count)
                return phases[index];
            return null;
        }
        public GamePhase GetNextPhase(int index)
        {
            index += 1;

            if (index >= 0 && index < phases.Count)
                return phases[index ];
            return null;
        }

        public List<GamePhase> GetPhases()
        {
            return phases;
        }
    }
}

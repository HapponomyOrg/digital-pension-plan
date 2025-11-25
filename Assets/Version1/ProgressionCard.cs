using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Version1.Utilities.GameModes;

namespace Version1
{
    public class ProgressionCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameTMPText;
        [SerializeField] private TMP_Text statusTMPText;

        private string phaseName;
        private PhaseStatus status;

        public void SetPhaseName(string newName)
        {
            phaseName = newName;
            nameTMPText.text = newName;
        }

        public string GetPhaseName()
        {
            return phaseName;

        }

        public PhaseStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(PhaseStatus newStatus)
        {
            status = newStatus;
            statusTMPText.text = newStatus.Description().ToString();
        }

    }
}

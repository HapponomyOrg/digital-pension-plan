using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Version1.Phases.DonatePoints.scripts
{
    public class ToasterScript : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] public TMP_Text toasterText;
        
        public void CloseToaster()
        {
            Destroy(this.gameObject);
        }
    }
}

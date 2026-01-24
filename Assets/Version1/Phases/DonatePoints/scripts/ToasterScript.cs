using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Version1.Phases.DonatePoints.scripts
{
    public class ToasterScript : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] public TMP_Text toasterText;
        [SerializeField] private float autoDestroyTime = 30f;

        private void Start()
        {
            // Automatically destroy after 30 seconds
            Destroy(gameObject, autoDestroyTime);
        }

        public void CloseToaster()
        {
            Destroy(gameObject);
        }
    }
}

using TMPro;
using UnityEngine;

namespace Version1.Loading
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Transform planet;
        [SerializeField] private TMP_Text loadingMessage;

        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private bool rotateRight = true;

        private void Update()
        {
            if (rotateRight)
                planet.transform.eulerAngles -= Vector3.forward * (rotationSpeed * Time.deltaTime);
            else
                planet.transform.eulerAngles += Vector3.forward * (rotationSpeed * Time.deltaTime);
        }

        public void SetLoadingMessage(string message)
        {
            loadingMessage.text = message;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Version1.Phases;
using TMPro;
using UnityEngine;

namespace Version1.Phases.Loading
{
    public class LoadingPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private Transform planet;
        [SerializeField] private TMP_Text loadingMessage;

        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private bool rotateRight = true;

        private void Start()
        {
            // Start the phase
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {

        }

        public void StopPhase()
        {

        }

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

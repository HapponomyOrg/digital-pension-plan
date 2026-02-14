using System;
using Assets.Version1.Phases;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Nats.Messages.Host;

namespace Version1.Phases
{
    public class BasePhaseManager : IPhaseManager
    {
        private IPhaseController _currentPhaseController;

        public IPhase[] Phases { get; set; }

        public IPhaseController CurrentPhaseController
        {
            get => _currentPhaseController;
            set
            {
                _currentPhaseController = value;

                if (_currentPhaseController != null)
                    _currentPhaseController
                        .StartPhase(); // Starts the phase when the controller gets assigned. This is to avoid unity scene loading errors
            }
        }

        public void StartPhases(StartGameMessage msg)
        {
            switch (msg.IntrestMode)
            {
                case 0:
                    Phases = GameModes.Sustainable;
                    break;
                case 1:
                    Phases = GameModes.DebtBased;
                    break;
                default:
                    throw new NotImplementedException();
            }

            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
            /*LoadPhase(0, Phases[0].Name);*/
        }

        public void LoadPhase(int index, string name)
        {
            if (index >= Phases.Length)
                throw new ArgumentOutOfRangeException("Phase number doesn't exist in phasemanager");

            if (Phases[index].Name != name)
                throw new Exception("Phase doesnt correspond to phase name");

            if (SceneManager.GetActiveScene().name == Phases[index].Scene)
            {
                Debug.Log("Scene is already loaded");
            }
            else
            {
                if (PlayerData.PlayerData.Instance.IsBankPlayer() && (name == PhaseLibrary.PayDebtPhase.Name || name == PhaseLibrary.TakeALoanPhase.Name))
                {
                    if (SceneManager.GetActiveScene().name != PhaseLibrary.LoadingPhase.Name)
                    {
                        SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
                    }
                }
                else
                {
                    SceneManager.LoadScene(Phases[index].Scene);
                }
            }
        }

        public void EndPhases()
        {
            throw new NotImplementedException();
        }
    }
}

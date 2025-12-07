using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Version1.Phases.tmp;

namespace Assets.Version1.Phases
{
    public class BasePhaseManager : IPhaseManager
    {
        public IPhase[] Phases
        {
            get
            {
                return new[]
                {
                    PhaseLibrary.MarketPhase,
                    PhaseLibrary.MoneyCorrectionPhase,
                    PhaseLibrary.LoadingPhase,
                    PhaseLibrary.MarketPhase,
                    PhaseLibrary.MoneyCorrectionPhase,
                    PhaseLibrary.LoadingPhase,
                    PhaseLibrary.MarketPhase,
                    PhaseLibrary.MoneyCorrectionPhase,
                    PhaseLibrary.LoadingPhase,
                    PhaseLibrary.MoneyToPointPhase,
                    PhaseLibrary.DonatePointsPhase,
                    PhaseLibrary.EndPhase
                };
            }
        }

        private IPhaseController _currentPhaseController;
        public IPhaseController CurrentPhaseController
        {
            get => _currentPhaseController;
            set
            {
                _currentPhaseController = value;

                if (_currentPhaseController != null)
                    _currentPhaseController.StartPhase(); // Starts the phase when the controller gets assigned. This is to avoid unity scene loading errors
            }
        }

        public void StartPhases()
        {
            LoadPhase(0, Phases[0].Name);
        }

        public void LoadPhase(int index, string name)
        {
            if (index >= Phases.Length)
                throw new ArgumentOutOfRangeException("Phase number doesn't exist in phasemanager");

            if (Phases[index].Name != name)
                throw new Exception("Phase doesnt correspond to phase name");

            SceneManager.LoadScene(Phases[index].Scene);
        }

        public void EndPhases()
        {
            throw new NotImplementedException();
        }
    }
}

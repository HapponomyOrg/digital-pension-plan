using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Version1.Phases.MoneyToPoint.scripts
{
    public class MoneyToPoints : MonoBehaviour
    {
        [SerializeField] private TMP_Text MoneyTMP;
        [SerializeField] private TMP_Text PointTMP;

        private int _balance;

        private int Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                MoneyTMP.text = _balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            }
        }

        private int _points;

        private int Points
        {
            get => _points;
            set
            {
                _points = value;
                PointTMP.text = _points.ToString();
            }
        }

        private void Start()
        {
            Balance = PlayerData.PlayerData.Instance.Balance;
            Points = PlayerData.PlayerData.Instance.Points;

            MoneyTMP.text = Balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            PointTMP.text = Points.ToString();

            StartCoroutine(StartCountDown());
        }

        private IEnumerator StartCountDown()
        {
            yield return new WaitForSeconds(1);

            while (Balance > 0)
            {
                // TODO if we change the money system so it is not 1000 change this
                Balance -= 1000;
                yield return new WaitForSeconds(0.2f);

                if (Balance % 2000 == 0)
                {
                    Points += 1;
                }
            }

            PlayerData.PlayerData.Instance.Balance = 0;
            PlayerData.PlayerData.Instance.Points = Points;

        }

        public void Continue()
        {
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }
    }
}

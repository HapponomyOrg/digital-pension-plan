using System.Collections;
using TMPro;
using UnityEngine;

namespace Version1.End.script
{
    // TODO change backgrounds to match the theme
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private TMP_Text description;

        private void OnEnable()
        {
            StartCoroutine(Print());
        }

        private IEnumerator Print()
        {
            switch (PlayerData.PlayerData.Instance.Points)
            {
                case > 1:
                {
                    var msg =
                        $"Good Job! You collected {PlayerData.PlayerData.Instance.Points} points , This means that you are:";
                    yield return StartCoroutine(DisplayTextLetterByLetter(text, msg));
                    break;
                }
                case 0:
                {
                    var msg =
                        $"That's unfortunate! You collected {PlayerData.PlayerData.Instance.Points} point, This means that you are:";
                    yield return StartCoroutine(DisplayTextLetterByLetter(text, msg));
                    break;
                }
                default:
                {
                    var msg =
                        $"Good Job! You collected {PlayerData.PlayerData.Instance.Points} point , This means that you are:";
                    yield return StartCoroutine(DisplayTextLetterByLetter(text, msg));
                    break;
                }
            }

            var message = PlayerData.PlayerData.Instance.Points switch
            {
                < 0 => "Suffering: You’re totally out of luck. Not only do you have nothing, you owe people!",
                < 2 and >= 0 =>
                    "Bankrupt. You just don’t have the resources to sustain yourself. You live out on the street.",
                < 5 and >= 2 => "Surviving. It’s not a life of luxury but you get by.",
                < 10 and >= 5 => "Comfortable. Retirement is a happy time and you don’t have to worry about anything.",
                >= 10 => "Luxurious. You have everything you want and then some!"
            };

            yield return StartCoroutine(DisplayTextLetterByLetter(description, message));
        }

        private IEnumerator DisplayTextLetterByLetter(TMP_Text Text, string message)
        {
            // Start by resetting the text
            Text.text = "";

            // The text you want to display

            // Loop through each character in the message and reveal it one by one
            foreach (char letter in message)
            {
                Text.text += letter; // Add one letter at a time
                yield return new WaitForSeconds(0.03f); // Wait for a specified time before displaying the next letter
            }
        }
    }
}
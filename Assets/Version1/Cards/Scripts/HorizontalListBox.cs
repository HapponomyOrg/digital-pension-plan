using UnityEngine;
using UnityEngine.UI;

namespace Version1.Cards.Scripts
{
    public class HorizontalListBox : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup LayoutGroup;

        private void Update()
        {
            print(LayoutGroup.transform.childCount);
        }
    }
}

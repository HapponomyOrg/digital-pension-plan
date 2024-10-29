using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        

        public List<Card> Cards { get; private set; }
        [field:SerializeField] public List<UICard> UiCards { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

    }
}

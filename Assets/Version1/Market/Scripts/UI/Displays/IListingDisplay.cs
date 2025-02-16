using System;
using System.Collections.Generic;
using UnityEngine;

namespace Version1.Market.Scripts.UI.Displays
{
    public interface IListingDisplay
    {
        public void Init(Listing l, Dictionary<ListingDisplayAction, Action> displayActions);

        public GameObject GameObject();
        
        public void UpdateDisplay();
    }
}

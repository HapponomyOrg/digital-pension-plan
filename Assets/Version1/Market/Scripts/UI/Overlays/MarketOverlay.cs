using UnityEngine;

namespace Version1.Market.Scripts.UI.Overlays
{
    public abstract class MarketOverlay : MonoBehaviour
    {
        public abstract void Open(Listing listing);
        public abstract void Close();
    }
}

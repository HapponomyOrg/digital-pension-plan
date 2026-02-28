using System;
using Version1.Market;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.Services.Listing
{
    public class BuyListingService
    {
        public event EventHandler<ListingEventArgs> BuyListing;

        public void BuyListingLocally(Market.Listing listing)
        {
            RemoveListing(listing);
            PlayerData.Instance.SubtractFromBalance(listing.Price);
            PlayerData.Instance.AddCards(listing.Cards);

            BuyListing?.Invoke(this, new ListingEventArgs(listing));


            var message = new BuyCardsRequestMessage(
                DateTime.Now.ToString("o"),
                PlayerData.Instance.LobbyID,
                PlayerData.Instance.PlayerId,
                listing.ListingId.ToString()
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        private void SellListing(Market.Listing listing)
        {
            PlayerData.Instance.AddToBalance(listing.Price);
        }

        public void BuyListingHandler(BuyCardsRequestMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (PlayerData.Instance.PlayerId == listing.Lister)
                SellListing(listing);

            RemoveListing(listing);
            BuyListing?.Invoke(this, new ListingEventArgs(listing));
        }

        private void RemoveListing(Market.Listing listing)
        {
            Utilities.GameManager.Instance.ListingRepository.RemoveListing(listing);
        }
    }
}

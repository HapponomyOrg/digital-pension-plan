using System;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Market
{ 
    public class BuyListingService
    {
        public event EventHandler<ListingEventArgs> BuyListing;

        public void BuyListingLocally(Listing listing)
        {
            RemoveListing(listing);
            PlayerData.PlayerData.Instance.SubtractFromBalance(listing.Price);
            PlayerData.PlayerData.Instance.AddCards(listing.Cards);

            BuyListing?.Invoke(this, new ListingEventArgs(listing));


            var message = new BuyCardsRequestMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString()
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        private void SellListing(Listing listing)
        {
            PlayerData.PlayerData.Instance.AddToBalance(listing.Price);
        }

        public void BuyListingHandler(BuyCardsRequestMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (PlayerData.PlayerData.Instance.PlayerId == listing.Lister)
                SellListing(listing);

            RemoveListing(listing);
            BuyListing?.Invoke(this, new ListingEventArgs(listing));
        }

        private void RemoveListing(Listing listing)
        {
            Utilities.GameManager.Instance.ListingRepository.RemoveListing(listing);
        }
    }
}

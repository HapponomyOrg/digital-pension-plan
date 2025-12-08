using System;
using Version1.Nats;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Market
{
    public class CreateListingService
    {
        public event EventHandler<ListingEventArgs> CreateListing;

        public void CreateListingLocally(Listing listing)
        {
            PlayerData.PlayerData.Instance.RemoveCards(listing.Cards);
            Utilities.GameManager.Instance.ListingRepository.AddListing(listing);

            CreateListing?.Invoke(this, new ListingEventArgs(listing));

            var message = new ListCardsmessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                PlayerData.PlayerData.Instance.PlayerName,
                listing.ListingId.ToString(),
                listing.Cards,
                listing.Price,
                listing.TimeStamp.ToString("o")
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        public void CreateListingHandler(ListCardsmessage message)
        {
            var listing = new Listing(
                Guid.Parse(message.AuctionID), 
                message.PlayerID, 
                DateTime.Parse(message.ListingDateTimeStamp), 
                message.Amount, 
                message.Cards
                );

            ReceivedCreateListing(listing);
        }

        private void ReceivedCreateListing(Listing listing)
        {
            Utilities.GameManager.Instance.ListingRepository.AddListing(listing);
            CreateListing?.Invoke(this, new ListingEventArgs(listing));
        }
    }
}

using System;
using Version1.Market;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.Services.Listing
{
    public class CreateListingService
    {
        public event EventHandler<ListingEventArgs> CreateListing;

        public void CreateListingLocally(Market.Listing listing)
        {
            PlayerData.Instance.RemoveCards(listing.Cards);
            Utilities.GameManager.Instance.ListingRepository.AddListing(listing);

            CreateListing?.Invoke(this, new ListingEventArgs(listing));

            var message = new ListCardsmessage(
                DateTime.Now.ToString("o"),
                PlayerData.Instance.LobbyID,
                PlayerData.Instance.PlayerId,
                PlayerData.Instance.PlayerName,
                listing.ListingId.ToString(),
                listing.Cards,
                listing.Price,
                listing.TimeStamp.ToString("o")
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        public void CreateListingHandler(ListCardsmessage message)
        {
            var listing = new Market.Listing(
                Guid.Parse(message.AuctionID),
                message.PlayerID,
                message.PlayerName,
                DateTime.Parse(message.ListingDateTimeStamp),
                message.Amount,
                message.Cards
                );

            ReceivedCreateListing(listing);
        }

        private void ReceivedCreateListing(Market.Listing listing)
        {
            Utilities.GameManager.Instance.ListingRepository.AddListing(listing);
            CreateListing?.Invoke(this, new ListingEventArgs(listing));
        }
    }
}

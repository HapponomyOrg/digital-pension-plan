using System;
using Version1.Nats.Messages.Client;

namespace Version1.Market
{
    public class EditListingService
    {
        public event EventHandler<ListingEventArgs> EditListing;

        public void EditListingLocally(Listing listing)
        {
            throw new NotImplementedException();
            //PlayerData.PlayerData.Instance.RemoveCards(listing.Cards);
            //Utilities.GameManager.Instance.ListingRepository.AddListing(listing);

            //CreateListing?.Invoke(this, new EditListingEventArgs(listing));

            //var message = new ListCardsmessage(
            //    DateTime.Now.ToString("o"),
            //    PlayerData.PlayerData.Instance.LobbyID,
            //    PlayerData.PlayerData.Instance.PlayerId,
            //    PlayerData.PlayerData.Instance.PlayerName,
            //    listing.ListingId.ToString(),
            //    listing.Cards,
            //    listing.Price
            //    );

            //Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);
        }

        public void EditListingHandler(ListCardsmessage message)
        {
            throw new NotImplementedException();

            //var listing = new Listing(
            //    Guid.Parse(message.AuctionID),
            //    message.PlayerID,
            //    DateTime.Parse(message.DateTimeStamp),
            //    message.Amount,
            //    message.Cards
            //    );

            //ReceivedCreateListing(listing);
        }

        private void ReceivedEditListing(Listing listing)
        {
            //Utilities.GameManager.Instance.ListingRepository.AddListing(listing);
            //CreateListing?.Invoke(this, new CreateListingEventArgs(listing));
        }
    }
}
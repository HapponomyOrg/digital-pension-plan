using System;
using Version1.Nats.Messages.Client;

namespace Version1.Market
{
    public class CancelListingService
    {
        public event EventHandler<ListingEventArgs> CancelListing;

        public CancelListingService()
        {
            Nats.NatsClient.C.OnCancelListing += CancelListingHandler;
        }

        public void CancelListingLocally(Listing listing)
        {
            PlayerData.PlayerData.Instance.AddCards(listing.Cards);
            Utilities.GameManager.Instance.ListingRepository.RemoveListing(listing);

            CancelListing?.Invoke(this, new ListingEventArgs(listing));


            var message = new CancelListingMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                listing.Lister,
                listing.ListingId.ToString()
                );

            Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);
        }

        public void CancelListingHandler(object sender, CancelListingMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            ReceivedCancelListing(listing);
        }

        private void ReceivedCancelListing(Listing listing)
        {
            var playerId = PlayerData.PlayerData.Instance.PlayerId;

            var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(playerId);

            if (lastBid != null && lastBid.Bidder == playerId && lastBid.BidStatus == EBidStatus.Active) {
                PlayerData.PlayerData.Instance.AddToBalance(lastBid.BidOffer);
            }

            Utilities.GameManager.Instance.ListingRepository.RemoveListing(listing);
            CancelListing?.Invoke(this, new ListingEventArgs(listing));
        }
    }
}
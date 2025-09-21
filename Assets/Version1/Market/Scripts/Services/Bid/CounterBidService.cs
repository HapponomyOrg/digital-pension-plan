using System;
using Version1.Nats.Messages.Client;

namespace Version1.Market
{
    public class CounterBidService
    {
        public event EventHandler<BidEventArgs> CounterBid;

        public void CounterBidLocally(Guid listingId, Bid bid)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var originalBidder = listing.Lister == PlayerData.PlayerData.Instance.PlayerId
                ? bid.Bidder
                : PlayerData.PlayerData.Instance.PlayerId;


            if (PlayerData.PlayerData.Instance.PlayerId == originalBidder)
                PlayerData.PlayerData.Instance.AddToBalance(bid.BidOffer);

            listing.BidRepository.RemoveBidBetweenPlayer(originalBidder, bid.BidId);

            CancelBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new CancelBiddingMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                originalBidder
                );

            Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);

            Utilities.GameManager.Instance.ListingRepository.RemoveListing(listing);
        }

        public void CounterBidHandler(RejectBiddingMessage message)
        {
            var listingId = Guid.Parse(message.AuctionID);
            var bidId = Guid.Parse(message.BidID);
            var originalBidder = message.OriginalBidderID;

            ReceivedCancelBid(listingId, bidId, originalBidder);
        }

        private void ReceivedCounterBid(Guid listingId, Guid bidId, int originalBidder)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var bid = listing.BidRepository.GetBidBetweenPlayer(originalBidder, bidId);

            listing.BidRepository.RemoveBidBetweenPlayer(originalBidder, bid.BidId);

            CancelBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}
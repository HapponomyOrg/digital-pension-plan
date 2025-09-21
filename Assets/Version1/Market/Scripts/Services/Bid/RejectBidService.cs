using System;
using Version1.Nats.Messages.Client;

namespace Version1.Market
{
    public class RejectBidService
    {
        public event EventHandler<BidEventArgs> RejectBid;

        public void RejectBidLocally(Guid listingId, Bid bid)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var originalBidder = listing.Lister == PlayerData.PlayerData.Instance.PlayerId 
                ? bid.Bidder 
                : PlayerData.PlayerData.Instance.PlayerId;


            listing.BidRepository.RemoveBidBetweenPlayer(originalBidder, bid.BidId);

            RejectBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new RejectBiddingMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                bid.Bidder,
                originalBidder,
                bid.BidderName
                );

            Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);
        }

        public void RejectBidHandler(RejectBiddingMessage message)
        {
            var listingId = Guid.Parse(message.AuctionID);
            var bidId = Guid.Parse(message.BidID);
            var originalBidder = message.OriginalBidderID;

            ReceivedRejectBid(listingId, bidId, originalBidder);
        }

        private void ReceivedRejectBid(Guid listingId, Guid bidId, int originalBidder)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var bid = listing.BidRepository.GetBidBetweenPlayer(originalBidder, bidId);

            listing.BidRepository.RemoveBidBetweenPlayer(originalBidder, bidId);

            if (originalBidder == PlayerData.PlayerData.Instance.PlayerId && originalBidder != listing.Lister)
                PlayerData.PlayerData.Instance.AddToBalance(bid.BidOffer);

            RejectBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}
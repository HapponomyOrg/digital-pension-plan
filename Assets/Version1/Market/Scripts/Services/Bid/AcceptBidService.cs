using System;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Market
{
    public class AcceptBidService
    {
        public event EventHandler<BidEventArgs> AcceptBid;

        public void AcceptBidLocally(Guid listingId, Bid bid)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var originalBidder = listing.Lister == PlayerData.PlayerData.Instance.PlayerId
                ? bid.Bidder
                : PlayerData.PlayerData.Instance.PlayerId;

            if (PlayerData.PlayerData.Instance.PlayerId == listing.Lister)
                PlayerData.PlayerData.Instance.AddToBalance(bid.BidOffer);

            AcceptBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new AcceptBiddingMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                originalBidder
                );

            Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);

            GameManager.Instance.ListingRepository.RemoveListing(listing);
        }

        public void AcceptBidHandler(object o, AcceptBiddingMessage message)
        {
            var listingId = Guid.Parse(message.AuctionID);
            var bidId = Guid.Parse(message.BidID);
            var originalBidder = message.OriginalBidderID;

            ReceivedAcceptBid(listingId, bidId, originalBidder);
        }

        private void ReceivedAcceptBid(Guid listingId, Guid bidId, int originalBidder)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var bid = listing.BidRepository.GetBidBetweenPlayer(originalBidder, bidId);

            if (PlayerData.PlayerData.Instance.PlayerId == originalBidder)
                PlayerData.PlayerData.Instance.AddCards(listing.Cards);

            GameManager.Instance.ListingRepository.RemoveListing(listing);

            AcceptBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}
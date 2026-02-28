using System;
using Version1.Market;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.Services.Bid
{
    public class CounterBidService
    {
        public event EventHandler<BidEventArgs> CounterBid;

        public void CounterBidLocally(Guid listingId, Market.Bid bid)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var originalBidder = listing.Lister == PlayerData.Instance.PlayerId
                ? bid.Bidder
                : PlayerData.Instance.PlayerId;


            var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(originalBidder);
            lastBid.BidStatus = EBidStatus.Rejected;

            if (PlayerData.Instance.PlayerId == originalBidder)
                PlayerData.Instance.SubtractFromBalance(bid.BidOffer);


            CounterBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new CounterBidMessage(
                DateTime.Now.ToString("o"),
                PlayerData.Instance.LobbyID,
                PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                originalBidder,
                bid.BidderName,
                bid.BidOffer,
                bid.TimeStamp.ToString("o")
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);

            listing.BidRepository.AddBid(PlayerData.Instance.PlayerId, bid);
        }

        public void CounterBidHandler(CounterBidMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (listing == null)
                return; // TODO Error handling

            var bid = new Market.Bid(
                Guid.Parse(message.BidID),
                message.PlayerID,
                message.PlayerName,
                message.CounterOfferPrice,
                DateTime.Parse(message.BidDateTimeStamp)
                );

            ReceivedCounterBid(listing, message.OriginalBidder, bid);
        }


        private void ReceivedCounterBid(Market.Listing listing, int originalBidder, Market.Bid bid)
        {
            var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(originalBidder);
            lastBid.BidStatus = EBidStatus.Rejected;

            if (lastBid.Bidder == originalBidder)
                PlayerData.Instance.AddToBalance(bid.BidOffer);

            listing.BidRepository.AddBid(originalBidder, bid);
            CounterBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}

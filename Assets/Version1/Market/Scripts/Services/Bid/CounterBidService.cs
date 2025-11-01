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


            var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(originalBidder);
            lastBid.BidStatus = EBidStatus.Rejected;

            if (PlayerData.PlayerData.Instance.PlayerId == originalBidder)
                PlayerData.PlayerData.Instance.SubtractFromBalance(bid.BidOffer);


            CounterBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new CounterBidMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                originalBidder,
                bid.BidderName,
                bid.BidOffer,
                bid.TimeStamp.ToString("o")
                );

            Nats.NatsClient.C.Publish(message.LobbyID.ToString(), message);

            listing.BidRepository.AddBid(PlayerData.PlayerData.Instance.PlayerId, bid);
        }

        public void CounterBidHandler(CounterBidMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (listing == null)
                return; // TODO Error handling

            var bid = new Bid(
                Guid.Parse(message.BidID),
                message.PlayerID,
                message.PlayerName,
                message.CounterOfferPrice,
                DateTime.Parse(message.BidDateTimeStamp)
                );

            ReceivedCounterBid(listing, message.OriginalBidder, bid);
        }


        private void ReceivedCounterBid(Listing listing, int originalBidder, Bid bid)
        {
            var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(originalBidder);
            lastBid.BidStatus = EBidStatus.Rejected;

            if (lastBid.Bidder == originalBidder)
                PlayerData.PlayerData.Instance.AddToBalance(bid.BidOffer);

            listing.BidRepository.AddBid(originalBidder, bid);
            CounterBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}
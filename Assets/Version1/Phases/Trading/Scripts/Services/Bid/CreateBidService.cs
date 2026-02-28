using System;
using Version1.Market;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.Services.Bid
{
    public class CreateBidService
    {
        public event EventHandler<BidEventArgs> CreateBid;

        public void CreateBidLocally(Guid listingId, Market.Bid bid)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var success = listing.BidRepository.AddBid(PlayerData.Instance.PlayerId, bid);

            if (!success)
                return; // TODO Error handling

            PlayerData.Instance.SubtractFromBalance(bid.BidOffer);

            CreateBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new CreateBidMessage(
                DateTime.Now.ToString("o"),
                PlayerData.Instance.LobbyID,
                PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                bid.Bidder,
                bid.BidderName,
                bid.BidOffer,
                bid.TimeStamp.ToString("o")
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        public void CreateBidHandler(CreateBidMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (listing == null)
                return; // TODO Error handling

            var bid = new Market.Bid(
                Guid.Parse(message.BidID),
                message.PlayerID,
                message.PlayerName,
                message.OfferPrice,
                DateTime.Parse(message.BidDateTimeStamp)
                );

            ReceivedCreateBid(listing, bid);
        }

        private void ReceivedCreateBid(Market.Listing listing, Market.Bid bid)
        {
            listing.BidRepository.AddBid(bid.Bidder, bid);
            CreateBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}

using System;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Market
{
    public class CreateBidService
    {
        public event EventHandler<BidEventArgs> CreateBid;

        public void CreateBidLocally(Guid listingId, Bid bid)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var success = listing.BidRepository.AddBid(PlayerData.PlayerData.Instance.PlayerId, bid);

            if (!success)
                return; // TODO Error handling

            PlayerData.PlayerData.Instance.SubtractFromBalance(bid.BidOffer);

            CreateBid?.Invoke(this, new BidEventArgs(listing, bid));


            var message = new CreateBidMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString(),
                bid.BidId.ToString(),
                bid.Bidder,
                bid.BidderName,
                bid.BidOffer,
                bid.TimeStamp.ToString("o")
                );

            NetworkManager.Instance.Publish(message.LobbyID.ToString(), message);
        }

        public void CreateBidHandler(object sender, CreateBidMessage message)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(Guid.Parse(message.AuctionID));

            if (listing == null)
                return; // TODO Error handling

            var bid = new Bid(
                Guid.Parse(message.BidID), 
                message.PlayerID, 
                message.PlayerName, 
                message.OfferPrice, 
                DateTime.Parse(message.BidDateTimeStamp)
                );

            ReceivedCreateBid(listing, bid);
        }

        private void ReceivedCreateBid(Listing listing, Bid bid)
        {
            listing.BidRepository.AddBid(bid.Bidder, bid);
            CreateBid?.Invoke(this, new BidEventArgs(listing, bid));
        }
    }
}

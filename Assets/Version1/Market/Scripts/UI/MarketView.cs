using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Utilities;

namespace Version1.Market
{
    public class MarketView : MonoBehaviour
    {
        [SerializeField] private MarketOfferList marketOfferList;
        [SerializeField] private OutgoingBidsList outgoingBidsList;
        [SerializeField] private PersonalListingList personalListingList;

        [SerializeField] private GameObject closeMarketOverlay;

        private void Start()
        {
            var marketServices = Utilities.GameManager.Instance.MarketServices;

            marketServices.CreateListingService.CreateListing += CreateListing;
            marketServices.BuyListingService.BuyListing += BuyListing;
            marketServices.CancelListingService.CancelListing += CancelListing;

            marketServices.CreateBidService.CreateBid += CreateBid;
            marketServices.CancelBidService.CancelBid += CancelBid;
            marketServices.AcceptBidService.AcceptBid += AcceptBid;
            marketServices.CounterBidService.CounterBid += CounterBid;
            marketServices.RejectBidService.RejectBid += RejectBid;
        }

        private void OnDestroy()
        {
            var marketServices = Utilities.GameManager.Instance.MarketServices;

            marketServices.CreateListingService.CreateListing -= CreateListing;
            marketServices.BuyListingService.BuyListing -= BuyListing;
            marketServices.CancelListingService.CancelListing -= CancelListing;

            marketServices.CreateBidService.CreateBid -= CreateBid;
            marketServices.CancelBidService.CancelBid -= CancelBid;
            marketServices.AcceptBidService.AcceptBid -= AcceptBid;
            marketServices.CounterBidService.CounterBid -= CounterBid;
            marketServices.RejectBidService.RejectBid -= RejectBid;
        }


        public void InitializeData()
        {
            var playerId = PlayerData.PlayerData.Instance.PlayerId;

            var peerListings = Utilities.GameManager.Instance.ListingRepository.GetPeerListings(playerId);
            var outgoingList = new List<(Guid listing, Guid bid)>();
            var personalListings = Utilities.GameManager.Instance.ListingRepository.GetPersonalListings(playerId);

            foreach (var listing in peerListings) 
            { 
                var bid = listing.BidRepository.GetLastBidBetweenPlayer(playerId);
                if (bid == null)
                    continue;
                
                outgoingList.Add((listing.ListingId, bid.BidId));
            }

            marketOfferList.InitializeData(peerListings.Select(l => l.ListingId).ToArray());
            outgoingBidsList.InitializeData(outgoingList);
            personalListingList.InitializeData(personalListings.Select(l => l.ListingId).ToArray());
        }

        public void OpenMarket()
        {
            closeMarketOverlay.SetActive(false);
        }

        public void CloseMarket()
        {
            closeMarketOverlay.SetActive(true);
        }

        private void CreateListing(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                personalListingList.CreateDisplay(e.Listing.ListingId);
            else
                marketOfferList.CreateDisplay(e.Listing.ListingId);
        }

        private void BuyListing(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            {
                personalListingList.RemoveDisplay(e.Listing.ListingId);

                personalListingList.DetailsDisplay.Clear();
                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.Clear();
            }
            else
            {
                if (marketOfferList.ContainsListing(e.Listing.ListingId))
                    marketOfferList.RemoveDisplay(e.Listing.ListingId);
                if (outgoingBidsList.ContainsListing(e.Listing.ListingId))
                    outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
            }
        }

        private void CancelListing(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            {
                personalListingList.RemoveDisplay(e.Listing.ListingId);

                personalListingList.DetailsDisplay.Clear();
                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.Clear();
            }
            else
            { 
                if (marketOfferList.ContainsListing(e.Listing.ListingId))
                    marketOfferList.RemoveDisplay(e.Listing.ListingId);
                if (outgoingBidsList.ContainsListing(e.Listing.ListingId))
                    outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
            }
        }



        private void CreateBid(object sender, BidEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            { 
                personalListingList.UpdateDisplay(e.Listing.ListingId);

                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.CreateDisplay(e.Listing.ListingId, e.Bid.BidId);
            }
            else if (e.Bid.Bidder == PlayerData.PlayerData.Instance.PlayerId)
            {
                marketOfferList.RemoveDisplay(e.Listing.ListingId);
                outgoingBidsList.CreateDisplay(e.Listing.ListingId, e.Bid.BidId);
            }


            // Remove listing display from market offers
            // Add bid to bid list
            // Update personal listing display to show bid count
        }

        private void CounterBid(object sender, BidEventArgs e)
        {
            // Update outgoing bid display
            // Update outgoing bid display
        }

        private void RejectBid(object sender, BidEventArgs e)
        {
            // Remove outgoing bid display
            // Add market listing display
            // Update personal listing display

            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            {
                personalListingList.UpdateDisplay(e.Listing.ListingId);

                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.RemoveDisplay(e.Bid.BidId);
            }
            else if (e.Bid.Bidder == PlayerData.PlayerData.Instance.PlayerId)
            {
                outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
                marketOfferList.CreateDisplay(e.Listing.ListingId);
            }
        }

        private void AcceptBid(object sender, BidEventArgs e)
        {
            // Remove outgoing bid display
            // Remove personal listing display
            // Clear bid list if necessary

            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            {
                personalListingList.RemoveDisplay(e.Listing.ListingId);

                personalListingList.DetailsDisplay.Clear();
                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.Clear();
            }
            else if (e.Bid.Bidder == PlayerData.PlayerData.Instance.PlayerId)
            {
                outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
            }
        }

        private void CancelBid(object sender, BidEventArgs e)
        {
            // Remove outgoing bid display
            // Create market listing display
            // Update personal listing display
            // Update bid list if necessary

            if (e.Bid.Bidder == PlayerData.PlayerData.Instance.PlayerId)
            {
                outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
                marketOfferList.CreateDisplay(e.Listing.ListingId);


            }
            else if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
            {
                personalListingList.UpdateDisplay(e.Listing.ListingId);

                if (personalListingList.ReceivedBidsList.ActiveListing == e.Listing.ListingId)
                    personalListingList.ReceivedBidsList.RemoveDisplay(e.Bid.BidId);
            }
        }


        public void CreateListingDisplay(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                marketOfferList.CreateDisplay(e.Listing.ListingId);
        }

        public void UpdateListingDisplay(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                marketOfferList.UpdateDisplay(e.Listing.ListingId);
        }

        public void RemoveListingDisplay(object sender, ListingEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                marketOfferList.RemoveDisplay(e.Listing.ListingId);
        }

        public void CreateBidDisplay(object sender, BidEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                outgoingBidsList.CreateDisplay(e.Listing.ListingId, e.Bid.BidId);
        }

        public void UpdateBidDisplay(object sender, BidEventArgs e)
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                outgoingBidsList.UpdateDisplay(e.Listing.ListingId, e.Bid.BidId);
        }

        public void RemoveBidDisplay(object sender, BidEventArgs e) 
        {
            if (e.Listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                return;
            else
                outgoingBidsList.RemoveDisplay(e.Listing.ListingId);
        }
    }
}

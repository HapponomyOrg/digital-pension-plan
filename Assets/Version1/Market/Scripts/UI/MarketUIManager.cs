using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;
using Version1.Market.Scripts.UI.Overlays;

namespace Version1.Market.Scripts.UI
{
    public class MarketUIManager : MonoBehaviour
    {
        [SerializeField] private Button AddListingButton;

        [SerializeField] private Transform closedMarketPanel;
        
        private Listing[] testListings = 
        {
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 10, 15, 22), 4500, new[] { 2, 1, 4, 3 }),
            new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 12, 30, 40), 5200, new[] { 3, 2, 5, 1 }),
            new Listing(Guid.NewGuid(), 2, new DateTime(2002, 10, 5, 14, 45, 10), 4800, new[] { 4, 3 }),
            new Listing(Guid.NewGuid(), 3, new DateTime(2002, 10, 5, 9, 10, 5), 5000, new[] { 5, 4, 4, 3, 14, 14, 13, 12 }),
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 15, 20, 30), 4900, new[] { 1, 2, 3, 4 }),
            new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 11, 5, 50), 4600, new[] { 6, 1, 2, 3 }),
            new Listing(Guid.NewGuid(), 2, new DateTime(2002, 10, 5, 16, 30, 15), 5300, new[] { 7, 2, 2, 2 }),
            new Listing(Guid.NewGuid(), 3, new DateTime(2002, 10, 5, 17, 20, 45), 4800, new[] { 8, 1 }),
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 18, 15, 25), 5100, new[] { 3, 5, 2, 6 }),
            new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 19, 50, 35), 4700, new[] { 1, 6, 3, 5 }),
        };
        
        private Tuple<int, int, int>[] testBids =
        {
            /*new (1, 1, 2000),
            new (0, 1, 8000),
            new (1, 1, 5000),
            new (0, 1, 6000),
            
            new (5, 5, 3000),
            new (0, 5, 7000),
            new (5, 5,4000),
            
            new (8, 8, 3000),
            new (0, 8, 7000),
            new (8, 8,4000),
            
            
            new (9, 9, 3000),
            new (0, 9, 7000),
            new (9, 9,4000),
            
            new (2, 2, 3000),
            new (0, 2, 7000),
            new (2, 2,4000),*/
        };

        [Header("Display lists")]
        [SerializeField] private Transform personalListings;
        [SerializeField] private Transform marketListings;
        [SerializeField] private Transform marketBidListings;

        
        [Header("Display prefabs")]
        [SerializeField] private PersonalListingDisplay personalListingDisplay;
        [SerializeField] private PersonalListingDetailsDisplay personalListingDetailsDisplay;
        [SerializeField] private MarketListingDisplay marketListingDisplay;
        [SerializeField] private MarketListingDetailsDisplay marketListingDetailsDisplay;
        [SerializeField] private MarketBidListingDisplay marketBidListingDisplay;

        //private readonly Dictionary<Guid, IListingDisplay> listingDisplays = new Dictionary<Guid, IListingDisplay>();

        
        [Header("Overlays")]
        [SerializeField] private MarketOverlay buyOverlay;
        [SerializeField] private MarketOverlay bidOverlay;
        [SerializeField] private MarketOverlay cancelBidOverlay;
        
        [SerializeField] private AddListingOverlay addListingOverlay;
        [SerializeField] private CancelListingOverlay cancelListingOverlay;


        private Dictionary<string, MarketListingDisplay> marketListingDisplays = new();
        private Dictionary<string, PersonalListingDisplay> personalListingDisplays = new();
        private Dictionary<string, MarketBidListingDisplay> marketBidListingDisplays = new();

        public void Init()
        {
            marketListingDisplays = new Dictionary<string, MarketListingDisplay>();
            personalListingDisplays = new Dictionary<string, PersonalListingDisplay>();
            marketBidListingDisplays = new Dictionary<string, MarketBidListingDisplay>();
            
            AddListingButton.onClick.AddListener(() => addListingOverlay.Open());

            Utilities.GameManager.Instance.MarketManager.ListingAdded += (sender, args) => { GenerateDisplays(); }; 
            Utilities.GameManager.Instance.MarketManager.ListingRemoved += (sender, args) =>
            {
                if (marketListingDisplays.ContainsKey(args))
                {
                    Destroy(marketListingDisplays[args]);
                    marketListingDisplays.Remove(args);
                    Debug.Log($"removed market listing {args}");
                }

                if (personalListingDisplays.ContainsKey(args))
                {
                    Destroy(personalListingDisplays[args]);
                    personalListingDisplays.Remove(args);
                    Debug.Log($"removed personal listing {args}");
                }

                if (marketBidListingDisplays.ContainsKey(args))
                {
                    Destroy(marketBidListingDisplays[args]);
                    marketBidListingDisplays.Remove(args);
                    Debug.Log($"removed bid listing {args}");
                }
            }; 

            /*foreach (var listing in testListings)
            {
                Utilities.GameManager.Instance.MarketManager.AddListing(listing);
                
                /*foreach (var bid in testBids)
                {
                    Utilities.GameManager.Instance.MarketManager.AddBidToListing(listing, bid.Item2, bid.Item3, bid.Item1 == 0);
                }#1#
            }*/
            //
            // Utilities.GameManager.Instance.MarketManager.AddBidToListing(testListings[2], 0, 1234);
            // Utilities.GameManager.Instance.MarketManager.AddBidToListing(testListings[3], 0, 1234);
            
            GenerateDisplays();
        }
        
        public void CloseMarket()
        {
            closedMarketPanel.gameObject.SetActive(true);
        }

        public void OpenMarket()
        {
            closedMarketPanel.gameObject.SetActive(false);
        }

        // TODO Only change display that changed instead of regenerating everything
        private void GenerateDisplays()
        {
            GenerateMarketListings();
            GenerateMarketBidListings();
            GeneratePersonalListings();
        }
        
        private void GenerateMarketListings()
        {
            var listings = Utilities.GameManager.Instance.MarketManager.PeerListingsWithoutBid(PlayerData.PlayerData.Instance.PlayerId);

            foreach (var l in listings)
                GenerateMarketListing(l);
        }


        private void GenerateMarketListing(Listing listing)
        {
            if (marketListingDisplays.ContainsKey(listing.ListingId.ToString()))
                return;
            
            var obj = Instantiate(marketListingDisplay, marketListings);
            obj.Init(listing,
                new Dictionary<ListingDisplayAction, Action>
                {
                    { ListingDisplayAction.Buy, () => { Debug.Log("Buy"); buyOverlay.Open(listing); } },
                    { ListingDisplayAction.Bid, () => { Debug.Log("Bid"); bidOverlay.Open(listing); } },
                    { ListingDisplayAction.Select, () => 
                        marketListingDetailsDisplay.Init(listing, 
                            new Dictionary<ListingDisplayAction, Action>
                            {
                                { ListingDisplayAction.Buy, () => { Debug.Log("Buy"); buyOverlay.Open(listing); } },
                                { ListingDisplayAction.Bid, () => { Debug.Log("Bid"); bidOverlay.Open(listing); } }
                            })
                    }
                });
            
            marketListingDisplays.Add(listing.ListingId.ToString(), obj);
        }
        
        
        private void GenerateMarketBidListings()
        {
            var listings = Utilities.GameManager.Instance.MarketManager.PeerListingsWithBid(PlayerData.PlayerData.Instance.PlayerId);

            foreach (var l in listings)
                GenerateMarketBidListing(l);
        }

        private void GenerateMarketBidListing(Listing listing)
        {
            var obj = Instantiate(marketBidListingDisplay, marketBidListings);
            obj.Init(listing,
                new Dictionary<ListingDisplayAction, Action>
                {
                    {
                        ListingDisplayAction.Cancel, () =>
                        {
                            Debug.Log($"Cancel bid {listing.ListingId}");
                            cancelBidOverlay.Open(listing);
                        }
                    },
                    { ListingDisplayAction.Select, () => { Debug.Log($"Selected bid: {listing.ListingId}"); } }
                }
            );

            //listingDisplays[listing.ListingId] = obj;
        }

        private void GeneratePersonalListings()
        {
            var listings = Utilities.GameManager.Instance.MarketManager.PersonalListings(PlayerData.PlayerData.Instance.PlayerId);
         
            foreach (var l in listings)
                GeneratePersonalListing(l);
        }

        private void GeneratePersonalListing(Listing listing)
        {
            if (personalListingDisplays.ContainsKey(listing.ListingId.ToString()))
                return;
            
            var obj = Instantiate(personalListingDisplay, personalListings);
            obj.Init(listing,
                new Dictionary<ListingDisplayAction, Action>
                {
                    { ListingDisplayAction.Cancel, () => { cancelListingOverlay.Open(listing); } },
                    { ListingDisplayAction.Select, () => { personalListingDetailsDisplay.Init(listing,
                            new Dictionary<ListingDisplayAction, Action>
                            {
                                { ListingDisplayAction.Cancel, () => { cancelListingOverlay.Open(listing); } }
                            });
                    } }
                });
            
            personalListingDisplays.Add(listing.ListingId.ToString(), obj);
        }

        
    }
}

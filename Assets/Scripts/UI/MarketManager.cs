using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NATS;
using TMPro;
using UI.Overlays;
using UnityEngine;

namespace UI
{
    public class MarketManager : MonoBehaviour
    {
        public static MarketManager Instance { get; private set; }

        [SerializeField] private TMP_Text balanceDisplay;

        [SerializeField] private BuyListingDisplay buyListingPrefab;
        [SerializeField] private Transform buyListingDisplay;

        [SerializeField] private SellListingDisplay sellListingPrefab;
        [SerializeField] private Transform sellListingDisplay;

        [SerializeField] private BiddingDisplay biddingDisplayPrefab;
        [SerializeField] private Transform biddingDisplay;

        [SerializeField] private BuyListingOverlay buyOverlay;
        [SerializeField] private CancelListingOverlay cancelOverlay;

        [SerializeField] private MakeBiddingOverlay makeBiddingOverlay;
        [SerializeField] private RespondToBiddingOverlay respondToBiddingOverlay;
        [SerializeField] private CounterOfferOverlay counterOfferOverlay;

        private readonly Dictionary<Listing, GameObject> activeListings = new Dictionary<Listing, GameObject>();

        private bool listenersAssigned;
        
        private Queue<Action> _counterOfferQueue = new Queue<Action>();
        private bool _isProcessing;

        private void Awake()
        {
            if (Instance != null)
                return;

            Instance = this;
        }

        private void Start()
        {
            balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";

            AddListeners();
            // AddSellListing(5000, new [] { 0, 2, 6, 6, 3, 1 });
            // AddSellListing(15000, new [] { 5, 2, 5, 3, 7, 9, 4, 3 });
            // AddSellListing(4000, new [] { 2, 2 });
            // AddSellListing(9000, new [] { 7, 2, 5, 7, 7, 3 });
            //
            // AddBuyListing("Mark", Guid.NewGuid().ToString(),9000, new [] { 9, 9, 9, 3, 7, 3 });
            // AddBuyListing("Jamal", Guid.NewGuid().ToString(),7000, new [] { 7, 2, 2, 3, 3 });
            // AddBuyListing("Thierry", Guid.NewGuid().ToString(),4000, new [] { 7, 2 });
            // AddBuyListing("Jaqueline", Guid.NewGuid().ToString(),11000, new [] { 11, 12, 5, 7, 7, 3, 5, 9, 10 });
            // AddBuyListing("Karel", Guid.NewGuid().ToString(),2000, new [] { 4, 2 });
        }

        public void UpdateBalance()
        {
            balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";
            // TODO check this function to disable the buttons in the first overlay of the listings
            //  ToggleAllBidButtons(PlayerManager.Instance.Balance > 0);
        }

        private void Update()
        {
            if (NatsClient.C == null)
                return;
            NatsClient.C.HandleMessages();
        }

        // TODO() FIX event handlers
        private void AddListeners()
        {
            if (listenersAssigned)
                return;
            if (NatsClient.C == null)
                return;
            NatsClient.C.OnListCards += (sender, msg) =>
            {
                AddBuyListing(msg.PlayerName, msg.AuctionID, msg.Amount, msg.Cards);
            };
            NatsClient.C.OnBuyCards += (sender, msg) => { SoldListing(msg.AuctionID); };
            NatsClient.C.OnCancelListing += (sender, msg) =>
            {
                if (msg.PlayerID == PlayerManager.Instance.PlayerId) return;

                var listing = activeListings.Keys.FirstOrDefault(key => key.AuctionId == msg.AuctionID);

                if (listing == null)
                {
                    print($"Can not find listing with AuctionID : {msg.AuctionID}");
                    return;
                }

                if (buyOverlay.isActiveAndEnabled && buyOverlay.Current == msg.AuctionID)
                {
                    // TODO() HERE SHOW A SCREEN THAT THE AUCTION HAS BEEN CANCELD
                    buyOverlay.gameObject.SetActive(false);
                }

                listing.RemoveAllBiddings(false);

                Destroy(activeListings[listing]);
                activeListings.Remove(listing);
            };

            PlayerManager.Instance.OnBalanceChange += (sender, args) =>
            {
                balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";
            };

            NatsClient.C.OnMakeBidding += (sender, msg) =>
            {
                BiddingReceived(new Bidding(msg.PlayerID, msg.PlayerName, msg.AuctionID, msg.OfferPrice));
            };
            NatsClient.C.OnAcceptBidding += (sender, msg) =>
            {
                var listing = GetListing(msg.AuctionID);

                if (listing == null)
                    return;

                if (msg.BidderID == PlayerManager.Instance.PlayerId)
                {
                    listing.RemoveAllBiddings(true);
                    PlayerManager.Instance.AddCards(listing.Cards);
                    NotificationList.Instance.AddNotification(
                        new Notification(
                            "Bidding accepted", 
                            $"Your bidding of {msg.OfferAmount} was accepted.", 
                            null, 
                            NotificationColor.Green
                            )
                        );
                }
                else
                {
                    listing.RemoveAllBiddings(false);
                }

                Destroy(activeListings[listing]);
                activeListings.Remove(listing);
            };
            NatsClient.C.OnCancelBidding += (sender, msg) =>
            {
                var l = GetListing(msg.AuctionID);

                var bidDisplay = activeListings[l].GetComponent<SellListingDisplay>().receivedBiddings
                    .FirstOrDefault(bidding => bidding.BidderId == msg.PlayerID);

                if (bidDisplay == null)
                    return;

                Destroy(bidDisplay.gameObject);
            };
            NatsClient.C.OnRejectBidding += (sender, msg) =>
            {
                var l = GetListing(msg.AuctionID);

                if (l == null)
                    return;

                if (msg.BidderID == PlayerManager.Instance.PlayerId)
                {
                    NotificationList.Instance.AddNotification(
                        new Notification(
                            "Bidding accepted", 
                            "Your bidding has been rejected.", 
                            null, 
                            NotificationColor.Red
                        )
                    );
                }
                
                l.RemoveAllBiddings(false);

                var listingDisplay = activeListings[l].GetComponent<BuyListingDisplay>();

                if (listingDisplay == null)
                    return;
                listingDisplay.TurnOnBiddingButton();
            };
            NatsClient.C.OnRespondBidding += (sender, msg) =>
            {
                if (PlayerManager.Instance.PlayerId != msg.BidderID)
                    return;
                
                NotificationList.Instance.AddNotification(
                    new Notification(
                        "Counter Bidding", 
                        "You Received a counter bidding", 
                        () => {
                            counterOfferOverlay.Open(
                                new Bidding(msg.PlayerID, msg.PlayerName, msg.AuctionID, msg.CounterOfferPrice),
                                msg.OriginalOfferPrice);
                        },
                        NotificationColor.Yellow
                        )
                    );
            };
            
            NatsClient.C.OnAcceptCounterBidding += (sender, msg) =>
            {
                if (PlayerManager.Instance.PlayerId == msg.CounterBidderID)
                {
                    PlayerManager.Instance.Balance += msg.OfferAmount;
                    NotificationList.Instance.AddNotification(
                        new Notification(
                            "Bidding accepted", 
                            $"Your counter bidding of {msg.OfferAmount} was accepted.", 
                            null, 
                            NotificationColor.Green
                        )
                    );
                    UpdateBalance();
                }

                var listing = GetListing(msg.AuctionID);
                Destroy(activeListings[listing]);
                activeListings.Remove(listing);
            };

            listenersAssigned = true;
        }
        
        // TODO CHECK IF THIS WORKS
        //{
            private void OnRespondBidding(object sender, RespondBiddingMessage msg)
            {
                if (PlayerManager.Instance.PlayerId != msg.BidderID)
                    return;

                EnqueueCounterOffer(() =>
                {
                    counterOfferOverlay.Open(
                        new Bidding(msg.PlayerID, msg.PlayerName, msg.AuctionID, msg.CounterOfferPrice),
                        msg.OriginalOfferPrice);
                });
            }

            public void EnqueueCounterOffer(Action action)
            {
                _counterOfferQueue.Enqueue(action);
                if (!_isProcessing)
                {
                    StartCoroutine(ProcessNextCounterOffer());
                }
            }

            private IEnumerator ProcessNextCounterOffer()
            {
                while (_counterOfferQueue.Count > 0)
                {
                    _isProcessing = true;
                    var action = _counterOfferQueue.Dequeue();
                    action();
                    yield return new WaitForSeconds(1.0f); // 1 second delay
                }

                _isProcessing = false;
            }

            private void OnDestroy()
            {
                // Unsubscribe from the event to avoid memory leaks
                //NatsClient.C.OnRespondBidding -= OnRespondBidding;
            }
        //}

        public void RemoveAllBiddings()
        {
            foreach (var listing in activeListings.Keys)
            {
                if (listing == null)
                    return;

                listing.RemoveAllBiddings(false);
            }
        }

        private void ToggleAllBidButtons(bool on)
        {
            foreach (var listing in activeListings.Values)
            {
                var buyDisplay = listing.GetComponent<BuyListingDisplay>();
                if (!buyDisplay)
                    continue;

                if (on)
                {
                    buyDisplay.TurnOnBiddingButton();
                }
                else
                {
                    buyDisplay.TurnOffBiddingButton();
                }
            }
        }

        public void AcceptCounterBidding(Bidding bidding, int remainingCost)
        {
            var listing = GetListing(bidding.AuctionId);

            var msg = new AcceptCounterBiddingMessage(
                DateTime.Now.ToString("o"),
                PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId,
                bidding.AuctionId,
                bidding.OfferPrice,
                bidding.SenderId
            );
            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), msg);

            listing.RemoveAllBiddings(true);

            PlayerManager.Instance.Balance -= remainingCost;
            UpdateBalance();
            PlayerManager.Instance.AddCards(listing.Cards);

            Destroy(activeListings[listing]);
            activeListings.Remove(listing);
            PlayerManager.Instance.CheckForSet();
        }

        private void BiddingReceived(Bidding bidding)
        {
            var listing = GetListing(bidding.AuctionId);

            if (listing == null)
                return;

            if (listing.Seller != PlayerManager.Instance.PlayerName)
                return;
            
            NotificationList.Instance.AddNotification(
                new Notification(
                    "Bidding received",
                    "You received a bidding.",
                    null,
                    NotificationColor.Yellow
                    )
                );

            var display = activeListings[listing].GetComponent<SellListingDisplay>();
            display.AddBidding(listing,bidding);
        }

        public void BiddingAccepted(Bidding bidding)
        {
            PlayerManager.Instance.Balance += bidding.OfferPrice;
            balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";
            var listing = GetListing(bidding.AuctionId);
            Destroy(activeListings[listing]);
            activeListings.Remove(listing);

            var message = new AcceptBiddingMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId, bidding.AuctionId,
                bidding.OfferPrice, bidding.SenderId);

            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), message);
        }

        public void RespondToBidding(ReceivedBiddingDisplay biddingDisplay, Bidding bidding, int originalSender)
        {
            Destroy(biddingDisplay.gameObject);
            Debug.Log($"Responded with new price: {bidding.OfferPrice}");

            var message = new RespondBiddingMessage(DateTime.Now.ToString("o"),
                PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId,
                bidding.AuctionId,
                PlayerManager.Instance.PlayerName,
                originalSender,
                biddingDisplay.Bid,
                bidding.OfferPrice);

            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), message);
        }

        public void MakeBidding(Bidding bidding)
        {
            var b = Instantiate(biddingDisplayPrefab, biddingDisplay);
            b.SetDisplay(bidding);


            var message = new MakeBiddingMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId, bidding.AuctionId,
                PlayerManager.Instance.PlayerName, bidding.OfferPrice);

            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), message);
        }

        public void BiddingRejected(ReceivedBiddingDisplay biddingDisplay, Bidding bidding)
        {
            Destroy(biddingDisplay.gameObject);

            var message = new RejectBiddingMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId, bidding.AuctionId,
                PlayerManager.Instance.PlayerName, bidding.SenderId);

            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), message);
        }

        public void CancelBidding(Bidding bidding, GameObject display)
        {
            var l = GetListing(bidding.AuctionId);
            var listingDisplay = activeListings[l].GetComponent<BuyListingDisplay>();
            listingDisplay.TurnOnBiddingButton();
            Destroy(display);

            var message = new CancelBiddingMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
                PlayerManager.Instance.PlayerId, bidding.AuctionId);

            NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), message);
        }

        public void OpenMakeBiddingOverlay(BuyListingDisplay buyDisplay, Listing listing)
        {
            makeBiddingOverlay.Open(buyDisplay, listing);
        }

        public void OpenRespondToBiddingOverlay(ReceivedBiddingDisplay biddingDisplay, Bidding bidding)
        {
            respondToBiddingOverlay.Open(biddingDisplay, bidding);
        }

        public void AddBuyListing(string sellerName, string auctionId, int price, int[] cards)
        {
            var listing = new Listing(sellerName, auctionId, price, cards);
            var display = Instantiate(buyListingPrefab, buyListingDisplay);
            display.SetDisplay(listing);
            activeListings.Add(listing, display.gameObject);
        }

        public void AddSellListing(int price, int[] cards)
        {
            var listing = new Listing(PlayerManager.Instance.PlayerName, Guid.NewGuid().ToString(), price, cards);
            var display = Instantiate(sellListingPrefab, sellListingDisplay);
            display.SetDisplay(listing);
            activeListings.Add(listing, display.gameObject);
            //
            // BiddingReceived(new Bidding(0, "Mark", listing.AuctionId, 4000));
            // BiddingReceived(new Bidding(1, "Jannes", listing.AuctionId, 7000));
            // BiddingReceived(new Bidding(1, "Jannes", listing.AuctionId, 2000));
            // BiddingReceived(new Bidding(2, "Gert", listing.AuctionId, 5000));
            // BiddingReceived(new Bidding(3, "Mo", listing.AuctionId, 9000));
            // BiddingReceived(new Bidding(1, "Jannes", listing.AuctionId, 1000));

            var sessionId = PlayerManager.Instance.LobbyID;
            var msg = new ListCardsmessage(DateTime.Now.ToString("o"), sessionId, PlayerManager.Instance.PlayerId,
                PlayerManager.Instance.PlayerName,
                listing.AuctionId, listing.Cards, listing.Price);
            NatsClient.C.Publish(sessionId.ToString(), msg);
        }

        public void OpenCancelOverlay(string auctionId)
        {
            cancelOverlay.Open(auctionId);
        }

        public void CancelListingRequest(string auctionId)
        {
            var sessionId = PlayerManager.Instance.LobbyID;
            var msg = new CancelListingMessage(DateTime.Now.ToString("o"), sessionId, PlayerManager.Instance.PlayerId,
                auctionId);

            NatsClient.C.Publish(sessionId.ToString(), msg);

            var listing = activeListings.Keys.FirstOrDefault(key => key.AuctionId == msg.AuctionID);

            if (listing == null)
            {
                print($"Can not find listing with AuctionID : {msg.AuctionID}");
                return;
            }

            ConfirmCancelListing(listing);
            
            PlayerManager.Instance.CheckForSet();
        }

        public void ConfirmCancelListing(Listing listing)
        {
            print("Auction removed");

            cancelOverlay.gameObject.SetActive(false);
            PlayerManager.Instance.AddCards(listing.Cards);
            Destroy(activeListings[listing]);
            activeListings.Remove(listing);
        }


        public void OpenBuyOverlay(string auctionId)
        {
            buyOverlay.Open(auctionId);
        }

        public void BuyListingRequest(string auctionId)
        {
            var sessionId = PlayerManager.Instance.LobbyID;
            var msg = new BuyCardsRequestMessage(DateTime.Now.ToString("o"), sessionId, PlayerManager.Instance.PlayerId,
                auctionId);

            NatsClient.C.Publish(sessionId.ToString(), msg);

            ConfirmBuyListing(auctionId);
            
            PlayerManager.Instance.CheckForSet();
        }

        public void ConfirmBuyListing(string auctionId)
        {
            print("Listing bought");

            var listing = activeListings.Keys.FirstOrDefault(key => key.AuctionId == auctionId);

            if (listing == null)
                return;

            listing.RemoveAllBiddings(false);

            buyOverlay.Current = "";
            buyOverlay.gameObject.SetActive(false);
            PlayerManager.Instance.AddCards(listing.Cards);
            PlayerManager.Instance.Balance -= listing.Price;
            balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";
            Destroy(activeListings[listing]);
            activeListings.Remove(listing);
        }

        public void SoldListing(string auctionId)
        {
            print("Listing Sold");

            var listing = activeListings.Keys.FirstOrDefault(key => key.AuctionId == auctionId);

            if (listing == null)
            {
                print($"Can not find listing with AuctionID : {auctionId}");
                return;
            }
            
            if (listing.Seller == PlayerManager.Instance.PlayerName)
            {
                PlayerManager.Instance.Balance += listing.Price;
                balanceDisplay.text = $"Balance: {PlayerManager.Instance.Balance}";
                
                NotificationList.Instance.AddNotification(
                    new Notification(
                        "Listing Sold", 
                        $"Your listing sold for {listing.Price}", 
                        null, 
                        NotificationColor.Green));
            }

            Destroy(activeListings[listing]);
            activeListings.Remove(listing);
        }

        public Listing GetListing(string auctionId)
        {
            return activeListings.Keys.FirstOrDefault(listing => listing.AuctionId == auctionId);
        }
    }
}
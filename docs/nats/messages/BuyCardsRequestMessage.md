## BuyCardsRequestMessage

This message sends a request to the server and the other player that this player want's to buy (a) certain card(s).

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string AuctionID;
```
Variables:\
`AuctionID` = AuctionID holds the auction id of a certain card(s).

Functions:\
`ToString()` = Shows the message in a human-readable text format.

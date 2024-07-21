## ConfirmBuyMessage

This message is sent by the host to confirm that a player has bought card(s).

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string AuctionID;
public int Seller;
public int Buyer;
public int Amount;
```
Variables:\
`AuctionID` = This ID is linked to an auction.\
`Seller` = The player ID of the seller of this auction.\
`Buyer` = The player ID of the buyer of this auction.\
`Amount` = The amount of where the card(s) were sold for.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
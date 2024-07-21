## BuyRequestMessage

This message sends message to confirm that it has bought a card that was for sale.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string AuctionID;
public int Amount;
public int OtherPlayer;
```
Variables:\
`AuctionID` = This ID is linked to an auction.\
`Amount` = The amount of money that a card is sold for.\
`OtherPlayer` = A reference to another player who is involved in the sold card.\

Functions:\
`ToString()` = Shows the message in a human-readable text format.
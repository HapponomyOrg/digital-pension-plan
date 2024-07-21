## ListCardMessage

This message is sent by a player that wants to sell certain amount of cards for an x amount of money.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string AuctionID;
public int[] Cards;
public int Amount;
```
Variables:\
`AuctionID` = AuctionID holds the auction id of a certain card(s), this id is created by the player.\
`Cards` = An array of cards that is put on the market, this also can contain only one card.\
`Amount` = The amount of money that you want to sell your card(s) for.\

Functions:\
`ToString()` = Shows the message in a human-readable text format.

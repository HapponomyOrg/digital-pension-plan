## ConfirmCancelListingMessage

This message is sent by the host to confirm the cancellation of a listing.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string AuctionID;
```
Variables:\
`AuctionID` = This ID is linked to an auction that the player wants to cancel.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
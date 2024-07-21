## ConfirmJoinMessage

This message is sent to the player when they want to join a lobby, and it is confirmed by the host.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int SessionPlayerID;
public string PlayerName;
```
Variables:\
`AuctionID` = The session id of the player, this is the id of the player that has been assigned by the host.\
`PlayerName` = The playername is linked to a playerid in this way.

Functions:\
`ToString()` = Shows the message in a human-readable text format.

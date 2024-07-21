## StartGameMessage

This message is sent by the host to every player in the session.\
It contains the start variables of every player.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int OtherPlayerID;
public int Balance;
public int[] Cards;
```
Variables:\
`OtherPlayerID` = The player id that these variables are for.\
`Balance` = The start balance for this player.\
`Cards` = The start cards of this player.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
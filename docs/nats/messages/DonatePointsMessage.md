## DonatePointsMessage

This message is sent by the player when they want to donate a certain amount of points to another player.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int Amount;
public int Reciever;
```
Variables:\
`Amount` = The amount of points that the player wants to donate to another player.
`Reciever` = The id of the other player where you want to donate the points to.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
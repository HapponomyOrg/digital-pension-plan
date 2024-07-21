## ConfirmHandInMessage

This message is sent by the host when the player wants to hand in cards.\
It returns an amount of points and 4 new cards.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int Receiver;
public int Points;
public int[4] Cards;
```
Variables:\
`Receiver` = The player that has handed in their cards.\
`Points` = Amount of points that are give to the player.\
`Cards` = 4 new cards that are returned to the player.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
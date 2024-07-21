## CardHandInMessage

This message informs players that one player has handed in their cards and received an amount of points
This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int[] Cards;
```
Variables:\
`Cards` = This is an array of card numbers that the player hands in, each card is connected to a value .\

Functions:\
`ToString()` = Shows the message in a human-readable text format.
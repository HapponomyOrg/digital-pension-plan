## StopRoundMessage

This message is sent by the host if he forced stopped a round or if the timer has run out at the host side.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int RoundNumber;
```
Variables:\
`RoundNumber` = The round number that the host has stopped.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
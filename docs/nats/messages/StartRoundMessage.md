## StartRoundMessage

This message is sent by the host if he pressed the StartRound button.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int RoundNumber;
public int Duration;
```
Variables:\
`RoundNumber` = The round number that the host has started.\
`Duration` = The amount of millis that the round timer has to do.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
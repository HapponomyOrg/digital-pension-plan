## HeartBeatMessage

This message is sent by the player in another thread.
This message is used by the host to show live data.
The client sends this message every 5 seconds but is configurable.
This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string PlayerName;
```
Variables:\
`PlayerName` = Playername is the name that the player fills in the form, two players cant have the same name.\

Functions:\
`ToString()` = Shows the message in a human-readable text format.
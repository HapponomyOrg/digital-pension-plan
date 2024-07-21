## JoinRequestMessage   

This message is sent by the player when he filled in the login form and requests to join.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string PlayerName;
```
Variables:\
`PlayerName` = Playername is the name that the player fills in the form, two players cant have the same name.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
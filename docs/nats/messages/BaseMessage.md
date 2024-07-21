## BaseMessage

Base message contains variables that are necessary to all messages.

Structure:
```
public string DateTimeStamp;
public string Subject;
public int LobbyID;
public int PlayerID;
```
Variables:\
`DateTimeStamp` = Date and time to make filtering easier.\
`Subject` = Subject of the [Nats](../nats.md) message.\
`LobbyID` = Contains the id of the lobby that the message is sent in.\
`PlayerID` = Contains the id of the player within the session.
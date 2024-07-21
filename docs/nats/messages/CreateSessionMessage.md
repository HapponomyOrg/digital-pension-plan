## CreateSessionMessage

This message is sent to the NATS but with no actual reciever.\
This mesage is only recieved by the yet to build analytics handler that creates a new record in the database for a new session.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public int SessionToken;
```
Variables:\
`SessionToken` = This Session Token is a generated int with 9 numbers that represents a lobby, this token is used to join a session and handle all of its messages but is not equivalent to the database id.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
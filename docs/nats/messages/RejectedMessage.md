## RejectedMessage

This message is a common rejected message.\
This message can be returned in multiple cases like : \
- When a player wants to buy a card that is no longer for sale.
- When a player wants to join with a name that is already taken.

This message implements [BaseMessage](BaseMessage.md).

Structure:
```
public string ReferenceID;
public string ErrorMessage;
```
Variables:\
`ReferenceID` = This Reference ID is used to store this error in the database to fix these errors in the future.\
`ErrorMessage` = ErrorMessage is a string which contains the error message as a string, so we could display this in the game.

Functions:\
`ToString()` = Shows the message in a human-readable text format.
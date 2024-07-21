## NATS

We make use of [UnityNats](https://github.com/barantaran/UnityNATS) to handle multiplayer connections and the transfer of data.

[NATS](https://nats.io/) is high-performance, extremely lightweight messaging system which makes use of subscribers and publishers roles.\
A client can publish messages with a subject on the Nats messages bus, Other clients can subscribe on such subjects and recieve all messages with these subjects.\
For local development we have created a [docker container](../docker/docker.md) which runs nats on port **4222**.

To connect with the nats messages service you need to hav an ip.
For local development we use nats://localhost:4222.
If you run nats somewhere else you will need its ip.

To increase performance of our game and networking we predefined messages.

All of our messages make use of a [BaseMessage](./messages/BaseMessage.md)

Whe have split up all of our messages: Client side and Host side. \
### Client
Gameplay
- [ListCardsMessage](./messages/ListCardsMessage.md) -> Player lists card(s) on the market.
- [BuyCardsRequestMessage](./messages/BuyCardsRequestMessage.md) -> Player requests to buy card(s) from market.
- [CancelListingMessage](./messages/CancelListingMessage.md) -> Cancels the listing of card(s).
- [DonateMoneyMessage](./messages/DonateMoneyMessage.md) -> Player donates money to charity.
- [DonatePointsMessage](./messages/DonatePointsMessage) -> Player donates pension point(s) to other player.
- [DeptUpdateMessage](./messages/DeptUpdateMessage.md) -> Player’s dept changes (either pays or increases)
- [CardHandInMessage](./messages/CardHandInMessage.md) -> Player hands in cards.
- [HeartBeatMessage](./messages/HeartBeatMessage.md) -> Sends player information to host every 5 seconds.

Lobby
- [JoinRequestMessage](./messages/JoinRequestMessage.md) -> Sends player information to host and wait for confirmation.

### Host
Hosting
- [ConfirmJoinMessage](./messages/ConfirmJoinMesage.md) -> The host confirms the join request of a player.
- [CreateSessionMessage](./messages/CreateSessionMessage.md) -> Creates a session with a certain id.
- [StartGameMessage](./messages/StartGameMessage.md) -> Starts a session with a certain id and gives the player their cards / money.
- [StartRoundMessage](./messages/StartRoundMessage.md) -> Starts the round when the host presses start round.
- [StopRoundMessage](./messages/StopRoundMessage.md) -> Stops the round after a timer or when the host presses a button.
- [EndOfRoundsMessage](./messages/EndOfRoundsMessage.md) -> Switches to the final stage of the game.
- [EndGameMessage](./messages/EndGameMessage.md) -> Force ends the game.
### Misc.
- [RejectedMessage](./messages/RejectedMessage.md) -> This is returned when there is an error and creates a pop up. 
- [ConfirmBuyMessage](./messages/ConfirmBuyMessage.md) -> Confirms a trade of card(s) that are listed and bought.
- [ConfirmHandInMessage](./messages/ConfirmHandInMessage.md) -> Confirms hand in and returns cards and points.
- [ConfirmCancelListingMessage](./messages/ConfirmCancelListingMessage.md) -> Confirms the cancellation of a listing.


## Issue:
During the development of the game we came across a problem.
If we want to build the game as a webgl version it is not possible to use the NATS library that we are currently using.
This library makes use of a TCP connection with an external NATS instance.
Due to security reasons webgl build do no support TCP connection and we have to work around this.
Due to time limitations we did not work on any of these solutions and just build the game for Windows but to make the 
game more accessible we want to build it for webgl.

A couple solutions are:

- TCP -> HTTP/Websocket Proxy.\
    What you can do is make a Proxy that proxies all the TCP calls to either an HTTP request or a Websocket connection.
  (NOTE) If you want to make the Websocket proxy you need to configure the NATS instance in the cloud environment to [accept websocket connections](https://docs.nats.io/running-a-nats-service/configuration/websocket/websocket_conf).
- Find another library (that makes use of the right .NET version).\
    We have looked into other libraries but the biggest issue is that this is one of the few libraries that support unity's .NET version.
    Maybe in the future the .NET version gets updated or there will be another library that supports Websocket/HTTP connections.
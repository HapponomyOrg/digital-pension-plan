using System;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Host;
using Version1.Websocket;

namespace Version1.Utilities
{
    public interface INetworkManager
    {
        event EventHandler<string> OnError;
        event EventHandler<RejectedMessage> OnRejected;
        void Subscribe(string topic);
        void Publish(string topic, BaseMessage message);
        WebsocketClient GetWsContext();
    }
}

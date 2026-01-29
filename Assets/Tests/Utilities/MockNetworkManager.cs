using System;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Tests.Utilities
{
    public class MockNetworkManager : INetworkManager
    {
        public event EventHandler<string> OnError;
        public event EventHandler<RejectedMessage> OnRejected;

        // Test helpers
        public bool SubscribeCalled { get; private set; }
        public string LastSubscribeTopic { get; private set; }

        public bool PublishCalled { get; private set; }
        public string LastPublishTopic { get; private set; }
        public object LastPublishMessage { get; private set; }

        public void Subscribe(string topic)
        {
            SubscribeCalled = true;
            LastSubscribeTopic = topic;
        }

        public void Publish(string topic, BaseMessage message)
        {
            PublishCalled = true;
            LastPublishTopic = topic;
            LastPublishMessage = message;
        }

        // Test helpers to trigger events
        public void TriggerError(string errorMessage)
        {
            OnError?.Invoke(this, errorMessage);
        }

        public void TriggerRejected(RejectedMessage message)
        {
            OnRejected?.Invoke(this, message);
        }

        public void Reset()
        {
            SubscribeCalled = false;
            PublishCalled = false;
            LastSubscribeTopic = null;
            LastPublishTopic = null;
            LastPublishMessage = null;
        }
    }
}

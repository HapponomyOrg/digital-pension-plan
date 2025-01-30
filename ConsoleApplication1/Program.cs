using System;
using System.Text;
using NATS;
using NATS.Client;
using UnityEngine;

namespace ConsoleApplication1
{     using System;
using System.Text;
using NATS.Client;

using NATS;  // Import your Unity project's namespace

namespace NatsConsoleApp
{
    class Program
    {
        private static IConnection NATSConnection;

        private static string natsUrl = "nats://localhost:4222"; // Replace with your NATS server URL

        static void Main(string[] args)
        {
            // Initialize NATS connection
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = natsUrl;

            try
            {
                NATSConnection = new ConnectionFactory().CreateConnection(opts);
                Console.WriteLine("Connected to NATS server");

                // Create and send a message
                var message = new AcceptBiddingMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
                    PlayerManager.Instance.PlayerId, "test'",
                    1000, 1);


                Publish("session123", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect or send message: " + ex.Message);
            }
            finally
            {
                // Close the connection
                NATSConnection?.Close();
                Console.WriteLine("Connection closed.");
            }
        }

        public static void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
        {
            string subject = sessionID + "." + baseMessage.Subject;

            string messageSerialized = JsonUtility.ToJson(baseMessage);
            byte[] messageEncoded = Encoding.UTF8.GetBytes(messageSerialized);

            NATSConnection.Publish(subject, messageEncoded);

            if (flushImmediately)
            {
                NATSConnection.Flush();
            }

            if (baseMessage.Subject == MessageSubject.HeartBeat)
                return;

            Console.WriteLine($"Message sent: {messageSerialized}, subject: {subject}");
        }
    }
}

}
using System;
using System.IO;
using Newtonsoft.Json;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TestConsole
{
    class Program
    {
        string targetChannel = "";
        static void Main(string[] args)
        {
            //Bot bot = new Bot();
            string json = File.ReadAllText(@"C:\Users\solar\source\repos\Send Twitch Message\Credentials.json");
            Console.WriteLine(json);
            ConnectionCredentials credentials = JsonConvert.DeserializeObject<ConnectionCredentials>(json);
            Console.WriteLine($"The credentials are \n username: {credentials.TwitchUsername}, \n authentication: " +
                $"{credentials.TwitchOAuth}, \n capabilities: {credentials.Capabilities}, \n websocketURI: {credentials.TwitchWebsocketURI}");
            string channelJoin = Console.ReadLine();

            Err

        }
    }

    

    class Bot
    {
        TwitchClient client;

        /// <summary>
        /// Instantiation of a twitch client that can send messages
        /// </summary>
        /// <param name="channel">The twitch channel to join upon the instantiation of the bot</param>
        public Bot(string channel)
        {
            //string json = File.ReadAllText(@"C:\Users\solar\source\repos\Send Twitch Message\Credentials.json");
            //ConnectionCredentials credentials = JsonConvert.DeserializeObject<ConnectionCredentials>(json);
            ConnectionCredentials credentials = new ConnectionCredentials("stemphy", "t67mjusqahbskv0h5e82szuum8euc8");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, channel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("badword"))
                client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}

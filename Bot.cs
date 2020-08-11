using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace LahmacBot_Twitch
{
    public class Bot
    {
        ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.ChannelName, TwitchInfo.BotToken);
        private TwitchClient _client;
        private ConsoleLogger _logger = new ConsoleLogger();

        public void Connect(bool isLogging)
        {
            _client = new TwitchClient();
            _client.Initialize(credentials, TwitchInfo.ChannelName);

            if (isLogging)
            {
                _client.OnLog += Client_OnLog;
            }

            _client.OnError += Client_OnError;

            _client.OnMessageReceived += Client_OnMessageReceived;

            _client.OnChatCommandReceived += Client_OnChatCommandReceived;

            _client.OnConnected += Client_OnConnected;

            _client.Connect();
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            switch (e.Command.CommandText.ToLower())
            {
                case "sa":
                    _client.SendMessage(TwitchInfo.ChannelName, "as"); 
                    break;
            }

            if (e.Command.ChatMessage.IsModerator || e.Command.ChatMessage.DisplayName == TwitchInfo.ChannelName)
            {
                if (e.Command.CommandText.Contains("blop")) _client.SendMessage(TwitchInfo.ChannelName, "blop");
            }
            
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            _logger.ChatMessageLogger(e.ChatMessage.DisplayName, e.ChatMessage.Message);

            if (e.ChatMessage.Message.ToLower().Contains("show feet"))
            {
                _client.SendMessage(TwitchInfo.ChannelName, "onlyfans.com");
            }
        }

        private void Client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
        {
            _logger.ErrorLog(e.Exception.Message);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            _logger.EventLog($"[Bot]; Connected to channel {e.AutoJoinChannel}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            _logger.Log(e.Data + " " + e.DateTime);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}
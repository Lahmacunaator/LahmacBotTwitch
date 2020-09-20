using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace LahmacBot_Twitch
{
    public class Bot
    {
        ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.ChannelName, TwitchInfo.BotToken);
        private TwitchClient _client;
        private TwitchAPI _api;
        private ConsoleLogger _logger = new ConsoleLogger();

        public void Connect(bool isLogging)
        {
            _client = new TwitchClient();
            _client.Initialize(credentials, TwitchInfo.ChannelName);
            _api = new TwitchAPI();

            _api.Settings.ClientId = TwitchInfo.ChannelName;
            _api.Settings.AccessToken = TwitchInfo.BotToken;

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
            var command = e.Command.CommandText.ToLower();
            var messageSender = e.Command.ChatMessage.DisplayName;
            var argumentList = e.Command.ArgumentsAsList;
            CommandHandler.HandleCommands(_client, command, _api, messageSender, argumentList);
            
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            _logger.ChatMessageLogger(e.ChatMessage.DisplayName, e.ChatMessage.Message);

            if (e.ChatMessage.Message.ToLower().Contains("show feet"))
            {
                CommandHandler.ShowFeet(_client);
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
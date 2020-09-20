using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;

namespace LahmacBot_Twitch
{
    internal class CommandHandler
    {
        public static async Task GetUptimeAndSendToChannel(TwitchClient client, TwitchAPI api)
        {
            var foundChannelResponse = await api.V5.Users.GetUserByNameAsync(TwitchInfo.ChannelName);
            var foundChannel = foundChannelResponse.Matches.FirstOrDefault();

            if (foundChannel is null) return;

            var online = await api.V5.Streams.BroadcasterOnlineAsync(foundChannel.Id);
            if (!online)
            {
                SendRpMessage($"{foundChannel.DisplayName} isn't streaming right now Ooops!", client);
                return;
            }

            var uptime = await api.V5.Streams.GetUptimeAsync(foundChannel.Id);

            if (!uptime.HasValue)
            {
                SendRpMessage("Error getting uptime :v", client);
                return;
            }

            SendRpMessage($"{foundChannel.DisplayName} Live for {uptime.Value.Hours} {(uptime.Value.Hours == 1 ? "hour" : "hours")} {uptime.Value.Minutes} {(uptime.Value.Minutes == 1 ? "minute" : "minutes")}.", client);
        }

        public static void ShowFeet(TwitchClient client)
        {
            var random = new Random();
            var rnd = random.Next(0, 2);

            switch (rnd)
            {
                case 0:
                    SendRpMessage("Look up at the stars and not down at your feet. Try to make sense of what you see, and wonder about what makes the universe exist. Be curious.", client);
                    SendRpMessage("- Stephen Hawking", client);
                    break;
                case 1:
                    SendRpMessage("onlyfans.com", client);
                    break;
                default:
                    SendMessage("nÖ", client);
                    break;
            }
        }

        public static void HandleCommands(TwitchClient client, string command, TwitchAPI api, string sender, List<string> argumentList)
        {
            switch (command)
            {
                case "sa":
                    SendMessage("as", client);
                    return;
                case "discord":
                    SendRpMessage("discord.gg/65ht8yp", client);
                    return;
                case "uptime":
                    GetUptime(client, api);
                    return;
                case "instagram":
                    SendRpMessage("instagram.com/bariscansoy", client);
                    return;
                case "uwu":
                    SendRpMessage("UwU", client);
                    return;
                case "coinflip":
                    //TODO
                    return;
                case "song":
                    DisplayCurrentlyPlayingSong(client);
                    return;
            }

            if (command.Contains("roll"))
            {
                RollDice(client, sender, argumentList);
            }
        }

        private static void DisplayCurrentlyPlayingSong(TwitchClient client)
        {
            var task = SpotifyStuff.DisplaySong();
            task.Wait();
            var result = task.Result;

            var artistsBuilder = new StringBuilder();

            foreach (var artist in result.item.artists)
            {
                artistsBuilder.Append(artist.name).Append(", ");
            }

            var artists = artistsBuilder.ToString();
            var displayText = $".me Now Playing: \t {result.item.name} \t by \t {artists} \t from the album \t {result.item.name}. \t ";
            var songLink = $".me Link: {result.item.external_urls.spotify}";

            SendMessage(displayText, client);
            SendMessage(songLink, client);
        }

        private static void RollDice(TwitchClient client, string sender, List<string> argList)
        {
            if (!argList.Any()) return;
            var rollStr = argList[0];

            var success = int.TryParse(rollStr, out var d);
            if (!success) return;
            var random = new Random();
            var roll = random.Next(1, d);
            SendRpMessage($"{sender} rolled {roll}!", client);
        }

        public static async void GetUptime(TwitchClient client, TwitchAPI api)
        {
            await GetUptimeAndSendToChannel(client, api);
        }

        private void CoinFlip(TwitchClient client)
        {
            var random = new Random();
            var result = random.Next(0, 2);
            SendRpMessage(result == 0 ? "Heads" : "Tails", client);
        }


        private static void SendMessage(string message, TwitchClient client)
        {
            client.SendMessage(TwitchInfo.ChannelName, message);
        }

        private static void SendRpMessage(string message, TwitchClient client)
        {
            client.SendMessage(TwitchInfo.ChannelName, $".me {message}");
        }
    }
}

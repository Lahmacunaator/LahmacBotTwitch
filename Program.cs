using System;

namespace LahmacBot_Twitch
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();

            bot.Connect(true);

            Console.ReadLine();

            bot.Disconnect();
        }
    }

    
}
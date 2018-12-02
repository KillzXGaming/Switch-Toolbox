using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox_Library
{
    /**
     * Code from the DiscordRpc examples:
     * https://github.com/Lachee/discord-rpc-csharp#usage
     */
    public class DiscordPresence
    {
        public DiscordRpcClient client;
        public String ClientID = "517901453935771668";

        public void Initialize()
        {
            client = new DiscordRpcClient(ClientID);

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();

            var timer = new System.Timers.Timer(150);
            timer.Elapsed += (sender, args) => { UpdatePresence(); };
            timer.Start();
        }

        void UpdatePresence()
        {
            client.SetPresence(new RichPresence()
            {
                Details = "Working on a mod",
                State ="",
                Assets = new Assets()
                {
                    LargeImageKey = "toolbox-logo",
                    LargeImageText = "Switch Toolbox"
                }
            });
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Mint_Chan
{
    internal class GlobalConfig
    {
        internal static SocketGuild? Server { get; set; }
        internal static DiscordSocketClient? Client { get; set; }

        internal static string token;
        internal static ulong guildID;

        static GlobalConfig()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddUserSecrets<GlobalConfig>();

            var configuration = configBuilder.Build();
            token = configuration["token"];
            guildID = ulong.Parse(configuration["GuildID"]);
        }
    }
}

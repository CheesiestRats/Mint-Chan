using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Mint_Chan
{
    internal class GlobalConfig
    {
        internal static SocketGuild Server { get; set; }
        internal static DiscordSocketClient Client { get; set; }

        internal static string token = "PUT_TOKEN_IN_HERE"; // Insert Bot Token Here
        internal static ulong guildID = 0; // Insert Guild ID here.
    }
}

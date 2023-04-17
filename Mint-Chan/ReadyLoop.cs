using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mint_Chan
{
    internal class ReadyLoop
    {
        internal static Task StartLoop()
        {
            GlobalConfig.Server = GlobalConfig.Client.GetGuild(GlobalConfig.guildID);
            if (GlobalConfig.Server != null) // Checks if bots are in the server
            {
                Console.WriteLine("| Server Detected: " + GlobalConfig.Server.Name);
            }
            else
            {
                Console.WriteLine($"| A server has not been defined.");
            }

            while (GlobalConfig.Server == null ||  GlobalConfig.Server.Name == null || GlobalConfig.Server.Name.Length < 1)
            {
                Console.WriteLine($"| Waiting for connection to be established by Discord...");
                Task.Delay(1200);
            }

            Program.botUserID = GlobalConfig.Client.CurrentUser.Id; // Bots user ID is detected and filled in automatically.

            Program.botName = GlobalConfig.Server.GetUser(Program.botUserID).Nickname ??
                              GlobalConfig.Server.GetUser(Program.botUserID).Username;

            return Task.CompletedTask;
        }
    }
}

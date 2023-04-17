using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mint_Chan
{
    internal class Functions
    {
        internal static string pingAndChannelTagDetectFilterRegexStr = @"<[@#]\d{15,}>";
        public static string FilterPingsAndChannelTags(string inputMsg)
        {
            Regex pingAndChannelTagDetectionRegex = new Regex(pingAndChannelTagDetectFilterRegexStr);
            // Replace pings and channel tags with their actual names
            var matches = pingAndChannelTagDetectionRegex.Matches(inputMsg);

            var uniqueMatches = matches.OfType<Match>().Select(m => m.Value).Distinct().ToList();

            foreach(var match in uniqueMatches)
            {
                string matchedTag = match;
                ulong matchedId = ulong.Parse(Regex.Match(matchedTag, @"\d+").Value);

                SocketGuildUser matchedUser;
                SocketGuildChannel matchedChannel;

                if (matchedTag.Contains("@"))
                {
                    try
                    {
                        matchedUser = GlobalConfig.Server.GetUser(matchedId);
                        inputMsg = inputMsg.Replace(matchedTag, $"@{matchedUser.Username}#{matchedUser.Discriminator}");
                    }
                    catch
                    {
                        break; // Not a real ID, break here.
                    }
                }else if (matchedTag.Contains("#"))
                {
                    try
                    {
                        matchedChannel = GlobalConfig.Server.GetChannel(matchedId);
                        inputMsg = inputMsg.Replace(matchedTag, $"#{matchedChannel.Name}");
                    }
                    catch
                    {
                        break; // Not a real ID, break here.
                    }
                }
                else
                {
                    break; // somehow escaped this function
                }
                return inputMsg;
            }
        }

        public static DateTime GetCurrentTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime currentEasternTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, easternTimeZone);
            return currentEasternTime;
        }
    }
}

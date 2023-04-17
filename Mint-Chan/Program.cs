using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace Mint_Chan
{
    internal class Program
    {
        private static Timer Loop; // A timer for the ReadyLoop

        private DiscordSocketClient _client; // Discord client variable

        internal static ulong botUserID; // Bots Client ID Number

        internal static string botName; // Bots Name

        internal static int oobaboogaThinking = 0; // Thinking Tick

        internal static int typing = 0; // If the bot is typing

        internal static int typingTicks = 0; // The ticks passing when the bot is thonking

        internal static int loopCounts = 0; // The number of loops that occured

        internal static string botLastReply = string.Empty; // The last reply the bot made

        internal static string oobServer = "127.0.0.1"; // The local address of webui server
        internal static int oobServerPort = 5000; // The port of the webui server

        internal static string oobAPIEndpoint = "/api/v1/generate"; // The API extension's endpoint bc oob api sucks ass

        public static bool longMsgWarningGiven = false; // Gives a warning for a long message but only once

        internal static string oobChatHistory = string.Empty; // <-- Chat history saves to this string over time.

        internal static bool chatHistoryDownloaded = false; // Records if we've downloaded the chat history before.

        internal static string oobInputPromptStart = $"";
        internal static string oobInputPromptEnd = $"[{botName}]: ";

        internal static string characterPrompt = $@""; // Anything we put here gets concatenated at the beginning of the prompt before the message history that loads in.

        internal static string oobInputPromptStartPic = $"\nAfter describing the image she took, {botName} may reply." +
            $"\nNouns of things in the photo: "; // Prints to the console when requesting an image to be taken.

        internal static string inputPromptEnding = $"\n[{botName}]: ";
        internal static string inputPromptEndingPic = $"\nAfter describing the image she took, {botName} may reply." +
            $"\nNouns of things in the photo: "; // Prints to the console when requesting an image to be taken.

        internal static string botReply = string.Empty; // The bots reply.

        internal static string token = string.Empty; // The bot token. This is set in the config.

        /* The Regex that alerts the bot to when you want it to take a photo, here is the breakdown: 
         * \b - matches a word boundary
         * (take|paint|generate|make|draw|create|show|...etc.) - Matches any of the words in the list
         * \b - Matches another woord boundary
         * (\S\s{0,10})? - optionally matches up to 10 words (seperated by whitespace) after the first matched word
         * (image|picture|painting|pic...etc) - Matches any word int he list
         * \b - Matches a word boundary
         */
        static internal string takeAPicRegexStr = @"\b(take|paint|generate|make|draw|create|show|give|snap|capture|send|display|share|shoot|see|provide|another)\b.*(\S\s{0,10})?(image|picture|painting|pic|photo|portrait|selfie)\b";

        /* This Regex detects when the end of the prompt occurs, here is the breakdown:
         * (?:\r\n?|\n): This matches a line break, which is often used to indicate the end of a message or prompt.
         * (?:(?![.\-*]).){2}: This matches any two characters that are not period, dash, or asterisk. This is used to match two consecutive characters that are not part of a common string used to indicate the end of a prompt.
         * (\n\[|\[end|<end|]:|>:|\[human|\[chat|\[sally|\[cc|<chat|<cc|\[@chat|\[@cc|bot\]:|<@chat|<@cc|\[.*]: |\[.*] : |\[[^\]]+\]\s*:|): This part of the pattern matches any of the following strings or characters:
         * A line break followed by a "[" character
         * The string "end"
         * The strings "<end", ":", or ">:"
         * Any of the strings "[human", "[chat", "[sally", "[cc", "<chat", "<cc", "[@chat", "[@cc", or "bot:"
         * Any string enclosed in square brackets, followed by a colon and optional whitespace
         */
        string promptEndDetectionRegexStr = @"(?:\r\n?|\n)(?:(?![.\-*]).){2}|(\n\[|\[end|<end|]:|>:|\[human|\[chat|\[sally|\[cc|<chat|<cc|\[@chat|\[@cc|bot\]:|<@chat|<@cc|\[.*]: |\[.*] : |\[[^\]]+\]\s*:)";

        // Link Detection.
        string linkDetectionRegexStr = @"[a-zA-Z0-9]((?i) dot |(?i) dotcom|(?i)dotcom|(?i)dotcom |\.|\. | \.| \. |\,)[a-zA-Z]*((?i) slash |(?i) slash|(?i)slash |(?i)slash|\/|\/ | \/| \/ ).+[a-zA-Z0-9]";

        // Regex object that matches to certain keywords as defined above, IgnoreCase ignores the capitalization.
        Regex takeAPicRegex = new Regex(takeAPicRegexStr, RegexOptions.IgnoreCase);



        // All the garbage we don't want discord logging
        private static readonly List<string> IgnoredSubStrings = new List<string>
        {
            "PRESENCE_UPDATE",
            "TYPING_START",
            "MESSAGE_CREATE",
            "MESSAGE_DELETE",
            "MESSAGE_UPDATE",
            "CHANNEL_UPDATE",
            "GUILD_",
            "REACTION_",
            "VOICE_STATE_UPDATE",
            "DELETE channels/",
            "POST channels/",
            "Heartbeat",
            "GET ",
            "PUT ",
            "Latency = ",
            "handler is blocking the"
        };

        private async Task AsyncMain()
        {
            try
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 1200, // Caches the number of messages in a channel
                    LogLevel = LogSeverity.Debug, // Keeps detailed Logs
                    AlwaysDownloadUsers = true, // Keeps track of ALL users
                    AlwaysResolveStickers = true, // Resolves Stickers
                    GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages
                });

                _client.Log += Client_Log;
                _client.Ready += ReadyLoop.StartLoop;
                _client.MessageReceived += Client_MessageRecieved;
                _client.GuildMemberUpdated += Client_GuildMemberUpdated;

                await _client.LoginAsync(TokenType.Bot, GlobalConfig.token);

                await _client.StartAsync();

                Loop = new Timer()
                {
                    Interval = 5900,
                    AutoReset = true,
                    Enabled = true
                };

            }
        }

        private Task? Client_GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
        {
            if(arg1.Value.Id == 438634979862511616)
            {
                if(arg2.Nickname == null || arg1.Value.Username != arg2.Username)
                {
                    botName = arg2.Username; // Sets a new username if no nickname is present
                }else if (arg1.Value.Nickname != arg2.Nickname)
                {
                    botName = arg2.Nickname; // Sets a new nickname
                }
            }
            return null;
        }

        private async Task Client_MessageRecieved(SocketMessage MsgParam)
        {
            try
            {
                var Msg = MsgParam as SocketUserMessage;
                var Context = new SocketCommandContext(_client, Msg);
                var user = Context.User as IGuildUser;

                string imagePresent = string.Empty;
                MatchCollection matches;

                // Get only unique matches
                List<string> uniqueMatches;

                // Downloads recent chat messages and puts them into the bot's memory.
                if(!chatHistoryDownloaded)
                {
                    chatHistoryDownloaded = true;

                    var downloadedMsges = await Msg.Channel.GetMessagesAsync(10).FlattenAsync();

                    // Get all users in the server at once instead of polling each user with a GetUser() call individually
                    var userDict = await Msg.Channel.GetUsersAsync().ToDictionaryAsync(u => u.Id, u => u);

                    foreach(var downloadedMsg in downloadedMsges.Where(m => m.Id != Msg.Id))
                    {
                        var downloadedMsgUser = userDict.GetValueOrDefault(downloadedMsg.Author.Id);

                        string downloadedMsgUserName = string.Empty;

                        if(downloadedMsgUser != null)
                        {
                            if(downloadedMsgUser.Nickname != null)
                                downloadedMsgUserName = downloadedMsgUser.Nickname;
                            else
                                downloadedMsgUserName = downloadedMsgUser.Username;
                        }

                        imagePresent = string.Empty;
                        if(downloadedMsg.Attachments.Count > 0)
                        {
                            // put something here so the bot knows an image was posted
                            imagePresent = "attachment.jpg>";
                        }
                        oobChatHistory = $"[{downloadedMsgUserName}]: {downloadedMsg.Content}{imagePresent}\n" + oobChatHistory;
                        oobChatHistory = Regex.Replace(oobChatHistory, linkDetectionRegexStr, "<url>");
                    }

                    oobChatHistory = Functions.FilterPingsAndChannelTags(oobChatHistory);
                }
            }
        }

        private void Client_Log(LogMessage msg)
        {
            if (msg.Message != null && !IgnoredSubStrings.Any(msg.Message.ToString().Contains))
            {
                Console.WriteLine($"|{DateTime.Now} - {msg.Source}| {msg.Message}");
            }
            else if (msg.Exception != null)
            {
                Console.WriteLine($"|{DateTime.Now} - {msg.Source}| {msg.Exception}");
            }
        }
    }
}
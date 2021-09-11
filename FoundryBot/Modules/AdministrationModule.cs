using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FoundryBot.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FoundryBot.Modules
{
    public class AdministrationModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<AdministrationModule> _logger;
        private readonly IHost _host;
        private readonly Color _successColor = new Color(98, 235, 52);
        private readonly Color _errorColor = new Color(235, 64, 52);
        // User IDs for Player Numbers
        private readonly Dictionary<string, ulong> _users = new Dictionary<string, ulong>
        {
            { "P1", 199384045996605441 },
            { "P2", 186300210664833024 },
            { "P3", 301921726030282762 },
            { "P4", 505901959610630185 },
            { "P5", 784633475579248652 },
            { "P6", 99262920935899136 },
            { "P7", 707448009096691785 },
            { "P8", 599512134128500737 },
            { "P9", 566788730573160448 },
            { "Gamemaster", 353645411438821377 },
            { "GM",         353645411438821377 }
        };

        public AdministrationModule(IHost host, ILogger<AdministrationModule> logger)
        {
            _host = host;
            _logger = logger;
        }

        [Command("mute")]
        //[RequireUserPermission(ChannelPermission.MuteMembers)]
        public async Task MuteChannel([Remainder] string channelName)
        {
            var message = Context.Message;

            // Ignore bot commands
            if (message.Source != MessageSource.User)
                return;

            // Get channel id from the channel name string.
            ulong channelid = await Context.Guild.Channels.ToAsyncEnumerable()
                .Where(x => x.Name == channelName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            // Get voice channel with id
            var channel = Context.Guild.GetVoiceChannel(channelid);

            if (channel is null)
            {
                await new EmbedBuilder()
                    .WithTitle("Could not mute users in voice channel.")
                    .WithDescription($"The channel \"{channelName}\" could not be found.")
                    .WithCurrentTimestamp()
                    .WithColor(_errorColor)
                    .BuildAndSendEmbed(Context.Channel);
                await message.DeleteAsync();
                return;
            }

            foreach (var channelUser in channel.Users)
            {
                if (_users.ContainsValue(channelUser.Id) && channelUser.Id != 353645411438821377)
                {
                    await channelUser.ModifyAsync(x => x.Mute = true);
                }
            }
            await new EmbedBuilder()
                    .WithTitle("Muted users in voice channel.")
                    .WithDescription($"All players in \"{channelName}\" were muted.\nThis will not mute users who come in after the command.")
                    .WithCurrentTimestamp()
                    .WithColor(_successColor)
                    .BuildAndSendEmbed(Context.Channel);
            await message.DeleteAsync();
        }

        [Command("unmute")]
        //[RequireUserPermission(ChannelPermission.MuteMembers)]
        public async Task UnmuteChannel([Remainder] string channelName)
        {
            var message = Context.Message;

            // Ignore bot commands
            if (message.Source != MessageSource.User)
                return;

            // Get channel id from the channel name string.
            ulong channelid = await Context.Guild.Channels.ToAsyncEnumerable()
                .Where(x => x.Name == channelName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            // Get voice channel with id
            var channel = Context.Guild.GetVoiceChannel(channelid);

            if (channel is null)
            {
                await new EmbedBuilder()
                    .WithTitle("Could not unmute users in voice channel.")
                    .WithDescription($"The channel \"{channelName}\" could not be found.")
                    .WithCurrentTimestamp()
                    .WithColor(_errorColor)
                    .BuildAndSendEmbed(Context.Channel);
                await message.DeleteAsync();
                return;
            }

            foreach (var channelUser in channel.Users)
            {
                if (_users.ContainsValue(channelUser.Id))
                {
                    await channelUser.ModifyAsync(x => x.Mute = false);
                }
            }
            await new EmbedBuilder()
                    .WithTitle("Unmuted users in voice channel.")
                    .WithDescription($"All players in \"{channelName}\" were unmuted.\nThis will not unmute users who come in after the command.")
                    .WithCurrentTimestamp()
                    .WithColor(_successColor)
                    .BuildAndSendEmbed(Context.Channel);
            await message.DeleteAsync();
        }

        [Command("move")]
        public async Task MoveUser([Remainder] string channelName)
        {
            // Change to get user id from the database
            ulong uid = _users[Context.User.Username];
            var user = Context.Guild.GetUser(uid);
            var message = Context.Message;

            // Ignore user commands, only read bot commands
            if (message.Source == MessageSource.User)
                return;

            if (user is null)
            {
                await new EmbedBuilder()
                    .WithTitle("Could not move user to voice channel.")
                    .WithDescription("The specified user was not found.")
                    .WithCurrentTimestamp()
                    .WithColor(_errorColor)
                    .BuildAndSendEmbed(Context.Channel);
                await message.DeleteAsync();
                return;
            }

            // Get the channel name from the input and cleanse input
            int firstIndex = channelName.IndexOf("\"") + 1;
            int lastIndex = channelName.LastIndexOf("\"");
            string cleanChannelName = channelName[firstIndex..lastIndex];

            // Get channel id from the channel name string.
            ulong channelid = await Context.Guild.Channels.ToAsyncEnumerable()
                .Where(x => x.Name == cleanChannelName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            // Get voice channel with id
            var channel = Context.Guild.GetVoiceChannel(channelid);

            if (channel is null)
            {
                await new EmbedBuilder()
                    .WithTitle("Could not move user to voice channel.")
                    .WithDescription($"The channel \"{channelName}\" could not be found.")
                    .WithCurrentTimestamp()
                    .WithColor(_errorColor)
                    .BuildAndSendEmbed(Context.Channel);
                await message.DeleteAsync();
                return;
            }

            if (user.VoiceChannel is null)
            {
                await new EmbedBuilder()
                    .WithTitle("Could not move user to voice channel.")
                    .WithDescription($"{user.Username} is not currently in a voice channel.")
                    .WithCurrentTimestamp()
                    .WithColor(_errorColor)
                    .BuildAndSendEmbed(Context.Channel);
                var embed = new EmbedBuilder()
                    .WithTitle("Could not move you to voice channel")
                    .WithDescription($"Foundry Assistant was unable to move you to the \"**{channel.Name}**\" voice channel.\nPlease join any voice channel in \"**The Nonary Game**\" server.")
                    .WithColor(_errorColor)
                    .WithCurrentTimestamp();
                await user.SendMessageAsync(embed: embed.Build());
                await message.DeleteAsync();
                return;
            }

            await user.ModifyAsync(x => x.Channel = channel);
            await new EmbedBuilder()
                    .WithTitle("Moved user to channel.")
                    .WithDescription($"Moved {user.Username} to {channel.Name}")
                    .WithCurrentTimestamp()
                    .WithColor(_successColor)
                    .BuildAndSendEmbed(Context.Channel);

            await message.DeleteAsync();
        }

        [Command("shutdown")]
        public async Task Stop()
        {
            if (Context.User.Id != 285106328790237195)
            {
                await ReplyAsync("You do not have sufficient permissions to use this command.\nAuthority Level: *[Bot Owner]*");
                return;
            }

            _ = _host.StopAsync();
        }

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel)Math.Abs((int)severity - 5);
    }
}

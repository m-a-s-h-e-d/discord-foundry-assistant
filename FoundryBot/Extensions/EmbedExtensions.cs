using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace FoundryBot.Extensions
{
    public static class EmbedExtensions
    {
        public static Task BuildAndSendEmbed(this EmbedBuilder builder, ISocketMessageChannel channel)
        {
            return channel.SendMessageAsync(embed: builder.Build());
        }

        public static Task BuildAndReplyEmbed(this EmbedBuilder builder, IUserMessage message)
        {
            return message.ReplyAsync(embed: builder.Build());
        }
    }
}

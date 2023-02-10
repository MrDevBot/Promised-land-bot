using System.Diagnostics.CodeAnalysis;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using PromisedLandDSPBot.Functions.Permissions;
using PromisedLandDSPBot.Logging;
using static PromisedLandDSPBot.Logging.Logger;

namespace PromisedLandDSPBot.Modules.Admin;
/// <summary>
/// This Module defines moderation functionality through the bot. (bans, kicks, etc.)
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Module
{
    [SlashCommandGroup("sudo", "Methods to assist server moderators."), RequireUserPermissions(Permissions.Administrator), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("ban", "bans a specified user")]
        [RequireGuild] [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx, 
            [Option("user", "the user you are attempting to ban")] DiscordUser targetUser,
            [Option("reason", "the reason you are banning this user")] string reason)
        {
            var target = ctx.Guild.GetMemberAsync(targetUser.Id);
            if(Hierarchy.Evaluate(ctx, ctx.Member, target.Result).Result && target.Result.IsOwner == false && (ctx.Member.Permissions & Permissions.BanMembers) != 0)
            {
                await ctx.Guild.BanMemberAsync(targetUser.Id, 7, reason);

                Log(Logger.Type.Command, $"User {ctx.User.Username} [{ctx.User.Id}] banned {targetUser.Username} [{targetUser.Id}]");
                await ctx.CreateResponseAsync($"Banned User {targetUser.Mention}");
            }
            else
            {
                Log(Logger.Type.Command, $"User {ctx.User.Username} [{ctx.User.Id}] failed to ban {targetUser.Username} [{targetUser.Id}]");
                await ctx.CreateResponseAsync($"Sorry, but you do not have permission to do that!");
            }
        }

        [SlashCommand("unban", "unbans a specified user")]
        [RequireGuild] [RequirePermissions(Permissions.BanMembers)]
        public async Task Unban(InteractionContext ctx, 
            [Option("id", "the id of the user you are attempting to unban")] DiscordUser targetUser,
            [Option("reason", "the id of the user you are attempting to unban")] string reason)
        {
            try
            {
                await ctx.Guild.UnbanMemberAsync(targetUser.Id, reason);
                await ctx.CreateResponseAsync($"user has been unbanned");
            }
            catch (DSharpPlus.Exceptions.NotFoundException)
            {
                await ctx.CreateResponseAsync(
                    "a ban for that user could not be found, please ensure you typed the **ID** or **@** correctly");
            }
            
        }
        
        [SlashCommand("kick", "bans a specified user")]
        [RequireGuild] [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(InteractionContext ctx, 
            [Option("user", "the user you are attempting to ban")] DiscordUser targetUser,
            [Option("reason", "the reason you are banning this user")] string reason)
        {
            var target = ctx.Guild.GetMemberAsync(targetUser.Id);
            if(Hierarchy.Evaluate(ctx, ctx.Member, target.Result).Result && target.Result.IsOwner == false && (ctx.Member.Permissions & Permissions.KickMembers) != 0)
            {
                await ctx.Guild.GetMemberAsync(targetUser.Id).Result.RemoveAsync();

                Log(Logger.Type.Command, $"User {ctx.User.Username} [{ctx.User.Id}] banned {targetUser.Username} [{targetUser.Id}]");
                await ctx.CreateResponseAsync($"Banned User {targetUser.Mention}");
            }
            else
            {
                Log(Logger.Type.Command, $"User {ctx.User.Username} [{ctx.User.Id}] failed to ban {targetUser.Username} [{targetUser.Id}]");
                await ctx.CreateResponseAsync($"Sorry, but you do not have permission to do that!");
            }
        }
    }
}
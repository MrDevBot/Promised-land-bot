using System.Diagnostics.CodeAnalysis;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using PromisedLandDSPBot.Functions.Permissions;
using Serilog;


namespace PromisedLandDSPBot.Modules.Admin;
/// <summary>
/// This Module defines moderation functionality through the bot. (bans, kicks, etc.)
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Module
{
    private const string ModuleName = "ADMIN";
    
    [SlashCommandGroup("sudo", "Methods to assist server moderators."), RequireUserPermissions(Permissions.Administrator), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("ban", "bans a specified user")]
        [RequireGuild] [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx, 
            [Option("user", "the user you are attempting to ban")] DiscordUser targetUser,
            [Option("reason", "the reason you are banning this user")] string reason)
        {
            Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) is attempting to ban {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId})", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
            Task<DiscordMember>? target = ctx.Guild.GetMemberAsync(targetUser.Id);
            if(Hierarchy.Evaluate(ctx, ctx.Member, target.Result).Result && target.Result.IsOwner == false && (ctx.Member.Permissions & Permissions.BanMembers) != 0)
            {
                await ctx.Guild.BanMemberAsync(targetUser.Id, 7, reason);

                Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) banned {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId})", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
                await ctx.CreateResponseAsync($"Banned User {targetUser.Mention}");
            }
            else
            {
                Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) failed to ban {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId}), they are higher in the hierarchy than the target user", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
                await ctx.CreateResponseAsync($"Sorry, but you do not have permission to do that! the user you are attempting to ban is higher in the role list than you are.");
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
                Log.Information($"[{Constants.Name}][{ModuleName}] user {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id}) unbanned {targetUser.Username}#{targetUser.Discriminator} ({targetUser.Id})");
                await ctx.CreateResponseAsync($"user has been unbanned");
            }
            catch (DSharpPlus.Exceptions.NotFoundException)
            {
                Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) failed to unban {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId}) as they are not banned.", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
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
            Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) is attempting to kick {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId})", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
            
            Task<DiscordMember>? target = ctx.Guild.GetMemberAsync(targetUser.Id);
            if(Hierarchy.Evaluate(ctx, ctx.Member, target.Result).Result && target.Result.IsOwner == false && (ctx.Member.Permissions & Permissions.KickMembers) != 0)
            {
                await ctx.Guild.GetMemberAsync(targetUser.Id).Result.RemoveAsync();
                Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) kicked {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId})", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
                await ctx.CreateResponseAsync($"Banned User {targetUser.Mention}");
            }
            else
            {
                Log.Information("[{Name}][{ModuleName}] user {UserUsername}#{UserDiscriminator} ({UserId}) kicked {TargetUserUsername}#{TargetUserDiscriminator} ({TargetUserId}), they are higher in the hierarchy than the target user", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id, targetUser.Username, targetUser.Discriminator, targetUser.Id);
                await ctx.CreateResponseAsync($"Sorry, but you do not have permission to do that! the user you are attempting to ban is higher in the role list than you are.");
            }
        }
    }
}
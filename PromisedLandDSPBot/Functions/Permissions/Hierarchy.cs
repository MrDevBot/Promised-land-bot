using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Functions.Permissions;

public class Hierarchy
{
    /// <summary>
    /// evaluates if a users permission level is greater than another users level on the role hierarchy
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="invoker"></param>
    /// <param name="target"></param>
    /// <param name="allowSameHierarchy"></param>
    /// <param name="allowSelf"></param>
    /// <returns></returns>
    public static Task<bool> Evaluate(CommandContext ctx, DiscordMember invoker, DiscordMember target, bool allowSameHierarchy = false, bool allowSelf = false, bool allowCurrent = false)
    {
        //if allow target self is false and target self, return false
        if (allowCurrent == false && target.Id == ctx.Client.CurrentUser.Id)
            return Task.FromResult(false);
                
        //if invoker is self and self targeting is enabled
        else if (invoker == target && allowSelf) //target is invoker, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if allow same hierarchy and same hierarchy 
        else if (allowSameHierarchy && invoker.Hierarchy == target.Hierarchy) //target is same hierarchy, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if the user id is listed under application owners in the discord development portal
        else if (ctx.Client.GetCurrentApplicationAsync().Result.Owners.Contains(invoker))
            return Task.FromResult(true);
        
        //if the invokers role hierarchy is higher than the targets
        return Task.FromResult(invoker.Hierarchy > target.Hierarchy); //this will return true / false. will not evaluate for members part of Application with Administrator or higher privilege
    }
    
    public static Task<bool> Evaluate(InteractionContext ctx, DiscordMember invoker, DiscordMember target, bool allowSameHierarchy = false, bool allowSelf = false, bool allowCurrent = false)
    {
        //if allow target self is false and target self, return false
        if (allowCurrent == false && target.Id == ctx.Client.CurrentUser.Id)
            return Task.FromResult(false);
        
        //if invoker is self and self targeting is enabled
        else if (invoker == target && allowSelf) //target is invoker, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if allow same hierarchy and same hierarchy 
        else if (allowSameHierarchy && invoker.Hierarchy == target.Hierarchy) //target is same hierarchy, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if the user id is listed under application owners in the discord development portal
        else if (ctx.Client.GetCurrentApplicationAsync().Result.Owners.Contains(invoker))
            return Task.FromResult(true);
        
        //if the invokers role hierarchy is higher than the targets
        return Task.FromResult(invoker.Hierarchy > target.Hierarchy); //this will return true / false. will not evaluate for members part of Application with Administrator or higher privilege
    }
}
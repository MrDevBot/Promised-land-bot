using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

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
    public static Task<bool> Evaluate(CommandContext ctx, DiscordMember invoker, DiscordMember target, bool allowSameHierarchy = false, bool allowSelf = false)
    {
        //if invoker is self and self targeting is enabled
        if (invoker == target && allowSelf) //target is invoker, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if allow same hierarchy and same hierarchy 
        if (allowSameHierarchy && invoker.Hierarchy == target.Hierarchy) //target is same hierarchy, check if this is allowed in argV
            return Task.FromResult(true);
        
        //if the user id is listed under application owners in the discord development portal
        if (ctx.Client.GetCurrentApplicationAsync().Result.Owners.Contains(invoker))
            return Task.FromResult(true);
        
        //if the invokers role hierarchy is higher than the targets
        return Task.FromResult(invoker.Hierarchy > target.Hierarchy); //this will return true / false. will not evaluate for members part of Application with Administrator or higher privilege
    }
}
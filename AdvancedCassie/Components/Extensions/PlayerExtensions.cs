using AdvancedCassie.Components.Features;
using CommandSystem;
using Exiled.API.Features;

namespace AdvancedCassie.Components.Extensions;

public static class PlayerExtensions
{
    public static Player AsPlayer(this ICommandSender sender)
        => Player.Get(sender);

    public static AdvancedCassieProperties AdvancedCassie(this Player player)
        => player.ReferenceHub.GetComponent<AdvancedCassieProperties>();
}
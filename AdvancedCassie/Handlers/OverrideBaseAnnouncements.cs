using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using PlayerRoles;

namespace AdvancedCassie.Handlers;

public class OverrideBaseAnnouncements
{
    public Dictionary<string, string> ScpLevels { get; set; } = new()
    {
        ["Scp-173"] = "[<color=yellow>Euclid</color>]",
        ["Scp-096"] = "[<color=yellow>Euclid</color>]",
        ["Scp-3114"] = "[<color=yellow>Euclid</color>]",
        ["Scp-079"] = "[<color=yellow>Euclid</color>]",
        ["Scp-049"] = "[<color=yellow>Euclid</color>]",
        ["Scp-939"] = "[<color=red>Keter</color>]",
        ["Scp-106"] = "[<color=red>Keter</color>]",
    };
    
    public Dictionary<string, CassieMessage> CassieMessages { get; set; } = new()
    {
        ["Игрок покинул сервер."] = new CassieMessage
        {
            Message = "SCP 1 7 3 Successfully terminated by target left",
            Subtitle = "успешно уничтожен. Игрок вышел с сервера."
        },
        [nameof(DamageType.Explosion)] = new CassieMessage
        {
            Message = "Terminated by explosion detonated",
            Subtitle = "успешно уничтожен в результате взрыва."
        },
        [nameof(DamageType.Unknown)] = new CassieMessage
        {
            Message = "Successfully terminated by termination cause Unspecified",
            Subtitle = "успешно уничтожен. Причина неизвестна."
        },
        [nameof(DamageType.Hypothermia)] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 2 4 4",
            Subtitle = "успешно уничтожен объектом Scp-244."
        },
        [nameof(DamageType.Decontamination)] = new CassieMessage
        {
            Message = "Lost in decontamination sequence",
            Subtitle = "утерян в процессе обеззараживания."
        },
        [nameof(DamageType.Warhead)] = new CassieMessage
        {
            Message = "Successfully terminated by Alpha Warhead",
            Subtitle = "успешно уничтожен боеголовкой Альфа."
        },
        [nameof(DamageType.Falldown)] = new CassieMessage
        {
            Message = "Successfully terminated by ground not detected",
            Subtitle = "успешно уничтожен. Падение с большой высоты."
        },
        [nameof(DamageType.Tesla)] = new CassieMessage
        {
            Message = "Successfully terminated by Automatic Security System",
            Subtitle = "успешно уничтожен Автоматической Системой Охраны."
        },
        [nameof(Team.ClassD)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit ClassD Personnel",
            Subtitle = "успешно сдержан персоналом Класса-Д."
        },
        [nameof(Team.Scientists)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit Science Personnel",
            Subtitle = "успешно сдержан научным персоналом."
        },
        [nameof(RoleTypeId.FacilityGuard)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit Facility Personnel",
            Subtitle = "успешно сдержан персоналом охраны."
        },
        [nameof(Team.FoundationForces)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit NineTailedFox",
            Subtitle = "успешно сдержан отрядом Девятихвостой Лисы."
        },
        [nameof(Team.ChaosInsurgency)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit ChaosInsurgency",
            Subtitle = "успешно сдержан Повстанцами Хаоса."
        }
    };
    
    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player.IsScp && ev.Player.Role != RoleTypeId.Scp0492)
        {
            var digits = ev.Player.Role.ToString().Where(char.IsDigit).ToArray();
            string spaced = "SCP " + string.Join(" ", digits);
            string dashed = "Scp-" + string.Concat(digits);
            
            ScpLevels.TryGetValue(dashed, out var scpLevel);
            
            if (CassieMessages.TryGetValue(ev.DamageHandler.Type.ToString(), out CassieMessage damageTypeTranslation))
            {
                LabApi.Features.Wrappers.Cassie.Message(spaced + " " + damageTypeTranslation.Message, true, true, true, dashed + $" {scpLevel} " + damageTypeTranslation.Subtitle);
                return;
            }

            if (ev.Attacker is not null && CassieMessages.TryGetValue(ev.Attacker.Role.Type.ToString(), out CassieMessage roleTypeTranslation))
            {
                LabApi.Features.Wrappers.Cassie.Message(spaced + " " + roleTypeTranslation.Message, true, true, true, dashed + $" {scpLevel} " + roleTypeTranslation.Subtitle);
                return;
            }

            if (ev.Attacker is not null && CassieMessages.TryGetValue(ev.Attacker.Role.Team.ToString(), out CassieMessage teamTranslation))
            {
                LabApi.Features.Wrappers.Cassie.Message(spaced + " " + teamTranslation.Message, true, true, true, dashed + $" {scpLevel} " + teamTranslation.Subtitle);
            }
        }
    }

    public void PlayerLeft(PlayerLeftEventArgs ev)
    {
        ev.Player.Kill("Игрок покинул сервер.");
    }
    
    public void OnAnnouncingScpTermination(CassieQueuingScpTerminationEventArgs ev)
    {
        ev.IsAllowed = false;
    }
    
    public class CassieMessage
    {
        public string Message { get; set; }
        public string Subtitle { get; set; }
    }
}
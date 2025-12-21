using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedCommands.Components.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using PlayerRoles;
using Respawning.Announcements;
using UnityEngine;

namespace AdvancedCassie.Handlers;

public class OverrideBaseAnnouncements
{
    public CassieMessage NtfAnnouncement { get; set; } = new CassieMessage()
    {
        Message =  "pitch_0.97 Mtfunit Epsilon 11 designated pitch_0.93 NineTailedFox pitch_0.97 hasentered . allremaining",
        
        Subtitle = "<color=#1E90FF>Мобильная Опергруппа</color> <size=30><b><color=#FFD700>Эпсилон-11</color></b></size>," +
                   " обозначенная как “<color=#FFD700>Девятихвостая лиса</color>”, вошла на территорию объекта... " +
                   "\nВсему оставшемуся персоналу рекомендуется следовать стандартным <b>протоколам эвакуации</b>," +
                   " пока <color=#1E90FF>отряд <b>МОГ</b></color> не прибудет к вашему местоположению... ",
    };
    
    public CassieMessage ChaosAnnouncement { get; set; } = new CassieMessage()
    {
        Message = ". Unauthorized units were detected at site of facility",
        Subtitle = "<color=#6aa84f>Неавторизованная группировка</color> обнаружены на территории комплекса...",
    };
    
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
        ["PlayerDisconnected"] = new CassieMessage
        {
            Message = "Successfully terminated by target left",
            Subtitle = "успешно уничтожен. Игрок покинул сервер."
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
        [nameof(DamageType.Scp018)] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 0 1 8",
            Subtitle = "успешно уничтожен объектом Scp-018."
        },
        [nameof(DamageType.Scp1507)] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 1 5 0 7",
            Subtitle = "успешно уничтожен объектом Scp-1507."
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

    public IEnumerator RecheckPlayerState(RoleTypeId leavedRole)
    {
        yield return new WaitForSeconds(1f);
        
        while (AdvancedCommands.Plugin.Instance.IwsHandler.IsLotteryProcessing)
            yield return new WaitForSeconds(1f);
        
        yield return new WaitForSeconds(1f);
        
        if (Player.List.FirstOrDefault(pl => pl.Role.Type == leavedRole) != null)
            yield break;
        
        AnnounceScpTermination(leavedRole);
    }
    
    public void OnDying(DyingEventArgs ev)
    {
        var userId = ev.Player.UserId;
        var digits = ev.Player.Role.ToString().Where(char.IsDigit).ToArray();
        string spaced = "SCP " + string.Join(" ", digits);
        string dashed = "Scp-" + string.Concat(digits);
        ScpLevels.TryGetValue(dashed, out var scpLevel);
        
        if (ev.Player.IsScp && ev.Player.Role != RoleTypeId.Scp0492)
        {
            Timing.CallDelayed(1f, () =>
            {
                if (Player.Get(userId) == null)
                    return;

                if (CassieMessages.TryGetValue(ev.DamageHandler.Type.ToString(),
                        out CassieMessage damageTypeTranslation))
                {
                    LabApi.Features.Wrappers.Cassie.Message(spaced + " " + damageTypeTranslation.Message, true,
                        true, true, dashed + $" {scpLevel} " + damageTypeTranslation.Subtitle);
                    return;
                }

                if (ev.Attacker is not null && CassieMessages.TryGetValue(ev.Attacker.Role.Type.ToString(),
                        out CassieMessage roleTypeTranslation))
                {
                    LabApi.Features.Wrappers.Cassie.Message(spaced + " " + roleTypeTranslation.Message, true, true,
                        true, dashed + $" {scpLevel} " + roleTypeTranslation.Subtitle);
                    return;
                }

                if (ev.Attacker is not null && CassieMessages.TryGetValue(ev.Attacker.Role.Team.ToString(),
                        out CassieMessage teamTranslation))
                {
                    LabApi.Features.Wrappers.Cassie.Message(spaced + " " + teamTranslation.Message, true, true,
                        true, dashed + $" {scpLevel} " + teamTranslation.Subtitle);
                }
            });
        }
    }

    private void AnnounceScpTermination(RoleTypeId leavedRole)
    {
        CassieMessages.TryGetValue("PlayerDisconnected", out var cassieMessage);

        if (cassieMessage == null)
            return;

        var digits = leavedRole.ToString().Where(char.IsDigit).ToArray();

        string spaced = "SCP " + string.Join(" ", digits);
        string dashed = "Scp-" + string.Concat(digits);
        ScpLevels.TryGetValue(dashed, out var scpLevel);

        LabApi.Features.Wrappers.Cassie.Message(spaced + " " + cassieMessage.Message, true, true, true,
            dashed + $" {scpLevel} " + cassieMessage.Subtitle);
    }
    
    public void PlayerLeft(LeftEventArgs ev)
    {
        if (ev.Player.IsScp && ev.Player.Role != RoleTypeId.Scp0492)
        {
            CoroutineRunner.Run(RecheckPlayerState(ev.Player.Role));
        }
    }
    
    public void OnAnnouncingScpTermination(CassieQueuingScpTerminationEventArgs ev)
    {
        ev.IsAllowed = false;
    }
    
    public void OnNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
    {
        ev.IsAllowed = false;
        LabApi.Features.Wrappers.Cassie.Message(NtfAnnouncement.Message, true, true, true, NtfAnnouncement.Subtitle);
    }
    
    public void OnChaosEntrance(AnnouncingChaosEntranceEventArgs ev)
    {
        ev.IsAllowed = false;
        LabApi.Features.Wrappers.Cassie.Message(ChaosAnnouncement.Message, true, true, true, ChaosAnnouncement.Subtitle);
    }
    
    public class CassieMessage
    {
        public string Message { get; set; }
        public string Subtitle { get; set; }
    }
}
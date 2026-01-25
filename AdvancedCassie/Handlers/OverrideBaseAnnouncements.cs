using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedCassie.Components.Extensions;
using AdvancedCommands.Components.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using PlayerRoles;
using Respawning.Announcements;
using Scp191.Components.Extensions;
using SerpentHands.Events.EventArgs.Round;
using SerpentHands.Extensions;
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
    
    public CassieMessage SerpentsHandAnnouncement { get; set; } = new CassieMessage()
    {
        Message = ".G2 .G3 Unauthorized $STUTTER_0.000_0.13_2 unit were .G5 $STUTTER_0.000_0.13_2 detected at site of " +
                  "facility .G4 target $STUTTER_0.000_0.13_2 initiated as Serpents Hand . take caution .G3 .G2",
        Subtitle = "<color=#FFA1D4>Неавторизованная группировка</color> обнаружена на территории комплекса.\n" +
                   "Субъекты определены как члены враждебной группировки <color=#FF1C73><b>Длань Змея</b></color>.\n" +
                   "Соблюдайте осторожность и избегайте встречи с субъектами!",
    };
    
    public Dictionary<string, string> ScpLevels { get; set; } = new()
    {
        ["Scp-173"] = "[<color=yellow>Euclid</color>]",
        ["Scp-096"] = "[<color=yellow>Euclid</color>]",
        ["Scp-3114"] = "[<color=yellow>Euclid</color>]",
        ["Scp-079"] = "[<color=yellow>Euclid</color>]",
        ["Scp-049"] = "[<color=yellow>Euclid</color>]",
        ["Scp-053"] = "[<color=yellow>Euclid</color>]",
        ["Scp-939"] = "[<color=red>Keter</color>]",
        ["Scp-106"] = "[<color=red>Keter</color>]",
        ["Scp-191"] = "[<color=green>Safe</color>]",
        ["Scp0492"] = "[<color=yellow>Euclid</color>]",
    };
    
    public Dictionary<string, CassieMessage> CassieMessages { get; set; } = new()
    {
        ["PlayerDisconnected"] = new CassieMessage
        {
            Message = "Successfully terminated by target left",
            Subtitle = "успешно уничтожен. Игрок покинул сервер." 
        },
        ["SerpentHands"] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit Serpents Hand",
            Subtitle = "успешно сдержан <color=#FF1C73>Дланью Змея.</color>"
        },
        ["Scp191"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 1 9 1",
            Subtitle = "успешно уничтожен объектом Scp-191 [<color=green>Safe</color>]."
        },
        ["Scp053"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 0 5 3",
            Subtitle = "успешно уничтожен объектом Scp-053 [<color=yellow>Euclid</color>]."
        },
        ["Scp939"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 9 3 9",
            Subtitle = "успешно уничтожен объектом Scp-939 [<color=red>Keter</color>]."
        },
        ["Scp049"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 0 4 9",
            Subtitle = "успешно уничтожен объектом Scp-049 [<color=yellow>Euclid</color>]."
        },
        ["Scp096"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 0 9 6",
            Subtitle = "успешно уничтожен объектом Scp-096 [<color=yellow>Euclid</color>]."
        },
        ["Scp173"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 1 7 3",
            Subtitle = "успешно уничтожен объектом Scp-173 [<color=yellow>Euclid</color>]."
        },
        ["Scp106"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 1 0 6",
            Subtitle = "успешно уничтожен объектом Scp-106 [<color=red>Keter</color>]."
        },
        ["Scp3114"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 3 1 1 4",
            Subtitle = "успешно уничтожен объектом Scp-3114 [<color=yellow>Euclid</color>]."
        },
        ["Scp0492"] = new CassieMessage
        {
            Message = "Successfully terminated by scpsubject 0 4 9 2",
            Subtitle = "успешно уничтожен объектом Scp-049-2 [<color=yellow>Euclid</color>]."
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
            Subtitle = "успешно сдержан <color=#F58E27>Персоналом Класса-Д.</color>"
        },
        [nameof(Team.Scientists)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit Science Personnel",
            Subtitle = "успешно сдержан <color=#FFEF63>Научным Персоналом.</color>"
        },
        [nameof(RoleTypeId.FacilityGuard)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit Facility Personnel",
            Subtitle = "успешно сдержан <color=#FFEF63>Персоналом Охраны.</color>"
        },
        [nameof(Team.FoundationForces)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit NineTailedFox",
            Subtitle = "успешно сдержан отрядом <color=#4F87FF>Девятихвостой Лисы.</color>"
        },
        [nameof(Team.ChaosInsurgency)] = new CassieMessage
        {
            Message = "ContainedSuccessfully ContainmentUnit ChaosInsurgency",
            Subtitle = "успешно сдержан <color=#1FA100>Повстанцами Хаоса.</color>"
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
    
    private string GetScpName(Player player)
    {
        if (player == null)
            return "Unknown";

        if (player.GetCustomRoles().Any(r => r.Id == 7) ||
            player.GetCustomRoles().Any(r => r.Id == 8) ||
            player.GetCustomRoles().Any(r => r.Id == 9))
        {
            return "Scp191";
        }

        if (player.GetCustomRoles().Any(r => r.Id == 10) ||
            player.GetCustomRoles().Any(r => r.Id == 11) ||
            player.GetCustomRoles().Any(r => r.Id == 12))
            return "Scp053";

        return player.Role.Type.ToString();
    }
    
    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player == null)
            return;

        if (ev.Player.IsNPC)
            return;

        if (ev.Player.Role.Type == RoleTypeId.ZombieFlamingo)
            return;

        if (!ev.Player.IsScp && !ev.Player.AdvancedCassie().PlayerProperties.IsCustomScp)
            return;

        if (ev.Player.Role.Type == RoleTypeId.Scp0492)
            return;

        var victimUserId = ev.Player.UserId;
        var victimScpName = GetScpName(ev.Player);

        var digits = victimScpName.Where(char.IsDigit).ToArray();
        var spaced = "SCP " + string.Join(" ", digits);
        var dashed = "Scp-" + string.Concat(digits);

        ScpLevels.TryGetValue(dashed, out var scpLevel);

        if (ev.Player.Role.Type == RoleTypeId.AlphaFlamingo)
        {
            spaced = "SCP 1 5 0 7 Alpha";
            dashed = "Scp-1507-Альфа";
            scpLevel = "[<color=yellow>Euclid</color>]";
        }

        Timing.CallDelayed(1f, () =>
        {
            var player = Player.Get(victimUserId);
            if (player == null || !player.IsDead)
                return;


            if (ev.Attacker != null)
            {
                var serpentProps = ev.Attacker.SerpentHandsProperties();

                if (serpentProps?.SerpentProps?.SerpentRole != null)
                {
                    if (CassieMessages.TryGetValue("SerpentHands", out var serpentMessage))
                    {
                        LabApi.Features.Wrappers.Cassie.Message(
                            spaced + " " + serpentMessage.Message,
                            true, true, true,
                            dashed + $" {scpLevel} " + serpentMessage.Subtitle
                        );
                        return;
                    }
                }

                var attackerScpName = GetScpName(ev.Attacker);

                if (CassieMessages.TryGetValue(attackerScpName, out var attackerScpMessage))
                {
                    LabApi.Features.Wrappers.Cassie.Message(
                        spaced + " " + attackerScpMessage.Message,
                        true, true, true,
                        dashed + $" {scpLevel} " + attackerScpMessage.Subtitle
                    );
                    return;
                }

                var attackerTeamKey = ev.Attacker.Role.Team.ToString();

                if (CassieMessages.TryGetValue(attackerTeamKey, out var teamMessage))
                {
                    LabApi.Features.Wrappers.Cassie.Message(
                        spaced + " " + teamMessage.Message,
                        true, true, true,
                        dashed + $" {scpLevel} " + teamMessage.Subtitle
                    );
                    return;
                }

                if (ev.Attacker.IsScp)
                {
                    LabApi.Features.Wrappers.Cassie.Message(
                        spaced + " Successfully terminated by scpsubject",
                        true, true, true,
                        dashed + $" {scpLevel} уничтожен другим SCP."
                    );
                    return;
                }
            }

            var damageKey = ev.DamageHandler.Type.ToString();

            if (CassieMessages.TryGetValue(damageKey, out var damageMessage)
                && ev.DamageHandler.Type != DamageType.Unknown)
            {
                LabApi.Features.Wrappers.Cassie.Message(
                    spaced + " " + damageMessage.Message,
                    true, true, true,
                    dashed + $" {scpLevel} " + damageMessage.Subtitle
                );
                return;
            }

            if (CassieMessages.TryGetValue(nameof(DamageType.Unknown), out var unknownMessage))
            {
                LabApi.Features.Wrappers.Cassie.Message(
                    spaced + " " + unknownMessage.Message,
                    true, true, true,
                    dashed + $" {scpLevel} " + unknownMessage.Subtitle
                );
            }
        });
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
        // SCP-191
        if (ev.Player.GetCustomRoles().Any(pl => pl.Id == 7 || pl.Id == 8 || pl.Id == 9))
        {
            CassieMessages.TryGetValue("PlayerDisconnected", out var cassieMessage);

            if (cassieMessage == null)
                return;
            
            string spaced = "SCP 1 9 1";
            string dashed = "Scp-191";
            ScpLevels.TryGetValue(dashed, out var scpLevel);

            LabApi.Features.Wrappers.Cassie.Message(spaced + " " + cassieMessage.Message, true, true, true,
                dashed + $" {scpLevel} " + cassieMessage.Subtitle);
            
            return;
        }
        
        // SCP-191
        if (ev.Player.GetCustomRoles().Any(pl => pl.Id == 10 || pl.Id == 11 || pl.Id == 12))
        {
            CassieMessages.TryGetValue("PlayerDisconnected", out var cassieMessage);

            if (cassieMessage == null)
                return;
            
            string spaced = "SCP 0 5 3";
            string dashed = "Scp-053";
            ScpLevels.TryGetValue(dashed, out var scpLevel);

            LabApi.Features.Wrappers.Cassie.Message(spaced + " " + cassieMessage.Message, true, true, true,
                dashed + $" {scpLevel} " + cassieMessage.Subtitle);
            
            return;
        }
        
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

    public void OnSerpentsHandEntrance(SerpentsHandRespawnedEventArgs ev)
    {
        LabApi.Features.Wrappers.Cassie.Message(
            SerpentsHandAnnouncement.Message, 
            true, true, true,
            SerpentsHandAnnouncement.Subtitle);
    }
    
    public class CassieMessage
    {
        public string Message { get; set; }
        public string Subtitle { get; set; }
    }
}
using System;
using AdvancedCassie.Components.Features;
using AdvancedCassie.Handlers;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using events = Exiled.Events.Handlers;
using lab = LabApi.Events.Handlers;

namespace AdvancedCassie
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "AdvancedCassie";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version RequiredExiledVersion => new(9, 1, 0);
        public override Version Version => new(2, 1, 0);
        
        public static Plugin Instance;
        public static Harmony Harmony;
        public OverrideBaseAnnouncements OverrideAnnouncements;
        
        public override void OnEnabled()
        {
            Instance = this;
            
            Harmony = new Harmony("ru.morkamo.advancedCassie.patches");
            Harmony.PatchAll();

            OverrideAnnouncements = Config.BaseAnnouncements;
            
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            
            OverrideAnnouncements = null;
            
            Harmony.UnpatchAll();
            Instance = null;
            
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            lab.ServerEvents.CassieQueuingScpTermination += OverrideAnnouncements.OnAnnouncingScpTermination;
            events.Player.Left += OverrideAnnouncements.PlayerLeft;
            events.Map.AnnouncingNtfEntrance += OverrideAnnouncements.OnNtfEntrance;
            events.Map.AnnouncingChaosEntrance += OverrideAnnouncements.OnChaosEntrance;
            events.Player.Dying += OverrideAnnouncements.OnDying;
            events.Player.Verified += OnVerified;
            SerpentHands.Events.EventManager.RoundEvents.SerpentsHandRespawned +=
                OverrideAnnouncements.OnSerpentsHandEntrance;
        }

        private void UnregisterEvents()
        {
            lab.ServerEvents.CassieQueuingScpTermination -= OverrideAnnouncements.OnAnnouncingScpTermination;
            events.Player.Left -= OverrideAnnouncements.PlayerLeft;
            events.Map.AnnouncingNtfEntrance -= OverrideAnnouncements.OnNtfEntrance;
            events.Map.AnnouncingChaosEntrance -= OverrideAnnouncements.OnChaosEntrance;
            events.Player.Dying -= OverrideAnnouncements.OnDying;
            events.Player.Verified -= OnVerified;
            SerpentHands.Events.EventManager.RoundEvents.SerpentsHandRespawned -=
                OverrideAnnouncements.OnSerpentsHandEntrance;
        }
        
        private void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player.IsNPC)
                return;
            
            if (ev.Player.ReferenceHub.gameObject.GetComponent<AdvancedCassieProperties>() != null)
                return;

            ev.Player.ReferenceHub.gameObject.AddComponent<AdvancedCassieProperties>();
        }
    }
}
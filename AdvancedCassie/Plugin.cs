using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using events = Exiled.Events.Handlers;

namespace AdvancedCassie
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "AdvancedCassie";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version RequiredExiledVersion => new(9, 1, 0);
        public override Version Version => new(1, 1, 0);

        public static Plugin Instance;
        public static Harmony Harmony;

        public override void OnEnabled()
        {
            Instance = this;
            
            Harmony = new Harmony("ru.morkamo.advancedCassie.patches");
            Harmony.PatchAll();
            
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            Harmony.UnpatchAll();
            Instance = null;
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            
        }

        private void UnregisterEvents()
        {
            
        }
    }
}
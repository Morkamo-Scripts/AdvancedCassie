using AdvancedCassie.Handlers;
using Exiled.API.Interfaces;

namespace AdvancedCassie
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        public OverrideBaseAnnouncements BaseAnnouncements { get; set; } = new();
    }
}
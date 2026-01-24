using AdvancedCassie.Components.Features.Components.Interfaces;
using UnityEngine;

namespace AdvancedCassie.Components.Features.Components;

public class PlayerProperties(AdvancedCassieProperties advancedCassieProperties) : IPropertyModule
{
    public AdvancedCassieProperties AdvancedCassieProperties { get; } = advancedCassieProperties;

    public bool IsCustomScp { get; set; } = false;
}
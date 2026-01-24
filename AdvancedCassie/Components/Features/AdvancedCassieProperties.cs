using AdvancedCassie.Components.Features.Components;
using Exiled.API.Features;
using UnityEngine;

namespace AdvancedCassie.Components.Features;

public sealed class AdvancedCassieProperties() : MonoBehaviour
{
    private void Awake()
    {
        Player = Player.Get(gameObject);
        PlayerProperties = new PlayerProperties(this);
    }
    
    public Player Player { get; private set; }
    public PlayerProperties PlayerProperties { get; private set; }
}
using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData {
    public float2 move;
    public float2 look;
    public float2 lookRaw;
    public boolean jump;
    internal boolean fire;
    internal boolean reload;
}


public class PlayerInputComponent : ComponentDataWrapper<PlayerInput> { }
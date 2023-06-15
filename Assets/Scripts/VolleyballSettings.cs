using UnityEngine;

public class VolleyballSettings : MonoBehaviour
{
    public float agentRunSpeed = 1.5f;
    public float agentJumpHeight = 2.75f;
    public float agentJumpVelocity = 777;
    public float agentJumpVelocityMaxChange = 10;

    // Slows down strafe & backward movement
    public float speedReductionFactor = 0.75f;
}

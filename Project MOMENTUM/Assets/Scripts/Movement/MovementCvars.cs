using UnityEngine;

[System.Serializable]
public class MovementCvars
{
    public float maxSpeed = 12.0f;

    [Header("Water")]
    public float waterSpeed = 10.0f;
    public float sinkSpeed = 1.25f;
    public float waterFriction = 0.8f;
    public float waterLevelToSwim = 0.35f;
    public float waterLevelToJumpOut = 0.15f;
    public float waterJumpOutPower = 12.0f;

    [Header("Noclip")]
    public float noclipSpeed = 30.0f;
    public float noclipFriction = 6.0f;

    [Header("Ladder")]
    public float climbingSpeed = 6.5f;

    [Header("Crouch")]
    public float duckMultiplier = 0.15f;
    public float checkRadius = 0.45f;
    public float checkHeight = 0.65f;
    public float playerHeight = 2.0f;
    public float crouchHeight = 1.0f; // The minimum capsule value (for (1, 2, 1))
    public float crouchTransitionSpeed = 4.0f;

    [Header("Other Variables")]
    public float airSpeedCap = 1.0f;
    public float slopeLimit = 45.0f;
    public float iceFriction = 0.5f;
    public float groundFriction = 6f;
    public float groundSpeed = 10.0f;
    public float airSpeed = 1.0f;
    public float jumpForce = 6.0f;
    public float acceleration = 10.0f;
    public float airAcceleration = 50.0f;
    public float shiftMultiplier = 0.2f;
}

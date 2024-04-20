using UnityEngine;

public static class BhopPhysics
{
    public static Vector3 GroundAccelerate(Vector3 wishvel, Vector3 wishdir, float wishspeed, float accel)
    {
        float curspeed = Vector3.Dot(wishvel, wishdir);

        float addspeed = wishspeed - curspeed;

        return Accelerate(wishdir, addspeed, accel, wishspeed);
    }

    public static Vector3 AirAccelerate(Vector3 wishvel, Vector3 wishdir, float wishspeed, float accel, float speedCap)
    {
        float wishspd = wishspeed;

        if (wishspd > speedCap)
            wishspd = speedCap;

        float currentspeed = Vector3.Dot(wishvel, wishdir);

        float addspeed = wishspd - currentspeed;

        return Accelerate(wishdir, addspeed, accel, wishspeed);
    }

    private static Vector3 Accelerate(Vector3 wishdir, float addspeed, float accel, float wishspeed)
    {
        if (addspeed <= 0)
            return Vector3.zero;

        float accelspeed = accel * wishspeed * Time.fixedDeltaTime;

        if (accelspeed > addspeed)
            accelspeed = addspeed;

        return wishdir * accelspeed;
    }
}

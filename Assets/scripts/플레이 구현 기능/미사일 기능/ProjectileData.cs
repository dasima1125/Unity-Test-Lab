using System.Numerics;

public struct ProjectileData2D
{
    public float ProjectileSpeed;
}
public struct GuidancePureData 
{
    public Vector2 targetPos;
}

public struct GuidanceLeadData 
{
    public Vector2 targetPos;
    public Vector2 targeVel;
    public float maxAngularVelocity; 
    
}

public struct GuidancePNData 
{
    public Vector2 targetPos;
    public Vector2 targeVel;
    public float maxAngularVelocity;
    public float PNgain;
}

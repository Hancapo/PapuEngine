using OpenTK.Mathematics;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;
using PhyVector3 = nkast.Aether.Physics2D.Common.Vector3;

namespace PapuEngine;

public class MiscHelper
{
    public static PhyVector2 ConvertVectorToAetherVector(Vector2 vector)
    {
        return new PhyVector2(vector.X, vector.Y);
    }

    public static Vector2 ConvertVectorToTkVector(PhyVector2 vector)
    {
        return new Vector2(vector.X, vector.Y);
    }

    public static PhyVector3 ConvertVectorToAetherVector(Vector3 vector)
    {
        return new PhyVector3(vector.X, vector.Y, vector.Z);
    }

    public static Vector3 ConvertVectorToTkVector(PhyVector3 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
}
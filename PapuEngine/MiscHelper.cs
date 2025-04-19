using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;
using PhyVector3 = nkast.Aether.Physics2D.Common.Vector3;

namespace PapuEngine;

public static class MiscHelper
{
    public static PhyVector2 ConvertVectorToAetherVector(Vector2D<float> vector)
    {
        return new PhyVector2(vector.X, vector.Y);
    }

    public static Vector2D<float> ConvertAetherVectorToVector(PhyVector2 vector)
    {
        return new Vector2D<float>(vector.X, vector.Y);
    }

    public static PhyVector3 ConvertVectorToAetherVector(Vector3D<float> vector)
    {
        return new PhyVector3(vector.X, vector.Y, vector.Z);
    }

    public static Vector3D<float> ConvertAetherVectorToVector(PhyVector3 vector)
    {
        return new Vector3D<float>(vector.X, vector.Y, vector.Z);
    }

    public static unsafe ReadOnlySpan<float> GetMatrixAsSpanUnsafe(Matrix2X2<float> matrix)
    {
        var localCopy = matrix;
        ref var firstElement = ref Unsafe.As<Matrix2X2<float>, float>(ref localCopy);
        return MemoryMarshal.CreateReadOnlySpan(ref firstElement, 4);
    }

    public static unsafe ReadOnlySpan<float> GetMatrixAsSpanUnsafe(Matrix3X3<float> matrix)
    {
        var localCopy = matrix;
        ref var firstElement = ref Unsafe.As<Matrix3X3<float>, float>(ref localCopy);
        return MemoryMarshal.CreateReadOnlySpan(ref firstElement, 9);
    }

    public static unsafe ReadOnlySpan<float> GetMatrixAsSpanUnsafe(Matrix4X4<float> matrix)
    {
        var localCopy = matrix;
        ref var firstElement = ref Unsafe.As<Matrix4X4<float>, float>(ref localCopy);
        return MemoryMarshal.CreateReadOnlySpan(ref firstElement, 16);
    }
}
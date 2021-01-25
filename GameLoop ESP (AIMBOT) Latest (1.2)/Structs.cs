using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PUBGMESP
{

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct FTTransform
    {
        [FieldOffset(0x00)]
        public Vector4 Rotation;
        [FieldOffset(0x10)]
        public Vector3 Translation;
        [FieldOffset(0x1C)]
        public Vector3 Scale3D;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct FTTransform2
    {
        [FieldOffset(0x00)]
        public Vector4 Rotation;
        [FieldOffset(0x10)]
        public Vector3 Translation;
        [FieldOffset(0x20)]
        public Vector3 Scale3D;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D3DMatrix
    {
        public float _11, _12, _13, _14;
        public float _21, _22, _23, _24;
        public float _31, _32, _33, _34;
        public float _41, _42, _43, _44;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {

        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w) : this()
        {
            this.W = w;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}

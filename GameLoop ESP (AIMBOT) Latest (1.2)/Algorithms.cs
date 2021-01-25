using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using ShpVector3 = SharpDX.Vector3;
using ShpVector2 = SharpDX.Vector2;
namespace PUBGMESP
{
    internal class Algorithms
    {
        /// <summary>
        /// Check if enemy is inside fov circle
        /// </summary>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool isInside(float circle_x, float circle_y,
                   float rad, float x, float y)
        {
            // Compare radius of circle with distance  
            // of its center from given point 
            if ((x - circle_x) * (x - circle_x) +
                (y - circle_y) * (y - circle_y) <= rad * rad)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get Distance Between Enemy And Player
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        /// <summary>
        /// Read View Matrix
        /// </summary>
        /// <param name="vAddv"></param>
        /// <returns></returns>
        public static D3DMatrix ReadViewMatrix(long vAddv) => Mem.ReadMemory<D3DMatrix>(vAddv);

        /// <summary>
        /// Read FTTransform
        /// </summary>
        /// <param name="vAddv"></param>
        /// <returns></returns>
        public static FTTransform ReadFTTransform(long vAddv) => Mem.ReadMemory<FTTransform>(vAddv);

        public static FTTransform2 ReadFTransform2(long vAddv) => Mem.ReadMemory<FTTransform2>(vAddv);

        /// <summary>
        /// Get Bone's world position
        /// </summary>
        /// <param name="actorAddv"></param>
        /// <param name="boneAddv"></param>
        /// <returns></returns>
        public static ShpVector3 GetBoneWorldPosition(long actorAddv, long boneAddv)
        {

            var bone = ReadFTransform2(boneAddv);
            var actor = ReadFTransform2(actorAddv);
            var boneMatrix = ToMatrixWithScale(bone.Translation, bone.Scale3D, bone.Rotation);
            var componentToWorldMatrix = ToMatrixWithScale(actor.Translation, actor.Scale3D, actor.Rotation);
            var newMatrix = MatrixMultiplication(boneMatrix, componentToWorldMatrix);
            ShpVector3 bonePos = new ShpVector3();
            bonePos.X = newMatrix._41;
            bonePos.Y = newMatrix._42;
            bonePos.Z = newMatrix._43;
            return bonePos;
        }

        /// <summary>
        /// To Matrix With Scale
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        private static D3DMatrix ToMatrixWithScale(Vector3 translation, Vector3 scale, Vector4 rot)
        {
            D3DMatrix m = new D3DMatrix
            {
                _41 = translation.X,
                _42 = translation.Y,
                _43 = translation.Z
            };

            float x2 = rot.X + rot.X;
            float y2 = rot.Y + rot.Y;
            float z2 = rot.Z + rot.Z;

            float xx2 = rot.X * x2;
            float yy2 = rot.Y * y2;
            float zz2 = rot.Z * z2;
            m._11 = (1.0f - (yy2 + zz2)) * scale.X;
            m._22 = (1.0f - (xx2 + zz2)) * scale.Y;
            m._33 = (1.0f - (xx2 + yy2)) * scale.Z;

            float yz2 = rot.Y * z2;
            float wx2 = rot.W * x2;
            m._32 = (yz2 - wx2) * scale.Z;
            m._23 = (yz2 + wx2) * scale.Y;

            float xy2 = rot.X * y2;
            float wz2 = rot.W * z2;
            m._21 = (xy2 - wz2) * scale.Y;
            m._12 = (xy2 + wz2) * scale.X;

            float xz2 = rot.X * z2;
            float wy2 = rot.W * y2;
            m._31 = (xz2 + wy2) * scale.Z;
            m._13 = (xz2 - wy2) * scale.X;

            m._14 = 0.0f;
            m._24 = 0.0f;
            m._34 = 0.0f;
            m._44 = 1.0f;

            return m;
        }

        /// <summary>
        /// D3DMatrix Mutiplication
        /// </summary>
        /// <param name="pM1"></param>
        /// <param name="pM2"></param>
        /// <returns></returns>
        public static D3DMatrix MatrixMultiplication(D3DMatrix pM1, D3DMatrix pM2)
        {
            D3DMatrix pOut = new D3DMatrix
            {
                _11 = pM1._11 * pM2._11 + pM1._12 * pM2._21 + pM1._13 * pM2._31 + pM1._14 * pM2._41,
                _12 = pM1._11 * pM2._12 + pM1._12 * pM2._22 + pM1._13 * pM2._32 + pM1._14 * pM2._42,
                _13 = pM1._11 * pM2._13 + pM1._12 * pM2._23 + pM1._13 * pM2._33 + pM1._14 * pM2._43,
                _14 = pM1._11 * pM2._14 + pM1._12 * pM2._24 + pM1._13 * pM2._34 + pM1._14 * pM2._44,
                _21 = pM1._21 * pM2._11 + pM1._22 * pM2._21 + pM1._23 * pM2._31 + pM1._24 * pM2._41,
                _22 = pM1._21 * pM2._12 + pM1._22 * pM2._22 + pM1._23 * pM2._32 + pM1._24 * pM2._42,
                _23 = pM1._21 * pM2._13 + pM1._22 * pM2._23 + pM1._23 * pM2._33 + pM1._24 * pM2._43,
                _24 = pM1._21 * pM2._14 + pM1._22 * pM2._24 + pM1._23 * pM2._34 + pM1._24 * pM2._44,
                _31 = pM1._31 * pM2._11 + pM1._32 * pM2._21 + pM1._33 * pM2._31 + pM1._34 * pM2._41,
                _32 = pM1._31 * pM2._12 + pM1._32 * pM2._22 + pM1._33 * pM2._32 + pM1._34 * pM2._42,
                _33 = pM1._31 * pM2._13 + pM1._32 * pM2._23 + pM1._33 * pM2._33 + pM1._34 * pM2._43,
                _34 = pM1._31 * pM2._14 + pM1._32 * pM2._24 + pM1._33 * pM2._34 + pM1._34 * pM2._44,
                _41 = pM1._41 * pM2._11 + pM1._42 * pM2._21 + pM1._43 * pM2._31 + pM1._44 * pM2._41,
                _42 = pM1._41 * pM2._12 + pM1._42 * pM2._22 + pM1._43 * pM2._32 + pM1._44 * pM2._42,
                _43 = pM1._41 * pM2._13 + pM1._42 * pM2._23 + pM1._43 * pM2._33 + pM1._44 * pM2._43,
                _44 = pM1._41 * pM2._14 + pM1._42 * pM2._24 + pM1._43 * pM2._34 + pM1._44 * pM2._44
            };

            return pOut;
        }

        /// <summary>
        /// Player's World To Screen Function
        /// </summary>
        /// <param name="vAddr"></param>
        /// <param name="pos"></param>
        /// <param name="screen"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <returns></returns>
        public static bool WorldToScreenPlayer(D3DMatrix viewMatrix, ShpVector3 pos, out ShpVector3 screen, out int distance, int windowWidth, int windowHeight)
        {
            screen = new ShpVector3();
            //ScreenW = (GameViewMatrix._14 * _Enemy_Point.x) + (GameViewMatrix._24* _Enemy_Point.y) + (GameViewMatrix._34 * _Enemy_Point.z + GameViewMatrix._44);
            float screenW = (viewMatrix._14 * pos.X) + (viewMatrix._24 * pos.Y) + (viewMatrix._34 * pos.Z + viewMatrix._44);
            distance = (int)(screenW / 100);
            if (screenW < 0.0001f) return false;

            // float ScreenY = (GameViewMatrix._12 * _Enemy_Point.x) + (GameViewMatrix._22 * _Enemy_Point.y) + (GameViewMatrix._32 * (_Enemy_Point.z + 85) + GameViewMatrix._42);
            float screenY = (viewMatrix._12 * pos.X) + (viewMatrix._22 * pos.Y) + (viewMatrix._32 * (pos.Z + 85) + viewMatrix._42);
            // float ScreenX = (GameViewMatrix._11 * _Enemy_Point.x) + (GameViewMatrix._21 * _Enemy_Point.y) + (GameViewMatrix._31 * _Enemy_Point.z + GameViewMatrix._41);
            float screenX = (viewMatrix._11 * pos.X) + (viewMatrix._21 * pos.Y) + (viewMatrix._31 * pos.Z + viewMatrix._41);
            screen.Y = (windowHeight / 2) - (windowHeight / 2) * screenY / screenW;
            screen.X = (windowWidth / 2) + (windowWidth / 2) * screenX / screenW;
            // float y1 = (pDxm->s_height / 2) - (GameViewMatrix._12*_Enemy_Point.x + GameViewMatrix._22 * _Enemy_Point.y + GameViewMatrix._32 *(_Enemy_Point.z - 95) + GameViewMatrix._42) *(pDxm->s_height / 2) / ScreenW;
            float y1 = (windowHeight / 2) - (viewMatrix._12 * pos.X + viewMatrix._22 * pos.Y + viewMatrix._32 * (pos.Z - 95) + viewMatrix._42) * (windowHeight / 2) / screenW;
            screen.Z = y1 - screen.Y;
            return true;
        }

        /// <summary>
        /// Bone's World to Screen Function
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="pos"></param>
        /// <param name="screen"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <returns></returns>
        public static bool WorldToScreenBone(D3DMatrix viewMatrix, ShpVector3 pos, out ShpVector2 screen, out int distance, int windowWidth, int windowHeight)
        => WorldToScreenItem(viewMatrix, pos, out screen, out distance, windowWidth, windowHeight);

        /// Item's World To Screen Function
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="pos"></param>
        /// <param name="screen"></param>
        /// <param name="distance"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <returns></returns>
        public static bool WorldToScreenItem(D3DMatrix viewMatrix, ShpVector3 pos, out ShpVector2 screen, out int distance, int windowWidth, int windowHeight)
        {
            screen = new ShpVector2();
            float screenW = (viewMatrix._14 * pos.X) + (viewMatrix._24 * pos.Y) + (viewMatrix._34 * pos.Z + viewMatrix._44);
            distance = (int)(screenW / 100);
            if (screenW < 0.0001f) return false;
            screenW = 1 / screenW;
            float sightX = (windowWidth / 2);
            float sightY = (windowHeight / 2);
            screen.X = sightX + (viewMatrix._11 * pos.X + viewMatrix._21 * pos.Y + viewMatrix._31 * pos.Z + viewMatrix._41) * screenW * sightX;
            screen.Y = sightY - (viewMatrix._12 * pos.X + viewMatrix._22 * pos.Y + viewMatrix._32 * pos.Z + viewMatrix._42) * screenW * sightY;
            return !float.IsNaN(screen.X) && !float.IsNaN(screen.Y);
        }


        public static bool WorldToScreen3DBox(D3DMatrix viewMatrix, ShpVector3 position, out ShpVector2 res, int sw, int sh)
        {
            res.X = 0f;
            res.Y = 0f;
            D3DMatrix matrix = viewMatrix;
            double num = (double)(position.X * matrix._14 + position.Y * matrix._24 + position.Z * matrix._34 + matrix._44);
            if (num < 0.100000001490116) return false;
            double num2 = (double)(position.X * matrix._11 + position.Y * matrix._21 + position.Z * matrix._31 + matrix._41);
            double num3 = (double)(position.X * matrix._12 + position.Y * matrix._22 + position.Z * matrix._32 + matrix._42);
            num2 /= num;
            num3 /= num;
            float num4 = (float)(sw / 2);
            float num5 = (float)(sh / 2);
            res.X = (float)((double)num4 * num2 + (num2 + (double)num4));
            res.Y = (float)(-(float)((double)num5 * num3) + (num3 + (double)num5));
            return !float.IsNaN(res.X) && !float.IsNaN(res.Y);
        }


    }
}

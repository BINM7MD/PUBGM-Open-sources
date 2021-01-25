using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBGMESP
{
    internal class Settings
    {
        public static bool PlayerESP = true;
        public static bool PlayerBone = true;
        public static bool PlayerBox = false ;
        public static bool Player3dBox = false;
        public static bool PlayerLines = true;
        public static bool PlayerHealth = true;
        public static bool ItemESP = false;
        public static bool VehicleESP = false;
        public static bool ShowMenu = true;
        public static bool Headmode = false;
        public static bool Waterbox = false;
        public static bool playername = true;
        public static bool ShowLootItem = true;
        public static bool FastLanding = false;
        public static bool NoRecoil = false;
        public static bool SpeedCar = false;
        public static bool INSTANTHIT = false;
        public static bool smallpointer = true;


        // aimbot
        public static bool aimEnabled = false;
        public static bool bDrawFow = false;
        public static int bSmooth = 11;
        public static int bFovInt = 4;
        public static int bPredict = 1;
        public static int bYAxis = 2;
        public static int bAimKeyINT = 2;
        public static string[] aimkeys = { "CAPSLOCK", "LBUTTON", "RBUTTON", "LSHIFT", "V", "E", "Q" };
        public static System.Windows.Forms.Keys[] bAimKeys = new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.CapsLock, System.Windows.Forms.Keys.LButton, System.Windows.Forms.Keys.RButton, System.Windows.Forms.Keys.LShiftKey, System.Windows.Forms.Keys.V, System.Windows.Forms.Keys.E, System.Windows.Forms.Keys.Q };
        public static float[] bFovArray = new float[]
        {
            60f,
            90f,
            120f,
            160f,
            300f,
            300f
        };

    }
}




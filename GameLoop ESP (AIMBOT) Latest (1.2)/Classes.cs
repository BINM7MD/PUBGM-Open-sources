using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using ShpVector3 = SharpDX.Vector3;
using ShpVector2 = SharpDX.Vector2;
namespace PUBGMESP
{
    public class DisplayData
    {
        public long ViewMatrixBase;
        public long myObjectAddress;
        public PlayerData[] Players;
        public ItemData[] Items;
        public VehicleData[] Vehicles;
        public BoxData[] Boxes;
        public GrenadeData[] Grenades;

        public DisplayData(long viewMatrixBase,long myObjectAddress)
        {
            this.ViewMatrixBase = viewMatrixBase;
            this.myObjectAddress = myObjectAddress;
        }
    }

    public class PlayerData
    {
        public string Type;
        public long Address;
        public int Status;
        public ShpVector3 Position;
        public int Pose;
        public float Health;
        public string Name;
        public bool IsRobot;
        public int TeamID;
        public int IsTeam;


        public ShpVector2 Screen2D;
        public float CrosshairDistance;
        public bool IsIn = false;
        public ShpVector3 Position3D;
    }

    public class ItemData
    {
        public Item Type;
        public ShpVector3 Position;
        public string Name;
    }

    public class VehicleData
    {
        public Vehicle Type;
        public ShpVector3 Position;
        public string Name;
        public int HP;
        public int Fuel;
    }

    public class BoxData
    {
        public string[] Items;
        public ShpVector3 Position;
    }

    public class GrenadeData
    {
        public Grenade Type;
        public ShpVector3 Position;
    }

    /// <summary>
    /// Item Type
    /// </summary>
    public enum Item
    {
        [Description("Useless")]
        Useless,
        [Description("Enegy Drink")]
        EnegyDrink,
        [Description("Epinephrine")]
        Epinephrine,
        [Description("Pain Killer")]
        PainKiller,
        [Description("First Aid Kit")]
        AidKit,
        [Description("Lv.3 Bag")]
        BagLv3,
        [Description("Lv.2 Bag")]
        BagLv2,
        [Description("Lv.2 Armor")]
        ArmorLv2,
        [Description("Lv.3 Armor")]
        ArmorLv3,
        [Description("Lv.3 Helmet")]
        HelmetLv3,
        [Description("Lv.2 Helmet")]
        HelmetLv2,
        [Description("AWM")]
        AWM,
        [Description("SCAR-L")]
        SCARL,
        [Description("Kar-98")]
        Kar98,
        [Description("M762")]
        M762,
        [Description("DP-28")]
        DP28,
        [Description("Groza")]
        Groza,
        [Description("AKM")]
        AKM,
        [Description("AUG")]
        AUG,
        [Description("QBZ")]
        QBZ,
        [Description("M249")]
        M249,
        [Description("M416")]
        M4A1,
        [Description("300 Magnum Ammo")]
        AmmoMagnum,
        [Description("7.62 Ammo")]
        Ammo762,
        [Description("5.56 Ammo")]
        Ammo556,
        [Description("4x Scope")]
        Scope4x,
        [Description("6x Scope")]
        Scope6x,
        [Description("8x Scope")]
        Scope8x,
        [Description("AR SUPPRESSOR")]
        RifleSilenter,
        [Description("AR Quick Extended Mag")]
        RifleMagazine,
        [Description("Ghillie Suit")]
        GhillieSuit,
        [Description("Flare Gun")]
        FlareGun,
        [Description("M24")]
        M24,
        [Description("SNIPER SUPPRESSOR")]
        SniperSilenter,
        [Description("Mk47")]
        Mk47,
        [Description("SKS")]
        SKS,
        [Description("Grenade")]
        Grenade,
        [Description("SLR")]
        SLR,
        [Description("Mini14")]
        Mini14,
        [Description("QBU")]
        QBU,
        [Description("G36")]
        G36,
        [Description("M16A4")]
        M16A4,
        [Description("Mk14")]
        Mk14,
        [Description("SawedOff")]
        SawedOff,
        [Description("S12K")]
        S12K,
        [Description("S686")]
        S686,
        [Description("S1897")]
        S1897,
        [Description("UZI")]
        Uzi,
        [Description("VSS")]
        VSS,
        [Description("Vector")]
        Vector,
        [Description("TommyGun")]
        TommyGun,
        [Description("UMP45")]
        UMP9,
        [Description("Pan")]
        Pan,

    }

    /// <summary>
    /// Grenade Type
    /// </summary>
    public enum Grenade
    {
        [Description("Unknown")]
        Unknown,
        [Description("Smoke Grenade")]
        Smoke,
        [Description("Cocktail")]
        Burn,
        [Description("Flash Grenade")]
        Flash,
        [Description("Grenade")]
        Explode
    }

    /// <summary>
    /// Vehicle Type
    /// </summary>
    public enum Vehicle
    {
        [Description("Unknown")]
        Unknown,
        [Description("BRDM")]
        BRDM,
        [Description("Scooter")]
        Scooter,
        [Description("Motorcycle")]
        Motorcycle,
        [Description("Motorcycle 3 Wheel")]
        MotorcycleCart,
        [Description("Snowmobile")]
        Snowmobile,
        [Description("Tuk")]
        Tuk,
        [Description("Buggy")]
        Buggy,
        [Description("Sports Car")]
        Sports,
        [Description("CAR")]
        Dacia,
        [Description("TRUCK")]
        Rony,
        [Description("PickUp")]
        PickUp,
        [Description("UAZ")]
        UAZ,
        [Description("MiniBus")]
        MiniBus,
        [Description("BOAT")]
        PG117,
        [Description("BOAT 2 SEAT")]
        AquaRail,
        [Description("AirPlane")]
        BP_AirDropPlane_C

    }

    /// <summary>
    /// Aimbot Position
    /// </summary>
    public enum AimPosition
    {
        [Description("Head")]
        Head,
        [Description("Chest")]
        Chest,
        [Description("Waist")]
        Waist
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static PUBGMESP.SigScanSharp;

namespace PUBGMESP
{


    public class SigScanSharp
    {
        public static JOYHAX ByPH;

        public class JOYHAX : IDisposable
        {
            [DllImport(@"C:\BypaPH.dll", EntryPoint = "CreateInstance")]
            static extern IntPtr BypaPH_ctor(uint pID);
            [DllImport(@"C:\BypaPH.dll", EntryPoint = "DeleteInstance")]
            static extern void BypaPH_dtor(IntPtr pInstance);
            [DllImport(@"C:\BypaPH.dll", EntryPoint = "RWVM")]
            static extern bool BypaPH_RWVM(IntPtr pInstance, UIntPtr BaseAddress, [Out] byte[] Buffer, UIntPtr BufferSize, out ulong NumberOfBytesReadOrWritten,
                                           bool read = true, bool unsafeRequest = false);
            IntPtr pInstance;
            public JOYHAX(int pID)
            {
                pInstance = BypaPH_ctor((uint)pID);
            }

          
            public bool ReadProcessMem(ulong BaseAddress, out byte[] Buffer, int BufferSize, out ulong NumberOfBytesRead)
            {
                byte[] buf = new byte[BufferSize];
                bool b = BypaPH_RWVM(pInstance, new UIntPtr(BaseAddress), buf, new UIntPtr((uint)buf.Length), out NumberOfBytesRead);

                Buffer = buf;
                return b;
            }
            public static byte[] magicByte = new byte[] {
            137, 21, 112, 0, 217, 3, 139, 85, 52, 162, 100, 0, 217, 3, 138, 130, 224, 253, 255, 255, 162, 104, 0,
            217, 3, 138, 130, 225, 253, 255, 255, 162, 108, 0, 217, 3, 160, 100, 0, 217, 3, 129, 61, 104, 0, 217,
            3, 220, 0, 0, 0, 116, 14, 139, 21, 112, 0, 217, 3, 199, 69, 104, 20, 131, 15, 41, 195, 129, 61, 108,
            0, 217, 3, 192, 0, 0, 0, 116, 2, 235, 228, 199, 2, 214, 23, 124, 62, 199, 66, 4, 156, 177, 199, 64, 235, 213 };
            public bool WriteProcessMem(ulong BaseAddress, byte[] Buffer, out ulong NumberOfBytesWittin)
            {
                return BypaPH_RWVM(pInstance, new UIntPtr(BaseAddress), Buffer, new UIntPtr((uint)Buffer.Length), out NumberOfBytesWittin, false);
            }
            public void Dispose()
            {
                BypaPH_dtor(pInstance);
            }
        }
        private IntPtr g_hProcess { get; set; }
        private byte[] g_arrModuleBuffer { get; set; }
        private long g_lpModuleBase { get; set; }

        private Win32.MEMORY_BASIC_INFORMATION MBI;

        private static byte question = (byte)'?';

        private Dictionary<string, string> g_dictStringPatterns { get; }

        public SigScanSharp(IntPtr hProc)
        {
            g_hProcess = hProc;
            g_dictStringPatterns = new Dictionary<string, string>();
        }

        public bool SelectModule(ProcessModule targetModule)
        {
            g_lpModuleBase = (long)targetModule.BaseAddress;
            g_arrModuleBuffer = new byte[targetModule.ModuleMemorySize];

            g_dictStringPatterns.Clear();

            return Win32.ReadProcessMemory(g_hProcess, g_lpModuleBase, g_arrModuleBuffer, targetModule.ModuleMemorySize);
        }

        public void AddPattern(string szPatternName, string szPattern)
        {
            g_dictStringPatterns.Add(szPatternName, szPattern);
        }

        private bool PatternCheck(int nOffset, byte[] arrPattern)
        {
            for (int i = 0; i < arrPattern.Length; i++)
            {
                if (arrPattern[i] == question)
                    continue;

                if (arrPattern[i] != this.g_arrModuleBuffer[nOffset + i])
                    return false;
            }

            return true;
        }

        private bool PatternCheck(int nOffset, byte[] arrPattern, byte[] source)
        {
            if (nOffset + arrPattern.Length > source.Length) return false;
            for (int i = 0; i < arrPattern.Length; i++)
            {
                if (arrPattern[i] == question)
                    continue;

                if (arrPattern[i] != source[nOffset + i])
                    return false;
            }

            return true;
        }

        public long FindPattern(string szPattern, out long lTime)
        {
            if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                throw new Exception("Selected module is null");

            Stopwatch stopwatch = Stopwatch.StartNew();

            byte[] arrPattern = ParsePatternString(szPattern);

            for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
            {
                if (this.g_arrModuleBuffer[nModuleIndex] != arrPattern[0])
                    continue;

                if (PatternCheck(nModuleIndex, arrPattern))
                {
                    lTime = stopwatch.ElapsedMilliseconds;
                    return g_lpModuleBase + (long)nModuleIndex;
                }
            }

            lTime = stopwatch.ElapsedMilliseconds;
            return 0;
        }

        public long[] FindPatternsAllRegion(string szPattern, long iStartAddress = 0, long iEndAddress = 0x7FFFFFFF)
        {
            byte[] arrPattern = ParsePatternString(szPattern);

            ulong num;

            long iAddress = iStartAddress;
            byte[] bBuffer;
            List<long> matchAddvs = new List<long>();
            do
            {
                int iRead = Win32.VirtualQueryEx(g_hProcess, (IntPtr)iAddress, out MBI, (uint)Marshal.SizeOf(MBI));
                if ((iRead > 0) && ((long)MBI.RegionSize > 0))
                {
                    //bBuffer = new byte[(long)MBI.RegionSize];
                    bBuffer = Mem.ReadMemory((long)MBI.BaseAddress, (int)MBI.RegionSize);
                    //Win32.ReadProcessMemory(g_hProcess, (long)MBI.BaseAddress, bBuffer, bBuffer.Length, ptrBytesRead);
                    //Win32.ReadProcessMemory(g_hProcess, (long)MBI.BaseAddress, bBuffer, bBuffer.Length, ptrBytesRead);
                    // ByPH.ReadProcessMem((ulong)MBI.BaseAddress, out bBuffer, bBuffer.Length, out num);

                    for (int i = 0; i < bBuffer.Length; i++)
                    {
                        if (bBuffer[i] != arrPattern[0]) continue;
                        if (PatternCheck(i, arrPattern, bBuffer))
                        {
                            matchAddvs.Add((long)(iAddress + i));
                            i += arrPattern.Length;
                        }
                    }
                }
                iAddress = (long)(MBI.BaseAddress.ToInt64() + MBI.RegionSize.ToInt64());
            } while (iAddress <= iEndAddress);
            return matchAddvs.ToArray();
        }

        public Dictionary<string, long> FindPatterns(out long lTime)
        {
            if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                throw new Exception("Selected module is null");

            Stopwatch stopwatch = Stopwatch.StartNew();

            byte[][] arrBytePatterns = new byte[g_dictStringPatterns.Count][];
            long[] arrResult = new long[g_dictStringPatterns.Count];

            // PARSE PATTERNS
            for (int nIndex = 0; nIndex < g_dictStringPatterns.Count; nIndex++)
                arrBytePatterns[nIndex] = ParsePatternString(g_dictStringPatterns.ElementAt(nIndex).Value);

            // SCAN FOR PATTERNS
            for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
            {
                for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                {
                    if (arrResult[nPatternIndex] != 0)
                        continue;

                    if (this.PatternCheck(nModuleIndex, arrBytePatterns[nPatternIndex]))
                        arrResult[nPatternIndex] = g_lpModuleBase + (long)nModuleIndex;
                }
            }

            Dictionary<string, long> dictResultFormatted = new Dictionary<string, long>();

            // FORMAT PATTERNS
            for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                dictResultFormatted[g_dictStringPatterns.ElementAt(nPatternIndex).Key] = arrResult[nPatternIndex];

            lTime = stopwatch.ElapsedMilliseconds;
            return dictResultFormatted;
        }

        private byte[] ParsePatternString(string szPattern)
        {
            List<byte> patternbytes = new List<byte>();

            foreach (var szByte in szPattern.Split(' '))
                patternbytes.Add(szByte == "?" ? (byte)'?' : Convert.ToByte(szByte, 16));

            return patternbytes.ToArray();
        }

        private static class Win32
        {
            [DllImport("kernel32.dll")]
            public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead = 0);

            [DllImport("kernel32.dll")]
            internal static extern Int32 VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out Win32.MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION
            {
                public IntPtr BaseAddress;
                public IntPtr AllocationBase;
                public uint AllocationProtect;
                public IntPtr RegionSize;
                public uint State;
                public uint Protect;
                public uint Type;
            }
        }
    }

    public class Mem
    {
        public static int m_iNumberOfBytesRead = 0;
        public static int m_iNumberOfBytesWritten = 0;
        public static Process m_Process;
        public static IntPtr m_pProcessHandle = IntPtr.Zero;
        public static Int64 BaseAddress;
        private const int PROCESS_VM_OPERATION = 8;
        private const int PROCESS_VM_READ = 16;
        private const int PROCESS_VM_WRITE = 32;

        public static void Initialize(string ProcessName)
        {
            if ((uint)Process.GetProcessesByName(ProcessName).Length > 0U)
            {
                Mem.m_Process = Process.GetProcessesByName(ProcessName)[0];
                Mem.BaseAddress = Process.GetProcessesByName(ProcessName)[0].MainModule.BaseAddress.ToInt64();
            }
            else
            {
                int num = (int)MessageBox.Show("Emulator should start first!!!");
                Environment.Exit(1);
            }
            Mem.m_pProcessHandle = Mem.OpenProcess(0x1F0FFF, false, Mem.m_Process.Id);
        }
        public static JOYHAX ByPH;
        public static void Initialize(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                ByPH = new JOYHAX((int)ptr);
                Mem.m_pProcessHandle = Mem.OpenProcess(0x1F0FFF, false, (int)ptr);
            }
        }

        public static Int64 GetModuleAdress(string ModuleName)
        {
            Int64 num;
            try
            {
                foreach (ProcessModule module in (ReadOnlyCollectionBase)Mem.m_Process.Modules)
                {
                    if (!ModuleName.Contains(".dll"))
                        ModuleName = ModuleName.Insert(ModuleName.Length, ".dll");
                    if (ModuleName == module.ModuleName)
                    {
                        num = (Int64)module.BaseAddress;
                        goto label_13;
                    }
                }
            }
            catch
            {
            }
            num = -1;
        label_13:
            return num;
        }

        public static string ReadString(Int64 address, int _Size)
        {
            return Encoding.Default.GetString(ReadMemory(address, _Size));
        }


        public static T ReadMemory<T>(Int64 Adress) where T : struct
        {
            ulong num;
            byte[] numArray = new byte[Marshal.SizeOf(typeof(T))];
            ByPH.ReadProcessMem((ulong)Adress, out numArray, numArray.Length, out num);
            //Mem.ReadProcessMemory((int)Mem.m_pProcessHandle, Adress, numArray, numArray.Length, ref Mem.m_iNumberOfBytesRead);
            return Mem.ByteArrayToStructure<T>(numArray);
        }

        public static byte[] ReadMemory(Int64 address, int _Size)
        {
            ulong num;
            byte[] numArray = new byte[_Size];
            ByPH.ReadProcessMem((ulong)address, out numArray, numArray.Length, out num);
            //Mem.ReadProcessMemory((int)Mem.m_pProcessHandle, address, numArray, _Size, ref Mem.m_iNumberOfBytesRead);
            return numArray;
        }

        public static float[] ReadMatrix<T>(Int64 Adress, int MatrixSize) where T : struct
        {
            byte[] numArray = new byte[Marshal.SizeOf(typeof(T)) * MatrixSize];
            ulong num;
            ByPH.ReadProcessMem((ulong)Adress, out numArray, numArray.Length, out num);
            return Mem.ConvertToFloatArray(numArray);
        }

        public static void WriteMemory<T>(Int64 Adress, object Value) where T : struct
        {
            byte[] byteArray = Mem.StructureToByteArray(Value);
            ulong num;
            ByPH.WriteProcessMem((ulong)Adress, byteArray, out num);
            //Mem.WriteProcessMemory((int)Mem.m_pProcessHandle, Adress, byteArray, byteArray.Length, out Mem.m_iNumberOfBytesWritten);
        }

        public static void WriteMemory(Int64 Adress, byte[] byteArray)
        {
            ulong num;
            ByPH.WriteProcessMem((ulong)Adress, byteArray, out num);
        }

        public static byte[] PatternToBytes(string pattern, int offset = 0)
        {
            var patternArr = pattern.Split(' ');
            List<byte> result = new List<byte>();
            for (int i = offset; i < patternArr.Length; i++)
            {
                if (patternArr[i] == "?") continue;
                result.Add(Convert.ToByte(patternArr[i], 16));
            }
            return result.ToArray();
        }

        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if ((uint)(bytes.Length % 4) > 0U)
                throw new ArgumentException();
            float[] numArray = new float[bytes.Length / 4];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = BitConverter.ToSingle(bytes, index * 4);
            return numArray;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle gcHandle = GCHandle.Alloc((object)bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                gcHandle.Free();
            }
        }

        public static IntPtr AllocateMemory(uint size)
        {
            return VirtualAllocEx(Mem.m_pProcessHandle, IntPtr.Zero, size, AllocationType.Commit, MemoryProtection.ReadWrite);
        }

        private static byte[] StructureToByteArray(object obj)
        {
            int length = Marshal.SizeOf(obj);
            byte[] destination = new byte[length];
            IntPtr num = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(obj, num, true);
            Marshal.Copy(num, destination, 0, length);
            Marshal.FreeHGlobal(num);
            return destination;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
    }

    public class GameMemSearch
    {
        SigScanSharp sgn;
        public GameMemSearch(SigScanSharp sgn)
        {
            this.sgn = sgn;
        }

        public long[] ViewWorldSearchCandidates(long startAddv = 0x26000000, long endAddv = 0x30000000)
        {
            long[] tmpViewWorlds = sgn.FindPatternsAllRegion(Patterns.viewWorldSearch, startAddv, endAddv);
            long[] viewWorlds = new long[tmpViewWorlds.Length];
            for (int i = 0; i < viewWorlds.Length; i++)
                viewWorlds[i] = tmpViewWorlds[i] - 32;
            return viewWorlds;
        }

        public long GetViewWorld(long[] cands)
        {
            long tmp;
            float t1, t2, t3, t4;
            for (int i = 0; i < cands.Length; i++)
            {
                tmp = Mem.ReadMemory<int>(Mem.ReadMemory<int>(cands[i]) + 32) + 512;
                t1 = Mem.ReadMemory<float>(tmp + 56);
                t2 = Mem.ReadMemory<float>(tmp + 40);
                t3 = Mem.ReadMemory<float>(tmp + 24);
                t4 = Mem.ReadMemory<float>(tmp + 8);
                if (t1 >= 3 && t2 == 0 && t3 == 0 && t4 == 0)
                {
                    return cands[i];
                }
            }
            return -1;
        }
    }

    public static class GameData
    {
        /// <summary>
        /// Tell if a entity is player
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPlayer(string str)
        {
            if (str.Contains("BP_PlayerPawn"))
                return true;
            return false;
        }


        /// <summary>
        /// Get Box Item From Item Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Item GetBoxItemType(long code)
        {
            if (code == 601001)
                return Item.EnegyDrink;
            if (code == 601002)
                return Item.Epinephrine;
            if (code == 601003)
                return Item.PainKiller;
            if (code == 601005)
                return Item.AidKit;
            if (code == 501006)
                return Item.BagLv3;
            if (code == 503003)
                return Item.ArmorLv3;
            if (code == 502003)
                return Item.HelmetLv3;
            if (code == 103003)
                return Item.AWM;
            if (code == 101003)
                return Item.SCARL;
            if (code == 103001)
                return Item.Kar98;
            if (code == 101008)
                return Item.M762;
            if (code == 105002)
                return Item.DP28;
            if (code == 101005)
                return Item.Groza;
            if (code == 101001)
                return Item.AKM;
            if (code == 101006)
                return Item.AUG;
            if (code == 101007)
                return Item.QBZ;
            if (code == 105001)
                return Item.M249;
            if (code == 101004)
                return Item.M4A1;
            if (code == 306001)
                return Item.AmmoMagnum;
            if (code == 302001)
                return Item.Ammo762;
            if (code == 303001)
                return Item.Ammo556;
            if (code == 203004)
                return Item.Scope4x;
            if (code == 203015)
                return Item.Scope6x;
            if (code == 203005)
                return Item.Scope8x;
            if (code == 201011)
                return Item.RifleSilenter;
            if (code == 204013)
                return Item.RifleMagazine;
            if (code == 403990 || code == 403187)
                return Item.GhillieSuit;
            if (code == 103009)
                return Item.SLR;
            if (code == 103006)
                return Item.Mini14;
            if (code == 103010)
                return Item.QBU;
            if (code == 101010)
                return Item.G36;
            if (code == 101002)
                return Item.M16A4;
            if (code == 103007)
                return Item.Mk14;
            if (code == 106006)
                return Item.SawedOff;
            if (code == 104003)
                return Item.S12K;
            if (code == 104001)
                return Item.S686;
            if (code == 104002)
                return Item.S1897;
            if (code == 205001)
                return Item.Uzi;
            if (code == 103005)
                return Item.VSS;
            if (code == 102003)
                return Item.Vector;
            if (code == 102004)
                return Item.TommyGun;
            if (code == 102002)
                return Item.UMP9;
            if (code == 108004)
                return Item.Pan;

            return Item.Useless;
        }


        /// <summary>
        /// Tell if an item is box
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBox(string str)
        {
            if (str.Contains("PlayerDeadInventoryBox") || str.Contains("PickUpListWrapperActor") || str.Contains("AirDrop"))
                return true;
            return false;
        }

        /// <summary>
        /// Get Grenade Type
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Grenade GetGrenadeType(string str)
        {
            if (str.Contains("BP_Grenade_Smoke_C"))
                return Grenade.Smoke;
            if (str.Contains("BP_Grenade_Burn_C"))
                return Grenade.Burn;
            if (str.Contains("BP_Grenade_tun_C"))
                return Grenade.Flash;
            if (str.Contains("BP_Grenade_Shoulei_C"))
                return Grenade.Explode;
            return Grenade.Unknown;
        }

        /// <summary>
        /// Get Vehicle Type
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vehicle GetVehicleType(string str)
        {
            if (str.Contains("BRDM"))
                return Vehicle.BRDM;
            if (str.Contains("Scooter"))
                return Vehicle.Scooter;
            if (str.Contains("Motorcycle"))
                return Vehicle.Motorcycle;
            if (str.Contains("MotorcycleCart"))
                return Vehicle.MotorcycleCart;
            if (str.Contains("Snowmobile"))
                return Vehicle.Snowmobile;
            if (str.Contains("Tuk"))
                return Vehicle.Tuk;
            if (str.Contains("Buggy"))
                return Vehicle.Buggy;
            if (str.Contains("open"))
                return Vehicle.Sports;
            if (str.Contains("close"))
                return Vehicle.Sports;
            if (str.Contains("Dacia"))
                return Vehicle.Dacia;
            if (str.Contains("Rony"))
                return Vehicle.Rony;
            if (str.Contains("UAZ"))
                return Vehicle.UAZ;
            if (str.Contains("MiniBus"))
                return Vehicle.MiniBus;
            if (str.Contains("PG117"))
                return Vehicle.PG117;
            if (str.Contains("AquaRail"))
                return Vehicle.AquaRail;
            if (str.Contains("BP_AirDropPlane_C"))
                return Vehicle.BP_AirDropPlane_C;
            //if (str.Contains("PickUp"))
            //{
            //    if (str.Contains("PickUp_BP"))
            //    {
            //        if (str != "PickUpListWrapperActor")
            //            return Vehicle.PickUp;
            //    }
            //}
            return Vehicle.Unknown;
        }

        /// <summary>
        /// Get Item's Type
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Item GetItemType(string str)
        {
            //if (!str.Contains("Pickup") || !str.Contains("PickUp"))
            //    return Item.Useless;
            if (str.Contains("Grenade_Shoulei_Weapon_Wra"))
                return Item.Grenade;
            if (str.Contains("MZJ_4X"))
                return Item.Scope4x;
            if (str.Contains("MZJ_6X"))
                return Item.Scope6x;
            if (str.Contains("MZJ_8X"))
                return Item.Scope8x;
            if (str.Contains("DJ_Large_EQ"))
                return Item.RifleMagazine;
            if (str.Contains("QK_Sniper_Suppressor"))
                return Item.SniperSilenter;
            if (str.Contains("QK_Large_Suppressor"))
                return Item.RifleSilenter;
            if (str.Contains("Ammo_556mm"))
                return Item.Ammo556;
            if (str.Contains("Ammo_762mm"))
                return Item.Ammo762;
            if (str.Contains("Ammo_300Magnum"))
                return Item.AmmoMagnum;
            if (str.Contains("Helmet_Lv3"))
                return Item.HelmetLv3;
            if (str.Contains("Armor_Lv3"))
                return Item.ArmorLv3;
            if (str.Contains("Bag_Lv3"))
                return Item.BagLv3;
            if (str.Contains("Helmet_Lv2"))
                return Item.HelmetLv2;
            if (str.Contains("Armor_Lv2"))
                return Item.ArmorLv2;
            if (str.Contains("Bag_Lv2"))
                return Item.BagLv2;
            if (str.Contains("Firstaid"))
                return Item.AidKit;
            if (str.Contains("Injection"))
                return Item.Epinephrine;
            if (str.Contains("Pills"))
                return Item.PainKiller;
            if (str.Contains("Drink"))
                return Item.EnegyDrink;
            if (!str.Contains("Wrapper"))
                return Item.Useless;
            if (str.Contains("Pistol_Flaregun"))
                return Item.FlareGun;
            if (str.Contains("AWM"))
                return Item.AWM;
            if (str.Contains("Kar98k"))
                return Item.Kar98;
            if (str.Contains("Mk47"))
                return Item.Mk47;
            if (str.Contains("DP28"))
                return Item.DP28;
            if (str.Contains("SKS"))
                return Item.SKS;
            if (str.Contains("Groza"))
                return Item.Groza;
            if (str.Contains("M762"))
                return Item.M762;
            if (str.Contains("AKM"))
                return Item.AKM;
            if (str.Contains("M249"))
                return Item.M249;
            if (str.Contains("M24"))
                return Item.M24;
            if (str.Contains("AUG"))
                return Item.AUG;
            if (str.Contains("QBZ"))
                return Item.QBZ;
            if (str.Contains("M416"))
                return Item.M4A1;
            if (str.Contains("SCAR"))
                return Item.SCARL;
            if (str.Contains("SLR"))
                return Item.SLR;
            if (str.Contains("Mini14"))
                return Item.Mini14;
            if (str.Contains("QBU"))
                return Item.QBU;
            if (str.Contains("G36"))
                return Item.G36;
            if (str.Contains("M16A4"))
                return Item.M16A4;
            if (str.Contains("Mk14"))
                return Item.Mk14;
            if (str.Contains("SawedOff"))
                return Item.SawedOff;
            if (str.Contains("S12K"))
                return Item.S12K;
            if (str.Contains("S686"))
                return Item.S686;
            if (str.Contains("S1897"))
                return Item.S1897;
            if (str.Contains("Uzi"))
                return Item.Uzi;
            if (str.Contains("VSS"))
                return Item.VSS;
            if (str.Contains("Vector"))
                return Item.Vector;
            if (str.Contains("TommyGun"))
                return Item.TommyGun;
            if (str.Contains("UMP9"))
                return Item.UMP9;
            if (str.Contains("Pan"))
                return Item.Pan;


            return Item.Useless;
        }

        /// <summary>
        /// Get Entity's Type
        /// </summary>
        /// <param name="gNames"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetEntityType(long gNames, long id)
        {
            string result = "";
            long gname = Mem.ReadMemory<int>(gNames);
            if (id > 0 && id < 2000000)
            {
                long page = id / 16384;
                long index = id % 16384;
                long secPartAddv = Mem.ReadMemory<int>(gname + page * 4);
                if (secPartAddv > 0)
                {
                    long nameAddv = Mem.ReadMemory<int>(secPartAddv + index * 4);
                    if (nameAddv > 0)
                    {
                        result = Mem.ReadString(nameAddv + 8, 32);
                    }
                }
            }
            return result;
        }


    }

    internal static class Utility
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static bool CheckFeasible(this Vector2 vec)
        {
            if (vec.X > 1 && vec.Y > 1)
                return true;
            return false;
        }
    }
}


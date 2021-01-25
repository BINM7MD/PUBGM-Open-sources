using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PUBGMESP.SigScanSharp;
using ShpVector3 = SharpDX.Vector3;
using ShpVector2 = SharpDX.Vector2;
using Microsoft.Win32;
using System.Net;

namespace PUBGMESP
{
    public partial class MainForm : Form
    {
        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);
        #region Modules
        SigScanSharp sigScan;
        GameMemSearch ueSearch;
        ESPForm espForm;
        #endregion

        #region Variables
        const string WINDOW_NAME = "腾讯手游助手【极速傲引擎】";
        const string WINDOW_NAME_G = "Gameloop【Turbo AOW Motoru】";
        IntPtr hwnd = IntPtr.Zero;
        RECT rect;
        long uWorld;
        long uWorlds;
        long uLevel;
        long gNames;
        long viewWorld;
        long gameInstance;
        long playerController;
        long playerCarry;
        int myTeamID;
        long uMyself;
        long myWorld;
        long uCamera;
        long uCursor;
        long uMyObject;
        Vector3 myObjectPos;
        long entityEntry;
        long entityCount;
        private bool arg1;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            comboBox1.Items.Add("FOV 1");
            comboBox1.Items.Add("FOV 2");
            comboBox1.Items.Add("FOV 3");
            comboBox1.Items.Add("FOV 4");
            comboBox1.Items.Add("FOV 5");



            comboBox2.Items.Add("CAPSLOCK");
            comboBox2.Items.Add("LBUTTON");
            comboBox2.Items.Add("RBUTTON");
            comboBox2.Items.Add("LSHIFT");
            comboBox2.Items.Add("V");
            comboBox2.Items.Add("E");
            comboBox2.Items.Add("Q");


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c sc stop KProcessHacker2 & sc delete KProcessHacker2";
            process.StartInfo = startInfo;
            process.Start();






        }

        private static void cmd(string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            process.Close();
        }
        private void InfoThread()
        {
            // offset
            int controllerOffset, posOffset, healthOffset, nameOffset, teamIDOffset, poseOffset, statusOffset;
            controllerOffset = 96;
            posOffset = 336;
            healthOffset = 2064;
            nameOffset = 1512;
            teamIDOffset = 1552;
            statusOffset = 868;
            poseOffset = 288;
            while (true)
            {
                // Read Basic Offset
                uWorlds = Mem.ReadMemory<int>(uWorld);
                uLevel = Mem.ReadMemory<int>(uWorlds + 32);
                gameInstance = Mem.ReadMemory<int>(uWorlds + 36);
                playerController = Mem.ReadMemory<int>(gameInstance + controllerOffset);
                playerCarry = Mem.ReadMemory<int>(playerController + 32);
                uMyObject = Mem.ReadMemory<int>(playerCarry + 792); //788 old value
                //uMyself = Mem.ReadMemory<int>(uLevel + 124);
                //uMyself = Mem.ReadMemory<int>(uMyself + 36);
                //uMyself = Mem.ReadMemory<int>(uMyself + 312);
                //uCamera = Mem.ReadMemory<int>(playerCarry + 804) + 832;
                //uCursor = playerCarry + 732;
                //myWorld = Mem.ReadMemory<int>(uMyObject + 312);
                //myObjectPos = Mem.ReadMemory<Vector3>(myWorld + posOffset);
                entityEntry = Mem.ReadMemory<int>(uLevel + 112);
                entityCount = Mem.ReadMemory<int>(uLevel + 120); //116 old value
                // Initilize Display Data
                DisplayData data = new DisplayData(viewWorld, uMyObject);
                List<PlayerData> playerList = new List<PlayerData>();
                List<ItemData> itemList = new List<ItemData>();
                List<VehicleData> vehicleList = new List<VehicleData>();
                List<BoxData> boxList = new List<BoxData>();
                List<GrenadeData> grenadeList = new List<GrenadeData>();
                for (int i = 0; i < entityCount; i++)
                {
                    long entityAddv = Mem.ReadMemory<int>(entityEntry + i * 4);
                    long entityStruct = Mem.ReadMemory<int>(entityAddv + 16);
                    string entityType = GameData.GetEntityType(gNames, entityStruct);
                    int shootOffset = 2400;
                    int shootSpeedOffset = 796;
                    int weaponOffset = 5400;
                 
                    long weaponGID = Mem.ReadMemory<int>(Mem.ReadMemory<int>(entityAddv + 4996) + 16);
                    string weaponType = GameData.GetEntityType(gNames, weaponGID);
                    Item weapon = GameData.GetItemType(weaponType);


                    //class VehicleCommon
                    //long VehicleCommon = Mem.ReadMemory<int>(entityAddv + 0x4E0);
                    long VehicleCommon = Mem.ReadMemory<int>(entityAddv + 0x5c4);
                    float currentHealth = Mem.ReadMemory<float>(VehicleCommon + 0x10C);

                    if (Settings.NoRecoil)
                    {
                        if (MainForm.GetAsyncKeyState(Keys.LButton))
                        {
                            int num10 = Mem.ReadMemory<int>(this.uMyObject + (long)5400);
                            int num11 = Mem.ReadMemory<int>((long)(num10 + 2400));
                            int num12 = Mem.ReadMemory<int>((long)(num11 + 828));
                            Mem.WriteMemory<float>((long)(num11 + 1624), 0f);
                            Mem.WriteMemory<float>((long)(num11 + 1628), 0f);
                            Mem.WriteMemory<float>((long)(num11 + 1632), 0f);
                            Mem.WriteMemory<float>((long)(Mem.ReadMemory<int>((long)(Mem.ReadMemory<int>(this.uMyObject + (long)num10) + num11)) + num12), 600000f);
                        }
                       /* var weaponOffSet = Mem.ReadMemory<int>(uMyObject + 5400);
                        int shootOffset = Mem.ReadMemory<int>(weaponOffSet + 2400);
                        Mem.WriteMemory<float>(shootOffset + 0x658, 0f);
                        Mem.WriteMemory<float>(shootOffset + 0x65c, 0f);
                        Mem.WriteMemory<float>(shootOffset + 0x660, 0f);*/
                    }
                    if (Settings.Headmode)
                    {
                        Mem.WriteMemory<float>(Mem.ReadMemory<int>(Mem.ReadMemory<int>(uMyObject + weaponOffset) + shootOffset) + shootSpeedOffset, 600000f);

                    }

                    if (Settings.INSTANTHIT)
                    {
                        var weaponOffSet = 5400;
                        //Mem.WriteMemory<float>(Mem.ReadMemory<int>(Mem.ReadMemory<int>(uMyObject + weaponOffset) + shootOffset) + shootSpeedOffset, 600000);
                        Mem.WriteMemory<float>(Mem.ReadMemory<int>(Mem.ReadMemory<int>(uMyObject + weaponOffSet) + shootOffset) + shootSpeedOffset, 600000f);

                    }

                    if (Settings.SpeedCar)
                    {
                        if (MainForm.GetAsyncKeyState(Keys.LShiftKey))
                        {
                            int num = Mem.ReadMemory<int>((long)(Mem.ReadMemory<int>((long)(Mem.ReadMemory<int>((long)(Mem.ReadMemory<int>((long)(Mem.ReadMemory<int>(this.uMyObject + 312L) + 1648)) + 1776)) + 404)) + 88));
                            Mem.WriteMemory<float>((long)num, 350f);
                            Mem.WriteMemory<float>((long)(num + 4), 350f);
                            Mem.WriteMemory<float>((long)(num + 8), 350f);
                            Mem.WriteMemory<float>((long)(num + 12), 350f);
                            Mem.WriteMemory<float>((long)(num + 16), 350f);
                            Mem.WriteMemory<float>((long)(num + 20), 350f);
                            Mem.WriteMemory<float>((long)(num + 24), 350f);
                            Mem.WriteMemory<float>((long)(num + 28), 350f);
                            Mem.WriteMemory<float>((long)(num + 32), 350f);
                            Mem.WriteMemory<float>((long)(num + 36), 350f);
                            Mem.WriteMemory<float>((long)(num + 40), 350f);
                            Mem.WriteMemory<float>((long)(num + 44), 350f);
                        }
                    }

                    if (Settings.FastLanding)
                    {
                        if (MainForm.GetAsyncKeyState(Keys.LShiftKey))
                        {
                            Mem.WriteMemory<float>((long)(Mem.ReadMemory<int>(this.uMyObject + (long)3136) + 464), 35000f);
                        }
                    }
                    if (Settings.PlayerESP)
                    {
                        // if entity is player
                        if (GameData.IsPlayer(entityType))
                        {
                            //Console.WriteLine(entityType);
                            long playerWorld = Mem.ReadMemory<int>(entityAddv + 312);
                            // read player info
                            // dead player continue
                            int status = Mem.ReadMemory<int>(playerWorld + statusOffset);

                            if (status == 6)
                                continue;

                            // Enemy weapon
                            var enemy_weapon = GameData.GetEntityType(gNames, Mem.ReadMemory<int>(Mem.ReadMemory<int>(entityAddv + 5124) + 16));
                            if (string.IsNullOrEmpty(enemy_weapon))
                                enemy_weapon = "Fist";
                            else
                            {
                                try
                                {
                                    var enemy_weaponList = enemy_weapon.Split('_').ToList();
                                    enemy_weapon = enemy_weaponList[2];
                                }
                                catch
                                {
                                    enemy_weapon = "Unknown";
                                }

                            }
                            Console.WriteLine(enemy_weapon);

                            string name = Encoding.Unicode.GetString(Mem.ReadMemory(Mem.ReadMemory<int>(entityAddv + nameOffset), 32));
                            name = name.Substring(0, name.IndexOf('\0'));
                            PlayerData playerData = new PlayerData
                            {
                                Type = entityType,
                                Address = entityAddv,
                                Position = Mem.ReadMemory<ShpVector3>(playerWorld + posOffset),
                                Status = status,
                                Pose = Mem.ReadMemory<int>(playerWorld + poseOffset),
                                IsRobot = Mem.ReadMemory<int>(entityAddv + 692) == 0 ? true : false,
                                Health = Mem.ReadMemory<float>(entityAddv + healthOffset),
                                Name = name,
                                TeamID = Mem.ReadMemory<int>(entityAddv + teamIDOffset),
                                //IsTeam = isTeam
                            };
                            if (playerData.Address == uMyObject || playerData.Address == uMyself)
                            {
                                myTeamID = playerData.TeamID;
                                continue;
                            }
                            if (playerData.TeamID == myTeamID)
                                continue;
                            //Console.WriteLine(entityType);
                            playerList.Add(playerData);
                            continue;
                        }
                    }
                    if (GameData.IsBox(entityType))
                    {
                        // Read Box Info
                        long boxEntity = Mem.ReadMemory<int>(entityAddv + 312);
                        BoxData boxData = new BoxData();
                        boxData.Position = Mem.ReadMemory<ShpVector3>(boxEntity + 336);
                        if (Settings.ShowLootItem)
                        {
                            long boxItemsCount = Mem.ReadMemory<int>(entityAddv + 1088);
                            if (boxItemsCount < 50 && boxItemsCount > 0)
                            {
                                List<string> boxItems = new List<string>();
                                long itemBase = Mem.ReadMemory<int>(entityAddv + 1084);
                                long itemAddv, ammoCount;
                                string itemName;
                                for (int j = 0; j < boxItemsCount; j++)
                                {
                                    itemAddv = itemBase + j * 48;
                                    Item boxItem = GameData.GetBoxItemType(Mem.ReadMemory<int>(itemAddv + 4));
                                    if (boxItem == Item.Useless) continue;
                                    ammoCount = Mem.ReadMemory<int>(itemAddv + 24);
                                    itemName = ammoCount > 0 ? string.Format("{0} * {1}", boxItem.GetDescription(), ammoCount) : boxItem.GetDescription();
                                    boxItems.Add(itemName);
                                }
                                boxData.Items = boxItems.ToArray();
                            }
                        }
                        boxList.Add(boxData);
                        continue;
                    }
                    if (Settings.ItemESP)
                    {
                        // check if this entity is item
                        Item item = GameData.GetItemType(entityType);
                        if (item != Item.Useless)
                        {
                            // Read Item Info
                            ItemData itemData = new ItemData
                            {
                                Name = item.GetDescription(),
                                Position = Mem.ReadMemory<ShpVector3>(Mem.ReadMemory<int>(entityAddv + 312) + posOffset),
                                Type = item
                            };
                            itemList.Add(itemData);
                        }
                        // check if this entity is box
                        if (GameData.IsBox(entityType))
                        {
                            // Read Box Info
                            long boxEntity = Mem.ReadMemory<int>(entityAddv + 312);
                            BoxData boxData = new BoxData();
                            boxData.Position = Mem.ReadMemory<ShpVector3>(boxEntity + posOffset);
                            boxList.Add(boxData);
                            continue;
                        }
                    }
                    if (Settings.VehicleESP)
                    {
                        Vehicle vehicle = GameData.GetVehicleType(entityType);
                        if (vehicle != Vehicle.Unknown)
                        {

                            //int vHP = 0x10C;
                            //int vHPMax = 0x108;
                            //int vFuel = 0x124;
                            //int vFuelMax = 0x120;

                            int vHP = 0x10C;
                            int vHPMax = 0x108;
                            int vFuel = 0x124;
                            int vFuelMax = 0x120;

                            float HP = Mem.ReadMemory<float>(VehicleCommon + vHP);

                            float HPMax = Mem.ReadMemory<float>(VehicleCommon + vHPMax);
                            int _HP = (int)(HP * 100 / HPMax);
                            float Fuel = Mem.ReadMemory<float>(VehicleCommon + vFuel);
                            float FuelMax = Mem.ReadMemory<float>(VehicleCommon + vFuelMax);
                            int _Fuel = (int)(Fuel * 100 / FuelMax);

                            // Read Vehicle Info
                            VehicleData vehicleData = new VehicleData
                            {
                                Position = Mem.ReadMemory<ShpVector3>(Mem.ReadMemory<int>(entityAddv + 312) + posOffset),
                                Type = vehicle,
                                Name = vehicle.GetDescription(),
                                HP = _HP,
                                Fuel = _Fuel,
                            };
                            vehicleList.Add(vehicleData);
                            continue;
                        }
                    }
                    // check if the entity is a grenade
                    Grenade grenade = GameData.GetGrenadeType(entityType);
                    if (grenade != Grenade.Unknown)
                    {
                        long grenadeEntity = Mem.ReadMemory<int>(entityAddv + 312);
                        GrenadeData greData = new GrenadeData
                        {
                            Type = grenade,
                            Position = Mem.ReadMemory<ShpVector3>(grenadeEntity + posOffset)
                        };
                        grenadeList.Add(greData);
                    }
                }
                data.Players = playerList.ToArray();
                data.Items = itemList.ToArray();
                data.Vehicles = vehicleList.ToArray();
                data.Boxes = boxList.ToArray();
                data.Grenades = grenadeList.ToArray();
                espForm.UpdateData(data);
                Thread.Sleep(10);
            }
        }

        private void ESPThread()
        {
            espForm.Initialize();
            while (true)
            {
                espForm.Update();
                //Thread.Sleep(10);
            }
        }
        private IntPtr FindTrueAndroidProcessHandle()
        {
            IntPtr AndroidProcessHandle = IntPtr.Zero;
            uint maxThread = 0;
            IntPtr handle = CreateToolhelp32Snapshot(0x2, 0);
            if ((int)handle > 0)
            {
                ProcessEntry32 pe32 = new ProcessEntry32();
                pe32.dwSize = (uint)Marshal.SizeOf(pe32);
                int bMore = Process32First(handle, ref pe32);
                while (bMore == 1)
                {
                    IntPtr temp = Marshal.AllocHGlobal((int)pe32.dwSize);
                    Marshal.StructureToPtr(pe32, temp, true);
                    ProcessEntry32 pe = (ProcessEntry32)Marshal.PtrToStructure(temp, typeof(ProcessEntry32));
                    Marshal.FreeHGlobal(temp);
                    if (pe.szExeFile.Contains("AndroidProcess.exe") && pe.cntThreads > maxThread)
                    {
                        maxThread = pe.cntThreads;
                        AndroidProcessHandle = (IntPtr)pe.th32ProcessID;
                    }

                    bMore = Process32Next(handle, ref pe32);
                }
                CloseHandle(handle);
            }
            return AndroidProcessHandle;
        }


        private bool EnableDebugPriv()
        {
            IntPtr hToken = IntPtr.Zero;
            if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken))
            {
                return false;
            }
            LUID luid = new LUID();
            if (!LookupPrivilegeValue(null, "SeDebugPrivilege", ref luid))
            {
                CloseHandle(hToken);
                return false;
            }
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES[1];
            tp.Privileges[0].Luid = luid;
            tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
            if (!AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                return false;
            }
            CloseHandle(hToken);
            return true;
        }



        #region WIN32 API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        [
         return: MarshalAs(UnmanagedType.Bool)
        ]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]


        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern bool GetAsyncKeyState(Keys vKey);

        private const int TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const int TOKEN_QUERY = 0x00000008;
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;

        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("kernel32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool CloseHandle(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public UInt32 Attributes;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }
        // Use this signature if you want the previous state information returned
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           UInt32 BufferLengthInBytes,
           IntPtr prev,
           IntPtr relen);

        [DllImport("KERNEL32.DLL ")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
        [DllImport("KERNEL32.DLL ")]
        public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);
        [DllImport("KERNEL32.DLL ")]
        public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        #endregion

        private void Loop_Tick(object sender, EventArgs e)
        {
            GetWindowRect(hwnd, out rect);
            if (espForm != null)
            {
                espForm._window.FitToWindow(hwnd, true);
            }
        }

        private void Update_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(Keys.End))
            {

                System.Environment.Exit(-1);
            }
            if (GetAsyncKeyState(Keys.End))
            {
                
                this.Hide();
                this.Close();
            }
            if (GetAsyncKeyState(Keys.Home))
            {
                Settings.ShowMenu = !Settings.ShowMenu;
            }
            if (GetAsyncKeyState(Keys.F2))
            {
                Settings.PlayerESP = !Settings.PlayerESP;
            }
            if (GetAsyncKeyState(Keys.F3))
            {
                Settings.Headmode = !Settings.Headmode;
            }



            if (GetAsyncKeyState(Keys.F5))
            {
                Settings.Waterbox = !Settings.Waterbox;
            }
            if (GetAsyncKeyState(Keys.F6))
            {
                Settings.INSTANTHIT = !Settings.INSTANTHIT;
            }
            if (GetAsyncKeyState(Keys.F4))
            {
                Settings.aimEnabled = !Settings.aimEnabled;
                
            }
            if (GetAsyncKeyState(Keys.F9))
            {
                
                Settings.SpeedCar = !Settings.SpeedCar;
            }
            if (GetAsyncKeyState(Keys.F7))
            {
                Settings.FastLanding = !Settings.FastLanding;
            }
            if (GetAsyncKeyState(Keys.F8))
            {
                Settings.NoRecoil = !Settings.NoRecoil;
            }
            if (GetAsyncKeyState(Keys.F10))
            {
                Settings.smallpointer = !Settings.smallpointer;
            }
        }
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop ESP
           // System.Environment.Exit(-1);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);

            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine($@"netsh advfirewall reset");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();

            }



            if (!File.Exists("C:\\SEJ.bat"))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://www.joycheat.com/wp-content/uploads/2020/10/SEJ.bat", "C:\\SEJ.bat");
                }
            }
            Process.Start(@"C:\SEJ.bat");
            base.Close();
            this.Close();
            base.Close();
            /* string filepath = (@"C:\Windows\System32\drivers\etc\hosts");

             if (File.Exists(filepath))
             {
                 File.Delete(filepath);
             }*/
            using (WebClient webClient = new WebClient())
            {
                // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox12_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/10/hosts-1"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                //MessageBox.Show("THANKS FOR USING JOY X");
            }
            System.Environment.Exit(-1);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }



        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }





        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Text = ("CHOSE FOVE");
            if (comboBox1.SelectedIndex == 0)
            {
                Settings.bFovInt = 1;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                Settings.bFovInt = 2;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                Settings.bFovInt = 3;
            }
            if (comboBox1.SelectedIndex == 3)
            {
                Settings.bFovInt = 4;
            }
            if (comboBox1.SelectedIndex == 4)
            {
                Settings.bFovInt = 5;
            }


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                Settings.bAimKeyINT = 0;
            }
            if (comboBox2.SelectedIndex == 1)
            {
                Settings.bAimKeyINT = 1;
            }
            if (comboBox2.SelectedIndex == 2)
            {
                Settings.bAimKeyINT = 2;
            }
            if (comboBox2.SelectedIndex == 3)
            {
                Settings.bAimKeyINT = 3;
            }
            if (comboBox2.SelectedIndex == 4)
            {
                Settings.bAimKeyINT = 4;
            }
            if (comboBox2.SelectedIndex == 5)
            {
                Settings.bAimKeyINT = 5;
            }
            if (comboBox2.SelectedIndex == 6)
            {
                Settings.bAimKeyINT = 6;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Settings.playername = true;
            }
            else
            {
                Settings.playername = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine($@"netsh advfirewall reset");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();

            }
            using (WebClient webClient = new WebClient())
            {
                // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox12_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/10/hosts-1"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                //MessageBox.Show("THANKS FOR USING JOY X");
            }
            {

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
               // process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block enable=yes remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255,113.105.0.0-113.105.255.255,219.133.0.0-219.133.255.255");
               // process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=in action=block program = D:\Pubgm2\TxGameAssistant\UI\AndroidEmulator.exe enable = yes");
               // process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMediaL dir=out action=block program=D:\Pubgm2\TxGameAssistant\UI\androidemulator.exe enable=yes");
              //  process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block program=D:\Pubgm2\TxGameAssistant\UI\QQPCExternal.exe enable=yes");
              //  process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block program=D:\Pubgm2\TxGameAssistant\UI\TSettoutgCenter enable=yes");
              //  process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block program=D:\Pubgm2\TxGameAssistant\UI\TxGaDcc.exe enable=yes");
              //  process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block program=D:\Pubgm2\TxGameAssistant\UI\Toutst.exe enable=yes");
                //process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WindowsMedia dir=out action=block program=D:\Pubgm2\TxGameAssistant\ui\TPDownLoad\Tenio\TenioDL\TenioDL.exe enable=yes");
                process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WINDOWS interface=any dir=out action=block remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255,219.133.0.0-219.133.255.255");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();

            }
            /* if (!File.Exists("C:\\DINESH.bat"))
         {
             using (WebClient client = new WebClient())
             {
                 client.DownloadFile("http://www.joycheat.com/wp-content/uploads/2020/10/DINESH.bat", "C:\\DINESH.bat");
             }
         }
         Process.Start(@"C:\DINESH.bat");*/

            string str;

            try
            {
                object value = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Tencent\\MobileGamePC\\UI").GetValue("InstallPath");
                if (value != null)
                {
                    str = value.ToString();
                }
                else
                {
                    str = null;
                }
                Process.Start(string.Concat(str, "\\AndroidEmulator.exe"));
            }
            catch
            {
                MessageBox.Show("Process blocked due to interference of another service");
            }
            /*string RCJ = (@"C:\DINESH.bat");

            if (File.Exists(RCJ))
            {
                File.Delete(RCJ);
            }*/
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Process.Start(@"DriverInstaller.exe");
            string filepath = (@"C:\JOY X.dll");

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            if (!File.Exists(@"C:\JOY X.dll"))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://www.joycheat.com/wp-content/uploads/2020/11/JOY-X.dll", @"C:\JOY X.dll");
               }
            }
            // Enable Debug Privilige
            EnableDebugPriv();
            // Get Window Handle
            // hwnd = FindWindow("TXGuiFoundation", WINDOW_NAME);
            Process[] processesByName = Process.GetProcessesByName("ProjectTitan");
            if (this.hwnd == IntPtr.Zero)
            {
                if (processesByName.Length == 1)
                    this.hwnd = MainForm.FindWindow("TitanRenderWindowClass", Process.GetProcessesByName("ProjectTitan")[0].MainWindowTitle);
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)3;
            }
            this.hwnd = MainForm.FindWindowEx(this.hwnd, 0U, "TitanRenderWindowClass", "SmartGaGa RenderWindow");
            Mem.Initialize(this.FindTrueAndroidProcessHandle());
            if (Mem.m_pProcessHandle == IntPtr.Zero)
            {
                int num1 = (int)MessageBox.Show("Please Open Emulator First !", "ERROR");
            }

            
            else
            {
                // Initialize SigScan
                sigScan = new SigScanSharp(Mem.m_pProcessHandle);
            }

            // Find UWorld Offset
            ueSearch = new GameMemSearch(sigScan);
            var cands = ueSearch.ViewWorldSearchCandidates();
            viewWorld = ueSearch.GetViewWorld(cands);
            uWorld = viewWorld - 4416288;
            gNames = viewWorld - 4453088;
           
            if (uWorld > 0)
            {
                // Start Drawing ESP
                LoopTimer.Enabled = true;
                UpdateTimer.Enabled = true;
                GetWindowRect(hwnd, out rect);
                espForm = new ESPForm(rect, ueSearch);
                new Thread(ESPThread).Start();
                new Thread(InfoThread).Start();
                ;
            }
            


            else
            {
                MessageBox.Show("Unable to initialize, please check if simulator and game is running");
            }
        }
       /* private void JOY_Load(object sender, EventArgs e)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c sc stop KProcessHacker & sc delete KProcessHacker & sc stop KProcessHacker2 & sc delete KProcessHacker2 & sc stop KProcessHacker3 & sc delete KProcessHacker3 & sc stop KProcessHacker1 & sc delete KProcessHacker1"
            };
            process.StartInfo = processStartInfo;
            process.Start();
        }*/
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Settings.PlayerBox = true;
            }
            else
            {
                Settings.PlayerBox = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                Settings.PlayerBone = true;
            }
            else
            {
                Settings.PlayerBone = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                Settings.PlayerLines = true;
            }
            else
            {
                Settings.PlayerLines = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                Settings.PlayerHealth = true;
            }
            else
            {
                Settings.PlayerHealth = false;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                Settings.ItemESP = true;
            }
            else
            {
                Settings.ItemESP = false;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                Settings.Player3dBox = true;
            }
            else
            {
                Settings.Player3dBox = false;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                Settings.VehicleESP = true;
            }
            else
            {
                Settings.VehicleESP = false;
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox9.Checked)
            {
                using (WebClient webClient = new WebClient())
                {
                   // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox9_CheckedChanged);
                    webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/11/hosts-6"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                }
                MainForm.cmd("taskkill /F /IM adb.exe");
                MainForm.cmd("adb shell monkey -p com.tencent.ig -c android.intent.category.LAUNCHER 1");
            }
            else
            { MainForm.cmd("taskkill /F /IM adb.exe"); }           
             

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            using (WebClient webClient = new WebClient())
            {
                //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox10_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/11/hosts-6"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                
            }
            MainForm.cmd("taskkill /F /IM adb.exe");
            MainForm.cmd("adb shell am start -n com.vng.pubgmobile/com.epicgames.ue4.SplashActivity");
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            using (WebClient webClient = new WebClient())
            {
                // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox11_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/11/hosts-6"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                
            }
            MainForm.cmd("taskkill /F /IM adb.exe");
            MainForm.cmd("adb shell am start -n com.pubg.krmobile/com.epicgames.ue4.SplashActivity");
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            using (WebClient webClient = new WebClient())
            {
               // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox13_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/11/hosts-6"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
            }
            MainForm.cmd("taskkill /F /IM adb.exe");
            MainForm.cmd("adb shell am start -n com.rekoo.pubgm/com.epicgames.ue4.SplashActivity");
        }


        private void CheckBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                Settings.bDrawFow = true;
            }
            else
            {
                Settings.bDrawFow = false;
            }
        }


    }
}

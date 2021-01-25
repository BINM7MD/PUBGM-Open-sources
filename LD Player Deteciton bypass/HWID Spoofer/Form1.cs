using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Diagnostics;
using System.Management;
using System.Net;
using LD_BYPASS;


namespace HWID_Spoofer
{
    public partial class Form1 : MetroForm
    {
        public static string GetIPAddress()
        {
            string IPADDRESS = new WebClient().DownloadString("http://ip-api.com/line/");
            return IPADDRESS;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            metroComboBox2.Items.Add("Global");
            metroComboBox2.Items.Add("Vietnam");
            metroComboBox2.Items.Add("Korea");
            metroComboBox2.Items.Add("Taiwan");
        }
        #region MACAddressSpoof

        Random rand = new Random();
        public const string Alphabet = "ABCDEF0123456789";
        public string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        string CurrentMAC()
        {
            RegistryKey mac;
            string MAC;
            mac = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\0012", true);
            MAC = (string)mac.GetValue("NetworkAddress");
            mac.Close();
            return MAC;
        }
        #endregion

        #region GUIDspoof

        public static string CurrentGUID()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return machineGuid.ToString();
                }
            }
        }
        #endregion

        #region ProductIDspoof

        public static string CurrentProductID()
        {
            string location = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string name = "ProductID";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object productID = rk.GetValue(name);
                    if (productID == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return productID.ToString();
                }
            }
        }
        #endregion

        #region InstallTimeSpoof
        Random random = new Random();
        public const string Alphabet1 = "abcdef0123456789";
        public string GenerateDate(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet1[random.Next(Alphabet1.Length)];
            }
            return new string(chars);
        }

        public static string CurrentInstallTime()
        {
            string location = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string name = "InstallTime";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object installtime = rk.GetValue(name);
                    if (installtime == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return installtime.ToString();
                }
            }
        }
        #endregion

        #region InstallDateSpoof

        public static string CurrentInstallDate()
        {
            string location = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string name = "InstallDate";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object installdate = rk.GetValue(name);
                    if (installdate == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return installdate.ToString();
                }
            }
        }
        #endregion

        #region HwProfileGUIDspoof
        public static string CurrentHwProfileGUID()
        {
            string location = @"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001";
            string name = "HwProfileGUID";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object hwprofileguid = rk.GetValue(name);
                    if (hwprofileguid == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return hwprofileguid.ToString();
                }
            }
        }
        #endregion

        #region HddSerialSpoof
        void ChangeSerialNumber(char volume, uint newSerial)
        {
            var fsInfo = new[]
            {
        new { Name = "FAT32", NameOffs = 0x52, SerialOffs = 0x43 },
        new { Name = "FAT", NameOffs = 0x36, SerialOffs = 0x27 },
        new { Name = "NTFS", NameOffs = 0x03, SerialOffs = 0x48 }
    };

            using (var disk = new Disk(volume))
            {
                var sector = new byte[512];
                disk.ReadSector(0, sector);

                var fs = fsInfo.FirstOrDefault(
                        f => Strncmp(f.Name, sector, f.NameOffs)
                    );
                if (fs == null) throw new NotSupportedException("This file system is not supported");

                var s = newSerial;
                for (int i = 0; i < 4; ++i, s >>= 8) sector[fs.SerialOffs + i] = (byte)(s & 0xFF);

                disk.WriteSector(0, sector);
            }
        }

        bool Strncmp(string str, byte[] data, int offset)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (data[i + offset] != (byte)str[i]) return false;
            }
            return true;
        }

        class Disk : IDisposable
        {
            private SafeFileHandle handle;

            public Disk(char volume)
            {
                var ptr = CreateFile(
                    String.Format("\\\\.\\{0}:", volume),
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    0,
                    IntPtr.Zero
                    );

                handle = new SafeFileHandle(ptr, true);

                if (handle.IsInvalid) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            public void ReadSector(uint sector, byte[] buffer)
            {
                if (buffer == null) throw new ArgumentNullException("buffer");
                if (SetFilePointer(handle, sector, IntPtr.Zero, EMoveMethod.Begin) == INVALID_SET_FILE_POINTER) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                uint read;
                if (!ReadFile(handle, buffer, buffer.Length, out read, IntPtr.Zero)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if (read != buffer.Length) throw new IOException();
            }

            public void WriteSector(uint sector, byte[] buffer)
            {
                if (buffer == null) throw new ArgumentNullException("buffer");
                if (SetFilePointer(handle, sector, IntPtr.Zero, EMoveMethod.Begin) == INVALID_SET_FILE_POINTER) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                uint written;
                if (!WriteFile(handle, buffer, buffer.Length, out written, IntPtr.Zero)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if (written != buffer.Length) throw new IOException();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (handle != null) handle.Dispose();
                }
            }

            enum EMoveMethod : uint
            {
                Begin = 0,
                Current = 1,
                End = 2
            }

            const uint INVALID_SET_FILE_POINTER = 0xFFFFFFFF;

            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateFile(
                string fileName,
                [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
                [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                int flags,
                IntPtr template);

            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            static extern uint SetFilePointer(
                 [In] SafeFileHandle hFile,
                 [In] uint lDistanceToMove,
                 [In] IntPtr lpDistanceToMoveHigh,
                 [In] EMoveMethod dwMoveMethod);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer,
                int nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

            [DllImport("kernel32.dll")]
            static extern bool WriteFile(SafeFileHandle hFile, [In] byte[] lpBuffer,
                int nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
                [In] IntPtr lpOverlapped);
        }
        #endregion

        #region SpoofPCName

        public static string CurrentPCName()
        {
            string location = @"SYSTEM\CurrentControlSet\Control\ComputerName\ActiveComputerName";
            string name = "ComputerName";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object pcname = rk.GetValue(name);
                    if (pcname == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));

                    return pcname.ToString();
                }
            }
        }
        #endregion

        private void metroButton2_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\XuanZhi\LDPlayer");
            object path = key.GetValue("InstallDir");
            String dir = path + "\\dnplayer.exe";
            Process.Start(dir);
        }
        private void metroButton7_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\XuanZhi\LDPlayer");
            object path = key.GetValue("InstallDir");
            String dir = path + "\\dnplayer.exe";
            Process.Start(dir);
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            string filepath = (@"C:\Windows\JOY.lua");

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            if (!File.Exists(@"C:\Windows\JOY.lua"))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://cdn.discordapp.com/attachments/790364491211603999/803297028162846771/JOYBYPASSV1.0.lua", @"C:\Windows\JOY.lua");
                }
            }
            {
                File.WriteAllBytes(@"C:\Windows\libcubehawk.so", JOY.Properties.Resources.libcubehawk);
            }
            {
                File.WriteAllBytes(@"C:\Windows\libgamemaster.so", JOY.Properties.Resources.libgamemaster);
            }
            {
                File.WriteAllBytes(@"C:\Windows\libIMSDK.so", JOY.Properties.Resources.libIMSDK);
            }
            {
                File.WriteAllBytes(@"C:\Windows\libst_engine.so", JOY.Properties.Resources.libst_engine);
            }
            {
                File.WriteAllBytes(@"C:\Windows\libTDataMaster.so", JOY.Properties.Resources.libTDataMaster);
            }
            if (metroComboBox2.SelectedIndex == 0) // Global
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine("@cd/d \"C:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"C:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("adb.exe kill-server");
                process.StandardInput.WriteLine("adb.exe start-server");
                process.StandardInput.WriteLine("adb fork-server server");
                process.StandardInput.WriteLine("adb.exe devices");
                process.StandardInput.WriteLine("adb root");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\JOY.lua /storage/emulated/0/JOY.lua");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libcubehawk.so /storage/emulated/0/Android/data/libcubehawk.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libgamemaster.so /storage/emulated/0/Android/data/libgamemaster.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libIMSDK.so /storage/emulated/0/Android/data/libIMSDK.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libst_engine.so /storage/emulated/0/Android/data/libst_engine.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libTDataMaster.so /storage/emulated/0/Android/data/libTDataMaster.so");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_webview_imsdk_inner_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/app_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/files\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.tencent.ig/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.tencent.ig/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.tencent.ig/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.tencent.ig/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/shared_prefs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.tencent.ig-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.tencent.ig-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.tencent.ig-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.tencent.ig-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libBugly.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libgcloudarch.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libhelpshiftlistener.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libigshare.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/liblbs.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.tencent.ig/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libcubehawk.so /data/user/0/com.tencent.ig/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libgamemaster.so /data/user/0/com.tencent.ig/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libIMSDK.so /data/user/0/com.tencent.ig/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libst-engine.so /data/user/0/com.tencent.ig/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libTDataMaster.so /data/user/0/com.tencent.ig/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.tencent.ig/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.tencent.ig/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.tencent.ig/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.tencent.ig/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.tencent.ig/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.tencent.ig/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.tencent.ig/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.tencent.ig/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.tencent.ig/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.tencent.ig/lib/libTDataMaster.so\"");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                MessageBox.Show("PATCH EMULATOR DETECTION 100%", "GLOBAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (metroComboBox2.SelectedIndex == 1) // Vietnam
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine("@cd/d \"C:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"C:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("adb.exe kill-server");
                process.StandardInput.WriteLine("adb.exe start-server");
                process.StandardInput.WriteLine("adb fork-server server");
                process.StandardInput.WriteLine("adb.exe devices");
                process.StandardInput.WriteLine("adb root");
                process.StandardInput.WriteLine("adb remount");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\JOY.lua /storage/emulated/0/JOY.lua");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libcubehawk.so /storage/emulated/0/Android/data/libcubehawk.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libgamemaster.so /storage/emulated/0/Android/data/libgamemaster.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libIMSDK.so /storage/emulated/0/Android/data/libIMSDK.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libst_engine.so /storage/emulated/0/Android/data/libst_engine.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libTDataMaster.so /storage/emulated/0/Android/data/libTDataMaster.so");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_webview_imsdk_inner_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/app_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/files\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.vng.pubgmobile/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.vng.pubgmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.vng.pubgmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.vng.pubgmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.vng.pubgmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/shared_prefs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.vng.pubgmobile-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.vng.pubgmobile-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.vng.pubgmobile-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.vng.pubgmobile-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libBugly.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libgcloudarch.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libhelpshiftlistener.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libigshare.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/liblbs.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.vng.pubgmobile/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libcubehawk.so /data/user/0/com.vng.pubgmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libgamemaster.so /data/user/0/com.vng.pubgmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libIMSDK.so /data/user/0/com.vng.pubgmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libst-engine.so /data/user/0/com.vng.pubgmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libTDataMaster.so /data/user/0/com.vng.pubgmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.vng.pubgmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.vng.pubgmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.vng.pubgmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.vng.pubgmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.vng.pubgmobile/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.vng.pubgmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.vng.pubgmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.vng.pubgmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.vng.pubgmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.vng.pubgmobile/lib/libTDataMaster.so\"");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                MessageBox.Show("PATCH EMULATOR DETECTION 100%", "Vietnam", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (metroComboBox2.SelectedIndex == 2) // Korea
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine("@cd/d \"C:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"C:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("adb.exe kill-server");
                process.StandardInput.WriteLine("adb.exe start-server");
                process.StandardInput.WriteLine("adb fork-server server");
                process.StandardInput.WriteLine("adb.exe devices");
                process.StandardInput.WriteLine("adb root");
                process.StandardInput.WriteLine("adb remount");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\JOY.lua /storage/emulated/0/JOY.lua");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libcubehawk.so /storage/emulated/0/Android/data/libcubehawk.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libgamemaster.so /storage/emulated/0/Android/data/libgamemaster.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libIMSDK.so /storage/emulated/0/Android/data/libIMSDK.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libst_engine.so /storage/emulated/0/Android/data/libst_engine.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libTDataMaster.so /storage/emulated/0/Android/data/libTDataMaster.so");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_webview_imsdk_inner_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/app_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/files\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.pubg.krmobile/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.pubg.krmobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.pubg.krmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.pubg.krmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.pubg.krmobile/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/shared_prefs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.pubg.krmobile-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.pubg.krmobile-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.pubg.krmobile-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.pubg.krmobile-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libBugly.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libgcloudarch.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libhelpshiftlistener.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libigshare.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/liblbs.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.pubg.krmobile/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libcubehawk.so /data/user/0/com.pubg.krmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libgamemaster.so /data/user/0/com.pubg.krmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libIMSDK.so /data/user/0/com.pubg.krmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libst-engine.so /data/user/0/com.pubg.krmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libTDataMaster.so /data/user/0/com.pubg.krmobile/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.pubg.krmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.pubg.krmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.pubg.krmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.pubg.krmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.pubg.krmobile/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.pubg.krmobile/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.pubg.krmobile/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.pubg.krmobile/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.pubg.krmobile/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.pubg.krmobile/lib/libTDataMaster.so\"");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                MessageBox.Show("PATCH EMULATOR DETECTION 100%", "Korea", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (metroComboBox2.SelectedIndex == 3) // Taiwan
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine("@cd/d \"C:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\XuanZhi\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"C:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"D:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("@cd/d \"E:\\leidian\\LDPlayer\"");
                process.StandardInput.WriteLine("adb.exe kill-server");
                process.StandardInput.WriteLine("adb.exe start-server");
                process.StandardInput.WriteLine("adb fork-server server");
                process.StandardInput.WriteLine("adb.exe devices");
                process.StandardInput.WriteLine("adb root");
                process.StandardInput.WriteLine("adb remount");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\JOY.lua /storage/emulated/0/JOY.lua");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libcubehawk.so /storage/emulated/0/Android/data/libcubehawk.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libgamemaster.so /storage/emulated/0/Android/data/libgamemaster.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libIMSDK.so /storage/emulated/0/Android/data/libIMSDK.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libst_engine.so /storage/emulated/0/Android/data/libst_engine.so");
                process.StandardInput.WriteLine("adb push C:\\Windows\\System32\\libTDataMaster.so /storage/emulated/0/Android/data/libTDataMaster.so");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_webview_imsdk_inner_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/app_webview\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/files\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/.backups\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/MidasOversea\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/tencent\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/sdk\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/.fff\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/.system_android_l2\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/ProgramBinaryCache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/ca-bundle.pem\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/cacheFile.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/login-identifier.txt\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /storage/emulated/0/Android/data/com.rekoo.pubgm/files/vmpcloudconfig.json\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/afd\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/IGH5Cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/ImageDownload\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Logs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/Pandora\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/PufferTmpDir\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/rawdata\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/TableDatas\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/RoleInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/UpdateInfo\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/GameErrorNoRecords\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /storage/emulated/0/Android/data/com.rekoo.pubgm/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra2/Saved/StatEventReportedFlag\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"mkdir /data/user/0/com.rekoo.pubgm/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 555 /data/user/0/com.rekoo.pubgm/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_bugly\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_crashrecord\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_database\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_geolocation\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_tbs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/app_textures\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown root:root /data/user/0/com.rekoo.pubgm/code_cache\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/databases\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/shared_prefs\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.rekoo.pubgm-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/app/com.rekoo.pubgm-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.rekoo.pubgm-1/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"touch /data/app/com.rekoo.pubgm-2/oat\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libBugly.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libgcloudarch.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libhelpshiftlistener.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libigshare.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/liblbs.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"rm -rf /data/user/0/com.rekoo.pubgm/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libcubehawk.so /data/user/0/com.rekoo.pubgm/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libgamemaster.so /data/user/0/com.rekoo.pubgm/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libIMSDK.so /data/user/0/com.rekoo.pubgm/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libst-engine.so /data/user/0/com.rekoo.pubgm/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"cp /storage/emulated/0/Android/data/libTDataMaster.so /data/user/0/com.rekoo.pubgm/lib\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.rekoo.pubgm/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.rekoo.pubgm/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.rekoo.pubgm/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.rekoo.pubgm/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chmod 755 /data/user/0/com.rekoo.pubgm/lib/libTDataMaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.rekoo.pubgm/lib/libcubehawk.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.rekoo.pubgm/lib/libgamemaster.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.rekoo.pubgm/lib/libIMSDK.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.rekoo.pubgm/lib/libst-engine.so\"");
                process.StandardInput.WriteLine("ld.exe -s 0 \"chown system:system /data/user/0/com.rekoo.pubgm/lib/libTDataMaster.so\"");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                MessageBox.Show("PATCH EMULATOR DETECTION 100%", "Taiwan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            string filepath0 = (@"C:\Windows\JOY.lua");

            if (File.Exists(filepath0))
            {
                File.Delete(filepath0);
            }
            string filepath1 = (@"C:\Windows\libcubehawk.so");

            if (File.Exists(filepath1))
            {
                File.Delete(filepath1);
            }
            string filepath2 = (@"C:\Windows\libgamemaster.so");

            if (File.Exists(filepath2))
            {
                File.Delete(filepath2);
            }
            string filepath3 = (@"C:\Windows\libIMSDK.so");

            if (File.Exists(filepath3))
            {
                File.Delete(filepath3);
            }
            string filepath4 = (@"C:\Windows\libst_engine.so");

            if (File.Exists(filepath4))
            {
                File.Delete(filepath4);
            }
            string filepath5 = (@"C:\Windows\libTDataMaster.so");

            if (File.Exists(filepath5))
            {
                File.Delete(filepath5);
            }
        }


        private void metroButton9_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(@"C:\Program Files\ldplayerbox\crashreport.dll");
            }
            catch { }
            MessageBox.Show("Esp Has Been Removed", "Completed");
        }

        private void siticoneControlBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void siticoneCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            String a1 = "/c @echo off";
            String a2 = "& @cd/d \"C:\\XuanZhi\\LDPlayer\"";
            String a3 = "& @cd/d \"D:\\XuanZhi\\LDPlayer\"";
            String a4 = "& @cd/d \"E:\\XuanZhi\\LDPlayer\"";
            String a5 = "& @cd/d \"F:\\XuanZhi\\LDPlayer\"";
            String a6 = "& @cd/d \"Y:\\XuanZhi\\LDPlayer\"";
            String a7 = "& @cd/d \"C:\\leidian\\LDPlayer4\"";
            String a8 = "& @cd/d \"D:\\leidian\\LDPlayer4\"";
            String a9 = "& @cd/d \"E:\\leidian\\LDPlayer4\"";
            String a10 = "& @cd/d \"F:\\leidian\\LDPlayer4\"";
            String a11 = "& @cd/d \"Y:\\leidian\\LDPlayer4\"";
            String a12 = " & ld.exe -s 0 \"iptables -A OUTPUT -p tcp --dport 80:443 -j REJECT\"";
            String a13 = " & dnconsole.exe launchex --index 0 --packagename \"com.tencent.ig\"";
            String a14 = " & ld.exe -s 0 \"mount -o remount,rw /\"";
            String a15 = " & ld.exe -s 0 \"mount -o remount,rw /system\"";
            String a16 = " & ld.exe -s 0 \"mount -o remount,rw /data\"";
            String a17 = " & ld.exe -s 0 \"mount -o remount,rw /data/data\"";
            String a18 = " & ld.exe -s 0 \"mount -o remount,rw /\"";
            String a19 = " & ld.exe -s 0 \"mount -o remount,rw /system\"";
            String a20 = " & ld.exe -s 0 \"mount -o remount,rw /dev\"";
            String a21 = " & ld.exe -s 0 \"mv /dev/vboxguest /dev/vboxguest1\"";
            String a22 = " & ld.exe -s 0 \"mv /dev/vboxuser /dev/vboxuser1\"";
            String a23 = " & ld.exe -s 0 \"busybox mount --bind /sbin /sys/module\"";
            String a24 = " & ld.exe -s 0 \"busybox mount --bind /sbin /sys/devices/virtual/misc\"";
            String a25 = " & ld.exe -s 0 \"busybox mount --bind /sbin /system/etc/security/cacerts\"";
            String a26 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/ImageDownload/\"";
            String a27 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/LightData/\"";
            String a28 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Logs/\"";
            String a29 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Pandora/\"";
            String a30 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferEifs0/\"";
            String a31 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferEifs1/\"";
            String a32 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/PufferTmpDir/\"";
            String a33 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/rawdata/\"";
            String a34 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/RoleInfo/\"";
            String a35 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/TableDatas/\"";
            String a36 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/UpdateInfo/\"";
            String a37 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/coverversion.ini\"";
            String a38 = " & ld.exe -s 0 \"rm -rf /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/StatEventReportedFlag\"";
            String paks = " & ld.exe -s 0 \"chmod 444 /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/Paks/\"";
            String a39 = " & adb push C:\\Windows\\Key.ini /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SrcVersion.ini";
            String a40 = " & ld.exe -s 0 \"chmod 444 /data/media/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SrcVersion.ini\"";
            String a41 = " & ld.exe -s 0 sleep 7";
            String a42 = " & ld.exe -s 0 \"chmod 444 /proc\"";
            System.Diagnostics.Process process6681 = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo6681 = new System.Diagnostics.ProcessStartInfo();
            startInfo6681.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo6681.FileName = "cmd.exe";
            startInfo6681.Arguments = a1 + a2 + a3 + a4 + a5 + a6 + a7 + a8 + a9 + a10 + a11 + a12 + a13 + a14 + a15 + a16 + a17 + a18 + a19 + a20 + a21 + a22 + a23 + a24 + a25 + a26 + a27 + a28 + a29 + a30 + a31 + a32 + a33 + a34 + a35 + a36 + a37 + a38 + paks + a39 + a40 + a41 + a42;
            process6681.StartInfo = startInfo6681;
            process6681.Start();
            process6681.WaitForExit();
            try
            {
                File.Delete(Environment.SystemDirectory + "\\..\\Key.ini");
            }
            catch { }
        }

        private void metroTabPage3_Click(object sender, EventArgs e)
        {

        }

        private void metroTabPage2_Click(object sender, EventArgs e)
        {

        }

        private void siticoneCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.StandardInput.WriteLine($@"TaskKill /F /IM dnplayer.exe");
                process.StandardInput.WriteLine($@"taskkill /F /im adb.exe");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
        }

        private void metroButton2_Click_1(object sender, EventArgs e)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\XuanZhi\LDPlayer");
            object path = key.GetValue("InstallDir");
            String dir = path + "\\dnplayer.exe";
            Process.Start(dir);
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            string var = App.GrabVariable(variable.Text);
            MessageBox.Show(var);
        }

        private void variable_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
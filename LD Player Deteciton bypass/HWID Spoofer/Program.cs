using LD_BYPASS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWID_Spoofer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String a1111 = "/c taskkill /IM adb.exe /F";
            String a21 = "& taskkill /f /im dnplayer.exe /F";
            String a31 = "& taskkill /f /im ld.exe /F";
            String a41 = "& taskkill /f /im ld.exe /F";
            String a51 = "& taskkill /f /im ld.exe /F";
            String a61 = "& taskkill /f /im conime.exe";
            String a71 = "& taskkill /f /im QQDL.EXE";
            String a81 = "& taskkill /f /im qqlogin.exe";
            String a91 = "& taskkill /f /im dnfchina.exe";
            String a101 = "& taskkill /f /im dnfchinatest.exe";
            String a111 = "& taskkill /f /im txplatform.exe";
            String a121 = " & taskkill /f /im aow_exe.exe";
            String a131 = " & taskkill /F /IM TitanService.exe";
            String a141 = " & taskkill /F /IM ProjectTitan.exe";
            String a151 = " & taskkill /F /IM Auxillary.exe";
            String a161 = " & taskkill /F /IM TP3Helper.exe";
            String a171 = " & taskkill /F /IM tp3helper.dat";
            String a181 = " & TaskKill /F /IM androidemulator.exe";
            String E1 = " & TaskKill /F /IM aow_exe.exe";
            String E2 = " & TaskKill /F /IM QMEmulatorService.exe";
            String E3 = " & TaskKill /F /IM RuntimeBroker.exe";
            String E4 = " & taskkill /F /im adb.exe";
            String E5 = " & taskkill /F /im GameLoader.exe";
            String E6 = " & taskkill /F /im TBSWebRenderer.exe";
            String E7 = " & taskkill /F /im AppMarket.exe";
            String E8 = " & taskkill /F /im AndroidEmulator.exe";
            String E9 = " & net stop QMEmulatorService";
            String E10 = " & net stop aow_drv";
            String E11 = " & del C:\\aow_drv.log";
            String E12 = " & del /s /f /q C:\\ProgramData\\Tencent";
            String E13 = " & del /s /f /q";
            String E14 = " & C:\\Users%USERNAME%\\AppData\\Local\\Tencent";
            String E15 = " & del /s /f /q";
            String E16 = " & C:\\Users%USERNAME%\\AppData\\Roaming\\Tencent";
            String E17 = " & net stop Tensafe";
            String E18 = " & taskkill /IM Procmon";
            String E19 = " & taskkill /IM Procmon";
            System.Diagnostics.Process process66811 = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo66811 = new System.Diagnostics.ProcessStartInfo();
            startInfo66811.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo66811.FileName = "cmd.exe";
            startInfo66811.Arguments = a1111 + a21 + a31 + a41 + a51 + a61 + a71 + a81 + a91 + a101 + a111 + a121 + a131 + a141 + a151 + a161 + a171 + a181 + E1 + E2 + E3 + E4 + E5 + E6 + E7 + E8 + E9 + E10 + E11 + E12 + E13 + E14 + E15 + E16 + E17 + E18 + E19;
            process66811.StartInfo = startInfo66811;
            process66811.Start();
            String a1 = "/c taskkill /IM ProcessHacker.exe";
            String a2 = "& taskkill /IM peview.exe";
            String a3 = "& taskkill /IM NLClientApp.exe";
            String a4 = "& taskkill /IM HTTPDebuggerUI.exe";
            String a5 = "& taskkill /IM Fiddler.exe";
            String a6 = "& taskkill /IM procexp.exe";
            String a7 = "& taskkill /IM procexp64.exe";
            String a8 = "& taskkill /IM procexp64a.exe";
            String a9 = "& taskkill /IM dnSpy.exe";
            String a10 = "& taskkill /IM dnSpy.Console.exe";
            String a11 = "& taskkill /IM taskmgr.exe";
            String a12 = " & taskkill /IM conhost.exe";
            String a13 = " & taskkill /IM Window-Title-Changer.exe";
            String a14 = " & taskkill /IM ProcessHacker.exe";
            String a15 = " & taskkill /IM ProcessHacker.exe";
            String a16 = " & taskkill /IM ProcessHacker";
            String a17 = " & taskkill /IM Procmon.exe";
            String a18 = " & taskkill /IM Procmon";
            System.Diagnostics.Process process6681 = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo6681 = new System.Diagnostics.ProcessStartInfo();
            startInfo6681.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo6681.FileName = "cmd.exe";
            startInfo6681.Arguments = a1 + a2 + a3 + a4 + a5 + a6 + a7 + a8 + a9 + a10 + a11 + a12 + a13 + a14 + a15 + a16 + a17 + a18;
            process6681.StartInfo = startInfo6681;
            process6681.Start();
              OnProgramStart.Initialize("JOY | BYPASS", "727394", "QnKyai4QJF9qWnGVwroPQHltGTu33W51g13", "1.0");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

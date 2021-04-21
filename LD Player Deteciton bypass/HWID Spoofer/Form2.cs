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
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Sending Files To System Directory 
           
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            
        }
        //////////////////////////////KILL PROCCESS HACKER STEP 2/////////////////////////////////
        //////////////
        #endregion

        private void timer3_Tick(object sender, EventArgs e)
        {
          
    }
}

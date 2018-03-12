using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using System.IO;
using System.Windows.Forms;



namespace CommonControls
{
    public class c_NetFrameworkVersion
    {
        private const string FRAMEWORK_PATH = "\\Microsoft.NET\\Framework";
        private const string WINDIR1 = "windir";
        private const string WINDIR2 = "SystemRoot";

        public static string FrameworkVersion
        {
            get
            {
                try
                {
                    return getHighestVersion(NetFrameworkInstallationPath);
                }
                catch (SecurityException)
                {
                    return "Unknown";
                }
            }
        }

        public static float FrameworkVersionNum
        {
            get
            {
                try
                {
                    return getHighestVersionNum(NetFrameworkInstallationPath);
                }
                catch (SecurityException)
                {
                    return 0;
                }
            }
        }
  
        static void LinkLabel_Click(object sender, EventArgs e)
        {
            string Link = ((LinkLabel)sender).Text;
            System.Diagnostics.Process.Start("http:\\" + Link);
        }

        public static bool CheckVersion(float Version)
        {
            float CurrentVersion = FrameworkVersionNum;
            if (CurrentVersion < Version)
            {
                Form DownloadNetFrameworkForm = new Form();
                DownloadNetFrameworkForm.Text = "Error !!! " + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                DownloadNetFrameworkForm.Icon = System.Drawing.SystemIcons.Error;
                DownloadNetFrameworkForm.StartPosition = FormStartPosition.CenterScreen;


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-us");

                Label MessLabel = new Label();
                MessLabel.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 10);
                MessLabel.Text = "Installed Net Framework version : " + CurrentVersion.ToString(ci) + ", need: " + Version.ToString(ci) + ".\n" +
                                 "You can download Net Framework "+ Version + " from Microsoft website :\n\n" +
                                 "Версия установленной Net Framework: " + CurrentVersion.ToString(ci) + ", необходима: " + Version.ToString(ci) + ".\n" +
                                 "Вы можете скачать Net Framework "+ Version + " с сайта Microsoft :";
                MessLabel.Location = new System.Drawing.Point(20,20);
                MessLabel.Size = new System.Drawing.Size(560,100);

                LinkLabel LinkLabel = new LinkLabel();
                LinkLabel.Font=MessLabel.Font;
                LinkLabel.Text = "www.microsoft.com/downloads/details.aspx?familyid=333325FD-AE52-4E35-B531-508D977D32A6&displaylang=en";
                LinkLabel.Location = new System.Drawing.Point(20, 105);
                LinkLabel.Size = new System.Drawing.Size(560, 100);
                LinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                LinkLabel.Click += new EventHandler(LinkLabel_Click);

                DownloadNetFrameworkForm.ClientSize = new System.Drawing.Size(600,200);
                DownloadNetFrameworkForm.Controls.Add(MessLabel);
                DownloadNetFrameworkForm.Controls.Add(LinkLabel);                
        
                DownloadNetFrameworkForm.ShowDialog();
                return false;
            }
            return true;

        }        
            

        private static string getHighestVersion(string installationPath)
        {
            string[] versions = Directory.GetDirectories(installationPath, "v*");
            string version = "Unknown";

            for (int i = versions.Length - 1; i >= 0; i--)
            {
                version = extractVersion(versions[i]);
                if (isNumber(version))
                    return version;
            }

            return version;
        }

        private static float getHighestVersionNum(string installationPath)
        {
            string[] versions = Directory.GetDirectories(installationPath, "v*");
            float version = 0;

            for (int i = versions.Length - 1; i >= 0; i--)
            {
                string versionstr = extractVersion(versions[i]);
                string[] VersionStrPies = versionstr.Split('.');
                if (VersionStrPies.Length < 2) continue;

                float tempver = 0;
                tempver += int.Parse(VersionStrPies[0]);
                tempver += float.Parse(VersionStrPies[1])/10;

                version = Math.Max(version, tempver);                    
            }

            return version;
        }

        private static string extractVersion(string directory)
        {
            int startIndex = directory.LastIndexOf("\\") + 2;
            return directory.Substring(startIndex, directory.Length - startIndex);
        }

        private static bool isNumber(string str)
        {
            return new Regex(@"^[0-9]+\.?[0-9]*$").IsMatch(str);
        }

        public static string NetFrameworkInstallationPath
        {
            get { return WindowsPath + FRAMEWORK_PATH; }
        }

        public static string WindowsPath
        {
            get
            {
                string winDir = Environment.GetEnvironmentVariable(WINDIR1);
                if (String.IsNullOrEmpty(winDir))
                    winDir = Environment.GetEnvironmentVariable(WINDIR2);

                return winDir;
            }
        }

    }
}

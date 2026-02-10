using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;
using System;


namespace ScholarSync.Commons
{
    public struct ConfigurationConstants
    {
        
        public static Icon LogoIcon { get; } = new Icon(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "../../../Resourses", "ScholarSync.ico"));
        public static int ScreenWidth { get; } = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight { get; } = Screen.PrimaryScreen.Bounds.Height;
       // public static string ConnectionString { get; } = "Host=ep-super-tree-a8g0f75w-pooler.eastus2.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_peLsj5fhm7aq;Database=neondb;SSL Mode=Require;";
       public static string ConnectionString { get; } = "Host=localhost;Port=5432;Username=postgres;Password=zubair1326;Database=scholarsync";




        // Colors 
        public static Color SSWhiteColor { get; } = ColorTranslator.FromHtml("#E7F0FA");
        public static Color SSLightNavyColor { get; } = ColorTranslator.FromHtml("#7BA4D0");
        public static Color SSDarkBlueColor { get; } = ColorTranslator.FromHtml("#2E5E99");
        public static Color SSDarkNavyColor { get; } = ColorTranslator.FromHtml("#0D2440");
        
        // UI-specific colors
        public static Color SSLightGrayBackground { get; } = Color.FromArgb(245, 245, 245);
        public static Color SSBorderGray { get; } = Color.FromArgb(224, 224, 224);
        
        // Status and Alert Colors
        public static Color SSSuccessGreen { get; } = Color.Green;
        public static Color SSErrorRed { get; } = Color.OrangeRed;
        public static Color SSWarningOrange { get; } = Color.Orange;

    }

}

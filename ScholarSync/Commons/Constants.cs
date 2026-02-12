using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;
using System;
using ScholarSync.Configuration;


namespace ScholarSync.Commons
{
    public struct ConfigurationConstants
    {
        
        public static Icon LogoIcon { get; } = new Icon(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources", "ScholarSync.ico"));
        public static Image LogoImage { get; } = new Bitmap(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources", "logo.png"));
        public static int ScreenWidth { get; } = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight { get; } = Screen.PrimaryScreen.Bounds.Height;
        
        // Connection string from configuration
        public static string ConnectionString => AppConfiguration.Instance.GetConnectionString();




        // Colors - loaded from configuration with fallback to defaults
        public static Color SSWhiteColor => GetColorFromConfig(
            AppConfiguration.Instance.UI?.BackgroundColor, "#E7F0FA");
        public static Color SSLightNavyColor => GetColorFromConfig(
            AppConfiguration.Instance.UI?.AccentColor, "#7BA4D0");
        public static Color SSDarkBlueColor => GetColorFromConfig(
            AppConfiguration.Instance.UI?.SecondaryColor, "#2E5E99");
        public static Color SSDarkNavyColor => GetColorFromConfig(
            AppConfiguration.Instance.UI?.PrimaryColor, "#0D2440");
        
        // UI-specific colors
        public static Color SSLightGrayBackground { get; } = Color.FromArgb(245, 245, 245);
        public static Color SSBorderGray { get; } = Color.FromArgb(224, 224, 224);
        
        // Status and Alert Colors
        public static Color SSSuccessGreen { get; } = Color.Green;
        public static Color SSErrorRed { get; } = Color.OrangeRed;
        public static Color SSWarningOrange { get; } = Color.Orange;

        /// <summary>
        /// Helper method to get color from configuration
        /// </summary>
        private static Color GetColorFromConfig(string configValue, string defaultValue)
        {
            try
            {
                var colorString = !string.IsNullOrWhiteSpace(configValue) ? configValue : defaultValue;
                return ColorTranslator.FromHtml(colorString);
            }
            catch
            {
                return ColorTranslator.FromHtml(defaultValue);
            }
        }
    }

}

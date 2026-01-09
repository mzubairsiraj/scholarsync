using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace ScholarSync.Commons
{
    public struct ConfigurationConstants
    {
        public static Icon LogoIcon { get; } = new Icon(Path.GetFullPath(@"../../ScholarSync.ico"));
        public static int ScreenWidth { get; } = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight { get; } = Screen.PrimaryScreen.Bounds.Height;

       


        // Colors 
        public static Color SSWhiteColor { get; } = ColorTranslator.FromHtml("#E7F0FA");
        public static Color SSLightNavyColor { get; } = ColorTranslator.FromHtml("#7BA4D0");
        public static Color SSDarkBlueColor { get; } = ColorTranslator.FromHtml("#2E5E99");

        public static Color SSDarkNavyColor { get; } = ColorTranslator.FromHtml("#0D2440");

    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScholarSync.Forms.SyncFusionForms;
using ScholarSync.Configuration;
using Syncfusion.Licensing;

namespace ScholarSync
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Initialize configuration
                var config = AppConfiguration.Instance;
                
                // Register Syncfusion Community License from configuration
                // Check if key exists
                if (config.Syncfusion?.LicenseKey != null)
                {
                    SyncfusionLicenseProvider.RegisterLicense(config.Syncfusion.LicenseKey);
                }

                // Configure QuestPDF Community License
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SfWelcomeForm());
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_log.txt");
                File.WriteAllText(logPath, $"CRITICAL ERROR: {ex.Message}\n{ex.StackTrace}\n");
                if (ex.InnerException != null)
                {
                    File.AppendAllText(logPath, $"Inner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}\n");
                }
                MessageBox.Show($"Application crashed. Check log at {logPath}\n\nError: {ex.Message}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; 
            }
        }
    }
}

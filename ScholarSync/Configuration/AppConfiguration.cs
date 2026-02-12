using System;
using System.Configuration;
using System.Xml.Linq;
using System.IO;
using System.Linq;

namespace ScholarSync.Configuration
{
    /// <summary>
    /// Manages application configuration loaded from XML configuration files
    /// </summary>
    public class AppConfiguration
    {
        private static AppConfiguration _instance;
        private static readonly object _lock = new object();

        public ConnectionStrings ConnectionStrings { get; set; }
        public AppSettings AppSettings { get; set; }
        public SyncfusionSettings Syncfusion { get; set; }
        public EmailSettings Email { get; set; }
        public SecuritySettings Security { get; set; }
        public LoggingSettings Logging { get; set; }
        public UISettings UI { get; set; }
        public FeatureFlags Features { get; set; }

        private AppConfiguration() { }

        /// <summary>
        /// Gets the singleton instance of AppConfiguration
        /// </summary>
        public static AppConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = LoadConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loads configuration from XML configuration file
        /// </summary>
        public static AppConfiguration LoadConfiguration()
        {
            var config = new AppConfiguration();

            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scholarsync.config.xml");
                
                // Create default config if it doesn't exist
                if (!File.Exists(configPath))
                {
                    CreateDefaultConfiguration(configPath);
                }

                XDocument doc = XDocument.Load(configPath);
                var root = doc.Root;

                // Load Connection Strings
                config.ConnectionStrings = new ConnectionStrings
                {
                    DefaultConnection = GetElementValue(root, "ConnectionStrings/DefaultConnection", "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=scholarsync"),
                    CloudConnection = GetElementValue(root, "ConnectionStrings/CloudConnection", "")
                };

                // Load App Settings
                config.AppSettings = new AppSettings
                {
                    ApplicationName = GetElementValue(root, "AppSettings/ApplicationName", "ScholarSync"),
                    Version = GetElementValue(root, "AppSettings/Version", "2.0.0"),
                    Environment = GetElementValue(root, "AppSettings/Environment", "Development"),
                    UseCloudDatabase = GetBoolValue(root, "AppSettings/UseCloudDatabase", false),
                    EnableDetailedErrors = GetBoolValue(root, "AppSettings/EnableDetailedErrors", true),
                    SessionTimeoutMinutes = GetIntValue(root, "AppSettings/SessionTimeoutMinutes", 30),
                    MaxLoginAttempts = GetIntValue(root, "AppSettings/MaxLoginAttempts", 5),
                    LockoutDurationMinutes = GetIntValue(root, "AppSettings/LockoutDurationMinutes", 15)
                };

                // Load Syncfusion Settings
                config.Syncfusion = new SyncfusionSettings
                {
                    LicenseKey = GetElementValue(root, "Syncfusion/LicenseKey", "")
                };

                // Load Email Settings
                config.Email = new EmailSettings
                {
                    SmtpHost = GetElementValue(root, "Email/SmtpHost", "smtp.gmail.com"),
                    SmtpPort = GetIntValue(root, "Email/SmtpPort", 587),
                    UseSsl = GetBoolValue(root, "Email/UseSsl", true),
                    SenderEmail = GetElementValue(root, "Email/SenderEmail", "noreply@scholarsync.edu"),
                    SenderName = GetElementValue(root, "Email/SenderName", "ScholarSync System"),
                    Username = GetElementValue(root, "Email/Username", ""),
                    Password = GetElementValue(root, "Email/Password", ""),
                    EnableEmailNotifications = GetBoolValue(root, "Email/EnableEmailNotifications", false)
                };

                // Load Security Settings
                config.Security = new SecuritySettings
                {
                    PasswordMinLength = GetIntValue(root, "Security/PasswordMinLength", 8),
                    RequireUppercase = GetBoolValue(root, "Security/RequireUppercase", true),
                    RequireLowercase = GetBoolValue(root, "Security/RequireLowercase", true),
                    RequireDigit = GetBoolValue(root, "Security/RequireDigit", true),
                    RequireSpecialChar = GetBoolValue(root, "Security/RequireSpecialChar", false),
                    BCryptWorkFactor = GetIntValue(root, "Security/BCryptWorkFactor", 12),
                    EnableTwoFactorAuth = GetBoolValue(root, "Security/EnableTwoFactorAuth", false)
                };

                // Load Logging Settings
                config.Logging = new LoggingSettings
                {
                    LogLevel = new LogLevelSettings
                    {
                        Default = GetElementValue(root, "Logging/LogLevel/Default", "Information"),
                        Microsoft = GetElementValue(root, "Logging/LogLevel/Microsoft", "Warning"),
                        System = GetElementValue(root, "Logging/LogLevel/System", "Warning")
                    },
                    EnableFileLogging = GetBoolValue(root, "Logging/EnableFileLogging", true),
                    LogDirectory = GetElementValue(root, "Logging/LogDirectory", "Logs"),
                    RetainDays = GetIntValue(root, "Logging/RetainDays", 30)
                };

                // Load UI Settings
                config.UI = new UISettings
                {
                    Theme = GetElementValue(root, "UI/Theme", "Office2019"),
                    PrimaryColor = GetElementValue(root, "UI/PrimaryColor", "#0D2440"),
                    SecondaryColor = GetElementValue(root, "UI/SecondaryColor", "#2E5E99"),
                    AccentColor = GetElementValue(root, "UI/AccentColor", "#7BA4D0"),
                    BackgroundColor = GetElementValue(root, "UI/BackgroundColor", "#E7F0FA"),
                    EnableAnimations = GetBoolValue(root, "UI/EnableAnimations", true),
                    DefaultPageSize = GetIntValue(root, "UI/DefaultPageSize", 25)
                };

                // Load Feature Flags
                config.Features = new FeatureFlags
                {
                    EnableAttendance = GetBoolValue(root, "Features/EnableAttendance", true),
                    EnableFeeManagement = GetBoolValue(root, "Features/EnableFeeManagement", true),
                    EnableLibrary = GetBoolValue(root, "Features/EnableLibrary", false),
                    EnableTimetable = GetBoolValue(root, "Features/EnableTimetable", true),
                    EnableEmailNotifications = GetBoolValue(root, "Features/EnableEmailNotifications", false),
                    EnableSMSNotifications = GetBoolValue(root, "Features/EnableSMSNotifications", false)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load application configuration", ex);
            }

            return config;
        }

        private static string GetElementValue(XElement root, string path, string defaultValue)
        {
            try
            {
                var element = root;
                foreach (var part in path.Split('/'))
                {
                    element = element?.Element(part);
                    if (element == null) return defaultValue;
                }
                return element.Value ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static bool GetBoolValue(XElement root, string path, bool defaultValue)
        {
            string value = GetElementValue(root, path, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        private static int GetIntValue(XElement root, string path, int defaultValue)
        {
            string value = GetElementValue(root, path, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        private static void CreateDefaultConfiguration(string configPath)
        {
            var doc = new XDocument(
                new XElement("Configuration",
                    new XElement("ConnectionStrings",
                        new XElement("DefaultConnection", "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=scholarsync"),
                        new XElement("CloudConnection", "Host=ep-super-tree-a8g0f75w-pooler.eastus2.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_peLsj5fhm7aq;Database=neondb;SSL Mode=Require;")
                    ),
                    new XElement("AppSettings",
                        new XElement("ApplicationName", "ScholarSync"),
                        new XElement("Version", "2.0.0"),
                        new XElement("Environment", "Development"),
                        new XElement("UseCloudDatabase", "false"),
                        new XElement("EnableDetailedErrors", "true"),
                        new XElement("SessionTimeoutMinutes", "30"),
                        new XElement("MaxLoginAttempts", "5"),
                        new XElement("LockoutDurationMinutes", "15")
                    ),
                    new XElement("Syncfusion",
                        new XElement("LicenseKey", "Ngo9BigBOggjHTQxAR8/V1JGaF1cXmhLYVJxWmFZfVhgcl9DZlZVRmYuP1ZhSXxVdkdiWX9YdXJRRGlYUEN9XEA=")
                    ),
                    new XElement("Email",
                        new XElement("SmtpHost", "smtp.gmail.com"),
                        new XElement("SmtpPort", "587"),
                        new XElement("UseSsl", "true"),
                        new XElement("SenderEmail", "noreply@scholarsync.edu"),
                        new XElement("SenderName", "ScholarSync System"),
                        new XElement("Username", ""),
                        new XElement("Password", ""),
                        new XElement("EnableEmailNotifications", "false")
                    ),
                    new XElement("Security",
                        new XElement("PasswordMinLength", "8"),
                        new XElement("RequireUppercase", "true"),
                        new XElement("RequireLowercase", "true"),
                        new XElement("RequireDigit", "true"),
                        new XElement("RequireSpecialChar", "false"),
                        new XElement("BCryptWorkFactor", "12"),
                        new XElement("EnableTwoFactorAuth", "false")
                    ),
                    new XElement("Logging",
                        new XElement("LogLevel",
                            new XElement("Default", "Information"),
                            new XElement("Microsoft", "Warning"),
                            new XElement("System", "Warning")
                        ),
                        new XElement("EnableFileLogging", "true"),
                        new XElement("LogDirectory", "Logs"),
                        new XElement("RetainDays", "30")
                    ),
                    new XElement("UI",
                        new XElement("Theme", "Office2019"),
                        new XElement("PrimaryColor", "#0D2440"),
                        new XElement("SecondaryColor", "#2E5E99"),
                        new XElement("AccentColor", "#7BA4D0"),
                        new XElement("BackgroundColor", "#E7F0FA"),
                        new XElement("EnableAnimations", "true"),
                        new XElement("DefaultPageSize", "25")
                    ),
                    new XElement("Features",
                        new XElement("EnableAttendance", "true"),
                        new XElement("EnableFeeManagement", "true"),
                        new XElement("EnableLibrary", "false"),
                        new XElement("EnableTimetable", "true"),
                        new XElement("EnableEmailNotifications", "false"),
                        new XElement("EnableSMSNotifications", "false")
                    )
                )
            );

            doc.Save(configPath);
        }

        /// <summary>
        /// Gets the active connection string based on configuration
        /// </summary>
        public string GetConnectionString()
        {
            return AppSettings?.UseCloudDatabase == true
                ? ConnectionStrings?.CloudConnection
                : ConnectionStrings?.DefaultConnection;
        }

        /// <summary>
        /// Reloads configuration from files
        /// </summary>
        public static void Reload()
        {
            lock (_lock)
            {
                _instance = LoadConfiguration();
            }
        }

        /// <summary>
        /// Saves current configuration to XML file
        /// </summary>
        public void Save()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scholarsync.config.xml");
            
            var doc = new XDocument(
                new XElement("Configuration",
                    new XElement("ConnectionStrings",
                        new XElement("DefaultConnection", ConnectionStrings?.DefaultConnection ?? ""),
                        new XElement("CloudConnection", ConnectionStrings?.CloudConnection ?? "")
                    ),
                    new XElement("AppSettings",
                        new XElement("ApplicationName", AppSettings?.ApplicationName ?? "ScholarSync"),
                        new XElement("Version", AppSettings?.Version ?? "2.0.0"),
                        new XElement("Environment", AppSettings?.Environment ?? "Development"),
                        new XElement("UseCloudDatabase", AppSettings?.UseCloudDatabase.ToString() ?? "false"),
                        new XElement("EnableDetailedErrors", AppSettings?.EnableDetailedErrors.ToString() ?? "true"),
                        new XElement("SessionTimeoutMinutes", AppSettings?.SessionTimeoutMinutes.ToString() ?? "30"),
                        new XElement("MaxLoginAttempts", AppSettings?.MaxLoginAttempts.ToString() ?? "5"),
                        new XElement("LockoutDurationMinutes", AppSettings?.LockoutDurationMinutes.ToString() ?? "15")
                    ),
                    new XElement("Syncfusion",
                        new XElement("LicenseKey", Syncfusion?.LicenseKey ?? "")
                    ),
                    new XElement("Email",
                        new XElement("SmtpHost", Email?.SmtpHost ?? ""),
                        new XElement("SmtpPort", Email?.SmtpPort.ToString() ?? "587"),
                        new XElement("UseSsl", Email?.UseSsl.ToString() ?? "true"),
                        new XElement("SenderEmail", Email?.SenderEmail ?? ""),
                        new XElement("SenderName", Email?.SenderName ?? ""),
                        new XElement("Username", Email?.Username ?? ""),
                        new XElement("Password", Email?.Password ?? ""),
                        new XElement("EnableEmailNotifications", Email?.EnableEmailNotifications.ToString() ?? "false")
                    ),
                    new XElement("Security",
                        new XElement("PasswordMinLength", Security?.PasswordMinLength.ToString() ?? "8"),
                        new XElement("RequireUppercase", Security?.RequireUppercase.ToString() ?? "true"),
                        new XElement("RequireLowercase", Security?.RequireLowercase.ToString() ?? "true"),
                        new XElement("RequireDigit", Security?.RequireDigit.ToString() ?? "true"),
                        new XElement("RequireSpecialChar", Security?.RequireSpecialChar.ToString() ?? "false"),
                        new XElement("BCryptWorkFactor", Security?.BCryptWorkFactor.ToString() ?? "12"),
                        new XElement("EnableTwoFactorAuth", Security?.EnableTwoFactorAuth.ToString() ?? "false")
                    ),
                    new XElement("Logging",
                        new XElement("LogLevel",
                            new XElement("Default", Logging?.LogLevel?.Default ?? "Information"),
                            new XElement("Microsoft", Logging?.LogLevel?.Microsoft ?? "Warning"),
                            new XElement("System", Logging?.LogLevel?.System ?? "Warning")
                        ),
                        new XElement("EnableFileLogging", Logging?.EnableFileLogging.ToString() ?? "true"),
                        new XElement("LogDirectory", Logging?.LogDirectory ?? "Logs"),
                        new XElement("RetainDays", Logging?.RetainDays.ToString() ?? "30")
                    ),
                    new XElement("UI",
                        new XElement("Theme", UI?.Theme ?? "Office2019"),
                        new XElement("PrimaryColor", UI?.PrimaryColor ?? "#0D2440"),
                        new XElement("SecondaryColor", UI?.SecondaryColor ?? "#2E5E99"),
                        new XElement("AccentColor", UI?.AccentColor ?? "#7BA4D0"),
                        new XElement("BackgroundColor", UI?.BackgroundColor ?? "#E7F0FA"),
                        new XElement("EnableAnimations", UI?.EnableAnimations.ToString() ?? "true"),
                        new XElement("DefaultPageSize", UI?.DefaultPageSize.ToString() ?? "25")
                    ),
                    new XElement("Features",
                        new XElement("EnableAttendance", Features?.EnableAttendance.ToString() ?? "true"),
                        new XElement("EnableFeeManagement", Features?.EnableFeeManagement.ToString() ?? "true"),
                        new XElement("EnableLibrary", Features?.EnableLibrary.ToString() ?? "false"),
                        new XElement("EnableTimetable", Features?.EnableTimetable.ToString() ?? "true"),
                        new XElement("EnableEmailNotifications", Features?.EnableEmailNotifications.ToString() ?? "false"),
                        new XElement("EnableSMSNotifications", Features?.EnableSMSNotifications.ToString() ?? "false")
                    )
                )
            );

            doc.Save(configPath);
        }
    }

    #region Configuration Classes

    public class ConnectionStrings
    {
        public ConnectionStrings() { }
        
        public string DefaultConnection { get; set; }
        public string CloudConnection { get; set; }
    }

    public class AppSettings
    {
        public AppSettings() { }
        
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public string Environment { get; set; }
        public bool UseCloudDatabase { get; set; }
        public bool EnableDetailedErrors { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public int MaxLoginAttempts { get; set; }
        public int LockoutDurationMinutes { get; set; }
    }

    public class SyncfusionSettings
    {
        public SyncfusionSettings() { }
        
        public string LicenseKey { get; set; }
    }

    public class EmailSettings
    {
        public EmailSettings() { }
        
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableEmailNotifications { get; set; }
    }

    public class SecuritySettings
    {
        public SecuritySettings() { }
        
        public int PasswordMinLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireSpecialChar { get; set; }
        public int BCryptWorkFactor { get; set; }
        public bool EnableTwoFactorAuth { get; set; }
    }

    public class LoggingSettings
    {
        public LoggingSettings() { }
        
        public LogLevelSettings LogLevel { get; set; }
        public bool EnableFileLogging { get; set; }
        public string LogDirectory { get; set; }
        public int RetainDays { get; set; }
    }

    public class LogLevelSettings
    {
        public LogLevelSettings() { }
        
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string System { get; set; }
    }

    public class UISettings
    {
        public UISettings() { }
        
        public string Theme { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string AccentColor { get; set; }
        public string BackgroundColor { get; set; }
        public bool EnableAnimations { get; set; }
        public int DefaultPageSize { get; set; }
    }

    public class FeatureFlags
    {
        public FeatureFlags() { }
        
        public bool EnableAttendance { get; set; }
        public bool EnableFeeManagement { get; set; }
        public bool EnableLibrary { get; set; }
        public bool EnableTimetable { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public bool EnableSMSNotifications { get; set; }
    }

    #endregion
}

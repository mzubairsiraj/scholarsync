using System;
using ScholarSync.Infrastructure.Security;

namespace ScholarSync.Configuration
{
    /// <summary>
    /// Simple static helper for password hashing
    /// Similar to DbConnector pattern - keeps it simple
    /// </summary>
    public static class ServiceHelper
    {
        private static PasswordHasher _passwordHasher;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the password hasher instance (singleton pattern)
        /// </summary>
        public static PasswordHasher GetPasswordHasher()
        {
            if (_passwordHasher == null)
            {
                lock (_lock)
                {
                    if (_passwordHasher == null)
                    {
                        _passwordHasher = new PasswordHasher();
                    }
                }
            }
            return _passwordHasher;
        }
    }
}

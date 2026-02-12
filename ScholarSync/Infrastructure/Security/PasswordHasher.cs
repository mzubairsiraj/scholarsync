using System;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
using ScholarSync.Configuration;

namespace ScholarSync.Infrastructure.Security
{
    /// <summary>
    /// Password hashing service using BCrypt
    /// Simple static-like usage pattern similar to DbConnector
    /// </summary>
    public class PasswordHasher
    {
        private readonly SecuritySettings _securitySettings;

        public PasswordHasher()
        {
            _securitySettings = AppConfiguration.Instance.Security ?? new SecuritySettings
            {
                PasswordMinLength = 8,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireSpecialChar = false,
                BCryptWorkFactor = 12
            };
        }

        /// <summary>
        /// Hashes a plain-text password using BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, _securitySettings.BCryptWorkFactor);
        }

        /// <summary>
        /// Verifies a plain-text password against a hashed password
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (SaltParseException)
            {
                // Hash is invalid or not a BCrypt hash
                return false;
            }
        }

        /// <summary>
        /// Validates password against security requirements
        /// </summary>
        public PasswordValidationResult ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                return PasswordValidationResult.Failure("Password is required");
            }

            // Check minimum length
            if (password.Length < _securitySettings.PasswordMinLength)
            {
                errors.Add($"Password must be at least {_securitySettings.PasswordMinLength} characters long");
            }

            // Check for uppercase
            if (_securitySettings.RequireUppercase && !password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            // Check for lowercase
            if (_securitySettings.RequireLowercase && !password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            // Check for digit
            if (_securitySettings.RequireDigit && !password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number");
            }

            // Check for special character
            if (_securitySettings.RequireSpecialChar && !password.Any(c => !char.IsLetterOrDigit(c)))
            {
                errors.Add("Password must contain at least one special character");
            }

            // Check for common weak passwords
            if (IsCommonPassword(password))
            {
                errors.Add("Password is too common. Please choose a more secure password");
            }

            return errors.Count == 0
                ? PasswordValidationResult.Success()
                : PasswordValidationResult.Failure(errors.ToArray());
        }

        /// <summary>
        /// Checks if password is in the list of common weak passwords
        /// </summary>
        private bool IsCommonPassword(string password)
        {
            // List of common weak passwords (extend this list as needed)
            var commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "password", "Password1", "12345678", "qwerty", "abc123",
                "password123", "admin", "admin123", "letmein", "welcome",
                "monkey", "dragon", "master", "sunshine", "princess",
                "football", "iloveyou", "shadow", "superman", "trustno1"
            };

            return commonPasswords.Contains(password);
        }

        /// <summary>
        /// Checks if a password needs to be rehashed (e.g., work factor changed)
        /// </summary>
        public bool NeedsRehash(string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, _securitySettings.BCryptWorkFactor);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Result of password validation
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public string[] Errors { get; set; }

        public PasswordValidationResult()
        {
            Errors = new string[0];
        }

        public static PasswordValidationResult Success()
        {
            return new PasswordValidationResult { IsValid = true };
        }

        public static PasswordValidationResult Failure(params string[] errors)
        {
            return new PasswordValidationResult { IsValid = false, Errors = errors };
        }
    }
}

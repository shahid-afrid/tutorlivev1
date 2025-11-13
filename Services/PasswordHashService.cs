using BCrypt.Net;

namespace TutorLiveMentor.Services
{
    /// <summary>
    /// Service for secure password hashing and verification using BCrypt
    /// </summary>
    public class PasswordHashService
    {
        /// <summary>
        /// Hash a plain text password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>BCrypt hashed password</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Using BCrypt with work factor 12 (recommended for security)
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Verify a password against its hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="hashedPassword">BCrypt hashed password</param>
        /// <returns>True if password matches, false otherwise</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (SaltParseException)
            {
                // Handle legacy plain text passwords
                // This allows gradual migration from plain text to hashed passwords
                return password == hashedPassword;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if a password needs rehashing (for security upgrades)
        /// </summary>
        /// <param name="hashedPassword">BCrypt hashed password</param>
        /// <returns>True if rehash is needed</returns>
        public bool NeedsRehash(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return true;

            // Check if it's a BCrypt hash (starts with $2a$, $2b$, or $2y$)
            if (!hashedPassword.StartsWith("$2"))
                return true; // Plain text password, needs hashing

            // Could add additional checks for old work factors if needed
            return false;
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>Tuple of (isValid, errorMessage)</returns>
        public (bool isValid, string errorMessage) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long");

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpperCase)
                return (false, "Password must contain at least one uppercase letter");

            if (!hasDigit)
                return (false, "Password must contain at least one digit");

            if (!hasSpecialChar)
                return (false, "Password must contain at least one special character");

            return (true, string.Empty);
        }
    }
}

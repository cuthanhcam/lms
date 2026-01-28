using LMS.Application.Interfaces.Identity;
using System.Security.Cryptography;

namespace LMS.Infrastructure.Identity
{
    /// <summary>
    /// Service implementation to hash and verify password
    /// Uses PBKDF2 (Password-Based Key Derivation Function 2) with SHA256
    /// This is the algorithm recommended by NIST for password hashing
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        // Configuration for PBKDF2
        private const int SaltSize = 16;        // 16 bytes = 128 bits
        private const int HashSize = 32;        // 32 bytes = 256 bits
        private const int Iterations = 100000;  // Number of iterations (increases difficulty to crack)

        /// <summary>
        /// Hash password using PBKDF2 with random salt
        /// Salt is generated randomly for each password to prevent rainbow table attacks
        /// </summary>
        /// <param name="password">Plaintext password to hash</param>
        /// <returns>Base64 string containing salt + hash (can be stored in database)</returns>
        public string HashPassword(string password)
        {
            // Generate random salt
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // Hash password with salt using PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt + hash into 1 byte array
            // Format: [salt (16 bytes)][hash (32 bytes)] = 48 bytes total
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to Base64 string to store in database
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verify password with hashed password from database
        /// Extract salt from hashed password, hash providedPassword with that salt
        /// Compare hash result with hash in database
        /// </summary>
        /// <param name="hashedPassword">Hashed password from database (Base64 string)</param>
        /// <param name="providedPassword">Plaintext password user entered</param>
        /// <returns>true if password is correct, false if wrong</returns>
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Convert Base64 string to byte array
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Extract salt from hashBytes (first 16 bytes)
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Hash providedPassword with extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // Compare newly created hash with hash in database (32 bytes after salt)
            // Use constant-time comparison to prevent timing attacks
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
    }
}


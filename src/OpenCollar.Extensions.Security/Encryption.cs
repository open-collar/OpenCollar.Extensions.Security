/*
 * This file is part of OpenCollar.Extensions.Collections.
 *
 * OpenCollar.Extensions.Collections is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Collections is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Collections.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using OpenCollar.Extensions.Validation;

namespace OpenCollar.Extensions.Security
{
    /// <summary>
    ///     Simple encryption utilities.
    /// </summary>
    public static class Encryption
    {
        // http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
        /// <summary>
        ///     This constant is used to determine the keysize of the encryption algorithm.
        /// </summary>
        private const int KeySize = 256;

        /// <summary>
        ///     The initial vector bytes.
        /// </summary>
        [JetBrains.Annotations.NotNull]
        private static readonly byte[] _initialVectorBytes =
            Convert.FromBase64String("kehB0z+zOguuaDI3CWjTXA==");

        /// <summary>
        ///     Decrypts the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">
        ///     The encrypted text to decrypt.
        /// </param>
        /// <param name="key">
        ///     The string the acts as a pass phrase.
        /// </param>
        /// <returns>
        ///     A plaintext string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="key" /> is zero length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="encryptedText" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="encryptedText" /> is zero length.
        /// </exception>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        [JetBrains.Annotations.NotNull]
        public static string Decrypt([JetBrains.Annotations.NotNull] string encryptedText, [JetBrains.Annotations.NotNull] string key)
        {
            encryptedText.Validate(nameof(encryptedText), StringIs.NotNullOrEmpty);
            key.Validate(nameof(key), StringIs.NotNullOrEmpty);

            var encryptedTextBytes = Convert.FromBase64String(encryptedText);
            Debug.Assert(encryptedTextBytes != null, "encryptedTextBytes != null");

            using(var password = new Rfc2898DeriveBytes(key, _initialVectorBytes))
            {
                var keyBytes = password.GetBytes(KeySize / 8);
                using(var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using(var decryptor = symmetricKey.CreateDecryptor(keyBytes, _initialVectorBytes))
                    {
                        using(var memoryStream = new MemoryStream(encryptedTextBytes))
                        {
                            using(var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[encryptedTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                                // ReSharper disable once PossibleNullReferenceException
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">
        ///     The plain text string to encrypt.
        /// </param>
        /// <param name="key">
        ///     The string the acts as a pass phrase.
        /// </param>
        /// <returns>
        ///     A string containing an encrypted representation of the text given
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="key" /> is zero length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="plainText" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="plainText" /> is zero length.
        /// </exception>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        [JetBrains.Annotations.NotNull]
        public static string Encrypt([JetBrains.Annotations.NotNull] string plainText, [JetBrains.Annotations.NotNull] string key)
        {
            plainText.Validate(nameof(plainText), StringIs.NotNullOrEmpty);
            key.Validate(nameof(key), StringIs.NotNullOrEmpty);

            // ReSharper disable once PossibleNullReferenceException
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Debug.Assert(plainTextBytes != null, "plainTextBytes != null");

            using(var password = new Rfc2898DeriveBytes(key, _initialVectorBytes))
            {
                var keyBytes = password.GetBytes(KeySize / 8);
                using(var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using(var encryptor = symmetricKey.CreateEncryptor(keyBytes, _initialVectorBytes))
                    {
                        using(var memoryStream = new MemoryStream())
                        {
                            using(var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var encryptedTextBytes = memoryStream.ToArray();
                                var encryptedText = Convert.ToBase64String(encryptedTextBytes);
                                return encryptedText;
                            }
                        }
                    }
                }
            }
        }
    }
}
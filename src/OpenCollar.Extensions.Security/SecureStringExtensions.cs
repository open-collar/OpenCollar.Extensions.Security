/*
 * This file is part of OpenCollar.Extensions.Security.
 *
 * OpenCollar.Extensions.Security is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Security is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Security.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using JetBrains.Annotations;

namespace OpenCollar.Extensions.Security
{
    /// <summary>
    ///     Extensions to the <see cref="SecureString" /> class and related helpers.
    /// </summary>
    public static class SecureStringExtensions
    {
        /// <summary>
        ///     Converts a standard .NET string into a secure string.
        /// </summary>
        /// <param name="value">
        ///     The .NET string to convert. If <see langword="null" /> then <see langword="null" /> is returned.
        /// </param>
        /// <returns>
        ///     A read-only secure string containing the value supplied in the .NET string, or <see langword="null" />
        ///     if that was supplied. It is the callers responsibility to dispose of the secure string when finished.
        /// </returns>
        [ContractAnnotation("null=>null;notnull=>notnull")]
        public static SecureString ToSecureString([CanBeNull] this string value)
        {
            if(ReferenceEquals(value, null))
            {
                return null;
            }

            var s = new SecureString();
            foreach(var c in value)
            {
                s.AppendChar(c);
            }

            s.MakeReadOnly();
            return s;
        }

        /// <summary>
        ///     Converts secure string to a standard .NET string.
        /// </summary>
        /// <param name="value">
        ///     The secure string to convert. If <see langword="null" /> then <see langword="null" /> is returned.
        /// </param>
        /// <returns>
        ///     A .NET string containing the value supplied in the secure string, or <see langword="null" /> if that was supplied.
        /// </returns>
        [ContractAnnotation("null=>null;notnull=>notnull")]
        public static string ToSystemString([CanBeNull] this SecureString value)
        {
            if(ReferenceEquals(value, null))
            {
                return null;
            }

            var b = new StringBuilder();
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                for(var n = 0; n < value.Length; n++)
                {
                    var unicodeChar = Marshal.ReadInt16(valuePtr, n * sizeof(char));
                    b.Append(new string(Convert.ToChar(unicodeChar), 1));
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }

            return b.ToString();
        }
    }
}
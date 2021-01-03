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
using System.Security.Principal;
using System.Threading;

namespace OpenCollar.Extensions.Security
{
    /// <summary>
    ///     A class providing static utility methods related to impersonation.
    /// </summary>
    public static class Threading
    {
        /// <summary>
        ///     Returns the user on the current thread.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the current thread's user identity.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification =
            "Use of method implies cost.")]
        [JetBrains.Annotations.NotNull]
        public static IPrincipal GetThreadPrincipal()
        {
            // This ensures that the current principal has been evaluated, otherwise our context may not be quite right.
            // See: http://www.grimes.demon.co.uk/workshops/secWSEight.htm
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            // This forces the current principal value to be re-evaluated, otherwise we just get whatever the last
            // evaluation returned.
            Thread.CurrentPrincipal = null;

            // ReSharper disable PossibleNullReferenceException
            Debug.Assert(Thread.CurrentPrincipal != null, "Thread.CurrentPrincipal != null");
            return Thread.CurrentPrincipal;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        ///     Returns the user on the current thread.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the current thread's user identity.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification =
            "Use of method implies cost.")]
        [JetBrains.Annotations.NotNull]
        public static string GetThreadUser()
        {
            var principal = GetThreadPrincipal();
            var identity = principal.Identity;

            Debug.Assert(identity.Name != null, "identity.Name != null");
            return identity.Name;
        }
    }
}
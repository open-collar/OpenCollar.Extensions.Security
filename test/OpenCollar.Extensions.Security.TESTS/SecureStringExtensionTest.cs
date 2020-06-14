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

using System.Security;

using Xunit;

namespace OpenCollar.Extensions.Security.TESTS
{
    public sealed class SecureStringExtensionTest
    {
        [Fact]
        public void TestSecureStringRoundTrip()
        {
            const string test = @"TEST STRING 1";

            var secure = test.ToSecureString();

            Assert.NotNull(secure);
            Assert.IsType<SecureString>(secure);

            var insecure = secure.ToSystemString();

            Assert.NotNull(insecure);
            Assert.IsType<string>(insecure);

            Assert.Equal(test, insecure);

            secure = ((string)null).ToSecureString();

            Assert.Null(secure);

            insecure = secure.ToSystemString();

            Assert.Null(insecure);
        }
    }
}
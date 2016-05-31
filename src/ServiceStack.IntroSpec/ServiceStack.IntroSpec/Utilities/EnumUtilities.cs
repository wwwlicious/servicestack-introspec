// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Utilities
{
    using System;
    using Infrastructure;
    using Logging;

    public class EnumUtilities
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EnumUtilities));

        /// <summary>
        /// Attempt to parse value to provided enum type. This is case insensitive.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="toParse">String value to parse</param>
        /// <returns>Result containing parsed value if successful</returns>
        /// <remarks>There is no generic constraint hence where T : struct, IConvertible constraint
        /// http://stackoverflow.com/questions/79126/create-generic-method-constraining-t-to-an-enum</remarks>
        public static Result<T> SafeParse<T>(string toParse)
            where T : struct, IConvertible
        {
            try
            {
                var result = (T)Enum.Parse(typeof(T), toParse, true);
                return Result<T>.Success(result);
            }
            catch (ArgumentException e)
            {
                log.Warn($"Error parsing value {toParse} to enum {typeof(T).Name}", e);
            }

            return Result<T>.Fail();
        }
    }
}

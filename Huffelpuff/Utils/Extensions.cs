﻿/*
 *  The Huffelpuff Irc Bot, versatile pluggable bot for IRC chats
 * 
 *  Copyright (c) 2008-2010 Thomas Bruderer <apophis@apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;

namespace Huffelpuff.Utils
{
    /// <summary>
    /// Description of Extensions.
    /// </summary>
    public static class Extensions
    {
        public static string ToStringAll(this object obj)
        {
            return null;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Fill(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static string FillKeyword(this string str, params string[] args)
        {
            if (args.Length % 2 != 0) throw new IndexOutOfRangeException("the arguments must be in pairs");
            var index = 0;

            while (index < args.Length)
            {
                var key = args[index];
                var value = args[index + 1];

                str = str.Replace(key, value);

                index += 2;
            }
            return str;
        }

        public static List<string> ToLines(this IEnumerable<string> list, int maxlinelength)
        {
            return list.ToLines(maxlinelength, ", ");
        }

        public static string ToRelativeTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                dateTime = dateTime.ToLocalTime();
            }

            var span = DateTime.Now - dateTime;
 
            if (span.TotalDays > 730)
                return "{0} years ago".Fill((int)(span.TotalDays / 365));
            if (span.TotalDays > 365)
                return "one year ago";
            if (span.TotalDays >= 2)
                return "{0} days ago".Fill((int)(span.TotalDays));
            if (span.TotalDays >= 1)
                return "one day ago";
            if (span.TotalHours >= 2)
                return "{0} hours ago".Fill((int)(span.TotalHours));
            if (span.TotalHours >= 1)
                return "one hour ago";
            if (span.TotalMinutes >= 2)
                return "{0} minutes ago".Fill((int)(span.TotalMinutes));
            if (span.TotalMinutes >= 1)
                return "one minute ago";
            if (span.TotalSeconds >= 2)
                return "{0} seconds ago".Fill((int)(span.TotalSeconds));
            if (span.TotalSeconds >= 1)
                return "one second ago";
            if (span.TotalSeconds > -300)
                return "moments ago".Fill(span.TotalDays);
            return "from the future";
        }


        public static List<string> ToLines(this IEnumerable<string> list, int maxlinelength, string seperator)
        {
            return list.ToLines(maxlinelength, ", ", null, null);
        }


        /// <summary>
        /// Makes a new list which concats all the elements togehther and has maximum line length
        /// </summary>
        /// <param name="list"></param>
        /// <param name="maxlinelength"></param>
        /// <param name="seperator"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static List<string> ToLines(this IEnumerable<string> list, int maxlinelength, string seperator, string prefix, string postfix)
        {
            if (prefix == null)
            {
                prefix = "";
            }
            var noSeparator = true;

            var result = new List<string> { prefix };

            foreach (string s in list)
            {
                if (result[result.Count - 1].Length + s.Length + seperator.Length > maxlinelength)
                {
                    if (!noSeparator)
                    {
                        result[result.Count - 1] = result[result.Count - 1] + seperator;
                    }
                    result.Add("");
                    noSeparator = true;
                }
                if (noSeparator)
                {
                    result[result.Count - 1] = result[result.Count - 1] + s;
                    noSeparator = false;
                }
                else
                {
                    result[result.Count - 1] = result[result.Count - 1] + seperator + s;
                }
            }
            if (!string.IsNullOrEmpty(postfix))
            {
                if (result[result.Count - 1].Length + postfix.Length > maxlinelength)
                {
                    result.Add("");
                }
                result[result.Count - 1] = result[result.Count - 1] + postfix;
            }
            return result;
        }

        /// <summary>
        /// Returns safely the Value at key, or the default value of this type. Alternative to TryGetValue without the out parameter.
        /// </summary>
        /// <typeparam name="TKey">Type of Key in dictionary</typeparam>
        /// <typeparam name="TValue">Type of Value in dictionary</typeparam>
        /// <param name="dictionary">this</param>
        /// <param name="key">The key</param>
        /// <returns>returns the Value or the default for this Type</returns>
        public static TValue GetSafe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;
            return dictionary.TryGetValue(key, out result) ? result : default(TValue);
        }

        public static string MessageTime(this DateTime time)
        {
            return time.ToString("HH:mm K", new CultureInfo("DE-ch", true));
        }

        private static readonly Regex WhiteSpaceMatch = new Regex(@"\s+");

        public static string RemoveDuplicateWhiteSpace(this string str)
        {
            return WhiteSpaceMatch.Replace(str, " ");
        }

        /// <summary>
        /// Recursively create directory
        /// </summary>
        /// <param name="dirInfo">Folder path to create.</param>
        public static void CreateDirectory(this DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null) CreateDirectory(dirInfo.Parent);
            if (!dirInfo.Exists) dirInfo.Create();
        }
    }
}

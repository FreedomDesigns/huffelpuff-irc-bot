﻿/*
 *  <project description>
 * 
 *  Copyright (c) 2008-2009 Thomas Bruderer <apophis@apophis.ch>
 *  File created by apophis at 07.09.2009 00:27
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

namespace Huffelpuff.Tools
{
    public static class Log {
        
        private static Logger instance;
        
        public static Logger Instance {
            get {
                if (instance != null) {
                    return instance;
                } else {
                    #if SERVICE
                    return instance = new WindowsServiceLogger();
                    #else
                    switch (PersistentMemory.Instance.GetValue("logger")) {
                        case "console":
                            return instance = new ConsoleLogger();
                        default:
                            return instance = new NullLogger();
                    }
                    #endif
                }
            }
        }
    }
    
    /// <summary>
    /// Description of Logger.
    /// </summary>
    public abstract class Logger
    {
        public abstract void Log(string Message);
        public abstract void Log(string Message, Level level);
        public abstract void Log(string Message, Level level, ConsoleColor color);
        public abstract Level MinLogLevel { get; set; }
    }
}

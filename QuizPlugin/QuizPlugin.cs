﻿/*
 *  The Radio Plugin controls a Radio Stream with the liquidsoap API
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


using Huffelpuff;
using Huffelpuff.Plugins;

namespace Plugin
{
    /// <summary>
    /// Description of QuizPlugin.
    /// </summary>
    public class QuizPlugin : AbstractPlugin
    {
        public QuizPlugin(IrcBot botInstance) :
            base(botInstance) { }

        public override string Name
        {
            get
            {
                return "Quiz Bot";
            }
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override string AboutHelp()
        {
            return "";
        }
    }
}
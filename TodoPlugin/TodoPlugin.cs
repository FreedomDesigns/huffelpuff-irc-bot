﻿/*
 *  Todo Plugin
 *  ---------------------------------------------------------
 * 
 *  Copyright (c) 2011 Thomas Bruderer <apophis@apophis.ch>
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

using apophis.SharpIRC;
using Huffelpuff;
using Huffelpuff.Plugins;

namespace Plugin
{
    public class TodoPlugin : AbstractPlugin
    {

        public TodoPlugin(IrcBot botInstance) :
            base(botInstance) { }
        public override string AboutHelp()
        {
            return "The Todo Plugin remembers todo items for you individually or on the channel.";
        }


        public override void Init()
        {

            base.Init();
        }

        public override void Activate()
        {
            BotMethods.AddCommand(new Commandlet("!todo", "HELP", TodoHandler, this));
            BotMethods.AddCommand(new Commandlet("!done", "HELP", DoneHandler, this));

            base.Activate();
        }

        public override void Deactivate()
        {
            BotMethods.RemoveCommand("!todo");
            BotMethods.RemoveCommand("!done");

            base.Deactivate();
        }

        public override void DeInit()
        {
            base.DeInit();
        }

        public override void OnTick()
        {

        }

        private void TodoHandler(object sender, IrcEventArgs e)
        {

        }

        private void DoneHandler(object sender, IrcEventArgs e)
        {

        }
    }
}
/*
 *  <project description>
 * 
 *  Copyright (c) 2008-2009 Thomas Bruderer <apophis@apophis.ch>
 *  File created by apophis at 04.07.2009 00:46
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
using Meebey.SmartIrc4net;
using System;

namespace Huffelpuff
{
    /// <summary>
    /// Description of SettingCommands.
    /// </summary>
    public class SettingCommands
    {
        private IrcBot bot;
        
        public SettingCommands(IrcBot bot)
        {
            this.bot = bot;
            this.bot.AddCommand(new Commandlet("!nick", "the command !nick <new nickname> changes the nick, not only temporarily but also in the settings", ChangeNick, this, CommandScope.Both, "nick_change_access"));
            this.bot.AddCommand(new Commandlet("!set", "the command !nick <new nickname> changes the nick, not only temporarily but also in the settings", ConfigSet, this, CommandScope.Both, "config_set_access"));
        }
        
        private bool isValidNick(string nick) {
            // TODO: Check if its a valid nick
            return true;
        }
        
        private void ChangeNick(object sender, IrcEventArgs e) {
            if (e.Data.MessageArray.Length > 1){
                if (isValidNick(e.Data.MessageArray[1])) {
                    this.bot.RfcNick(e.Data.MessageArray[1]);
                    PersistentMemory.Instance.ReplaceValue("nick", e.Data.MessageArray[1]);
                }
            }
        }
        
        private void ConfigSet(object sender, IrcEventArgs e) {
            
        }
    }
}
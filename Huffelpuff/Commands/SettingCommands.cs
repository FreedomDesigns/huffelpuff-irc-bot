/*
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

using System.Linq;
using apophis.SharpIRC;
using Huffelpuff.Properties;
using Huffelpuff.Utils;

namespace Huffelpuff.Commands
{
    /// <summary>
    /// Description of SettingCommands.
    /// </summary>
    public class SettingCommands
    {
        private readonly IrcBot bot;

        public SettingCommands(IrcBot bot)
        {
            this.bot = bot;
            this.bot.AddCommand(new Commandlet("!nick", "the command !nick <new nickname> changes the nick, not only temporarily but also in the settings", ChangeNick, this, CommandScope.Both, "nick_change_access"));
            this.bot.AddCommand(new Commandlet("!get", "the command !get can read bot settings", ConfigGet, this, CommandScope.Both, "config_get_access"));
            this.bot.AddCommand(new Commandlet("!set", "the command !set can change bot settings", ConfigSet, this, CommandScope.Both, "config_set_access"));
        }

        private bool isValidNick(string nick)
        {
            // TODO: Check if its a valid nick
            return true;
        }

        private void ChangeNick(object sender, IrcEventArgs e)
        {
            if (e.Data.MessageArray.Length <= 1) { return; }            
            if (!isValidNick(e.Data.MessageArray[1])) { return; }

            bot.RfcNick(e.Data.MessageArray[1]);
            Settings.Default.Nick = e.Data.MessageArray[1];
            Settings.Default.Save();
        }

        private void ConfigGet(object sender, IrcEventArgs e)
        {
            string sendto = e.Data.Channel.IsNullOrEmpty() ? e.Data.Nick : e.Data.Channel;
            if (e.Data.MessageArray.Length < 2)
            {
                foreach (var line in bot.Properties.GetType().GetProperties().Where(property => property.CanRead).Select(property => property.Name).ToLines(350))
                {
                    bot.SendMessage(SendType.Message, sendto, line);
                }
            }
            else
            {
                var propertyInfos = bot.Properties.GetType().GetProperties().Where(property => property.CanRead && property.Name == e.Data.MessageArray[1]).SingleOrDefault();
                if (propertyInfos != null)
                    bot.SendMessage(SendType.Message, sendto, "Current Value: " + propertyInfos.GetValue(bot.Properties, null));
                else
                    bot.SendMessage(SendType.Message, sendto, "Dont know that property");
            }
        }


        private void ConfigSet(object sender, IrcEventArgs e)
        {
            string sendto = e.Data.Channel.IsNullOrEmpty() ? e.Data.Nick : e.Data.Channel;
            if (e.Data.MessageArray.Length < 3)
            {
                foreach (var line in bot.GetType().GetProperties().Where(property => property.CanWrite).Select(property => property.Name).ToLines(350))
                {
                    bot.SendMessage(SendType.Message, sendto, line);
                }
            }
            else
            {
                var propertyInfos = bot.GetType().GetProperties().Where(property => property.CanWrite && property.Name == e.Data.MessageArray[1]).SingleOrDefault();
                if (propertyInfos != null)
                {
                    if (propertyInfos.PropertyType == typeof(int))
                        propertyInfos.SetValue(bot, int.Parse(e.Data.MessageArray[2]), null);
                    if (propertyInfos.PropertyType == typeof(string))
                        propertyInfos.SetValue(bot, e.Data.MessageArray[2], null);

                    bot.SendMessage(SendType.Message, sendto, "Value set (if string or int)");
                }
                else
                    bot.SendMessage(SendType.Message, sendto, "Dont know that property");
            }
        }
    }
}

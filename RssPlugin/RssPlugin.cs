﻿/*
 *  UT3GlobalStatsPlugin, Access to the GameSpy Stats for UT3
 * 
 *  Copyright (c) 2007-2009 Thomas Bruderer <apophis@apophis.ch> <http://www.apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 2 of the License, or
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
using System.Xml;
using System.Timers;
using System.Collections.Generic;

using Huffelpuff;
using Huffelpuff.Plugins;
using Meebey.SmartIrc4net;

namespace Plugin
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class RssPlugin : AbstractPlugin
    {
        public RssPlugin(IrcBot botInstance) :
            base(botInstance) {}
        
        private Timer checkInterval;
        private bool firstrun = true;
        private DateTime lastpost = DateTime.MinValue;
        
        
        public override void Init()
        {
            checkInterval = new Timer();
            checkInterval.Elapsed += checkInterval_Elapsed;
            checkInterval.Interval = 1 * 60 * 1000; // 1 minute
            PersistentMemory.Instance.GetValueOrTodo("rssFeed"); // make sure we have one!
            base.Init();
        }

        void checkInterval_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!BotMethods.IsConnected)
                return;
            //var feeds = PersistentMemory.Instance.GetValues("rssFeed")
            List<RssItem> rss = null;
            if (firstrun) {
                firstrun = false;
                foreach(string chan in this.BotMethods.GetChannels()) {
                    //BotMethods.SendMessage(SendType.Notice, chan, "RSS Plugin loaded with Feed: " + PersistentMemory.GetValue("rssFeed"));
                    //BotMethods.SendMessage(SendType.Notice, chan, "Last Post: " + IrcConstants.IrcBold + rss[0].Title + IrcConstants.IrcBold  + " was published on " + rss[0].Published.ToString() + " by " + IrcConstants.IrcBold + IrcConstants.IrcColor + ((int)IrcColors.Blue) + rss[0].Author + IrcConstants.IrcBold + IrcConstants.IrcColor + " in " + rss[0].Category + " -> " + rss[0].Link);
                }
                lastpost = rss[0].Published;
            } else if (lastpost < rss[0].Published) {
                foreach(string chan in this.BotMethods.GetChannels()) {
                    BotMethods.SendMessage(SendType.Notice, chan, "New Post: " + IrcConstants.IrcBold + rss[0].Title + IrcConstants.IrcBold  + " was published on " + rss[0].Published.ToString() + " by " + IrcConstants.IrcBold + IrcConstants.IrcColor + ((int)IrcColors.Blue) + rss[0].Author + IrcConstants.IrcBold + IrcConstants.IrcColor + " in " + rss[0].Category + " -> " + rss[0].Link);
                }
                lastpost = rss[0].Published;
            }
            
        }
        
        public override void Activate()
        {
            BotMethods.AddCommand(new Commandlet("!rss", "With the command !rss you'll get a list of the bots configured RSS feed.", showRss, this));
            checkInterval.Enabled = true;
            
            base.Activate();
        }
        
        public override void Deactivate()
        {
            BotMethods.RemoveCommand("!rss");
            
            base.Deactivate();
        }
        
        public override string AboutHelp()
        {
            return "The Rss Plugins reports new posts on the configured RSS feed to the channel, and it provides access to the complete rss via the !rss command";
        }
        
        private void showRss(object sender, IrcEventArgs e) {
            string sendto = (string.IsNullOrEmpty(e.Data.Channel))?e.Data.Nick:e.Data.Channel;
            int idx = 0;
            if (e.Data.MessageArray.Length > 1) {
                int.TryParse(e.Data.MessageArray[1], out idx);
            }
            
            List<RssItem> items = getRss(PersistentMemory.Instance.GetValue("rssFeed"));
            if ((idx <= items.Count) && (idx > 0)) {
                idx--;
            } else {
                idx = 0;
            }
            
            BotMethods.SendMessage(SendType.Notice, e.Data.Channel, items[idx].Title + " was published on " + items[idx].Published.ToString() + " by " + items[idx].Author + " in " + items[idx].Category + " -> " + items[idx].Link);
            
            /*            } else {
                foreach(RssItem item in getRss(PersistentMemory.Instance.GetValue("rssFeed"))) {
                    BotMethods.SendMessage(SendType.Notice, sendto, item.Title + " was published on " + item.Published.ToString() + " by " + item.Author + " in " + item.Category + " -> " + item.Link);
                }
            } */
        }

        
        private List<RssItem> getRss(string uri)
        {
            List<RssItem> rss = new List<RssItem>();

            XmlReader feed = XmlReader.Create(uri);
            while(feed.Read()){
                if ((feed.NodeType == XmlNodeType.Element) && (feed.Name == "item")) {
                    rss.Add(getItem(feed));
                }
            }
            return rss;
        }
        
        private RssItem getItem(XmlReader feed)  {

            string title = "", author = ""; string link = ""; string desc = ""; string category = ""; string content = "";

            DateTime published = DateTime.MinValue;
            
            while(feed.Read()){
                if ((feed.NodeType == XmlNodeType.EndElement) && (feed.Name == "item")) {
                    break;
                }
                if (feed.NodeType == XmlNodeType.Element) {
                    switch(feed.Name) {
                            // Main Items every RSS feed has
                            case "title": feed.Read();
                            title = feed.ReadContentAsString();
                            break;
                            case "link":feed.Read();
                            link = feed.ReadContentAsString();
                            break;
                            case "description":feed.Read();
                            desc = feed.ReadContentAsString();
                            break;
                            // The pubDate is important for notifying
                            case "pubDate":feed.Read();
                            published = DateTime.Parse(feed.ReadContentAsString());
                            break;
                            // Some more RSS 2.0 Standard fields.
                            case "category":feed.Read();
                            category = feed.ReadContentAsString();
                            break;
                            case "author":feed.Read();
                            author = feed.ReadContentAsString();
                            break;
                            case "guid":feed.Read();
                            //link = feed.ReadContentAsString();
                            break;
                            // Special ones (for vBulletin)
                            case "content:encoded":feed.Read();
                            content = feed.ReadContentAsString();
                            break;
                            case "dc:creator":feed.Read();
                            author = feed.ReadContentAsString();
                            break;
                            case "comments":feed.Read();
                            //Comment
                            break;
                            case "wfw:commentRss":feed.Read();
                            //Comment LInk
                            break;
                            default: Console.WriteLine("unparsed Element: " + feed.Name);
                            break;
                    }
                }
            }


            return new RssItem(title, author, published, link, desc, category, content);
        }
    }
}
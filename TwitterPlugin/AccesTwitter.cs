/*
 *  <project description>
 * 
 *  Copyright (c) 2008-2009 Thomas Bruderer <apophis@apophis.ch>
 *  File created by apophis at 04.07.2009 16:10
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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Plugin
{
    /// <summary>
    /// Description of AccesTwitter.
    /// </summary>
    public class AccesTwitter
    {
        private string baseUrlApi;
        private string baseUrlUser;

        private string user;
        
        public string User {
            get { return user; }
            set { user = value; }
        }
        
        public string FeedUrl {
            get { return baseUrlUser + user + "/"; }
        }
        
        private string pass;
        
        public string Pass {
            get { return pass; }
            set { pass = value; }
        }
        
        public AccesTwitter(string baseUrlApi, string baseUrlUser)
        {
            this.baseUrlApi = baseUrlApi;
            this.baseUrlUser = baseUrlUser;
            
        }
        
        public IEnumerable<TwitterMention> GetNewMentions() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrlApi + "statuses/mentions.xml");
            request.Credentials = new NetworkCredential(user, pass);
            request.PreAuthenticate = true;
            request.Method = "GET";

            WebResponse webResponse = request.GetResponse();
            XmlTextReader reader = new XmlTextReader(webResponse.GetResponseStream());
            
            bool isUser = false;
            TwitterMention mention = null;
            List<TwitterMention> mentions = new List<TwitterMention>();
            while (reader.Read())
            {
                if ((reader.Name == "status") && (reader.NodeType == XmlNodeType.Element)) {
                    mention = new TwitterMention(this.baseUrlUser);
                    mentions.Add(mention);
                }
                if (reader.Name == "user") {
                    if (reader.NodeType == XmlNodeType.Element) {
                        isUser = true;
                    } else {
                        isUser = false;
                    }
                }
                
                //mention
                if ((!isUser) && (reader.Name == "id") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.Id = long.Parse(reader.Value);
                }
                if ((!isUser) && (reader.Name == "text") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.Text = reader.Value;
                }
                if ((!isUser) && (reader.Name == "created_at") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.Created = DateTime.ParseExact(reader.Value.Insert(23, ":"), "ddd MMM dd HH:mm:ss K yyyy", new CultureInfo("EN-us", true));
                }
                if ((!isUser) && (reader.Name == "source") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.Source = reader.Value;
                }
                
                //user
                if ((isUser) && (reader.Name == "id") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Id = long.Parse(reader.Value);
                }
                if ((isUser) && (reader.Name == "name") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Name = reader.Value;
                }
                if ((isUser) && (reader.Name == "screen_name") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Nick = reader.Value;
                }
                if ((isUser) && (reader.Name == "location") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Location = reader.Value;
                }
                
                if ((isUser) && (reader.Name == "description") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Description = reader.Value;
                }
                if ((isUser) && (reader.Name == "followers_count") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Followers = int.Parse(reader.Value);
                }
                if ((isUser) && (reader.Name == "friends_count") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Friends = int.Parse(reader.Value);
                }
                if ((isUser) && (reader.Name == "created_at") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Created = DateTime.ParseExact(reader.Value.Insert(23, ":"), "ddd MMM dd HH:mm:ss K yyyy", new CultureInfo("EN-us", true));
                }
                if ((isUser) && (reader.Name == "statuses_count") && (reader.NodeType == XmlNodeType.Element)) {
                    reader.Read();
                    mention.User.Statuses = int.Parse(reader.Value);
                }
            }
            return mentions;
        }
        
        public void StatusUpdate(string status) {
            StatusUpdate(status, null);
        }
        
        public void StatusUpdate(string status, long? in_reply_to_status_id) {
            System.Net.ServicePointManager.Expect100Continue = false;
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrlApi + "statuses/update.xml");
            request.Credentials = new NetworkCredential(user, pass);
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            status = TinyUrl.GetTinyUrl(status);
            
            if(status.Length > 140) {
                throw new Exception("Your message was too long (" + status.Length + "), please rephrase!");
            }
            
            ASCIIEncoding encoding=new ASCIIEncoding();
            string postData = "status=" + HttpUtility.UrlEncode(status);
            if (in_reply_to_status_id != null) {
                postData = postData + "&in_reply_to_status_id=" + HttpUtility.UrlEncode(in_reply_to_status_id.ToString());
            }
            byte[]  data = encoding.GetBytes(postData);
            
            request.ContentType="application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            
            Stream newStream=request.GetRequestStream();
            newStream.Write(data,0,data.Length);
            newStream.Close();
            WebResponse webResponse = request.GetResponse();
        }
        
    }
}
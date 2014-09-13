///////////////////////////////////////////////////////////////////////
// Conversational Library //////////////////// Conversational Config //
/////////////////// 2008  Magrathean Technologies /////////////////////
///////////////////////////////////////////////////////////////////////
//                                                                   //
// This program is free software; you can redistribute it and/or     //
// modify it under the terms of the GNU General Public License       //
// as published by the Free Software Foundation; either version 2    //
// of the License, or any later version.                             //
//                                                                   //
// This program is distributed in the hope that it will be useful,   //
// but WITHOUT ANY WARRANTY; without even the implied warranty of    // 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     //
// GNU General Public License for more details.                      //
//                                                                   //
// You should have received a copy of the GNU General Public License //
// along with this program; if not, write to the                     //
// Free Software Foundation, Inc.,                                   //
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.     //
//                                                                   //
//// Released under the GPL /////////////////// August 15th, 2008 /////
///////////////////////////////////////////////////////////////////////
////////////////////////////// Coded by: Fox Diller ///////////////////
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace ConversationalAPI
{
    /// <summary>
    /// This is a multiple choice response system for the Game's NPC's
    /// Written by Fox
    /// </summary>
    public class Conversational
    {
        #region Singleton Code
        /// <summary>
        /// Singleton Instance Object
        /// </summary>
        private static Conversational _instance;

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static Conversational Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Singleton Static Startup Function.
        /// Warning: Use this only once...
        /// </summary>
        /// <param name="file">The location of the Conversation SQL Lite Database file.</param>
        public static void Initialize(FileInfo file)
        {
            if (file.Exists)
            {
                _instance = new Conversational(file);
            }
            else
            {
                throw new ApplicationException("Conversational was unable to find it's mind.");
            }
        }

        /// <summary>
        /// Destroy the singleton
        /// Only for use with GUI configurator.
        /// </summary>
        public static void Destroy()
        {
            _instance._SQL.Close();
            _instance._SQL.Dispose();
            _instance._SQL = null;
            _instance._file = null;

            _instance = null;

            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Create a new brain file.
        /// </summary>
        /// <param name="filename">The filename to create</param>
        /// <returns>Successful or not. True or false.</returns>
        public static bool CreateNewBrainFile(string filename)
        {
            try
            {
                SQLiteConnection.CreateFile(filename);

                SQLiteConnection SQL = new SQLiteConnection(@"Data Source=" + filename);

                SQL.Open();

                SQLiteCommand createBotsTable = SQL.CreateCommand();
                createBotsTable.CommandText = "CREATE TABLE bots (bot_id INTEGER PRIMARY KEY, bot_name TEXT)";
                createBotsTable.ExecuteNonQuery();
                createBotsTable.Dispose();
                createBotsTable = null;

                SQLiteCommand createMacrosTable = SQL.CreateCommand();
                createMacrosTable.CommandText = "CREATE TABLE macros (macro_id INTEGER PRIMARY KEY, macro_search TEXT, macro_replace TEXT)";
                createMacrosTable.ExecuteNonQuery();
                createMacrosTable.Dispose();
                createMacrosTable = null;

                SQL.Close();
                SQL.Dispose();

                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
        #endregion Singleton

        FileInfo _file;
        SQLiteConnection _SQL;

        /// <summary>
        /// Class constructor
        /// </summary>
        public Conversational(FileInfo file)
        {
            this._file = file;
            _SQL = new SQLiteConnection(@"Data Source=" + _file.ToString());

            try
            {
                _SQL.Open();
            }
            catch (SQLiteException e)
            {
                throw new ApplicationException("Conversational was unable to parse it's brain. Please visit insane asylm.", e);
            }

            SQLiteCommand dataCheck = _SQL.CreateCommand();
            dataCheck.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name";
            SQLiteDataReader dataCheckReader = dataCheck.ExecuteReader();

            List<string> tables = new List<string>();

            while (dataCheckReader.Read())
            {
                tables.Add(dataCheckReader.GetString(0));
            }

            if (!tables.Contains("bots") || !tables.Contains("macros"))
            {
                throw new ApplicationException("This isn't a proper brains. I need real conversational brains!");
            }
        }

        #region Access Function Code
        /// <summary>
        /// Get the first ever ConversationalItem for the specified bot
        /// </summary>
        /// <param name="bot_name">The bot to retrieve the first ConversationalItem from</param>
        /// <returns>The ConversationalItem or null</returns>
        public ConversationalItem GetBotFirstConversationItem(string bot_name)
        {
            SQLiteCommand command = _SQL.CreateCommand();
            command.CommandText = "SELECT * FROM " + bot_name + " LIMIT 1";
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                return new ConversationalItem(bot_name, reader.GetInt32(0), reader.GetString(1), ConversationalResponseItems.DeSerialize(reader.GetString(2)));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get a ConversationalItem from a certain point in the conversation by specifing an ID
        /// </summary>
        /// <param name="bot_name">The bot to retrieve the specific ConversationalItem from</param>
        /// <param name="id">The ID of the conversational item needed</param>
        /// <returns>The ConversationalItme or null</returns>
        public ConversationalItem GetBotConversationByID(string bot_name, int id)
        {
            SQLiteCommand command = _SQL.CreateCommand();
            command.CommandText = "SELECT * FROM " + bot_name + " WHERE conversation_id = " + id.ToString();
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                return new ConversationalItem(bot_name, reader.GetInt32(0), reader.GetString(1), ConversationalResponseItems.DeSerialize(reader.GetString(2)));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Config Function Code
        public bool CreateNewBot(string name)
        {
            // Add the bot
            SQLiteCommand verifyExistCommand = _SQL.CreateCommand();
            verifyExistCommand.CommandText = "SELECT bot_id FROM bots WHERE bot_name = $name";
            verifyExistCommand.Parameters.AddWithValue("$name", name);

            SQLiteDataReader verifyExistReader = verifyExistCommand.ExecuteReader();

            if (!verifyExistReader.HasRows)
            {
                SQLiteCommand createBotCommmand = _SQL.CreateCommand();
                createBotCommmand.CommandText = "INSERT INTO bots (bot_name) VALUES($name)";
                createBotCommmand.Parameters.AddWithValue("$name", name);
                int createBotCommandAffectRows = createBotCommmand.ExecuteNonQuery();

                // Check to see if the insert command actually worked
                if (createBotCommandAffectRows != 0)
                {
                    // create the table for the conversation tree
                    SQLiteCommand createBotTableCommand = _SQL.CreateCommand();
                    createBotTableCommand.CommandText = "CREATE TABLE " + name + " (conversation_id INTEGER PRIMARY KEY, conversation_say TEXT, conversation_responseitems BLOB)";
                    createBotTableCommand.ExecuteNonQuery();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DeleteBot(string name)
        {
            SQLiteCommand verifyExistCommand = _SQL.CreateCommand();
            verifyExistCommand.CommandText = "SELECT bot_id FROM bots WHERE bot_name = $name";
            verifyExistCommand.Parameters.AddWithValue("$name", name);

            SQLiteDataReader verifyExistReader = verifyExistCommand.ExecuteReader();

            if (verifyExistReader.HasRows)
            {
                verifyExistCommand.Dispose();
                verifyExistReader.Dispose();

                SQLiteCommand removeBotTable = _SQL.CreateCommand();
                removeBotTable.CommandText = "DROP TABLE " + name;
                removeBotTable.ExecuteNonQuery();

                SQLiteCommand removeBot = _SQL.CreateCommand();
                removeBot.CommandText = "DELETE FROM bots WHERE bot_name = '" + name + "'";
                removeBot.ExecuteNonQuery();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddNewConversation(string bot_name, string say, ConversationalResponseItems items)
        {
            SQLiteCommand addNewResponseCommand = _SQL.CreateCommand();
            addNewResponseCommand.CommandText = "INSERT INTO " + bot_name + " (conversation_say, conversation_responseitems) VALUES($say, $items)";
            addNewResponseCommand.Parameters.AddWithValue("$say", say);
            addNewResponseCommand.Parameters.AddWithValue("$items", ConversationalResponseItems.Serialize(items));
            addNewResponseCommand.ExecuteNonQuery();
        }

        public void UpdateConversation(string bot_name, int id, string say, ConversationalResponseItems cri)
        {
            SQLiteCommand command = _SQL.CreateCommand();
            command.CommandText = "UPDATE " + bot_name + " SET conversation_say = $say, conversation_responseitems = $cri WHERE conversation_id = " + id.ToString();
            command.Parameters.AddWithValue("$say", say);
            command.Parameters.AddWithValue("$cri", ConversationalResponseItems.Serialize(cri));
            command.ExecuteNonQuery();
        }

        public void DeleteConversation(string bot_name, int id)
        {
            SQLiteCommand command = _SQL.CreateCommand();
            command.CommandText = "DELETE FROM " + bot_name + " WHERE conversation_id = " + id.ToString();
            command.ExecuteNonQuery();
        }

        public List<string> ListAllBots()
        {
            SQLiteCommand listAllBotsCommand = _SQL.CreateCommand();
            listAllBotsCommand.CommandText = "SELECT bot_name FROM bots";
            SQLiteDataReader reader = listAllBotsCommand.ExecuteReader();

            List<string> bots = new List<string>();

            while (reader.Read())
            {
                bots.Add(reader.GetString(0));
            }

            return bots;
        }

        public Dictionary<int, string> GetBotConversations(string bot_name)
        {
            SQLiteCommand getBotConversations = _SQL.CreateCommand();
            getBotConversations.CommandText = "SELECT conversation_id, conversation_say FROM " + bot_name;
            SQLiteDataReader reader = getBotConversations.ExecuteReader();

            if (reader.HasRows)
            {
                Dictionary<int, string> conversations = new Dictionary<int, string>();

                while (reader.Read())
                {
                    conversations.Add(reader.GetInt32(0), reader.GetString(1));
                }

                return conversations;
            }
            else
            {
                return new Dictionary<int, string>();
            }
        }

        public ConversationalResponseItems GetBotCRI(string bot_name, int conversationID)
        {
            SQLiteCommand command = _SQL.CreateCommand();
            command.CommandText = "SELECT conversation_responseitems FROM " + bot_name + " WHERE conversation_id = " + conversationID.ToString();
            SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();

            return ConversationalResponseItems.DeSerialize(reader.GetString(0));
        }
        #endregion

        /// <summary>
        /// The full path to the loaded brains.
        /// </summary>
        public string DatabaseFile
        {
            get 
            { 
                return _file.ToString(); 
            }
        }

    }

    /// <summary>
    /// This contains a conversation item itself...
    /// </summary>
    public class ConversationalItem
    {
        private string _botname;
        private int _conversationID;
        private string _say;
        private ConversationalResponseItems _cri;

        /// <summary>
        /// Create a new ConversationalItem
        /// </summary>
        /// <param name="botname">The botname for the brains</param>
        /// <param name="conversationID">The Conversation_ID for this conversation</param>
        /// <param name="say">What the bot says to the user</param>
        /// <param name="cri">The responses alloted to the user to respond</param>
        public ConversationalItem(string botname, int conversationID, string say, ConversationalResponseItems cri)
        {
            this._botname = botname;
            this._conversationID = conversationID;
            this._say = say;
            this._cri = cri;
        }

        /// <summary>
        /// The name of the bot that this ConversationalItem is linked to
        /// </summary>
        public string BotName
        {
            get { return _botname; }
        }

        /// <summary>
        /// The ID of the current conversation this ConversationalItem holds
        /// </summary>
        public int ConversationID
        {
            get { return _conversationID; }
        }

        /// <summary>
        /// What the bot will say during this conversation
        /// </summary>
        public string Say
        {
            get { return _say; }
        }

        /// <summary>
        /// The response(s) for the user to be given.
        /// </summary>
        public ConversationalResponseItems ConversationalResponseItems
        {
            get { return _cri; }
        }
    }
}

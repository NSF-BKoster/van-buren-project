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
using System.Runtime.InteropServices;

namespace ConversationalAPI
{
    public class ConversationalResponseItems
    {
        private List<ConversationalResponseItem> _responseItems;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConversationalResponseItems()
        {
            this._responseItems = new List<ConversationalResponseItem>();
        }

        /// <summary>
        /// Add a response to this conversational response item set
        /// </summary>
        /// <param name="responseAction">The conversation_id to move to after this selection.</param>
        /// <param name="responseText">The text choice to display to the user.</param>
        public void AddResponse(int responseAction, string responseText, string altresponse, string engineCommand, string visibilityOptions)
        {
            _responseItems.Add(new ConversationalResponseItem(responseAction, responseText, altresponse, engineCommand, visibilityOptions));
        }

        #region Fields
        /// <summary>
        /// Get the response items in this instance.
        /// </summary>
        public List<ConversationalResponseItem> ResponseItems
        {
            get { return _responseItems; }
        }
        #endregion

        #region Static Helper Functions
        /// <summary>
        /// Serialize a ConversationalResponseItems object into a string for SQL.
        /// </summary>
        /// <param name="cri">The ConversationalResponseItems object to serialize.</param>
        /// <returns>A string representing a ConversationalResponseItems object.</returns>
        public static string Serialize(ConversationalResponseItems cri)
        {
            StringBuilder serializePayload = new StringBuilder();

            serializePayload.Append("{");

            foreach (ConversationalResponseItem responseItem in cri.ResponseItems)
            {
                serializePayload.Append(responseItem.To.ToString());
                serializePayload.Append(":");
                serializePayload.Append("\"");
                serializePayload.Append(responseItem.Response);
                serializePayload.Append("\"");

                serializePayload.Append(":");
                serializePayload.Append(responseItem.AltResponse);

                serializePayload.Append(":");
                serializePayload.Append(responseItem.EngineCommand);

                serializePayload.Append(":");
                serializePayload.Append(responseItem.VisibilityOptions);


                serializePayload.Append("||||");
            }

            serializePayload.Append("}");

            return serializePayload.ToString();
        }

        /// <summary>
        /// DeSerialize a string into a ConversationalResponseItems object.
        /// </summary>
        /// <param name="payload">The string to de-serialize.</param>
        /// <returns>A parsed ConversationalResponseItems object from the string.</returns>
        public static ConversationalResponseItems DeSerialize(string payload)
        {
            string rawData = payload.Substring(1, payload.Length - 2);

            string[] seperator = new string[1] { "||||" };

            string[] splitdata = rawData.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            ConversationalResponseItems returnCRI = new ConversationalResponseItems();

            foreach (string data in splitdata)
            {
                string[] responseItemData = data.Split(':');

                string[] args = {"", "", ""};

                for (int i=0; i < 3; i++)
                {
                    if (responseItemData.Length > i+2 )
                        args[i] = responseItemData[i + 2];
                }

                returnCRI.AddResponse(int.Parse(responseItemData[0]), responseItemData[1].Substring(1, responseItemData[1].Length - 2), args[0], args[1], args[2]);
            }

            return returnCRI;
        }
        #endregion
    }
}

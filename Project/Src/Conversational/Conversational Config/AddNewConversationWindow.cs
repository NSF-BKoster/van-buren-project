//////////////////////////////////////////////////////////////////////
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ConversationalAPI;

namespace Conversational_Config
{
    public partial class AddNewConversationWindow : Form
    {
        private bool savedState;
        private string botName;
        private ConversationalResponseItems cri;

        public AddNewConversationWindow(string bot_name)
        {
            this.botName = bot_name;
            this.cri = new ConversationalResponseItems();

            InitializeComponent();
        }

        private void AddNewConversationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!savedState)
            {
                if (MessageBox.Show("Are you sure you want to close this window?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void criButton_Click(object sender, EventArgs e)
        {
            criWindow criWin = new criWindow(botName, ref cri);
            criWin.ShowDialog();
            criWin.Dispose();

            criTotal.Text = cri.ResponseItems.Count.ToString();

        }

        private void addNewConversationSaveButton_Click(object sender, EventArgs e)
        {
            Conversational.Instance.AddNewConversation(botName, newConversationSayTextBox.Text, cri);

            savedState = true;

            this.Close();
        }

        private void addNewConversationCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

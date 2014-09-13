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
using System.IO;
using System.Text.RegularExpressions;
using ConversationalAPI;

namespace Conversational_Config
{
    public partial class ConversationalConfigForm : Form
    {
        private bool isFileLoaded;

        public ConversationalConfigForm()
        {
            InitializeComponent();
        }
        
        #region Main Menu Event Handlers
        private void newBrainsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isFileLoaded)
            {
                CloseData();
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Conversational Brains DB (*.db3)|*.db3";
            sfd.FilterIndex = 1;

            DialogResult sfdResult = sfd.ShowDialog();

            if (sfdResult == DialogResult.OK)
            {
                if (Conversational.CreateNewBrainFile(sfd.FileName))
                {
                    Conversational.Initialize(new FileInfo(sfd.FileName));
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Unable to create new brain file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();

            loadFile.Filter = "Conversational Brains DB (*.db3)|*.db3";
            loadFile.FilterIndex = 1;

            DialogResult result = loadFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    Conversational.Initialize(new FileInfo(loadFile.FileName));
                }
                catch (ApplicationException)
                {
                    MessageBox.Show("This is not a conversational brains file.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadData();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseData();
        }

        private void cleanBrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cleaning a brain will clean any formatting errors with the database.\n\nThis is not recomended to use outside of Magrathean's Projects.\n\nAre you sure you wish to clean brains?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                MessageBox.Show("Could not find server.");
            }
        }

        private void quitConversationalConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by:\nFox Diller\n\nVersion 1.0\nMagrathean Technologies Internal Product\nFor Internal Use\n\nReleased Under GPL 2", "About Conversational Config", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }


        private void projectHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/conversational/");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/conversational/w/list");
        }
        #endregion

        #region File Methods
        private void LoadData()
        {
            if (isFileLoaded)
            {
                CloseData();
            }

            isFileLoaded = true;

            ReloadBotList();

            Text = Conversational.Instance.DatabaseFile;

            doInterfaceEnables(true);
        }

        private void CloseData()
        {
            if (MessageBox.Show("Are you sure you wish to close these brains?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Conversational.Destroy();

                isFileLoaded = false;

                Text = "Conversational Config 1.0";

                doInterfaceEnables(false);
            }
        }

        private void doInterfaceEnables(bool loaded)
        {
            if (loaded)
            {
                loadToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = true;
                cleanBrainToolStripMenuItem.Enabled = true;

                tabControl.Visible = true;
            }
            else
            {
                loadToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = false;
                cleanBrainToolStripMenuItem.Enabled = false;

                tabControl.Visible = false;
            }
        }

        private void ReloadBotList()
        {
            List<string> bots = Conversational.Instance.ListAllBots();

            listBoxOfBots.Items.Clear();

            if (bots.Count != 0)
            {
                foreach (string bot in bots)
                {
                    listBoxOfBots.Items.Add(bot);
                }
            }
        }

        private void LoadConversationalGrid()
        {
            conversationDataGrid.Visible = false;

            Dictionary<int, string> conversations = Conversational.Instance.GetBotConversations(listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString());

            conversationDataGrid.Rows.Clear();

            if (conversations.Count != 0)
            {
                foreach (int key in conversations.Keys)
                {
                    conversationDataGrid.Rows.Add(key, conversations[key]);
                }
            }

            conversationDataGrid.Visible = true;
        }
        #endregion

        #region ListBox Bots Event Handling Methods
        private void addBotToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ToolStripControlHost addNewBotTextBox = sender as ToolStripControlHost;

                string newName = Regex.Replace(addNewBotTextBox.Text, @"[\d+\W]+", "");

                if (newName.Length != 0)
                {
                    if (!Conversational.Instance.CreateNewBot(newName))
                    {
                        MessageBox.Show("This bot name is already in use, please try a variation...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        ReloadBotList();
                        addNewBotTextBox.Text = string.Empty;
                        botsContextMenuStrip.Hide();
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxOfBots.SelectedIndex >= 0 && listBoxOfBots.SelectedIndex < listBoxOfBots.Items.Count)
            {
                string botName = listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString();

                if (MessageBox.Show("Are you sure you want to delete the '" + botName + "' bot?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Conversational.Instance.DeleteBot(botName);

                    conversationDataGrid.Visible = false;
                    
                    ReloadBotList();
                }
            }
        }

        private void listBoxOfBots_SelectedValueChanged(object sender, EventArgs e)
        {
            // MessageBox.Show(listBoxOfBots.SelectedIndex.ToString());

            if (listBoxOfBots.SelectedIndex >= 0 && listBoxOfBots.SelectedIndex < listBoxOfBots.Items.Count)
            {
                LoadConversationalGrid();
            }
        }

        private void listBoxOfBots_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int indexOver = listBoxOfBots.IndexFromPoint(e.X, e.Y);

                if (indexOver >= 0 && indexOver < listBoxOfBots.Items.Count)
                {
                    listBoxOfBots.SelectedIndex = indexOver;
                    botsContextMenuStrip.Items["deleteToolStripMenuItem"].Enabled = true;
                }
                else
                {
                    botsContextMenuStrip.Items["deleteToolStripMenuItem"].Enabled = false;
                }
            }
        }
        #endregion

        #region Conversation DataGrid Event Handlers
        private void conversationDataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hit = conversationDataGrid.HitTest(e.X, e.Y);

                if (hit.RowIndex != -1)
                {
                    conversationDataGrid.Rows[hit.RowIndex].Selected = true;

                    conversationContextMenuStrip.Items["editConversationResponseItemsToolStripMenuItem"].Enabled = true;
                    conversationContextMenuStrip.Items["deleteConversationToolStripMenuItem"].Enabled = true;
                }
                else
                {
                    if (conversationDataGrid.SelectedRows.Count != 0)
                    {
                        conversationDataGrid.SelectedRows[0].Selected = false;
                    }

                    conversationContextMenuStrip.Items["editConversationResponseItemsToolStripMenuItem"].Enabled = false;
                    conversationContextMenuStrip.Items["deleteConversationToolStripMenuItem"].Enabled = false;
                }
            }
        }

        private void editConversationResponseItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conversationDataGrid.SelectedRows.Count != 0)
            {
                string botName = listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString();

                DataGridViewRow row = conversationDataGrid.SelectedRows[0];

                int conversationID = int.Parse(row.Cells[0].Value.ToString());
                string say = row.Cells[1].Value.ToString();

                LoadEditConversationWindow(botName, conversationID, say);
            }
        }

        private void LoadEditConversationWindow(string botName, int conversationID, string say)
        {
            ConversationalResponseItems cri = Conversational.Instance.GetBotCRI(botName, conversationID);

            EditConversationWindow editWindow = new EditConversationWindow(botName, conversationID, say, cri);
            editWindow.ShowDialog();
            editWindow.Dispose();

            LoadConversationalGrid();
        }

        private void deleteConversationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string botname = listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString();
            int id = int.Parse(conversationDataGrid.SelectedRows[0].Cells[0].Value.ToString());

            if (MessageBox.Show("Are you sure you want to delete this conversation from bot '" + botname + "'?\n\n" + conversationDataGrid.SelectedRows[0].Cells[1].Value.ToString(), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Conversational.Instance.DeleteConversation(botname, id);
                LoadConversationalGrid();
            }
        }

        private void conversationDataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = conversationDataGrid.HitTest(e.X, e.Y);

            if (hit.RowIndex != -1)
            {
                conversationDataGrid.Rows[hit.RowIndex].Selected = true;

                string botName = listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString();

                DataGridViewRow row = conversationDataGrid.Rows[hit.RowIndex];

                int conversationID = int.Parse(row.Cells[0].Value.ToString());
                string say = row.Cells[1].Value.ToString();

                LoadEditConversationWindow(botName, conversationID, say);
            }
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewConversationWindow conversationWindow = new AddNewConversationWindow(listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString());
            conversationWindow.ShowDialog();
            conversationWindow.Dispose();

            LoadConversationalGrid();
        }

        private void testBotsConversationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabPageTesting);

            string whichBot = listBoxOfBots.Items[listBoxOfBots.SelectedIndex].ToString();

            int index = whichBotTestComboBox.Items.IndexOf(whichBot);

            whichBotTestComboBox.SelectedIndex = index;

            LoadConversation(whichBot);
        }
        #endregion

        #region Testing Tab Methods
        private void tabPageTesting_Enter(object sender, EventArgs e)
        {
            tableLayoutPanel1.Visible = true;

            whichBotTestComboBox.Items.Clear();
            whichBotTestComboBox.SelectedIndex = -1;
            whichBotTestComboBox.Text = string.Empty;

            foreach (object obj in listBoxOfBots.Items)
            {
                string bot = (string)obj;

                whichBotTestComboBox.Items.Add(bot);
            }
        }

        private void whichBotTestComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            string selectedBot = (string)comboBox.SelectedItem;

            LoadConversation(selectedBot);
        }

        private void LoadConversation(string selectedBot)
        {
            ConversationalItem ci = Conversational.Instance.GetBotFirstConversationItem(selectedBot);

            if (ci != null)
            {
                ChangeConversation(ci);
            }
            else
            {
                tableLayoutPanel1.Visible = false;

                MessageBox.Show("This bot has no conversations to test, please add some...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeConversation(ConversationalItem ci)
        {
            string selectedBot = ci.BotName;

            conversationSayTextBox.Text = ci.Say;

            conversationResponseListBox.Items.Clear();

            foreach (ConversationalResponseItem responseItem in ci.ConversationalResponseItems.ResponseItems)
            {
                conversationResponseListBox.Items.Add(responseItem);
            }

            currentIDTextBox.Text = ci.ConversationID.ToString();

            tableLayoutPanel1.Visible = true;
        }

        private void conversationResponseListBox_MouseClick(object sender, MouseEventArgs e)
        {
            string botName = (string)whichBotTestComboBox.SelectedItem;

            int selectedItem = conversationResponseListBox.IndexFromPoint(e.X, e.Y);
            if (selectedItem == -1)
            {
            }
            else
            {
                ConversationalResponseItem cri = (ConversationalResponseItem)conversationResponseListBox.Items[selectedItem];

                if (cri.EngineCommand != "")
                {
                    MessageBox.Show("Parse engine command "+cri.EngineCommand);
                }

                if (cri.To != 0)
                {
                    ConversationalItem ci = Conversational.Instance.GetBotConversationByID(botName, cri.To);

                    if (ci != null)
                    {
                        ChangeConversation(ci);
                    }
                    else
                    {
                        tableLayoutPanel1.Visible = false;

                        MessageBox.Show("There is no conversation at ID: " + cri.To.ToString() + "\nPlease check your logic", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    tableLayoutPanel1.Visible = false;

                    whichBotTestComboBox.SelectedIndex = 0;
                    whichBotTestComboBox.Text = string.Empty;

                    MessageBox.Show("You have completed your conversation with " + botName, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tabControl.SelectTab(tabPageConfig);
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string bot = (string)whichBotTestComboBox.SelectedItem;

            ConversationalItem ci = null;

            try
            {
                ci = Conversational.Instance.GetBotConversationByID(bot, int.Parse(textBox1.Text));
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter only numbers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ci != null)
            {
                ChangeConversation(ci);
            }
            else
            {
                MessageBox.Show("There is no conversation at ID: " + textBox1.Text + "\nPlease check your logic", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void ConversationalConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isFileLoaded)
            {
                if (MessageBox.Show("Are you sure you want to quit Conversational Config?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

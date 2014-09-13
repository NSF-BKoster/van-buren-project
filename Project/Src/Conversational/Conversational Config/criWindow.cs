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
    public partial class criWindow : Form
    {
        private bool savedState;
        private ConversationalResponseItems criItems;
        private string botName;

        // Drag And Drop Reordering
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        public criWindow(string bot_name, ref ConversationalResponseItems cri)
        {
            criItems = cri;
            botName = bot_name;

            InitializeComponent();

            currentSaysComboBox.Enabled = false;

            if (cri.ResponseItems.Count != 0)
            {
                foreach (ConversationalResponseItem responseItem in cri.ResponseItems)
                {
                    criDataGrid.Rows.Add(responseItem.To, responseItem.Response, responseItem.AltResponse, responseItem.EngineCommand, responseItem.VisibilityOptions);
                }
            }

            Dictionary<int, string> currentConversations = Conversational.Instance.GetBotConversations(botName);

            if (currentConversations.Count != 0)
            {
                currentSaysComboBox.Enabled = true;

                foreach (int key in currentConversations.Keys)
                {
                    string conversation = currentConversations[key].ToString();

                    currentSaysComboBox.Items.Add(key.ToString() + " - " + conversation);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.criItems.ResponseItems.Clear();

            if (criDataGrid.Rows.Count != 0)
            {
                foreach (DataGridViewRow criItem in criDataGrid.Rows)
                {
                    if (criItem.IsNewRow)
                    {
                        continue;
                    }

                    string[] args = {"", "", ""};
                    for (int i=0; i < 3; i++)
                    {
                        if (criItem.Cells[i+2].Value != null)
                            args[i] = criItem.Cells[i+2].Value.ToString();
                    }

                    this.criItems.AddResponse(int.Parse(criItem.Cells[0].Value.ToString()), criItem.Cells[1].Value.ToString(), args[0], args[1], args[2]);
                }
            }

            savedState = true;

            this.Close();
        }

        private void criWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!savedState)
            {
                if (MessageBox.Show("Are you sure you want to close this window?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void currentSaysComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            string rawData = (string)comboBox.SelectedItem;

            string[] seperator = new string[1] { " - " };

            string[] splitdata = rawData.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            int addedRow = criDataGrid.Rows.Add(splitdata[0], "Response Directed To [" + splitdata[1] + "]");

            criDataGrid.CurrentCell = criDataGrid.Rows[addedRow].Cells[1];
            criDataGrid.BeginEdit(true);

            currentSaysComboBox.SelectedIndex = -1;
        }

        private void criDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = criDataGrid.DoDragDrop(criDataGrid.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
                }
            }
        }

        private void criDataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            rowIndexFromMouseDown = criDataGrid.HitTest(e.X, e.Y).RowIndex;

            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                                                dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void criDataGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void criDataGrid_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = criDataGrid.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop = criDataGrid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                criDataGrid.Rows.RemoveAt(rowIndexFromMouseDown);
                criDataGrid.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);

            }
        }
    }
}

namespace Conversational_Config
{
    partial class criWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.criDataGrid = new System.Windows.Forms.DataGridView();
            this.cri_to = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cri_response = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cri_altresponse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cri_command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cri_visoptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.okButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.currentSaysComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.criDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // criDataGrid
            // 
            this.criDataGrid.AllowDrop = true;
            this.criDataGrid.AllowUserToOrderColumns = true;
            this.criDataGrid.AllowUserToResizeRows = false;
            this.criDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.criDataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.criDataGrid.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.criDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.criDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cri_to,
            this.cri_response,
            this.cri_altresponse,
            this.cri_command,
            this.cri_visoptions});
            this.criDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.criDataGrid.Location = new System.Drawing.Point(0, 0);
            this.criDataGrid.Margin = new System.Windows.Forms.Padding(2);
            this.criDataGrid.MultiSelect = false;
            this.criDataGrid.Name = "criDataGrid";
            this.criDataGrid.RowTemplate.Height = 24;
            this.criDataGrid.Size = new System.Drawing.Size(875, 331);
            this.criDataGrid.TabIndex = 0;
            this.criDataGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.criDataGrid_DragDrop);
            this.criDataGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.criDataGrid_DragOver);
            this.criDataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.criDataGrid_MouseDown);
            this.criDataGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.criDataGrid_MouseMove);
            // 
            // cri_to
            // 
            this.cri_to.HeaderText = "To What Conversation ID";
            this.cri_to.Name = "cri_to";
            // 
            // cri_response
            // 
            this.cri_response.HeaderText = "Response";
            this.cri_response.Name = "cri_response";
            // 
            // cri_altresponse
            // 
            this.cri_altresponse.HeaderText = "Alt Response";
            this.cri_altresponse.Name = "cri_altresponse";
            // 
            // cri_command
            // 
            this.cri_command.HeaderText = "Engine Command";
            this.cri_command.Name = "cri_command";
            // 
            // cri_visoptions
            // 
            this.cri_visoptions.HeaderText = "Visibility Options";
            this.cri_visoptions.Name = "cri_visoptions";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(348, 338);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(58, 20);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(410, 338);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(56, 20);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // currentSaysComboBox
            // 
            this.currentSaysComboBox.FormattingEnabled = true;
            this.currentSaysComboBox.Location = new System.Drawing.Point(9, 338);
            this.currentSaysComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.currentSaysComboBox.Name = "currentSaysComboBox";
            this.currentSaysComboBox.Size = new System.Drawing.Size(336, 21);
            this.currentSaysComboBox.TabIndex = 3;
            this.currentSaysComboBox.SelectionChangeCommitted += new System.EventHandler(this.currentSaysComboBox_SelectionChangeCommitted);
            // 
            // criWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 381);
            this.Controls.Add(this.criDataGrid);
            this.Controls.Add(this.currentSaysComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "criWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conversation Response Items";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.criWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.criDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView criDataGrid;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox currentSaysComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn cri_to;
        private System.Windows.Forms.DataGridViewTextBoxColumn cri_response;
        private System.Windows.Forms.DataGridViewTextBoxColumn cri_altresponse;
        private System.Windows.Forms.DataGridViewTextBoxColumn cri_command;
        private System.Windows.Forms.DataGridViewTextBoxColumn cri_visoptions;
    }
}
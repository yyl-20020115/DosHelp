namespace HelpBrowser
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lstTopics = new System.Windows.Forms.ListBox();
            webBrowserReading = new System.Windows.Forms.WebBrowser();
            txtSource = new System.Windows.Forms.TextBox();
            splitContainerMain = new System.Windows.Forms.SplitContainer();
            tabControlIndices = new System.Windows.Forms.TabControl();
            tabTopics = new System.Windows.Forms.TabPage();
            tabContexts = new System.Windows.Forms.TabPage();
            lstContexts = new System.Windows.Forms.ListBox();
            tabErrors = new System.Windows.Forms.TabPage();
            lstErrors = new System.Windows.Forms.ListBox();
            panel1 = new System.Windows.Forms.Panel();
            cbDatabases = new System.Windows.Forms.ComboBox();
            btnAddArchive = new System.Windows.Forms.Button();
            btnRemoveArchive = new System.Windows.Forms.Button();
            tabControlReading = new System.Windows.Forms.TabControl();
            tabHtml = new System.Windows.Forms.TabPage();
            txtTopicTitle = new System.Windows.Forms.TextBox();
            tabText = new System.Windows.Forms.TabPage();
            txtNoFormat = new System.Windows.Forms.TextBox();
            tabSource = new System.Windows.Forms.TabPage();
            menuStripMain = new System.Windows.Forms.MenuStrip();
            mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mnuViewUnresolvedLinks = new System.Windows.Forms.ToolStripMenuItem();
            mnuViewErrors = new System.Windows.Forms.ToolStripMenuItem();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            tabControlIndices.SuspendLayout();
            tabTopics.SuspendLayout();
            tabContexts.SuspendLayout();
            tabErrors.SuspendLayout();
            panel1.SuspendLayout();
            tabControlReading.SuspendLayout();
            tabHtml.SuspendLayout();
            tabText.SuspendLayout();
            tabSource.SuspendLayout();
            menuStripMain.SuspendLayout();
            SuspendLayout();
            // 
            // lstTopics
            // 
            lstTopics.Dock = System.Windows.Forms.DockStyle.Fill;
            lstTopics.FormattingEnabled = true;
            lstTopics.HorizontalScrollbar = true;
            lstTopics.ItemHeight = 15;
            lstTopics.Location = new System.Drawing.Point(3, 3);
            lstTopics.Margin = new System.Windows.Forms.Padding(4);
            lstTopics.Name = "lstTopics";
            lstTopics.Size = new System.Drawing.Size(227, 292);
            lstTopics.TabIndex = 0;
            lstTopics.SelectedIndexChanged += lstTopics_SelectedIndexChanged;
            // 
            // webBrowserReading
            // 
            webBrowserReading.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowserReading.Location = new System.Drawing.Point(0, 23);
            webBrowserReading.MinimumSize = new System.Drawing.Size(20, 20);
            webBrowserReading.Name = "webBrowserReading";
            webBrowserReading.Size = new System.Drawing.Size(481, 298);
            webBrowserReading.TabIndex = 1;
            webBrowserReading.Navigating += webBrowser1_Navigating;
            // 
            // txtSource
            // 
            txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSource.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtSource.Location = new System.Drawing.Point(3, 3);
            txtSource.Multiline = true;
            txtSource.Name = "txtSource";
            txtSource.ReadOnly = true;
            txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtSource.Size = new System.Drawing.Size(475, 313);
            txtSource.TabIndex = 2;
            txtSource.WordWrap = false;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerMain.Location = new System.Drawing.Point(0, 25);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(tabControlIndices);
            splitContainerMain.Panel1.Controls.Add(panel1);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(tabControlReading);
            splitContainerMain.Size = new System.Drawing.Size(734, 349);
            splitContainerMain.SplitterDistance = 241;
            splitContainerMain.TabIndex = 4;
            // 
            // tabControlIndices
            // 
            tabControlIndices.Controls.Add(tabTopics);
            tabControlIndices.Controls.Add(tabContexts);
            tabControlIndices.Controls.Add(tabErrors);
            tabControlIndices.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlIndices.Location = new System.Drawing.Point(0, 23);
            tabControlIndices.Multiline = true;
            tabControlIndices.Name = "tabControlIndices";
            tabControlIndices.SelectedIndex = 0;
            tabControlIndices.Size = new System.Drawing.Size(241, 326);
            tabControlIndices.TabIndex = 1;
            // 
            // tabTopics
            // 
            tabTopics.Controls.Add(lstTopics);
            tabTopics.Location = new System.Drawing.Point(4, 24);
            tabTopics.Name = "tabTopics";
            tabTopics.Padding = new System.Windows.Forms.Padding(3);
            tabTopics.Size = new System.Drawing.Size(233, 298);
            tabTopics.TabIndex = 0;
            tabTopics.Text = "Topics";
            tabTopics.UseVisualStyleBackColor = true;
            // 
            // tabContexts
            // 
            tabContexts.Controls.Add(lstContexts);
            tabContexts.Location = new System.Drawing.Point(4, 26);
            tabContexts.Name = "tabContexts";
            tabContexts.Padding = new System.Windows.Forms.Padding(3);
            tabContexts.Size = new System.Drawing.Size(233, 296);
            tabContexts.TabIndex = 1;
            tabContexts.Text = "Contexts";
            tabContexts.UseVisualStyleBackColor = true;
            // 
            // lstContexts
            // 
            lstContexts.Dock = System.Windows.Forms.DockStyle.Fill;
            lstContexts.FormattingEnabled = true;
            lstContexts.ItemHeight = 15;
            lstContexts.Location = new System.Drawing.Point(3, 3);
            lstContexts.Name = "lstContexts";
            lstContexts.Size = new System.Drawing.Size(227, 290);
            lstContexts.TabIndex = 0;
            lstContexts.SelectedIndexChanged += lstContexts_SelectedIndexChanged;
            // 
            // tabErrors
            // 
            tabErrors.Controls.Add(lstErrors);
            tabErrors.Location = new System.Drawing.Point(4, 26);
            tabErrors.Name = "tabErrors";
            tabErrors.Size = new System.Drawing.Size(233, 296);
            tabErrors.TabIndex = 2;
            tabErrors.Text = "Errors";
            tabErrors.UseVisualStyleBackColor = true;
            // 
            // lstErrors
            // 
            lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            lstErrors.FormattingEnabled = true;
            lstErrors.ItemHeight = 15;
            lstErrors.Location = new System.Drawing.Point(0, 0);
            lstErrors.Name = "lstErrors";
            lstErrors.Size = new System.Drawing.Size(233, 296);
            lstErrors.TabIndex = 0;
            lstErrors.SelectedIndexChanged += lstErrors_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(cbDatabases);
            panel1.Controls.Add(btnAddArchive);
            panel1.Controls.Add(btnRemoveArchive);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(241, 23);
            panel1.TabIndex = 2;
            // 
            // cbDatabases
            // 
            cbDatabases.Dock = System.Windows.Forms.DockStyle.Top;
            cbDatabases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbDatabases.FormattingEnabled = true;
            cbDatabases.Location = new System.Drawing.Point(0, 0);
            cbDatabases.Name = "cbDatabases";
            cbDatabases.Size = new System.Drawing.Size(187, 23);
            cbDatabases.TabIndex = 1;
            cbDatabases.SelectedIndexChanged += cbDatabases_SelectedIndexChanged;
            // 
            // btnAddArchive
            // 
            btnAddArchive.Dock = System.Windows.Forms.DockStyle.Right;
            btnAddArchive.Location = new System.Drawing.Point(187, 0);
            btnAddArchive.Name = "btnAddArchive";
            btnAddArchive.Size = new System.Drawing.Size(27, 23);
            btnAddArchive.TabIndex = 3;
            btnAddArchive.Text = "+";
            btnAddArchive.UseVisualStyleBackColor = true;
            btnAddArchive.Click += btnAddArchive_Click;
            // 
            // btnRemoveArchive
            // 
            btnRemoveArchive.Dock = System.Windows.Forms.DockStyle.Right;
            btnRemoveArchive.Location = new System.Drawing.Point(214, 0);
            btnRemoveArchive.Name = "btnRemoveArchive";
            btnRemoveArchive.Size = new System.Drawing.Size(27, 23);
            btnRemoveArchive.TabIndex = 2;
            btnRemoveArchive.Text = "-";
            btnRemoveArchive.UseVisualStyleBackColor = true;
            btnRemoveArchive.Click += btnRemoveArchive_Click;
            // 
            // tabControlReading
            // 
            tabControlReading.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            tabControlReading.Controls.Add(tabHtml);
            tabControlReading.Controls.Add(tabText);
            tabControlReading.Controls.Add(tabSource);
            tabControlReading.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlReading.Location = new System.Drawing.Point(0, 0);
            tabControlReading.Name = "tabControlReading";
            tabControlReading.SelectedIndex = 0;
            tabControlReading.Size = new System.Drawing.Size(489, 349);
            tabControlReading.TabIndex = 2;
            // 
            // tabHtml
            // 
            tabHtml.Controls.Add(webBrowserReading);
            tabHtml.Controls.Add(txtTopicTitle);
            tabHtml.Location = new System.Drawing.Point(4, 4);
            tabHtml.Name = "tabHtml";
            tabHtml.Size = new System.Drawing.Size(481, 321);
            tabHtml.TabIndex = 0;
            tabHtml.Text = "HTML";
            tabHtml.UseVisualStyleBackColor = true;
            // 
            // txtTopicTitle
            // 
            txtTopicTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtTopicTitle.Dock = System.Windows.Forms.DockStyle.Top;
            txtTopicTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            txtTopicTitle.Location = new System.Drawing.Point(0, 0);
            txtTopicTitle.Margin = new System.Windows.Forms.Padding(0);
            txtTopicTitle.Name = "txtTopicTitle";
            txtTopicTitle.ReadOnly = true;
            txtTopicTitle.Size = new System.Drawing.Size(481, 23);
            txtTopicTitle.TabIndex = 2;
            txtTopicTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabText
            // 
            tabText.Controls.Add(txtNoFormat);
            tabText.Location = new System.Drawing.Point(4, 4);
            tabText.Name = "tabText";
            tabText.Size = new System.Drawing.Size(481, 319);
            tabText.TabIndex = 2;
            tabText.Text = "Text";
            tabText.UseVisualStyleBackColor = true;
            // 
            // txtNoFormat
            // 
            txtNoFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            txtNoFormat.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtNoFormat.Location = new System.Drawing.Point(0, 0);
            txtNoFormat.Multiline = true;
            txtNoFormat.Name = "txtNoFormat";
            txtNoFormat.ReadOnly = true;
            txtNoFormat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtNoFormat.Size = new System.Drawing.Size(481, 319);
            txtNoFormat.TabIndex = 0;
            txtNoFormat.WordWrap = false;
            // 
            // tabSource
            // 
            tabSource.Controls.Add(txtSource);
            tabSource.Location = new System.Drawing.Point(4, 4);
            tabSource.Name = "tabSource";
            tabSource.Padding = new System.Windows.Forms.Padding(3);
            tabSource.Size = new System.Drawing.Size(481, 319);
            tabSource.TabIndex = 1;
            tabSource.Text = "Source";
            tabSource.UseVisualStyleBackColor = true;
            // 
            // menuStripMain
            // 
            menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuFile, viewToolStripMenuItem });
            menuStripMain.Location = new System.Drawing.Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Size = new System.Drawing.Size(734, 25);
            menuStripMain.TabIndex = 5;
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuFileOpen, toolStripMenuItem1, mnuFileExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new System.Drawing.Size(39, 21);
            mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            mnuFileOpen.Name = "mnuFileOpen";
            mnuFileOpen.Size = new System.Drawing.Size(180, 22);
            mnuFileOpen.Text = "&Open...";
            mnuFileOpen.Click += mnuFileOpen_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // mnuFileExit
            // 
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.Size = new System.Drawing.Size(180, 22);
            mnuFileExit.Text = "E&xit";
            mnuFileExit.Click += mnuFileExit_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuViewUnresolvedLinks, mnuViewErrors });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            viewToolStripMenuItem.Text = "&View";
            // 
            // mnuViewUnresolvedLinks
            // 
            mnuViewUnresolvedLinks.Name = "mnuViewUnresolvedLinks";
            mnuViewUnresolvedLinks.Size = new System.Drawing.Size(184, 22);
            mnuViewUnresolvedLinks.Text = "&Unresolved Links...";
            mnuViewUnresolvedLinks.Click += mnuViewUnresolvedLinks_Click;
            // 
            // mnuViewErrors
            // 
            mnuViewErrors.Name = "mnuViewErrors";
            mnuViewErrors.Size = new System.Drawing.Size(184, 22);
            mnuViewErrors.Text = "&Errors...";
            mnuViewErrors.Click += mnuViewErrors_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "DOS help files|*.hlp|QuickHelp markup source|*.txt";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Open DOS Help Files";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(734, 374);
            Controls.Add(splitContainerMain);
            Controls.Add(menuStripMain);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripMain;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "DOS Help Viewer";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel1.PerformLayout();
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            tabControlIndices.ResumeLayout(false);
            tabTopics.ResumeLayout(false);
            tabContexts.ResumeLayout(false);
            tabErrors.ResumeLayout(false);
            panel1.ResumeLayout(false);
            tabControlReading.ResumeLayout(false);
            tabHtml.ResumeLayout(false);
            tabHtml.PerformLayout();
            tabText.ResumeLayout(false);
            tabText.PerformLayout();
            tabSource.ResumeLayout(false);
            tabSource.PerformLayout();
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox lstTopics;
        private System.Windows.Forms.WebBrowser webBrowserReading;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TabControl tabControlReading;
        private System.Windows.Forms.TabPage tabHtml;
        private System.Windows.Forms.TabPage tabSource;
        private System.Windows.Forms.TextBox txtNoFormat;
        private System.Windows.Forms.TabControl tabControlIndices;
        private System.Windows.Forms.TabPage tabTopics;
        private System.Windows.Forms.TabPage tabContexts;
        private System.Windows.Forms.ListBox lstContexts;
        private System.Windows.Forms.TabPage tabText;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtTopicTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuViewUnresolvedLinks;
        private System.Windows.Forms.ToolStripMenuItem mnuViewErrors;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbDatabases;
        private System.Windows.Forms.Button btnAddArchive;
        private System.Windows.Forms.Button btnRemoveArchive;
        private System.Windows.Forms.ListBox lstErrors;
    }
}


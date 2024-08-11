﻿namespace HelpBrowser;

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
        splitContainerMain = new System.Windows.Forms.SplitContainer();
        tabControlIndices = new System.Windows.Forms.TabControl();
        tabTopics = new System.Windows.Forms.TabPage();
        lstTopics = new System.Windows.Forms.ListBox();
        tabContexts = new System.Windows.Forms.TabPage();
        lstContexts = new System.Windows.Forms.ListBox();
        tabErrors = new System.Windows.Forms.TabPage();
        lstErrors = new System.Windows.Forms.ListBox();
        panelMain = new System.Windows.Forms.Panel();
        cbDatabases = new System.Windows.Forms.ComboBox();
        btnAddArchive = new System.Windows.Forms.Button();
        btnRemoveArchive = new System.Windows.Forms.Button();
        tabControlReading = new System.Windows.Forms.TabControl();
        tabHtml = new System.Windows.Forms.TabPage();
        webBrowserReading = new System.Windows.Forms.WebBrowser();
        txtTopicTitle = new System.Windows.Forms.TextBox();
        tabText = new System.Windows.Forms.TabPage();
        txtNoFormat = new System.Windows.Forms.TextBox();
        tabSource = new System.Windows.Forms.TabPage();
        txtSource = new System.Windows.Forms.TextBox();
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
        panelMain.SuspendLayout();
        tabControlReading.SuspendLayout();
        tabHtml.SuspendLayout();
        tabText.SuspendLayout();
        tabSource.SuspendLayout();
        menuStripMain.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainerMain
        // 
        resources.ApplyResources(splitContainerMain, "splitContainerMain");
        splitContainerMain.Name = "splitContainerMain";
        // 
        // splitContainerMain.Panel1
        // 
        splitContainerMain.Panel1.Controls.Add(tabControlIndices);
        splitContainerMain.Panel1.Controls.Add(panelMain);
        // 
        // splitContainerMain.Panel2
        // 
        splitContainerMain.Panel2.Controls.Add(tabControlReading);
        // 
        // tabControlIndices
        // 
        tabControlIndices.Controls.Add(tabTopics);
        tabControlIndices.Controls.Add(tabContexts);
        tabControlIndices.Controls.Add(tabErrors);
        resources.ApplyResources(tabControlIndices, "tabControlIndices");
        tabControlIndices.Multiline = true;
        tabControlIndices.Name = "tabControlIndices";
        tabControlIndices.SelectedIndex = 0;
        // 
        // tabTopics
        // 
        tabTopics.Controls.Add(lstTopics);
        resources.ApplyResources(tabTopics, "tabTopics");
        tabTopics.Name = "tabTopics";
        tabTopics.UseVisualStyleBackColor = true;
        // 
        // lstTopics
        // 
        resources.ApplyResources(lstTopics, "lstTopics");
        lstTopics.FormattingEnabled = true;
        lstTopics.Name = "lstTopics";
        lstTopics.SelectedIndexChanged += lstTopics_SelectedIndexChanged;
        // 
        // tabContexts
        // 
        tabContexts.Controls.Add(lstContexts);
        resources.ApplyResources(tabContexts, "tabContexts");
        tabContexts.Name = "tabContexts";
        tabContexts.UseVisualStyleBackColor = true;
        // 
        // lstContexts
        // 
        resources.ApplyResources(lstContexts, "lstContexts");
        lstContexts.FormattingEnabled = true;
        lstContexts.Name = "lstContexts";
        lstContexts.SelectedIndexChanged += lstContexts_SelectedIndexChanged;
        // 
        // tabErrors
        // 
        tabErrors.Controls.Add(lstErrors);
        resources.ApplyResources(tabErrors, "tabErrors");
        tabErrors.Name = "tabErrors";
        tabErrors.UseVisualStyleBackColor = true;
        // 
        // lstErrors
        // 
        resources.ApplyResources(lstErrors, "lstErrors");
        lstErrors.FormattingEnabled = true;
        lstErrors.Name = "lstErrors";
        lstErrors.SelectedIndexChanged += lstErrors_SelectedIndexChanged;
        // 
        // panel1
        // 
        resources.ApplyResources(panelMain, "panel1");
        panelMain.Controls.Add(cbDatabases);
        panelMain.Controls.Add(btnAddArchive);
        panelMain.Controls.Add(btnRemoveArchive);
        panelMain.Name = "panel1";
        // 
        // cbDatabases
        // 
        resources.ApplyResources(cbDatabases, "cbDatabases");
        cbDatabases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cbDatabases.FormattingEnabled = true;
        cbDatabases.Name = "cbDatabases";
        cbDatabases.SelectedIndexChanged += cbDatabases_SelectedIndexChanged;
        // 
        // btnAddArchive
        // 
        resources.ApplyResources(btnAddArchive, "btnAddArchive");
        btnAddArchive.Name = "btnAddArchive";
        btnAddArchive.UseVisualStyleBackColor = true;
        btnAddArchive.Click += btnAddArchive_Click;
        // 
        // btnRemoveArchive
        // 
        resources.ApplyResources(btnRemoveArchive, "btnRemoveArchive");
        btnRemoveArchive.Name = "btnRemoveArchive";
        btnRemoveArchive.UseVisualStyleBackColor = true;
        btnRemoveArchive.Click += btnRemoveArchive_Click;
        // 
        // tabControlReading
        // 
        resources.ApplyResources(tabControlReading, "tabControlReading");
        tabControlReading.Controls.Add(tabHtml);
        tabControlReading.Controls.Add(tabText);
        tabControlReading.Controls.Add(tabSource);
        tabControlReading.Name = "tabControlReading";
        tabControlReading.SelectedIndex = 0;
        // 
        // tabHtml
        // 
        tabHtml.Controls.Add(webBrowserReading);
        tabHtml.Controls.Add(txtTopicTitle);
        resources.ApplyResources(tabHtml, "tabHtml");
        tabHtml.Name = "tabHtml";
        tabHtml.UseVisualStyleBackColor = true;
        // 
        // webBrowserReading
        // 
        resources.ApplyResources(webBrowserReading, "webBrowserReading");
        webBrowserReading.Name = "webBrowserReading";
        webBrowserReading.Navigating += webBrowser1_Navigating;
        // 
        // txtTopicTitle
        // 
        txtTopicTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        resources.ApplyResources(txtTopicTitle, "txtTopicTitle");
        txtTopicTitle.Name = "txtTopicTitle";
        txtTopicTitle.ReadOnly = true;
        // 
        // tabText
        // 
        tabText.Controls.Add(txtNoFormat);
        resources.ApplyResources(tabText, "tabText");
        tabText.Name = "tabText";
        tabText.UseVisualStyleBackColor = true;
        // 
        // txtNoFormat
        // 
        resources.ApplyResources(txtNoFormat, "txtNoFormat");
        txtNoFormat.Name = "txtNoFormat";
        txtNoFormat.ReadOnly = true;
        // 
        // tabSource
        // 
        tabSource.Controls.Add(txtSource);
        resources.ApplyResources(tabSource, "tabSource");
        tabSource.Name = "tabSource";
        tabSource.UseVisualStyleBackColor = true;
        // 
        // txtSource
        // 
        resources.ApplyResources(txtSource, "txtSource");
        txtSource.Name = "txtSource";
        txtSource.ReadOnly = true;
        // 
        // menuStripMain
        // 
        menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuFile, viewToolStripMenuItem });
        resources.ApplyResources(menuStripMain, "menuStripMain");
        menuStripMain.Name = "menuStripMain";
        // 
        // mnuFile
        // 
        mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuFileOpen, toolStripMenuItem1, mnuFileExit });
        mnuFile.Name = "mnuFile";
        resources.ApplyResources(mnuFile, "mnuFile");
        // 
        // mnuFileOpen
        // 
        mnuFileOpen.Name = "mnuFileOpen";
        resources.ApplyResources(mnuFileOpen, "mnuFileOpen");
        mnuFileOpen.Click += mnuFileOpen_Click;
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");
        // 
        // mnuFileExit
        // 
        mnuFileExit.Name = "mnuFileExit";
        resources.ApplyResources(mnuFileExit, "mnuFileExit");
        mnuFileExit.Click += mnuFileExit_Click;
        // 
        // viewToolStripMenuItem
        // 
        viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuViewUnresolvedLinks, mnuViewErrors });
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        resources.ApplyResources(viewToolStripMenuItem, "viewToolStripMenuItem");
        // 
        // mnuViewUnresolvedLinks
        // 
        mnuViewUnresolvedLinks.Name = "mnuViewUnresolvedLinks";
        resources.ApplyResources(mnuViewUnresolvedLinks, "mnuViewUnresolvedLinks");
        mnuViewUnresolvedLinks.Click += mnuViewUnresolvedLinks_Click;
        // 
        // mnuViewErrors
        // 
        mnuViewErrors.Name = "mnuViewErrors";
        resources.ApplyResources(mnuViewErrors, "mnuViewErrors");
        mnuViewErrors.Click += mnuViewErrors_Click;
        // 
        // openFileDialog1
        // 
        resources.ApplyResources(openFileDialog1, "openFileDialog1");
        openFileDialog1.Multiselect = true;
        // 
        // contextMenuStrip1
        // 
        contextMenuStrip1.Name = "contextMenuStrip1";
        resources.ApplyResources(contextMenuStrip1, "contextMenuStrip1");
        // 
        // MainForm
        // 
        resources.ApplyResources(this, "$this");
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        Controls.Add(splitContainerMain);
        Controls.Add(menuStripMain);
        MainMenuStrip = menuStripMain;
        Name = "MainForm";
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
        panelMain.ResumeLayout(false);
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
    private System.Windows.Forms.Panel panelMain;
    private System.Windows.Forms.ComboBox cbDatabases;
    private System.Windows.Forms.Button btnAddArchive;
    private System.Windows.Forms.Button btnRemoveArchive;
    private System.Windows.Forms.ListBox lstErrors;
}


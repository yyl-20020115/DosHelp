﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuickHelp;
using QuickHelp.Formatters;
using QuickHelp.Serialization;

namespace HelpBrowser;

public partial class MainForm : Form
{
    private readonly HelpViewModel viewModel = new();

    public MainForm()
    {
        InitializeComponent();

        viewModel.DatabaseAdded += OnDatabasesChanged;
        viewModel.DatabaseRemoved += OnDatabasesChanged;
        viewModel.ActiveDatabaseChanged += OnActiveDatabaseChanged;
        viewModel.ActiveTopicChanged += OnActiveTopicChanged;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        viewModel.LoadSettings();
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        viewModel.SaveSettings();
        Properties.Settings.Default.Save();
    }

    private void OnDatabasesChanged(object sender, EventArgs e)
    {
        cbDatabases.Items.Clear();
        foreach (var database in viewModel.Databases)
        {
            cbDatabases.Items.Add(new HelpDatabaseViewItem(database));
            if (database == viewModel.ActiveDatabase)
                cbDatabases.SelectedIndex = cbDatabases.Items.Count - 1;
        }
    }

    private void OnActiveDatabaseChanged(object sender, EventArgs e)
    {
        for (int i = 0; i < cbDatabases.Items.Count; i++)
        {
            var item = cbDatabases.Items[i] as HelpDatabaseViewItem;
            if (item.Database == viewModel.ActiveDatabase)
            {
                cbDatabases.SelectedIndex = i;
                break;
            }
        }

        lstTopics.Items.Clear();
        lstContexts.Items.Clear();
        lstErrors.Items.Clear();

        if (viewModel.ActiveDatabase == null)
        {
            tabTopics.Text = "Topics";
            tabContexts.Text = "Contexts";
            tabErrors.Text = "Errors";
            return;
        }

        lstTopics.Visible = false;
        int topicIndex = 0;
        foreach (var topic in viewModel.ActiveDatabase.Topics)
        {
            var item = new HelpTopicViewItem(topicIndex, topic);
            lstTopics.Items.Add(item);
            topicIndex++;
        }
        lstTopics.Visible = true;
        tabTopics.Text = string.Format("Topics ({0})", lstTopics.Items.Count);

        foreach (var contextString in viewModel.ActiveDatabase.ContextStrings)
        {
            lstContexts.Items.Add(contextString);
        }
        tabContexts.Text = string.Format("Contexts ({0})", lstContexts.Items.Count);

        foreach (var error in viewModel.DeserializationErrors)
        {
            if (error.Topic.Database == viewModel.ActiveDatabase)
            {
                lstErrors.Items.Add(new HelpTopicErrorViewItem(error));
            }
        }
        tabErrors.Text = string.Format("Errors ({0})", lstErrors.Items.Count);
    }

    private void OnActiveTopicChanged(object sender, EventArgs e)
    {
        var topic = viewModel.ActiveTopic;
        if (topic == null)
            return;

        // ---- lstTopics ----
        foreach (HelpTopicViewItem item in lstTopics.Items)
        {
            if (item.Topic == topic)
            {
                lstTopics.SelectedItem = item;
                break;
            }
        }

        // ---- Right-hand side panel ----
        var formatter = new EmbeddedHtmlFormatter
        {
            FixLinks = true
        };
        var html = formatter.FormatTopic(topic);
        var text = TextFormatter.FormatTopic(topic);
        txtNoFormat.Text = text;
        webBrowser1.DocumentText = html;
        txtTopicTitle.Text = HelpTopicViewItem.GetTopicDisplayTitle(topic);
        if (topic.Source is string)
            txtSource.Text = (string)topic.Source;
        else if (topic.Source is byte[])
            txtSource.Text = FormatHexData((byte[])topic.Source);
        else
            txtSource.Text = "";

        // Special handling for commands.
        if (topic.IsHidden && !string.IsNullOrEmpty(topic.ExecuteCommand))
        {
            if (topic.ExecuteCommand[0] == 'P')
            {
                // TODO: parse mark
                var redirectTarget = topic.ExecuteCommand.Substring(2);
                if (MessageBox.Show(this, "Redirect to " + redirectTarget + "?",
                    "Redirect", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    viewModel.NavigateTo(new HelpUri(redirectTarget));
                }
            }
        }
    }

    private void lstTopics_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTopics.SelectedItem is not HelpTopicViewItem item)
            return;

        var topic = item.Topic;
        viewModel.ActiveTopic = topic;
    }

    private static string FormatHexData(byte[] data)
    {
        var sbText = new StringBuilder(8);
        var sb = new StringBuilder();
        sb.Append("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");

        for (int i = 0; i < data.Length; i++)
        {
            if (i % 16 == 0)
            {
                sb.Append("    ");
                sb.Append(sbText.ToString());
                sbText.Remove(0, sbText.Length);

                sb.AppendLine();
                sb.Append(i.ToString("X4"));
                sb.Append("  ");
            }
            else if (i % 8 == 0)
            {
                sb.Append(" ");
            }
            sb.Append(' ');
            sb.Append(data[i].ToString("X2"));

            if (data[i] >= 32 && data[i] < 127)
                sbText.Append((char)data[i]);
            else
                sbText.Append('.');
        }
        if (data.Length % 16 != 0)
        {
            for (int i = 0; i < 16 - data.Length % 16; i++)
                sb.Append("   ");
            if (data.Length % 16 <= 8)
                sb.Append(' ');
        }
        sb.Append("    ");
        sb.Append(sbText.ToString());
        return sb.ToString();
    }

    private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        string url = e.Url.OriginalString;
        if (!url.StartsWith("about:blank?"))
            return;

        string target = url.Substring(12);
        HelpUri link = new HelpUri(target);
        if (!viewModel.NavigateTo(link))
        {
            MessageBox.Show("Cannot resolve link: " + target);
            e.Cancel = true;
        }
    }

    private void lstContexts_SelectedIndexChanged(object sender, EventArgs e)
    {
        string contextString = lstContexts.SelectedItem as string;
        if (contextString == null)
            return;

        viewModel.NavigateTo(new HelpUri(contextString));
    }

    private void mnuFileExit_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void mnuFileOpen_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() != DialogResult.OK)
            return;

        string[] fileNames = openFileDialog1.FileNames;
        foreach (string fileName in fileNames)
        {
            viewModel.LoadDatabases(fileName);
        }
    }

    private void cbDatabases_SelectedIndexChanged(object sender, EventArgs e)
    {
        HelpDatabaseViewItem item = cbDatabases.SelectedItem as HelpDatabaseViewItem;
        if (item == null)
            viewModel.ActiveDatabase = null;
        else
            viewModel.ActiveDatabase = item.Database;
    }

    private void btnAddArchive_Click(object sender, EventArgs e)
    {
        mnuFileOpen_Click(sender, e);
    }

    private void btnRemoveArchive_Click(object sender, EventArgs e)
    {
        int index = cbDatabases.SelectedIndex;
        if (index < 0)
            return;

        HelpDatabaseViewItem item = cbDatabases.Items[index] as HelpDatabaseViewItem;
        viewModel.RemoveDatabase(item.Database);

        if (index < cbDatabases.Items.Count)
        {
            item = cbDatabases.Items[index] as HelpDatabaseViewItem;
            viewModel.ActiveDatabase = item.Database;
        }
    }

    private void mnuViewUnresolvedLinks_Click(object sender, EventArgs e)
    {
        viewModel.DumpHierarchy();
    }

    private void mnuViewErrors_Click(object sender, EventArgs e)
    {
        MessageBox.Show("There are " + viewModel.TopicsWithError.Count + " errors.");
        foreach (HelpTopic topic in viewModel.TopicsWithError)
        {
            if (MessageBox.Show(string.Format(
                "{0}: {1}: {2}", topic.Database.Name, topic.TopicIndex, topic.Title),
                "Error", MessageBoxButtons.OKCancel) != DialogResult.OK)
                break;
        }
    }

    private void RecoverTopicError(HelpTopic topic)
    {

    }

    private void lstErrors_SelectedIndexChanged(object sender, EventArgs e)
    {
        HelpTopicErrorViewItem item = lstErrors.SelectedItem as HelpTopicErrorViewItem;
        if (item == null)
            return;

        HelpTopic topic = item.Topic;
        viewModel.ActiveTopic = topic;
    }
}

class HelpViewModel
{
    readonly HelpSystem system = new HelpSystem();
    readonly Dictionary<HelpDatabase, string> databaseFileNames =
        new Dictionary<HelpDatabase, string>();
    readonly List<HelpTopic> history = new List<HelpTopic>();

    private HelpDatabase activeDatabase;
    private HelpTopic activeTopic;

    public void LoadSettings()
    {
        var fileNames = Properties.Settings.Default.OpenFileNames;
        if (fileNames != null)
        {
            foreach (string fileName in fileNames)
            {
                // TODO: handle exceptions
                LoadDatabases(fileName);
            }
        }
    }

    public void SaveSettings()
    {
        var fileNames = new SortedDictionary<string, string>(
            StringComparer.InvariantCultureIgnoreCase);

        foreach (string fileName in databaseFileNames.Values)
        {
            fileNames[fileName] = fileName;
        }

        var savedNames = new System.Collections.Specialized.StringCollection();
        foreach (string fileName in fileNames.Keys)
        {
            savedNames.Add(fileName);
        }
        Properties.Settings.Default.OpenFileNames = savedNames;
    }

    public IEnumerable<HelpDatabase> Databases
    {
        get { return system.Databases; }
    }

    public void AddDatabase(HelpDatabase database, string fileName)
    {
        if (database == null)
            throw new ArgumentNullException(nameof(database));

        system.Databases.Add(database);
        databaseFileNames[database] = Path.GetFullPath(fileName).ToLowerInvariant();
        if (DatabaseAdded != null)
            DatabaseAdded(this, null);
        if (this.ActiveDatabase == null)
            this.ActiveDatabase = database;
    }

    public void RemoveDatabase(HelpDatabase database)
    {
        if (database == null)
            throw new ArgumentNullException(nameof(database));

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].Database == database)
                history.RemoveAt(i);
        }

        if (database == this.ActiveDatabase)
            this.ActiveDatabase = null;

        databaseFileNames.Remove(database);
        system.Databases.Remove(database);
        if (DatabaseRemoved != null)
            DatabaseRemoved(this, null);
    }

    public void LoadDatabases(string fileName)
    {
        if (fileName == null)
            throw new ArgumentNullException(nameof(fileName));

        var decoder = new QuickHelp.Serialization.BinaryHelpDeserializer();
        decoder.InvalidTopicData += OnInvalidTopicData;

        using (FileStream stream = File.OpenRead(fileName))
        {
            while (stream.Position < stream.Length)
            {
                long startPosition = stream.Position;
                HelpDatabase database;
                try
                {
                    database = decoder.DeserializeDatabase(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(
                        "Cannot decode database file {0} @ {1}: {2}",
                        fileName, startPosition, ex.Message));
                    break;
                }
                if (system.FindDatabase(database.Name) == null)
                {
                    AddDatabase(database, fileName);
                }
            }
        }
    }

    private List<HelpTopic> topicsWithError = new List<HelpTopic>();

    public List<HelpTopic> TopicsWithError
    {
        get { return topicsWithError; }
    }

    private List<InvalidTopicDataEventArgs> deserializationErrors =
        new List<InvalidTopicDataEventArgs>();

    public List<InvalidTopicDataEventArgs> DeserializationErrors
    {
        get { return this.deserializationErrors; }
    }

    private void OnInvalidTopicData(object sender, QuickHelp.Serialization.InvalidTopicDataEventArgs e)
    {
        this.deserializationErrors.Add(e);
    }

    public event EventHandler DatabaseAdded;

    public event EventHandler DatabaseRemoved;

    public HelpDatabase ActiveDatabase
    {
        get { return activeDatabase; }
        set
        {
            if (this.activeDatabase == value)
                return;

            this.activeDatabase = value;
            if (ActiveDatabaseChanged != null)
                ActiveDatabaseChanged(this, null);

            if (this.activeTopic == null ||
                this.activeTopic.Database != activeDatabase)
                this.ActiveTopic = GetDefaultTopicOfDatabase(activeDatabase);
        }
    }

    public HelpTopic ActiveTopic
    {
        get { return activeTopic; }
        set
        {
            if (this.activeTopic == value)
                return;

            this.activeTopic = value;
            if (activeTopic != null)
            {
                if (history.Count == 0 || history[history.Count - 1] != value)
                    history.Add(value);
                this.ActiveDatabase = activeTopic.Database;
            }
            if (ActiveTopicChanged != null)
                ActiveTopicChanged(this, null);
        }
    }

    public event EventHandler ActiveDatabaseChanged;

    public event EventHandler ActiveTopicChanged;

    public bool NavigateTo(HelpUri uri)
    {
        if (uri.Type == HelpUriType.Command)
        {
            if (uri.Target == "!B")
            {
                if (history.Count >= 2)
                {
                    history.RemoveAt(history.Count - 1);
                    ActiveTopic = history[history.Count - 1];
                    return true;
                }
            }
            return false;
        }

        HelpTopic topic = system.ResolveUri(activeDatabase, uri);
        if (topic == null)
        {
            if (uri.Type == HelpUriType.GlobalContext &&
                system.FindDatabase(uri.DatabaseName) == null)
            {
                string fileName = FindFile(activeDatabase, uri.DatabaseName);
                if (fileName != null)
                {
                    LoadDatabases(fileName);
                    topic = system.ResolveUri(activeDatabase, uri);
                }
            }
        }
        if (topic != null)
        {
            ActiveTopic = topic;
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerable<string> GetSearchPaths(HelpDatabase referrer)
    {
        string path = this.databaseFileNames[referrer];
        int maxLevels = 2;
        for (int level = 0; level < maxLevels; level++)
        {
            path = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(path))
            {
                break;
            }
            yield return path;
        }
    }

    private IEnumerable<string> GetSearchPaths()
    {
        Dictionary<string, bool> searched = new Dictionary<string, bool>();
        foreach (string databaseFileName in this.databaseFileNames.Values)
        {
            string dirPath = Path.GetDirectoryName(databaseFileName);
            string parentPath = Path.GetDirectoryName(dirPath);
            if (!string.IsNullOrEmpty(parentPath) &&
                !searched.ContainsKey(parentPath.ToLowerInvariant()))
            {
                searched[parentPath.ToLowerInvariant()] = true;
                yield return parentPath;
            }
        }
    }

    private string FindFile(HelpDatabase referrer, string fileName)
    {
        if (referrer == null)
            throw new ArgumentNullException(nameof(referrer));
        if (fileName == null)
            throw new ArgumentNullException(nameof(fileName));

        foreach (string searchPath in GetSearchPaths(referrer))
        {
            string[] fileNames = Directory.GetFiles(
                searchPath, fileName, SearchOption.AllDirectories);
            foreach (string filePath in fileNames)
            {
                if (!this.databaseFileNames.ContainsValue(
                    filePath.ToLowerInvariant()))
                    return filePath;
            }
        }
        return null;
    }

    public static HelpTopic GetDefaultTopicOfDatabase(HelpDatabase database)
    {
        if (database == null)
            return null;

        HelpTopic topic = database.ResolveContext("h.contents");
        if (topic != null)
            return topic;

        if (database.Topics.Count > 0)
            return database.Topics[0];

        return null;
    }

    public void DumpHierarchy()
    {
        HelpTopic root = system.ResolveUri(null, new HelpUri("h.contents"));
        if (root == null)
            return;

        //TopicHierarchy tree = new TopicHierarchy();
        //tree.Build(system, root);
        //tree.Dump();
    }
}

class HelpDatabaseViewItem
{
    readonly HelpDatabase database;

    public HelpDatabaseViewItem(HelpDatabase database)
    {
        this.database = database;
    }

    public HelpDatabase Database
    {
        get { return this.database; }
    }

    public override string ToString()
    {
        HelpTopic titleTopic = database.ResolveContext("h.title");
        if (titleTopic != null &&
            titleTopic.Lines.Count > 0 &&
            titleTopic.Lines[0].Text != null)
            return titleTopic.Lines[0].Text;

        if (database.Name == null)
            return "(unnamed database)";
        else
            return database.Name;
    }
}

class HelpTopicViewItem
{
    readonly int topicIndex;
    readonly HelpTopic topic;

    public HelpTopicViewItem(int topicIndex, HelpTopic topic)
    {
        this.topicIndex = topicIndex;
        this.topic = topic;
    }

    public HelpTopic Topic
    {
        get { return topic; }
    }

    public override string ToString()
    {
        return GetTopicDisplayTitle(topic);
    }

    public static string GetTopicDisplayTitle(HelpTopic topic)
    {
        if (topic == null)
            return null;

        if (topic.Title != null)
            return topic.Title;

        // try find a context string that points to this topic
        // TODO: make this part of topic's member
        List<string> contextStrings = new List<string>();
        foreach (string contextString in topic.Database.ContextStrings)
        {
            if (topic.Database.ResolveContext(contextString) == topic)
                contextStrings.Add(contextString);
        }
        if (contextStrings.Count > 0)
        {
            contextStrings.Sort();
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            foreach (string contextString in contextStrings)
            {
                if (sb.Length > 1)
                    sb.Append(", ");
                sb.Append(contextString);
            }
            sb.Append(']');
            return sb.ToString();
        }

        //return string.Format("[{0:000}] {1}", topicIndex, topic);
        return "(Untitled Topic)";
    }
}

class HelpTopicErrorViewItem
{
    readonly InvalidTopicDataEventArgs m_error;

    public HelpTopicErrorViewItem(InvalidTopicDataEventArgs error)
    {
        m_error = error;
    }

    public HelpTopic Topic
    {
        get { return m_error.Topic; }
    }

    public override string ToString()
    {
        return string.Format("[{0}: {1}] {2}",
            m_error.Topic.TopicIndex, m_error.Topic.Title,
            m_error.Message);
    }
}

using System;
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
        webBrowserReading.DocumentText = html;
        txtTopicTitle.Text = HelpTopicViewItem.GetTopicDisplayTitle(topic);
        txtSource.Text = topic.Source is string _text ? _text : topic.Source is byte[] bytes ? FormatHexData(bytes) : "";

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

    private void LstTopics_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTopics.SelectedItem is not HelpTopicViewItem item)
            return;

        var topic = item.Topic;
        viewModel.ActiveTopic = topic;
    }

    private static string FormatHexData(byte[] data)
    {
        var text_builder = new StringBuilder(8);
        var builder = new StringBuilder();
        builder.Append("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");

        for (int i = 0; i < data.Length; i++)
        {
            if (i % 16 == 0)
            {
                builder.Append("    ");
                builder.Append(text_builder.ToString());
                text_builder.Remove(0, text_builder.Length);

                builder.AppendLine();
                builder.Append(i.ToString("X4"));
                builder.Append("  ");
            }
            else if (i % 8 == 0)
            {
                builder.Append(" ");
            }
            builder.Append(' ');
            builder.Append(data[i].ToString("X2"));

            if (data[i] >= 32 && data[i] < 127)
                text_builder.Append((char)data[i]);
            else
                text_builder.Append('.');
        }
        if (data.Length % 16 != 0)
        {
            for (int i = 0; i < 16 - data.Length % 16; i++)
                builder.Append("   ");
            if (data.Length % 16 <= 8)
                builder.Append(' ');
        }
        builder.Append("    ");
        builder.Append(text_builder.ToString());
        return builder.ToString();
    }

    private void WebBrowserContent_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        var url = e.Url.OriginalString;
        if (!url.StartsWith("about:blank?"))
            return;

        var target = url[12..];
        var link = new HelpUri(target);
        if (!viewModel.NavigateTo(link))
        {
            MessageBox.Show("Cannot resolve link: " + target);
            e.Cancel = true;
        }
    }

    private void LstContexts_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstContexts.SelectedItem is not string contextString)
            return;

        viewModel.NavigateTo(new HelpUri(contextString));
    }

    private void MnuFileExit_Click(object sender, EventArgs e) => this.Close();

    private void MnuFileOpen_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() != DialogResult.OK)
            return;

        var fileNames = openFileDialog1.FileNames;
        foreach (var fileName in fileNames)
        {
            viewModel.LoadDatabases(fileName);
        }
    }

    private void CbDatabases_SelectedIndexChanged(object sender, EventArgs e)
    {
        viewModel.ActiveDatabase = cbDatabases.SelectedItem is not HelpDatabaseViewItem item ? null : item.Database;
    }

    private void BtnAddArchive_Click(object sender, EventArgs e)
    {
        MnuFileOpen_Click(sender, e);
    }

    private void BtnRemoveArchive_Click(object sender, EventArgs e)
    {
        int index = cbDatabases.SelectedIndex;
        if (index < 0)
            return;

        var item = cbDatabases.Items[index] as HelpDatabaseViewItem;
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
            if (MessageBox.Show($"{topic.Database.Name}: {topic.TopicIndex}: {topic.Title}",
                "Error", MessageBoxButtons.OKCancel) != DialogResult.OK)
                break;
        }
    }

    private void LstErrors_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstErrors.SelectedItem is not HelpTopicErrorViewItem item)
            return;

        viewModel.ActiveTopic = item.Topic;
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
            foreach (var fileName in fileNames)
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

        foreach (var fileName in databaseFileNames.Values)
        {
            fileNames[fileName] = fileName;
        }

        var savedNames = new System.Collections.Specialized.StringCollection();
        foreach (var fileName in fileNames.Keys)
        {
            savedNames.Add(fileName);
        }
        Properties.Settings.Default.OpenFileNames = savedNames;
    }

    public IEnumerable<HelpDatabase> Databases => system.Databases;

    public void AddDatabase(HelpDatabase database, string fileName)
    {
        ArgumentNullException.ThrowIfNull(database);

        system.Databases.Add(database);
        databaseFileNames[database] = Path.GetFullPath(fileName).ToLowerInvariant();
        DatabaseAdded?.Invoke(this, null);
        this.ActiveDatabase ??= database;
    }

    public void RemoveDatabase(HelpDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].Database == database)
                history.RemoveAt(i);
        }

        if (database == this.ActiveDatabase)
            this.ActiveDatabase = null;

        databaseFileNames.Remove(database);
        system.Databases.Remove(database);
        DatabaseRemoved?.Invoke(this, null);
    }

    public void LoadDatabases(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        var decoder = new BinaryHelpDeserializer();
        decoder.InvalidTopicData += OnInvalidTopicData;

        using var stream = File.OpenRead(fileName);
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

    private List<HelpTopic> topicsWithError = [];

    public List<HelpTopic> TopicsWithError => topicsWithError;

    public List<InvalidTopicDataEventArgs> DeserializationErrors { get; } = [];

    private void OnInvalidTopicData(object sender, QuickHelp.Serialization.InvalidTopicDataEventArgs e)
    {
        this.DeserializationErrors.Add(e);
    }

    public event EventHandler DatabaseAdded;

    public event EventHandler DatabaseRemoved;

    public HelpDatabase ActiveDatabase
    {
        get => activeDatabase;
        set
        {
            if (this.activeDatabase == value)
                return;

            this.activeDatabase = value;
            ActiveDatabaseChanged?.Invoke(this, null);

            if (this.activeTopic == null ||
                this.activeTopic.Database != activeDatabase)
                this.ActiveTopic = GetDefaultTopicOfDatabase(activeDatabase);
        }
    }

    public HelpTopic ActiveTopic
    {
        get => activeTopic;
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
            ActiveTopicChanged?.Invoke(this, null);
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

        var topic = system.ResolveUri(activeDatabase, uri);
        if (topic == null)
        {
            if (uri.Type == HelpUriType.GlobalContext &&
                system.FindDatabase(uri.DatabaseName) == null)
            {
                var fileName = FindFile(activeDatabase, uri.DatabaseName);
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
        var path = this.databaseFileNames[referrer];
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
        Dictionary<string, bool> searched = [];
        foreach (var databaseFileName in this.databaseFileNames.Values)
        {
            var dirPath = Path.GetDirectoryName(databaseFileName);
            var parentPath = Path.GetDirectoryName(dirPath);
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
        ArgumentNullException.ThrowIfNull(referrer);

        ArgumentNullException.ThrowIfNull(fileName);

        foreach (var searchPath in GetSearchPaths(referrer))
        {
            var fileNames = Directory.GetFiles(
                searchPath, fileName, SearchOption.AllDirectories);
            foreach (var filePath in fileNames)
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

        var topic = database.ResolveContext("h.contents");
        if (topic != null)
            return topic;

        if (database.Topics.Count > 0)
            return database.Topics[0];

        return null;
    }

    public void DumpHierarchy()
    {
        var root = system.ResolveUri(null, new HelpUri("h.contents"));
        if (root == null)
            return;

        //TopicHierarchy tree = new TopicHierarchy();
        //tree.Build(system, root);
        //tree.Dump();
    }
}

class HelpDatabaseViewItem(HelpDatabase database)
{
    readonly HelpDatabase database = database;

    public HelpDatabase Database => this.database;

    public override string ToString()
    {
        var titleTopic = database.ResolveContext("h.title");
        return titleTopic != null &&
            titleTopic.Lines.Count > 0 &&
            titleTopic.Lines[0].Text != null
            ? titleTopic.Lines[0].Text
            : database.Name ?? "(unnamed database)"
            ;
    }
}

class HelpTopicViewItem(int topicIndex, HelpTopic topic)
{
    readonly int topicIndex = topicIndex;
    readonly HelpTopic topic = topic;

    public HelpTopic Topic => topic;

    public int TopicIndex => topicIndex;

    public override string ToString() => GetTopicDisplayTitle(topic);

    public static string GetTopicDisplayTitle(HelpTopic topic)
    {
        if (topic == null)
            return null;

        if (topic.Title != null)
            return topic.Title;

        // try find a context string that points to this topic
        // TODO: make this part of topic's member
        List<string> contextStrings = [];
        foreach (var contextString in topic.Database.ContextStrings)
        {
            if (topic.Database.ResolveContext(contextString) == topic)
                contextStrings.Add(contextString);
        }
        if (contextStrings.Count > 0)
        {
            contextStrings.Sort();
            StringBuilder builder = new();
            builder.Append('[');
            foreach (var contextString in contextStrings)
            {
                if (builder.Length > 1)
                    builder.Append(", ");
                builder.Append(contextString);
            }
            builder.Append(']');
            return builder.ToString();
        }

        //return string.Format("[{0:000}] {1}", topicIndex, topic);
        return "(Untitled Topic)";
    }
}

class HelpTopicErrorViewItem(InvalidTopicDataEventArgs error)
{
    readonly InvalidTopicDataEventArgs m_error = error;

    public HelpTopic Topic => m_error.Topic;

    public override string ToString()
        => $"[{m_error.Topic.TopicIndex}: {m_error.Topic.Title}] {m_error.Message}";
}

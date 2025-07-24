using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PcTools
{
    public partial class MainForm : Form
    {
        private class CacheEntry
        {
            public string DataType { get; set; }
            public int Count { get; set; }
            public string Path { get; set; }
        }

        private BindingList<CacheEntry> caches = new BindingList<CacheEntry>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void CacheCleaner_Load(object sender, EventArgs e)
        {
            // === DataGridView setup ===
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataType",
                HeaderText = "Category",
                Width = 200
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Count",
                HeaderText = "Items",
                Width = 80
            });
            dataGridView1.DataSource = caches;

            // === Button hooks ===
            button1.Click += button1_Click;       // fetch & close
            button1.Click += (s, e) => ScanCaches();
            button2.Click += (s, e) => ClearCaches();

            label1.Text = "Ready";

            // check all by default
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1) find running browsers
            var browserNames = new[] { "chrome", "msedge", "firefox" };
            var running = Process.GetProcesses()
                                 .Where(p => browserNames.Contains(
                                     Path.GetFileNameWithoutExtension(p.ProcessName),
                                     StringComparer.OrdinalIgnoreCase))
                                 .ToList();

            if (running.Count > 0)
            {
                var msg = $"Detected {running.Count} browser process(es) open.\n" +
                          "I need to close them before scanning caches.\n\n" +
                          "Kill all browser processes now?";
                if (MessageBox.Show(msg, "Close Browsers?",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning)
                    != DialogResult.Yes)
                {
                    label1.Text = "Cancelled: browsers still running.";
                    return;
                }

                // 2) kill them
                foreach (var p in running)
                {
                    try
                    {
                        p.Kill();
                        Debug.WriteLine($"Killed {p.ProcessName} (PID {p.Id})");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to kill {p.ProcessName}: {ex.Message}");
                    }
                }
                Thread.Sleep(500);
            }

            label1.Text = "Browsers closed. Fetching data…";
        }

        private void ScanCaches()
        {
            caches.Clear();
            label1.Text = "Scanning…";
            Application.DoEvents();

            foreach (string category in checkedListBox1.CheckedItems)
            {
                try
                {
                    switch (category)
                    {
                        case "Internet Cache":
                            TryAddDir("Internet Cache",
                                Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
                            break;

                        case "Cookies":
                            TryAddDir("Cookies",
                                Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
                            break;

                        case "Internet History":
                            TryAddDir("Internet History",
                                Environment.GetFolderPath(Environment.SpecialFolder.History));
                            break;

                        case "Download History":
                            TryAddDir("Download History",
                                Path.Combine(Environment.GetFolderPath(
                                    Environment.SpecialFolder.UserProfile),
                                    "Downloads"));
                            break;

                        case "Last Download Location":
                            TryAddPreferenceFile("Last Download Location",
                                "Edge",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default\\Preferences"));
                            TryAddPreferenceFile("Last Download Location",
                                "Chrome",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Google\\Chrome\\User Data\\Default\\Preferences"));
                            break;

                        case "Session":
                            TryAddDir("Session",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default\\Sessions"));
                            break;

                        case "Saved Form Information":
                            TryAddDir("Saved Form Information",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default\\Web Data"));
                            break;

                        case "Saved Passwords":
                            TryAddFile("Saved Passwords",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default\\Login Data"));
                            TryAddFile("Saved Passwords",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Google\\Chrome\\User Data\\Default\\Login Data"));
                            break;

                        case "Compact Databases":
                            TryAddDir("Compact Databases",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Google\\Chrome\\User Data\\Default"));
                            TryAddDir("Compact Databases",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default"));
                            break;

                        case "Metrics Temp Files":
                            TryAddDir("Metrics Temp Files",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Google\\Chrome\\User Data\\Default\\Metrics"));
                            TryAddDir("Metrics Temp Files",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Edge\\User Data\\Default\\Metrics"));
                            break;

                        // --- NEW CCLEANER-WINDOWS ITEMS ---

                        case "Thumbnail Cache":
                            TryAddDir("Thumbnail Cache",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Windows\\Explorer"),
                                "thumbcache_*.db");
                            break;

                        case "Recycle Bin":
                            // per-drive Recycle folder
                            foreach (var drv in DriveInfo.GetDrives().Where(d => d.IsReady))
                            {
                                var rb = Path.Combine(drv.RootDirectory.FullName, "$Recycle.Bin");
                                TryAddDir("Recycle Bin", rb);
                            }
                            break;

                        case "System Temporary Files":
                            TryAddDir("System Temporary Files",
                                Path.GetTempPath());
                            break;

                        case "Windows Log Files":
                            TryAddDir("Windows Log Files",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.Windows),
                                    "System32\\winevt\\Logs"),
                                "*.evtx");
                            break;

                        case "Windows Event Trace Logs":
                            TryAddDir("Windows Event Trace Logs",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.Windows),
                                    "System32\\winevt\\Logs"),
                                "*.etl");
                            break;

                        case "Windows Web Cache":
                            TryAddDir("Windows Web Cache",
                                Path.Combine(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft\\Windows\\WebCache"));
                            break;

                        default:
                            Debug.WriteLine($"Unknown category: {category}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error scanning {category}: {ex.Message}");
                }
            }

            dataGridView1.Refresh();
            var total = caches.Sum(c => c.Count);
            label1.Text = $"Scan complete — {total} item(s) found";
        }

        private void ClearCaches()
        {
            label1.Text = "Clearing…";
            Application.DoEvents();

            foreach (var entry in caches)
            {
                try
                {
                    if (entry.Count == 0) continue;

                    if (Directory.Exists(entry.Path))
                    {
                        Directory.Delete(entry.Path, true);
                        Directory.CreateDirectory(entry.Path);
                    }
                    else
                    {
                        var folder = Path.GetDirectoryName(entry.Path);
                        var mask = Path.GetFileName(entry.Path);
                        if (Directory.Exists(folder))
                            foreach (var f in Directory.GetFiles(folder, mask))
                                File.Delete(f);
                    }
                    entry.Count = 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error clearing {entry.DataType} ({entry.Path}): {ex.Message}");
                }
            }

            dataGridView1.Refresh();
            label1.Text = "Selected data cleared";
        }

        // helpers with pattern support
        private void TryAddDir(string name, string folder, string pattern = "*")
        {
            if (!Directory.Exists(folder))
            {
                Debug.WriteLine($"Skipped {name}: folder not found ({folder})");
                return;
            }
            var files = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);
            caches.Add(new CacheEntry { DataType = name, Count = files.Length, Path = folder });
        }

        private void TryAddFile(string name, string path)
        {
            if (File.Exists(path))
                caches.Add(new CacheEntry { DataType = name, Count = 1, Path = path });
            else
                Debug.WriteLine($"Skipped {name}: file not found ({path})");
        }

        private void TryAddPreferenceFile(string name, string browser, string path)
        {
            if (File.Exists(path))
                caches.Add(new CacheEntry
                { DataType = $"{name} ({browser})", Count = 1, Path = path });
            else
                Debug.WriteLine($"Skipped {name} for {browser}: file not found ({path})");
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class CopyUtility : Form
    {
        public CopyUtility()
        {
            InitializeComponent();
            this.FormClosed += CopyUtility_FormClosed;
            button1.MouseHover += Button1_MouseHover;
            button2.MouseHover += Button2_MouseHover;
            checkBox1.MouseHover += CheckBox1_MouseHover;

            output.TextChanged += Output_TextChanged;
        }

        private void Output_TextChanged(object sender, EventArgs e)
        {
            output.SelectionStart = output.Text.Length;
            output.ScrollToCaret();
        }

        private void CheckBox1_MouseHover(object sender, EventArgs e)
        {
            Control senderObject = sender as Control;

            ToolTip info = new ToolTip
            {
                AutomaticDelay = 500
            };
            info.SetToolTip(senderObject, "Replaces old files from dst with new files from src");
        }

        private void Button2_MouseHover(object sender, EventArgs e)
        {
            Control senderObject = sender as Control;

            ToolTip info = new ToolTip
            {
                AutomaticDelay = 500
            };
            info.SetToolTip(senderObject, "Files here will be replaced by newer or sustain unchanged");
        }

        private void Button1_MouseHover(object sender, EventArgs e)
        { 
            Control senderObject = sender as Control;

            ToolTip info = new ToolTip
            {
                AutomaticDelay = 500
            };
            info.SetToolTip(senderObject, "This files will be copied into dst folder");
        }

        private void CopyUtility_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.Show();
            System.GC.Collect();
        }

        private Form1 mainWindow;

        public void init(Form1 window)
        {
            mainWindow = window;
        }


        private string dstFolder = "";
        private string srcFolder = "";

        private void button2_Click(object sender, EventArgs e)
        {
            /* Destination folder */
            dstFolder = openFolder();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* Source folder */
            srcFolder = openFolder();
        }

        private string openFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Environment.SpecialFolder.Desktop.ToString();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return "";
            }
        }


        private bool copying = false;

        private List<Thread> _threads = new List<Thread>();


        

        private void button3_Click(object sender, EventArgs e)
        {

            if (srcFolder.Length < 2)
            {
                MessageBox.Show("Please select a src folder");
                return;
            }
            if (dstFolder.Length < 2)
            {
                MessageBox.Show("Please select a dst folder");
                return;
            }
            if (!copying)
            {
                button3.Text = "Abort";
                Thread t = new Thread(() =>
                {
                    MessageBox.Show("Click OK to start!");
                    copying = true;
                    DirectoryInfo dirSource = new DirectoryInfo(srcFolder);
                    DirectoryInfo dirTarget = new DirectoryInfo(dstFolder);
                    copy(dirSource, dirTarget, checkBox1.Checked);

                    MessageBox.Show("Finished copying!");
                });

                t.Start();
                _threads.Add(t);
                return;
            }

            /* Start or abort */

            if (_threads.Count == 0)
            {
                copying = false;
                button3.Text = "Start";
            }

            foreach (Thread t in _threads.ToList())
            {
                t.Abort();
                if (!t.IsAlive)
                    _threads.Remove(t);
            }

            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            mainWindow.Show();
        }

        private List<string> logQueue = new List<string>();

        private void log(string input)
        {
            if(logQueue.Count > 750)
            {
                List<string> queueCopy = logQueue.ToList();
                logQueue.Clear();
                string append = "";
                foreach (string str in queueCopy)
                {
                    append = append + str + "\n";
                }

                if (output.InvokeRequired)
                {
                    output.BeginInvoke(new Action(delegate ()
                    {

                        output.AppendText(append + "\n");

                    }));
                }
                else
                    output.AppendText(append + "\n");
            }
            else
            {
                logQueue.Add(input);
            }

            
        }


        private void copy(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {

            Directory.CreateDirectory(target.FullName);
            
            foreach(FileInfo fi in source.GetFiles())
            {
                if(File.Exists(Path.Combine(target.FullName, fi.Name)))
                {
                    if ((File.GetLastWriteTime(Path.Combine(target.FullName, fi.Name)) < (File.GetLastWriteTime(Path.Combine(source.FullName, fi.Name)))) && overwrite)
                    {
                        fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                        log("Overwrite " + fi.Name);
                    }
                    else
                        continue;
                }
                else
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                    log("Copying " + fi.Name);
                }
            }

            foreach(DirectoryInfo diSourceSub in source.GetDirectories())
            {
                DirectoryInfo nextTarget = target.CreateSubdirectory(diSourceSub.Name);
                copy(diSourceSub, nextTarget, overwrite);
            }


        }






    }
}

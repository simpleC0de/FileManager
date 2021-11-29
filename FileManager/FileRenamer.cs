using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileManager
{
    public partial class FileRenamer : Form
    {

        private Form1 mainWindow;
        private string selectedFolder = "";
        private bool pathValid = false;

        private List<renameFile> files = new List<renameFile>();

        public FileRenamer()
        {
            InitializeComponent();
        }

        private void FileRenamer_Load(object sender, EventArgs e)
        {
            /* Set the status label to help the user */
            textBox1.TextChanged += TextBox1_TextChanged;
            findText.TextChanged += FindText_TextChanged;
            this.FormClosed += FileRenamer_FormClosed;
            textBox1.MouseClick += TextBox1_MouseClick;
            dataGridView1.DataSourceChanged += DataGridView1_DataSourceChanged;
        }

        private void DataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            foundLabel.Text = "found " + ((List<renameFile>)dataGridView1.DataSource).Count + " entries";
        }

        private void TextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            button2_Click(this, new EventArgs());
        }

        private void FileRenamer_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.Show();
            System.GC.Collect();
        }

        private void FindText_TextChanged(object sender, EventArgs e)
        { 
            if(dataGridView1.DataSource != null && pathValid)
            {
                List<renameFile> fileCopy = new List<renameFile>();
                foreach(renameFile file in files)
                {
                    if (file.name.ToLower().Contains(findText.Text.ToLower()))
                        fileCopy.Add(file);
                }

                dataGridView1.DataSource = fileCopy;
                dataGridView1.Refresh();
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            /* Check if user has valid path */
            if (Directory.Exists(textBox1.Text))
            {
                statusLabel.Text = "OK";
                statusLabel.ForeColor = Color.Green;
                pathValid = true;
            }
            else
            {
                if (File.Exists(textBox1.Text))
                {
                    statusLabel.Text = "OK";
                    statusLabel.ForeColor = Color.Green;
                    pathValid = true;
                }
                else
                {
                    statusLabel.Text = "Not found";
                    statusLabel.ForeColor = Color.Red;
                    pathValid = false;
                }
            }
            

        }

        public void init(Form1 window)
        {
            mainWindow = window;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            mainWindow.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = Environment.SpecialFolder.DesktopDirectory.ToString();
            
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = folderBrowserDialog.SelectedPath;
                textBox1.Text = selectedFolder;
            }
            button3_Click(this, new EventArgs());
            System.GC.Collect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /* Initializes scan */


            if (!pathValid)
            {
                MessageBox.Show("Select a correct folder..");
                return;
            }

            files.Clear();
            dataGridView1.DataSource = null;

            DirectoryInfo dirInfo = new DirectoryInfo(selectedFolder);
            FileInfo[] fileInfos = dirInfo.GetFiles();

            foreach(FileInfo f in fileInfos)
            {
                renameFile file = new renameFile();
                file.datatype = f.Extension;
                file.name = f.Name;

                file.name = file.name.Replace(f.Extension, "");

                files.Add(file);
            }

            dataGridView1.DataSource = files;
            dataGridView1.Refresh();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            /* Find and Replace with... */

            if(replaceText.Text.Length == 0)
            {
                MessageBox.Show("Cant replace with nothing");
                return;
            }

            if (((List<renameFile>)dataGridView1.DataSource).Count > 0)
            {
                int fileIndex = 0;
                if (findText.Text.Length == 0 | (findText.Text.Replace(" ", "")).Length == 0)
                {
                    foreach (string file in Directory.GetFiles(selectedFolder))
                    {
                        File.Move(file, selectedFolder + @"\" + replaceText.Text + "(" + fileIndex + ")" + Path.GetExtension(file));
                        fileIndex += 1;
                    }
                }
                else
                {
                    foreach (renameFile f in files)
                    {
                        if (f.name.ToLower().Contains(findText.Text.ToLower()))
                        {

                            File.Move(selectedFolder + @"\" + f.name + f.datatype, selectedFolder + @"\" + f.name.Replace(findText.Text, replaceText.Text) + f.datatype);

                        }

                    }
                }
                

            }
            else
                MessageBox.Show("No file found..");

            button3_Click(this, new EventArgs());
        }
    }

    class renameFile
    {
        public string name { get; set; }
        public string datatype { get; set; }

    }
}

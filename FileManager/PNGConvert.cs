using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class PNGConvert : Form
    {
        public PNGConvert()
        {
            InitializeComponent();
            this.FormClosed += PNGConvert_FormClosed;
            checkBox1.MouseHover += CheckBox1_MouseHover;
            checkBox1.Checked = true;
        }

        private void CheckBox1_MouseHover(object sender, EventArgs e)
        {
            Control senderObject = sender as Control;
            string hoveredControl = senderObject.Name.ToString();
            if(hoveredControl != "")
            {
                ToolTip info = new ToolTip
                {
                    AutomaticDelay = 500
                };
                info.SetToolTip(senderObject, "Delete old files or keep them");
            }
        }

        private void PNGConvert_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.Show();
            System.GC.Collect();
        }

        private Form1 mainWindow;

        public void init(Form1 window)
        {
            mainWindow = window;
        }


        private List<customImage> selectedFiles = new List<customImage>();
        private List<customImage> usableFiles = new List<customImage>();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = true;
            opf.Filter = "Images (*.PNG;*.JPG;)|*.PNG;*.JPG";
            opf.Title = "Select images to convert";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                //selectedFiles = opf.FileNames.ToList();
                selectedFiles = new List<customImage>();
                foreach(string str in opf.FileNames)
                {

                    customImage ci = new customImage();
                    ci.extension = Path.GetExtension(str);
                    ci.name = (Path.GetFileName(str)).Replace(ci.extension, "");
                    ci.homeDir = Path.GetDirectoryName(str);
                    selectedFiles.Add(ci);
                }

            }
            dataGridView1.DataSource = selectedFiles;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            mainWindow.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select to convert");
                return;
            }
            if(safeDir == "")
            {
                MessageBox.Show("Please select a directory to safe in");
                return;
            }

            try
            {
                foreach (customImage ci in usableFiles)
                {
                    Bitmap bitMap = (Bitmap)Bitmap.FromFile(ci.homeDir + @"\" + ci.name + ci.extension);
                    System.GC.Collect();
                    using (MemoryStream ms = new MemoryStream())
                    {

                        ImageFormat imf;
                        string extension;
                        if (pngCheckTo.Checked)
                        {
                            imf = ImageFormat.Png;
                            extension = ".png";
                        }
                        if (jpgCheckTo.Checked)
                        {
                            imf = ImageFormat.Jpeg;
                            extension = ".jpg";
                        }
                        else
                        {
                            imf = ImageFormat.Png;
                            extension = ".png";
                        }

                        bitMap.Save(ms, imf);
                        Image img = Image.FromStream(ms);
                        if (safeDir == "")
                            img.Save(safeDir + @"\" + ci.name + extension);
                        else
                            img.Save(safeDir + @"\" + ci.name + extension);

                        img.Dispose();
                        bitMap.Dispose();
                        Thread.Sleep(250);
                    }
                    try
                    {
                        if (!checkBox1.Checked)
                        {
                            File.Delete(ci.homeDir + @"\" + ci.name + ci.extension);
                        }
                    }
                    catch (Exception) { }
                    System.GC.Collect();

                }
            }
            catch (Exception) { }
            
            MessageBox.Show("Finished converting");

        }


        private void sortDataGrid(string format)
        {
            usableFiles = new List<customImage>();
            foreach (customImage ci in selectedFiles)
            {
                if (ci.extension.Contains(format))
                {
                    usableFiles.Add(ci);
                }
            }

            dataGridView1.DataSource = usableFiles;
            dataGridView1.Refresh();

        }

        private void PNGConvert_Load(object sender, EventArgs e)
        {
            jpgCheckFrom.CheckedChanged += JpgCheckFrom_CheckedChanged;
            pngCheckFrom.CheckedChanged += PngCheckFrom_CheckedChanged;
            pngCheckTo.CheckedChanged += PngCheckTo_CheckedChanged;
            jpgCheckTo.CheckedChanged += JpgCheckTo_CheckedChanged;
        }

        private void JpgCheckTo_CheckedChanged(object sender, EventArgs e)
        {
            if (pngCheckTo.Checked)
                jpgCheckTo.Checked = false;
            if (jpgCheckFrom.Checked)
                jpgCheckTo.Checked = false;
        }

        private void PngCheckTo_CheckedChanged(object sender, EventArgs e)
        {
            if (jpgCheckTo.Checked)
                pngCheckTo.Checked = false;
            if (pngCheckFrom.Checked)
                pngCheckTo.Checked = false;
        }

        private void PngCheckFrom_CheckedChanged(object sender, EventArgs e)
        {
            if (jpgCheckFrom.Checked)
                pngCheckFrom.Checked = false;

            if (pngCheckTo.Checked)
                pngCheckFrom.Checked = false;

            if (pngCheckFrom.Checked)
                sortDataGrid("png");
            else
                sortDataGrid("");
            

        }

        private void JpgCheckFrom_CheckedChanged(object sender, EventArgs e)
        { 
            if (pngCheckFrom.Checked)
                jpgCheckFrom.Checked = false;

            if (jpgCheckTo.Checked)
                jpgCheckFrom.Checked = false;

            if (jpgCheckFrom.Checked)
                sortDataGrid("jpg");
            else
                sortDataGrid("");
        }


        private string safeDir = "";

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = selectedFiles[0].homeDir;

            if(fbd.ShowDialog() == DialogResult.OK)
            {
                safeDir = fbd.SelectedPath;
            }
        }
    }

    class customImage
    {
        public string name { get; set; }
        public string extension { get; set; }

        public string homeDir { get; set; }
    }
}

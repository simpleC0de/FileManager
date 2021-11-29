using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {

        private FileRenamer fileRenamer;
        private PNGConvert pngConvert;
        private CopyUtility copyUtility;
        private Form1 mainWindow;
        public Form1()
        {
            InitializeComponent();
            
            mainWindow = this;

            /* Initiate all former classes and forms */
            fileRenamer = new FileRenamer();
            fileRenamer.init(mainWindow);

            pngConvert = new PNGConvert();
            pngConvert.init(mainWindow);

            copyUtility = new CopyUtility();
            copyUtility.init(mainWindow);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* File Rename */
            this.Hide();
            try { fileRenamer.Show(); } catch(Exception) { fileRenamer = new FileRenamer(); fileRenamer.Show(); fileRenamer.init(this); }
            System.GC.Collect();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /* JPG to PNG */
            this.Hide();
            try { pngConvert.Show(); } catch (Exception) { pngConvert = new PNGConvert(); pngConvert.Show(); pngConvert.init(this); }
            System.GC.Collect();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            /* Copy Utility */
            this.Hide();
            try { copyUtility.Show(); } catch (Exception) { copyUtility = new CopyUtility(); copyUtility.Show(); copyUtility.init(this); }
            System.GC.Collect();
        }
    }
}

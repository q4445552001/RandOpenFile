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
using System.Diagnostics;

namespace Rand_open_file
{
    public partial class Form1 : Form
    {
        string filedata = @"Rand_open_file.temp";
        List<string> excludefile = new List<string>();
        int i = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void dirfile(string folder)
        {
            bool j = true ;
            foreach (string f in Directory.GetDirectories(folder))
                dirfile(f);

            foreach (string filename in Directory.GetFiles(folder))
            {
                foreach (string efi in excludefile)
                    if ((filename.ToLower().IndexOf(efi) != -1 && !String.IsNullOrEmpty(efi.Trim())))
                        j = false;
                if (j || String.IsNullOrEmpty(textBox2.Text.Trim()))
                    File.AppendAllText(filedata, filename + Environment.NewLine);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(filedata))
            {
                using (StreamReader sr = new StreamReader(filedata))
                {
                    while (sr.ReadLine() != null)
                        i++;
                    sr.Close();
                }
            }

            if (i == 0)
            {
                if (textBox2.Text.IndexOf(";") != -1)
                    excludefile = textBox2.Text.ToLower().Split(';').ToList();
                else
                {
                    excludefile.Clear();
                    excludefile.Add(textBox2.Text.ToLower());
                }
                dirfile(textBox3.Text);
                button1_Click(sender, e);
            }
            else
            {
                Random Rnd = new Random();
                int rand = Rnd.Next(0, i);
                List<string> lines = new List<string>(File.ReadAllLines(filedata));
                Console.WriteLine(lines[rand]);
                var run = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine("explorer.exe"),
                        Arguments = lines[rand],
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false
                    }
                };
                run.Start();
                lines.RemoveAt(rand);
                File.WriteAllLines(filedata, lines.ToArray());
                i = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox3.Text = path.SelectedPath;
        }
    }

}

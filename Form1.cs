using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace RandOpenFile
{
    public partial class Form1 : Form
    {
        #region 初始化

        private readonly string fileTemp = "RandOpenFile.temp";

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = Settings.Default.path;
            textBox2.Text = Settings.Default.excludefile;
            textBox3.Text = Settings.Default.excludefolder;
        }

        #endregion

        #region Button
        /// <summary>
        /// 關閉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.path = textBox1.Text;
            Settings.Default.excludefile = textBox2.Text;
            Settings.Default.excludefolder = textBox3.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKBtn_Click(object sender, EventArgs e)
        {
            var fileCount = FileCount();

            if (fileCount == 0)
            {
                if (String.IsNullOrEmpty(textBox1.Text.Trim()))
                {
                    MessageBox.Show("請先選擇路徑", "提示");
                }
                else
                {
                    DirFile(textBox1.Text);

                    if (FileCount() == 0)
                        MessageBox.Show("找不到任何檔案", "提示");
                    else
                        OKBtn_Click(sender, e);
                }
            }
            else
            {
                var Rnd = new Random();
                var rand = Rnd.Next(0, fileCount);
                var lines = new List<string>(File.ReadAllLines(fileTemp));

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
                File.WriteAllLines(fileTemp, lines.ToArray());
            }
        }

        /// <summary>
        /// 選擇檔案瀏覽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List1_Click(object sender, EventArgs e)
        {
            var path = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true,
            };

            if (path.ShowDialog() == CommonFileDialogResult.Ok)
                textBox1.Text = string.Join(Environment.NewLine, path.FileNames.ToList());
        }

        /// <summary>
        /// 排除檔案瀏覽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List2_Click(object sender, EventArgs e)
        {
            var path = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Video Files(*.mp4;*.mkv)|*.mp4;*.mkv",
            };

            if (path.ShowDialog() == DialogResult.OK)
                textBox2.Text = string.Join(Environment.NewLine, path.FileNames.ToList());
        }

        /// <summary>
        /// 排除資料夾
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List3_Click(object sender, EventArgs e)
        {
            var path = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true,
            };

            if (path.ShowDialog() == CommonFileDialogResult.Ok)
                textBox3.Text = string.Join(Environment.NewLine, path.FileNames.ToList());
        }

        /// <summary>
        /// 清除暫存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, EventArgs e)
        {
            if (File.Exists(fileTemp))
            {
                File.Delete(fileTemp);
                MessageBox.Show("刪除成功", "提示");
            }
            else
            {
                MessageBox.Show("找不到暫存", "提示");
            }

        }

        #endregion

        #region 副程式
        /// <summary>
        /// 檔案迴圈
        /// </summary>
        /// <param name="folder"></param>
        private void DirFile(string folder)
        {
            var Filter = new string[] { ".mp4", ".mkv", ".txt" };
            var excludeFile = textBox2.Text.ToLower().Split('\n');
            var excludeFolder = textBox3.Text.ToLower().Split('\n');

            foreach (var f in Directory.GetDirectories(folder))
                DirFile(f);

            if (!excludeFolder.Where(x => x.ToLower().Contains(folder.ToLower())).Any()) //未在排除資料夾名單內
            {
                foreach (var filename in Directory.GetFiles(folder))
                {
                    if (!excludeFile.Where(x => x.ToLower().Contains(filename.ToLower())).Any()) //未在排除檔案名單內
                        if (Filter.Where(x => filename.ToLower().Contains(x.ToLower())).Any()) //有在Filter名單內
                            File.AppendAllText(fileTemp, $"{filename}{Environment.NewLine}");
                }
            }
        }

        /// <summary>
        /// 計算數量
        /// </summary>
        /// <returns></returns>
        private int FileCount()
        {
            var fileCount = 0;
            if (File.Exists(fileTemp))
            {
                using var sr = new StreamReader(fileTemp);
                while (sr.ReadLine() != null)
                    fileCount++;
                sr.Close();
            }

            return fileCount;
        }

        #endregion
    }

}

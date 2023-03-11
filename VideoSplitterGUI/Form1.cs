using Accord.Video.FFMPEG;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace VideoSplitterGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.FileOk += FileOpened;
            saveFileDialog1.FileOk += DirectoryToSave;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void DirectoryToSave(object sender, CancelEventArgs e)
        {
            textBox2.Text = Path.GetDirectoryName(saveFileDialog1.FileName) + '\\';
            button4.Enabled = false;
        }

        private void FileOpened(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text)) return;
            VideoFileReader lettore = new VideoFileReader();
            lettore.Open(textBox1.Text);
            for (int i = 0; i < lettore.FrameCount; i++) lettore.ReadVideoFrame().Save(Path.Combine(textBox2.Text, $"frame{i}.bmp"));
            lettore.Close();
            MessageBox.Show("Video convertito con successo!");
            Process.Start("explorer", textBox2.Text);

        }

        private void button3_Click(object sender, EventArgs e) => openFileDialog1.ShowDialog();

        private void button4_Click(object sender, EventArgs e) => saveFileDialog1.ShowDialog();
    }
}

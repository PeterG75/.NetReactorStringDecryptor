using System;
using System.Drawing;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System.Windows.Forms;
using System.Reflection;

namespace DotNetReactorStringDecryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.RestoreDirectory = true;
            op.Multiselect = false;
            op.Title = "Select a .net assembly";
            op.Filter = ".Net assembly |*.exe;*.dll";
            if (op.ShowDialog() == DialogResult.OK && Path.GetExtension(op.FileName).ToLower() == ".exe")
            {
                textBox1.Text = op.FileName;
                label2.Text = "Result : Assembly successfully loaded !";
                label2.ForeColor = Color.SteelBlue;
            }
            else if (op.FileName != "")
            {
                label2.Text = "Result : Invalid assembly !";
                label2.ForeColor = Color.Firebrick;
            }
        }
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                if (Path.GetExtension(files[0]).ToLower() == ".exe")
                {
                    textBox1.Text = files[0];
                    label2.Text = "Result : Assembly successfully loaded !";
                    label2.ForeColor = Color.SteelBlue;
                }
                else
                {
                    label2.Text = "Result : Invalid assembly !";
                    label2.ForeColor = Color.Firebrick;
                }

            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CodeStrikers [CS-RET]\r\nCoded by SychicBoy\r\n\r\nWebsite : CodeStrikers.org\r\nForum : Forum.CodeStrikers.org\r\nTelegram Channel : @CodeStrikers\r\nSecound Telegram Channel : @CSSOFT\r\nEmail : SychicBoy@outlook.com", "About coder", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
            {
                label2.Text = "Please select target assembly !";
                label2.ForeColor = Color.Firebrick;
                return;
            }
            try
            {
                ModuleDefMD.Load(textBox1.Text);
            }
            catch (BadImageFormatException ex)
            {
                label2.Text = "Result : Invalid assembly !";
                label2.ForeColor = Color.Firebrick;
                return;
            }
            Context.module = ModuleDefMD.Load(textBox1.Text);
            Context.assembly = Assembly.LoadFrom(textBox1.Text);
            int count = StringDecryptor.Execute();
            ModuleWriterOptions options = new ModuleWriterOptions(Context.module);
            options.Logger = DummyLogger.NoThrowInstance;
            options.MetadataOptions.Flags = MetadataFlags.PreserveAll & MetadataFlags.KeepOldMaxStack;
            Context.module.Write(Path.GetDirectoryName(textBox1.Text) + "\\" + Context.module.Assembly.Name + "_strdec.exe", options);
            label2.Text = "Result : " + count + " Strings successfully decrypted !";
        }
    }
}

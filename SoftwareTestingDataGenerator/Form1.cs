using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareTestingDataGenerator
{
    public partial class Form1 : Form
    {

        List<string> _names = new List<string>{ "tom", "robert", "albert"};

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Verify directory
            if(!Directory.Exists(textBox1.Text))
            {
                if(MessageBox.Show("Your directory doesn't exist brah, create it?", "Hey Buddy", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Directory.CreateDirectory(textBox1.Text);
                }
                else
                {
                    return;
                }
            }

            string preprocess = textBox2.Text.Split(',')[0];

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            string str = rgx.Replace(preprocess, "");
            string fullFilePath = Path.Combine(textBox1.Text, str + ".txt");

            if (File.Exists(fullFilePath))
            {
                if(MessageBox.Show("File already exists! Replace it?", "File Eixsts", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    File.Delete(fullFilePath);
                }
                else
                {
                    return;
                }
            }

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            numMain.Enabled = false;

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (object obj, DoWorkEventArgs ev) =>
            {
                progressBar1.Maximum = (int)numMain.Value;

                List<string> toAdd = new List<string>();
                for (int i = 0; i < numMain.Value; i++)
                {
                    string name = randomName();
                    List<string> infoString = new List<string> { name, i.ToString(), name + "@gmail.com" };
                    infoString.AddRange(randomScores());
                    toAdd.Add(string.Join(",", infoString));

                    if (i % 10000 == 0)
                    {
                        File.AppendAllLines(fullFilePath, toAdd.ToArray());
                        progressBar1.Value = i;
                        toAdd.Clear();
                    }
                }
                //Get the strays
                File.AppendAllLines(fullFilePath, toAdd.ToArray());
            };

            bw.RunWorkerCompleted += (object obj, RunWorkerCompletedEventArgs ev) =>
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                numMain.Enabled = true;
                progressBar1.Value = progressBar1.Maximum;

                MessageBox.Show("DONE!");

            };

            bw.RunWorkerAsync();

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(fullFilePath))
            //{
            //    foreach (string line in toAdd)
            //    {
            //        file.WriteLine(line);
            //    }
            //}
        }

        Random rand = new Random();
        private string randomName()
        {
            
            int random = rand.Next(_names.Count);
            return _names[random];
        }

        private List<string> randomScores()
        {
            return new List<string> { rand.Next(4).ToString(), rand.Next(4).ToString(), rand.Next(4).ToString() };
        }
    }
}

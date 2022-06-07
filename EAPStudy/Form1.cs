using EAPStudy.EPA_Single;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAPStudy
{
    public partial class Form1 : Form
    {

        private CalculateFactorial calculate_single = new CalculateFactorial();
        public Form1()
        {
            InitializeComponent();
            calculate_single.WorkerReportsProgress = true;
            calculate_single.WorkerSupportsCancellation = true;

            calculate_single.CalculateCompleted += Calculate_single_CalculateCompleted;
            calculate_single.CalculateProgressChanged += Calculate_single_CalculateProgressChanged;
        }

        private void Calculate_single_CalculateProgressChanged(object sender, EventArgs.CalculateProgressChangedEventArgs e)
        {
            this.label2.Text = e.Percent.ToString() + "%";
            this.progressBar1.Value = e.Percent;
        }

        private void Calculate_single_CalculateCompleted(object sender, EventArgs.CalculateCompletedEventAgrs e)
        {
            if( !e.Cancelled )
                this.textBox2.Text = e.Result.ToString();
            else
            {
                this.label2.Text = "";
                this.label3.Text = "value : ";
                this.textBox2.Text = "";
                this.progressBar1.Value = 0;
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            textBox1.Text = "10";
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;

            try
            {
                label3.Text = textBox1.Text + "! :";
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                calculate_single.CalculateAsync(Convert.ToInt32(textBox1.Text));
            }
            catch { }

            
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            calculate_single.CancelAsync();
        }
    }
}

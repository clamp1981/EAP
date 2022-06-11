
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EAPStudy;

namespace EAPStudy
{
    public partial class Form1 : Form
    {

        private EAP_Single.CalculateFactorial calculate_single = null;
        private EAP_Multi.CalculateFactorial calculate_multi = null;
        Guid taskId = Guid.NewGuid();

        public Form1()
        {
            InitializeComponent();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            InitializeCalculateFactorial();

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
                if (radioButton1.Checked)
                    calculate_single.CalculateAsync(Convert.ToInt32(textBox1.Text));
                else
                {
                    calculate_multi.CalculateAsync(Convert.ToInt32(textBox1.Text), taskId);
                }
            }
            catch { }


        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (radioButton1.Checked)
            {
                calculate_single.CancelAsync();
            }                
            else
            {
                calculate_multi.CancelCalculateAsync(taskId);                
            }

        }

        private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
        {
            InitializeCalculateFactorial();
        }

        private void InitializeCalculateFactorial()
        {
            if (radioButton1.Checked)
            {
                if (calculate_single == null)
                {
                    calculate_single = new EAP_Single.CalculateFactorial();
                    calculate_single.WorkerReportsProgress = true;
                    calculate_single.WorkerSupportsCancellation = true;
                    calculate_single.CalculateCompleted += Calculate_single_CalculateCompleted;
                    calculate_single.CalculateProgressChanged += Calculate_single_CalculateProgressChanged;
                }

            }
            else
            {
                if (calculate_multi == null)
                {
                    calculate_multi = new EAP_Multi.CalculateFactorial();
                    calculate_multi.WorkerReportsProgress = true;
                    calculate_multi.CalculateProgressChanged += Calculate_multi_CalculateProgressChanged;
                    calculate_multi.CalculateFactorialCompleted += Calculate_multi_CalculateFactorialCompleted;
                }

            }
        }

        private void Calculate_multi_CalculateFactorialCompleted(object sender, EventArgs.CalculateFactorialCompletedEventArgs e)
        {
            if (!e.Cancelled)
                this.textBox2.Text = e.ResultValue.ToString();
            else
            {
                this.label2.Text = "";
                this.label3.Text = "value : ";
                this.textBox2.Text = "";
                this.progressBar1.Value = 0;
            }
        }

        private void Calculate_multi_CalculateProgressChanged(object sender, EventArgs.CalculateProgressChangedEventArgs e)
        {
            this.label2.Text = e.Percent.ToString() + "%";
            this.progressBar1.Value = e.Percent;
        }

        private void button3_Click(object sender, System.EventArgs e)
        {

        }
    }
}

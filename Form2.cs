using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.AcceptButton = button2;
            if (Form1.DEchipher)
            {

            }
            else
            {
                textBox1.Visible= false;
                label1.Visible= false;
            }
        }
        public BigInteger d_key = 0;
        public long p_key = 0;
        public long q_key = 0;
        public string Key_RC6;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {

                string file = openFile.FileName; //d-p-q
                StreamReader reader = new StreamReader(file);
                string[] keys_str = reader.ReadToEnd().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    d_key = new BigInteger(long.Parse(keys_str[0]));
                    p_key = long.Parse(keys_str[1]);
                    q_key = long.Parse((keys_str[2]));
                    Key_RC6 = keys_str[3];
                }
                catch (Exception exptn)
                {
                    if (exptn is FormatException || exptn is OverflowException || exptn is ArgumentNullException)
                    {
                        MessageBox.Show("Один або всі ключі не є представленнями цілих чисел");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(d_key > 0 && p_key > 0 && q_key > 0)
            {
                this.Close();
            }
            else if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && textBox3.Text.Length > 0)
            {
                try
                {
                    d_key = new BigInteger(long.Parse(textBox1.Text));
                    p_key = long.Parse(textBox2.Text);
                    q_key = long.Parse(textBox3.Text);
                    Key_RC6 = textBox4.Text;
                    this.Close();
                }
                catch (Exception exptn)
                {
                    if (exptn is FormatException || exptn is OverflowException || exptn is ArgumentNullException)
                    {
                        MessageBox.Show("Один або всі ключі не є представленнями цілих чисел");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ви не ввели значення для ключа/ключів");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            int p = int.Parse(textBox6.Text);
            int q = int.Parse(textBox5.Text);
            int s = int.Parse(textBox7.Text);
            BBS(p, q, s, p * q, 16);
        }
       
        void BBS(int p, int q, int s, int n, int num)
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            if (isprime(p))
            {
                if (p % 4 == 3)
                {
                    if (isprime(q))
                    {
                        if (q % 4 == 3)
                        {
                            if (s % p != 0 && s % q != 0)
                            {
                                long pow2 = (long)s * (long)s;
                                long X0 = pow2 % n;
                                string rand_numbs = $"{X0 % 2}";
                                //textBox4.Text += $"{X0 % 2}";
                                //List<long> listt = new List<long>();
                                //listt.Add(X0);
                                for (int i = 0; i < num * 8 - 1; i++)
                                {
                                    //long Xnext = (X0 * X0) % n;
                                    long Xnext = ((X0 * X0) % n) + (int)Math.Round(ramCounter.NextValue(), 3);
                                    rand_numbs += $"{Xnext % 2}";
                                    X0 = Xnext;
                                    //listt.Add(Xnext);
                                }
                                string byte_str = "";
                                for (int i = 0; i < num * 8; i++)
                                {
                                    byte_str += rand_numbs[i];
                                    if (byte_str.Length % 8 == 0 && byte_str.Length != 0)
                                    {
                                        textBox4.Text += $"{Convert.ToInt32(byte_str, 2)} ";
                                        byte_str = "";
                                    }
                                }
                                ;
                            }
                            else
                            {
                                MessageBox.Show("s % p != 0 && s % q != 0 !");
                            }
                        }
                        else
                        {
                            MessageBox.Show("q % 4 == 3!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("p");
                    }
                }
                else
                {
                    MessageBox.Show("p % 4 == 3!");
                }

            }
            else
            {
                MessageBox.Show("p");
            }
        }
        public static bool isprime(int num)
        {
            for (int i = 2; i < num; i++)
            {
                if (num % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                button3.Visible = true;
                textBox5.Visible = true;
                textBox6.Visible = true;
                textBox7.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label9.Visible = true;
            }
            else
            {
                button3.Visible = false;
                textBox5.Visible = false;
                textBox6.Visible = false;
                textBox7.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label9.Visible = false;
            }
        }
    }
}

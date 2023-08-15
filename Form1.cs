using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
//using System.Numerics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.Dock = DockStyle.Fill;
        }
        byte[] data;
        bool textFile = true;
        private void button4_Click(object sender, EventArgs e)// file for Alice
        {
            if (textBox1.Text.Length > 0)
            {
                MessageBox.Show("У разі вибору файлу, введене повідомлення буде проігноровано");
                textBox1.Text = "";
            }

            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = openFile.FileName;
            string extension = System.IO.Path.GetExtension(fileName);
            if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
            {
                textFile = false;
            }
            else if (extension == ".txt")
            {
                textFile = true;
            }
            else if (extension == ".pdf" || extension == ".cs" || extension == ".c" || extension == ".cpp" || extension == ".h" || extension == ".py" || extension == ".ipynb")
            {
                textFile = true;
            }

            FileInfo fileInfo = new FileInfo(fileName);

            // The byte[] to save the data in
            data = new byte[fileInfo.Length];

            // Load a filestream and put its content into the byte[]
            using (FileStream fs = fileInfo.OpenRead())
            {
                fs.Read(data, 0, data.Length);
            }
            // Delete the temporary file
            //fileInfo.Delete();

        }
        private void button1_Click_1(object sender, EventArgs e)// keys for Alice
        {
            Form2 key_window = new Form2();
            key_window.ShowDialog();
            d = key_window.d_key;
            p = key_window.p_key;
            q = key_window.q_key;
            Key_RC6 = key_window.Key_RC6;
            //button7.BackColor = Color.OliveDrab;
        }
        private void button5_Click(object sender, EventArgs e) // Run
        {
            if (textBox1.Text.Length > 0)
            {
                if (data != null)
                {
                    MessageBox.Show("У разі вибору файлу, введене повідомлення буде проігноровано");
                }
                else
                {
                    data = Encoding.ASCII.GetBytes(textBox1.Text);// change ASCII
                }
            }
            if(data.Length > 0)
            {
                //Key_RC6 = "16181126528151491619213423221724";
                //byte[] RC6_bytes = Encode_RC6(data);
                //
                int plus = 0;
                for (int a = 0; (data.Length + a) % 16 != 0; a++)// ABCD * 4 bytes
                {
                    plus++;
                }
                ;
                byte[] append_data  = new byte[data.Length + plus];
                for (int i = 0; i < data.Length; i++)
                {
                    append_data[i] = data[i];
                }
                ;
                byte[] RC_orig = Encode_RC6_original(append_data);
                ;
                //byte[] dechipher_RC6 = Decode_RC6(RC_orig);
                ;
                List<uint> hash = Run_SHA256(RC_orig);
                List<BigInteger> subscript = RSA(hash);
                
                string result_file = "";
                for(int i = 0; i < subscript.Count; i++)
                {
                    result_file += $"{subscript[i]} ";
                }
                result_file += "\n";
                /*if (textFile)
                //{
                //    // якшо кирилиця, то знов піздєц- будуть символи > 127 (ASCII)
                //    result_file += Encoding.ASCII.GetString(data);
                //}
                //else
                //{*/
                for (int i = 0; i < RC_orig.Length; i++)
                {
                    //result_file += RC6_bytes[i].ToString() + " ";
                    //result_file += data[i].ToString() + " ";
                    result_file += RC_orig[i].ToString() + " ";
                }
                richTextBox1.Text = "";
                richTextBox1.Text += result_file;
                /* //result_file += Encoding.ASCII.GetString(data);
                     // comment out all below
                     //string test1 = chars.ToString();                ;
                     //char[] chars2 = test1.ToCharArray();
                     ;
                     //char[] chars = Encoding.ASCII.GetChars(data);
                     //byte[] test2 = Encoding.ASCII.GetBytes(chars);

                     //string stringg = BitConverter.
                     //byte[] test2 = new byte[stringg.Length];
                     //for (int i = 0; i < stringg.Length; i++)
                     //{
                     //    test2[i] = BitConverter.GetBytes(stringg[i]);
                     //}
                     //;
                 //}*/
                StreamWriter stream_w = new StreamWriter("chipherED.txt");
                stream_w.AutoFlush = true;
                stream_w.Write(result_file);
                stream_w.Close();
                panel1.Visible = false;
                MessageBox.Show("Підписане та зашифроване повідомлення знаходиться у файлі chipherED.txt");
               
            }
            else
            {
                MessageBox.Show("Ви не ввели жодного значення у вміст повідомлення");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            data = null;
            textBox1.Text = string.Empty;
            d = 0;
            p = 0;
            q = 0;
            Key_RC6 = "";
            panel1.Visible = true;
        }
        BigInteger d;
        long p;
        long q;
        List<BigInteger> RSA(List<uint> input_numbers)
        {

            //long p = 7879564093;//10 digits
            //long q = 7879564261;
            //long p = 344683;
            //long q = 55619;
            BigInteger n;
            n = p * q;
            BigInteger e = 2;
            BigInteger phi = (p - 1) * (q - 1);
            while (e < phi)
            {
                if (gcd(e, phi) == 1)
                    break;
                else
                    e++;
            }
            //d = e.modInverse(phi);

            List<BigInteger> chipher = new List<BigInteger>();
            //input_numbers.Clear();
            //input_numbers.Add(125163099);                     
            for (int i = 0; i < input_numbers.Count; i++)
            {
                chipher.Add(((BigInteger)input_numbers[i]).modPow(e, n));
                textBox3.Text = textBox3.Text + $"{chipher[i]} ";
            }

            richTextBox1.Text = textBox1.Text + "\nsubscript:" + textBox3.Text;
            return chipher;
            ;
        }

        //--------------------Bob--------------
        BigInteger d_key;
        long n_Bob;
        string fileName_subs_msg;
        private void button6_Click(object sender, EventArgs e) // file for Bob
        {
            OpenFileDialog openFile = new OpenFileDialog();

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                fileName_subs_msg = openFile.FileName;
                button6.BackColor = Color.OliveDrab;
            }
        }
        private void button7_Click(object sender, EventArgs e)// keys for Bob
        {
            Form2 key_window = new Form2();
            key_window.ShowDialog();
            //MessageBox.Show("ghjkl");
            d_key = key_window.d_key;
            n_Bob = key_window.p_key * key_window.q_key;
            Key_RC6 = key_window.Key_RC6;
            button7.BackColor = Color.OliveDrab;
        }
        private void button2_Click(object sender, EventArgs e) //Run
        {
            StreamReader reader = new StreamReader(fileName_subs_msg);

            string subscript_str = reader.ReadLine();
            string without_subscript = reader.ReadToEnd();
            string[] array_ints = without_subscript.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<byte> bytes_message = new List<byte>();
            for (int i = 0; i < array_ints.Length; i++)
            {
                bytes_message.Add((byte)(int.Parse(array_ints[i])));
            }

            string[] arr = subscript_str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<BigInteger> dechipher = new List<BigInteger>();
            richTextBox2.Text = richTextBox2.Text + "Розшифрована криптограма(підпис): ";
            string for_compare = "";
            
            for (int i = 0; i < arr.Length; i++)
            {
                dechipher.Add(((BigInteger)long.Parse(arr[i])).modPow(d_key, n_Bob));
                richTextBox2.Text = richTextBox2.Text + $"{dechipher[i]} ";
                for_compare = for_compare + $"{dechipher[i]} ";
            }
            richTextBox2.Text = richTextBox2.Text + "\n\nПеревірка\n\n";
            string Bob_hash = "";
            List<uint> uints = Run_SHA256(bytes_message.ToArray());
            for (int i = 0; i < uints.Count; i++)
            {
                Bob_hash = Bob_hash + $"{uints[i]} ";
            }
            byte[] dechipher_RC6 = Decode_RC6(bytes_message.ToArray());

            if (for_compare == Bob_hash)
            {
                richTextBox2.Text = richTextBox2.Text + "\t Хеші збігаються, повідомлення не змінено";

                try
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(dechipher_RC6)))
                    {
                        image.Save("output_image.png", ImageFormat.Png);
                    }

                    MessageBox.Show("Розшифроване повідомлення знаходиться у файлі output_image.png");
                }
                catch (Exception ex)
                {
                    StreamWriter writer = new StreamWriter("output_text.txt");
                    writer.AutoFlush = true;
                    //for (int i = 0; i < dechipher_RC6.Length; i++)
                    //{
                    //}
                    writer.Write(Encoding.ASCII.GetString(dechipher_RC6));
                    writer.Close();
                    MessageBox.Show("Розшифроване повідомлення знаходиться у файлі output_text.txt");
                }

            }
            else
            {
                richTextBox2.Text = richTextBox2.Text + "\t Хеші не збігаються, первинний вміст повідомлення змінено";
            }

            reader.Close();
        }
      

        long mod_pow(long a, long exp, long mod)
        {
             long c;
            if (mod == 1)
                return 0;
            c = 1;
            for (long e_prime = 0; e_prime < exp; e_prime++)
            {
                c = (c * a) % mod;
            }
            return c;
        }
        int modInverse(int a, int n)
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }
        public static BigInteger gcd(BigInteger a, BigInteger h)
        {
            /*
             * This function returns the gcd or greatest common
             * divisor
             */
            BigInteger temp;
            while (true)
            {
                temp = a % h;
                if (temp == 0)
                    return h;
                a = h;
                h = temp;
            }
        }
        static bool isPrime(int n)
        {
            // Corner cases
            if (n <= 1)
            {
                return false;
            }
            if (n <= 3)
            {
                return true;
            }

            // This is checked so that we can skip
            // middle five numbers in below loop
            if (n % 2 == 0 || n % 3 == 0)
            {
                return false;
            }

            for (int i = 5; i * i <= n; i = i + 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /* Iterative Function to calculate (x^n)%p in
        O(logy) */
        static int power(int x, int y, int p)
        {
            int res = 1;     // Initialize result

            x = x % p; // Update x if it is more than or
                       // equal to p

            while (y > 0)
            {
                // If y is odd, multiply x with result
                if (y % 2 == 1)
                {
                    res = (res * x) % p;
                }

                // y must be even now
                y = y >> 1; // y = y/2
                x = (x * x) % p;
            }
            return res;
        }

        // Utility function to store prime factors of a number
        static void findPrimefactors(HashSet<int> s, int n)
        {
            // Print the number of 2s that divide n
            while (n % 2 == 0)
            {
                s.Add(2);
                n = n / 2;
            }

            // n must be odd at this point. So we can skip
            // one element (Note i = i +2)
            for (int i = 3; i <= Math.Sqrt(n); i = i + 2)
            {
                // While i divides n, print i and divide n
                while (n % i == 0)
                {
                    s.Add(i);
                    n = n / i;
                }
            }

            // This condition is to handle the case when
            // n is a prime number greater than 2
            if (n > 2)
            {
                s.Add(n);
            }
        }

        // Function to find smallest primitive root of n
        static int findPrimitive(int n)
        {
            HashSet<int> s = new HashSet<int>();

            // Check if n is prime or not
            if (isPrime(n) == false)
            {
                return -1;
            }

            // Find value of Euler Totient function of n
            // Since n is a prime number, the value of Euler
            // Totient function is n-1 as there are n-1
            // relatively prime numbers.
            int phi = n - 1;

            // Find prime factors of phi and store in a set
            findPrimefactors(s, phi);

            // Check for every number from 2 to phi
            for (int r = 2; r <= phi; r++)
            {
                // Iterate through all prime factors of phi.
                // and check if we found a power with value 1
                bool flag = false;
                foreach (int a in s)
                {

                    // Check if r^((phi)/primefactors) mod n
                    // is 1 or not
                    if (power(r, phi / (a), n) == 1)
                    {
                        flag = true;
                        break;
                    }
                }

                // If there was no power with value 1.
                if (flag == false)
                {
                    return r;
                }
            }

            // If no primitive root found
            return -1;
        }
        public static bool DEchipher = false;
        private void tabControl2_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 0)
            {
                DEchipher = false;
            }
            else
            {
                DEchipher = true;
            }
        }
    }
}

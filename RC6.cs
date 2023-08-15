using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        //public static uint StringToUInt(string strochka)
        //{
        //    uint result;
        //    result = ((uint)strochka[0]);
        //    result += ((uint)strochka[1] << 8);
        //    result += ((uint)strochka[2] << 16);
        //    result += ((uint)strochka[3] << 24);
        //    return result;
        //}

        //public static string UIntToString(uint uinT)
        //{
        //    System.Text.StringBuilder result = new System.Text.StringBuilder();
        //    result.Append((char)((uinT & 0xFF)));
        //    result.Append((char)((uinT >> 8) & 0xFF));
        //    result.Append((char)((uinT >> 16) & 0xFF));
        //    result.Append((char)((uinT >> 24) & 0xFF));
        //    string chay = String.Format("{0:X}", result.ToString());
        //    string Hex = String.Format("{0:X}", uinT);
        //    return chay;
        //}


        static int R = 20;
        static uint[] S = new uint[2 * R + 4];
        const int W_RC6 = 32;
        string Key_RC6;
        const uint P32 = 0xB7E15163;
        const uint Q32 = 0x9E3779B9;
      //  byte[] encoded;


        //private void button2_(object sender, EventArgs e)
        //{
        //    R = int.Parse(textBox4.Text);
        //    S = new uint[2 * R + 4];
        //    Key_RC6 = Encoding.UTF8.GetBytes(textBox2.Text);
        //    Encode_RC6(textBox1.Text);
        //    textBox3.Text = "";
        //    for (int i = 0; i < encoded.Length; i = i + 4)
        //    {
        //        textBox3.Text = textBox3.Text + UIntToString(encoded[i]);
        //        textBox3.Text = textBox3.Text + UIntToString(encoded[i + 1]);
        //        textBox3.Text = textBox3.Text + UIntToString(encoded[i + 2]);
        //        textBox3.Text = textBox3.Text + UIntToString(encoded[i + 3]);
        //    }
        //}
        public byte[] Encode_RC6_original(byte[] text)
        {

            string[] key_parse = Key_RC6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] Key_RC6_bytes = new byte[key_parse.Length];
            for (int i = 0; i < key_parse.Length; i++)
            {
                Key_RC6_bytes[i] = (byte)int.Parse(key_parse[i]);
            }
            //string old_key = "16181126528151491619213423221724";
            //byte[] old_key_byte = Encoding.ASCII.GetBytes(old_key);
            int c = Key_RC6_bytes.Length * 8 / W; //128 / w
            int ii;
            int j;
            uint[] L = new uint[c];

            for (ii = 0; ii < c; ii++)
            {
                L[ii] = BitConverter.ToUInt32(Key_RC6_bytes, ii * 4);
            }
            S[0] = P32;
            for (ii = 1; ii < 2 * R + 4; ii++)
                S[ii] = S[ii - 1] + Q32;
            uint A_, B_;
            A_ = B_ = 0;
            ii = j = 0;
            int V = 3 * Math.Max(c, 2 * R + 4);
            for (int s = 1; s <= V; s++)
            {
                A_ = S[ii] = LeftShift((S[ii] + A_ + B_), 3);
                B_ = L[j] = LeftShift((L[j] + A_ + B_), (int)(A_ + B_));
                ii = (ii + 1) % (2 * R + 4);
                j = (j + 1) % c;
            }

            uint A, B, C, D;
            //for (int a = 0; input_text.Length % 16 != 0; a++)// ABCD * 4 bytes
            //{
            //    input_text += "0";
            //}

            //byte[] text = Encoding.UTF8.GetBytes(input_text);
            byte[] encoded = new byte[text.Length];
            int log = (int)(Math.Log(W, 2));
            for (int i = 0; i < text.Length; i = i + 16)
            {
                A = BitConverter.ToUInt32(text, i);
                B = BitConverter.ToUInt32(text, i + 4);
                C = BitConverter.ToUInt32(text, i + 8);
                D = BitConverter.ToUInt32(text, i + 12);
                //A = StringToUInt(input_text.Substring(i, 4));
                //B = StringToUInt(input_text.Substring(i + 4, 4));
                //C = StringToUInt(input_text.Substring(i + 8, 4));
                //D = StringToUInt(input_text.Substring(i + 12, 4));

                B = B + S[0];
                D = D + S[1];
                for (int k = 1; k <= R; k++)
                {
                    uint t = LeftShift((B * (2 * B + 1)), log);
                    uint u = LeftShift((D * (2 * D + 1)), log);
                    A = (LeftShift((A ^ t), (int)u)) + S[k * 2];
                    C = (LeftShift((C ^ u), (int)t)) + S[k * 2 + 1];
                    uint tem = A;
                    A = B;
                    B = C;
                    C = D;
                    D = tem;
                }
                A = A + S[2 * R + 2];
                C = C + S[2 * R + 3];

                uint[] uints_arr = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(uints_arr);
                block.CopyTo(encoded, i);
            }
            return encoded;
        }
        public byte[] Encode_RC6(byte[] input_text)
        {

            int c = Key_RC6.Length * 8 / W_RC6; //128 / w
            byte[] Key_RC6_bytes = Encoding.ASCII.GetBytes(Key_RC6);
            int ii;
            int j;
            uint[] L = new uint[c];
            for (ii = 0; ii < c; ii++)
            {
                L[ii] = BitConverter.ToUInt32(Key_RC6_bytes, ii * 4);
            }
            S[0] = P32;
            for (ii = 1; ii < 2 * R + 4; ii++)
                S[ii] = S[ii - 1] + Q32;
            uint A_, B_;
            A_ = B_ = 0;
            ii = j = 0;
            int V = 3 * Math.Max(c, 2 * R + 4);
            for (int s = 1; s <= V; s++)
            {
                A_ = S[ii] = LeftShift((S[ii] + A_ + B_), 3);
                B_ = L[j] = LeftShift((L[j] + A_ + B_), (int)(A_ + B_));
                ii = (ii + 1) % (2 * R + 4);
                j = (j + 1) % c;
            }

            uint A, B, C, D;
            List<byte> text = new List<byte>();
            for (int i = 0; i < input_text.Length; i++)
            {
                text.Add(input_text[i]);
            }
            for (int a = 0; text.Count % 16 != 0; a++)// ABCD * 4 bytes
            {
                text.Add(0);
            }

            byte[] text12 = text.ToArray();
            byte[] encoded = new byte[text12.Length];
            int log = (int)(Math.Log(W_RC6, 2));
            for (int i = 0; i < text12.Length; i = i + 16)
            {
                A = BitConverter.ToUInt32(text12, i);
                B = BitConverter.ToUInt32(text12, i + 4);
                C = BitConverter.ToUInt32(text12, i + 8);
                D = BitConverter.ToUInt32(text12, i + 12);


                B = B + S[0];
                D = D + S[1];
                for (int k = 1; k <= R; k++)
                {
                    uint t = LeftShift((B * (2 * B + 1)), log);
                    uint u = LeftShift((D * (2 * D + 1)), log);
                    A = (LeftShift((A ^ t), (int)u)) + S[k * 2];
                    C = (LeftShift((C ^ u), (int)t)) + S[k * 2 + 1];
                    uint tem = A;
                    A = B;
                    B = C;
                    C = D;
                    D = tem;
                }
                A = A + S[2 * R + 2];
                C = C + S[2 * R + 3];

                uint[] uints_arr = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(uints_arr);
                block.CopyTo(encoded, i);
            }
            return text12;
        }
       
        public byte[] Decode_RC6(byte[] cipherText)
        {
            string[] key_parse = Key_RC6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] Key_RC6_bytes = new byte[key_parse.Length];
            for (int a = 0; a < key_parse.Length; a++)
            {
                Key_RC6_bytes[a] = (byte)int.Parse(key_parse[a]);
            }
            int c = Key_RC6_bytes.Length * 8 / W; //128 / w
            int ii;
            int jj;
            uint[] L = new uint[c];
           // byte[] Key_RC6_bytes = Encoding.ASCII.GetBytes(Key_RC6);
            for (ii = 0; ii < c; ii++)
            {
                L[ii] = BitConverter.ToUInt32(Key_RC6_bytes, ii * 4);
            }
            S[0] = P32;
            for (ii = 1; ii < 2 * R + 4; ii++)
                S[ii] = S[ii - 1] + Q32;
            uint A_, B_;
            A_ = B_ = 0;
            ii = jj = 0;
            int V = 3 * Math.Max(c, 2 * R + 4);
            for (int s = 1; s <= V; s++)
            {
                A_ = S[ii] = LeftShift((S[ii] + A_ + B_), 3);
                B_ = L[jj] = LeftShift((L[jj] + A_ + B_), (int)(A_ + B_));
                ii = (ii + 1) % (2 * R + 4);
                jj = (jj + 1) % c;
            }

            uint A, B, C, D;
            int i;
            byte[] input_text = new byte[cipherText.Length];
            for (i = 0; i < cipherText.Length; i = i + 16)
            {
                A = BitConverter.ToUInt32(cipherText, i);
                B = BitConverter.ToUInt32(cipherText, i + 4);
                C = BitConverter.ToUInt32(cipherText, i + 8);
                D = BitConverter.ToUInt32(cipherText, i + 12);
                C = C - S[2 * R + 3];
                A = A - S[2 * R + 2];
                for (int j = R; j >= 1; j--)
                {
                    uint temp = D;
                    D = C;
                    C = B;
                    B = A;
                    A = temp;
                    uint u = LeftShift((D * (2 * D + 1)), (int)Math.Log(W, 2));
                    uint t = LeftShift((B * (2 * B + 1)), (int)Math.Log(W, 2));
                    C = RightShift((C - S[2 * j + 1]), (int)t) ^ u;
                    A = RightShift((A - S[2 * j]), (int)u) ^ t;
                }
                D = D - S[1];
                B = B - S[0];

                uint[] uints_arr = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(uints_arr);

                block.CopyTo(input_text, i);
            }
            return input_text;
        }

 //private static byte[] ToArrayBytes(uint[] uints)
        //{
        //    byte[] arrayBytes = new byte[16];
        //    for (int i = 0; i < 4; i++)
        //    {
        //        byte[] temp = BitConverter.GetBytes(uints[i]);
        //        temp.CopyTo(arrayBytes, i * 4);
        //    }
        //    return arrayBytes;
        //}
        //private static uint RightShift(uint value, int shift)
        //{
        //    return (value >> shift) | (value << (W - shift));
        //}

        //private static uint LeftShift(uint value, int shift)
        //{
        //    return (value << shift) | (value >> (W - shift));
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    ;
        //    //for (int a = 0; textBox3.Text.Length % 16 != 0; a++)// 128/8 = 16
        //    //{
        //    //    textBox3.Text += "\0";
        //    //}
        //    ;

        //    //byte[] arr =  DecodeRc6(Encoding.UTF8.GetBytes(textBox3.Text));
        //    textBox3.Text = "";
        //    byte[] arr = Decode_RC6(encoded);
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        textBox1.Text += UIntToString(arr[i]);
        //    }
        //}
    }
}

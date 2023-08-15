using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static uint StringToUInt(string strochka)
        {
            uint result;
            result = ((uint)strochka[0]);
            result += ((uint)strochka[1] << 8);
            result += ((uint)strochka[2] << 16);
            result += ((uint)strochka[3] << 24);
            return result;
        }

        public static string UIntToString(uint uinT)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append((char)((uinT & 0xFF)));
            result.Append((char)((uinT >> 8) & 0xFF));
            result.Append((char)((uinT >> 16) & 0xFF));
            result.Append((char)((uinT >> 24) & 0xFF));
            string chay = String.Format("{0:X}", result.ToString());
            string Hex = String.Format("{0:X}", uinT);
            return chay;
        }

        unsafe public static byte[] GetBytesBE(long value)
        {
            return GetBytesSwap(BitConverter.IsLittleEndian, (byte*)&value, 8);
        }
        unsafe static byte[] GetBytesSwap(bool swap, byte* ptr, int count)
        {
            byte[] ret = new byte[count];

            if (swap)
            {
                int t = count - 1;
                for (int i = 0; i < count; i++)
                {
                    ret[t - i] = ptr[i];
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    ret[i] = ptr[i];
                }
            }
            return ret;
        }
        private List<uint> Run_SHA256(byte[] init_bytes)
        {
            byte[] input = new byte[init_bytes.Length + 1];
            init_bytes.CopyTo(input, 0);
            input[input.Length - 1] = (byte)128; // 1000 0000
            int size = input.Length;
            for (; size % 64 != 0; size++) // 1 byte == 8 bit
            {

            }
            size = size - 8;
            Int64 L = (Int64)init_bytes.Length;
            byte[] changed_input = new byte[size + 8];
            input.CopyTo(changed_input, 0);
            byte[] got_bytes = GetBytesBE(L);
            got_bytes.CopyTo(changed_input, changed_input.Length - 1 - 8);/// 454 елемент- 10 чогось 
            return Encode(changed_input);

        }
        static void reverseArray(byte[] arr, int start,
                                        int end)
        {
            while (start < end)
            {
                byte temp = arr[start];
                arr[start] = arr[end];
                arr[end] = temp;
                start++;
                end--;
            }
        }

        // Function to right rotate
        // arr[] of size n by d
        static void rightRotate(byte[] arr, int d, int n)
        {
            reverseArray(arr, 0, n - 1);
            reverseArray(arr, 0, d - 1);
            reverseArray(arr, d, n - 1);
        }
        public List<uint> Encode(byte[] input_text)
        {
            UInt32 h0 = 0x6a09e667;
            UInt32 h1 = 0xbb67ae85;
            UInt32 h2 = 0x3c6ef372;
            UInt32 h3 = 0xa54ff53a;
            UInt32 h4 = 0x510e527f;
            UInt32 h5 = 0x9b05688c;
            UInt32 h6 = 0x1f83d9ab;
            UInt32 h7 = 0x5be0cd19;
            UInt32[] K = new UInt32[] {0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
   0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
   0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
   0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
   0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
   0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
   0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
   0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2};
            List<UInt32> hash = new List<UInt32>();
            for (int i = 0; i < input_text.Length; i = i + 64)
            {
                byte[] working = new byte[64];
                // input_text.CopyTo(working, i);
                Array.Copy(input_text, i, working, 0, working.Length);
                UInt32[] w = new UInt32[64];
                for (int ii = 0, counter = 0; ii < 16; ii++, counter = counter + 4)
                {
                    w[ii] = BitConverter.ToUInt32(working, counter);

                }
                for (int k = 16; k < 64; k++)
                {
                    //rightRotate(BitConverter.GetBytes(w[a - 15]), 7, 4);
                    UInt32 s0 = (RightShift(w[k - 15], 7)) ^ (RightShift(w[k - 15], 18)) ^ RightShift(w[k - 15], 3);
                    UInt32 s1 = (RightShift(w[k - 2], 17)) ^ (RightShift(w[k - 2], 19)) ^ RightShift(w[k - 2], 10);
                    w[k] = w[k - 16] + s0 + w[k - 7] + s1;
                }
                UInt32 a = h0;
                UInt32 b = h1;
                UInt32 c = h2;
                UInt32 d = h3;
                UInt32 e = h4;
                UInt32 f = h5;
                UInt32 g = h6;
                UInt32 h = h7;
                for (int k = 0; k < 64; k++)
                {
                    UInt32 S1 = RightShift(e, 6) ^ RightShift(e, 11) ^ RightShift(e, 25);
                    // https://stackoverflow.com/questions/37881537/c-sharp-not-bit-wise-operator-returns-negative-values
                    UInt32 ch = (e & f) ^ (((byte)~e) & g);
                    UInt32 temp1 = h + S1 + ch + K[k] + w[k];
                    UInt32 S0 = RightShift(a, 2) ^ RightShift(a, 13) ^ RightShift(a, 22);
                    UInt32 maj = (a & b) ^ (a & c) ^ (b & c);
                    UInt32 temp2 = S0 + maj;
                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;

                }
                h0 = h0 + a;
                h1 = h1 + b;
                h2 = h2 + c;
                h3 = h3 + d;
                h4 = h4 + e;
                h5 = h5 + f;
                h6 = h6 + g;
                h7 = h7 + h;
            }
            hash.Add(h0);
            hash.Add(h1);
            hash.Add(h2);
            hash.Add(h3);
            hash.Add(h4);
            hash.Add(h5);
            hash.Add(h6);
            hash.Add(h7);
            string result = "";
            for (int i = 0; i < hash.Count; i++)
            {
                result = result + hash[i].ToString();
            }
            StreamWriter streamm = new StreamWriter("hash.txt");

            streamm.Write(result);
            streamm.Close();
            return hash;
        }
        private static byte[] ToArrayBytes(uint[] uints)
        {
            byte[] arrayBytes = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                byte[] temp = BitConverter.GetBytes(uints[i]);
                temp.CopyTo(arrayBytes, i * 4);
            }
            return arrayBytes;
        }


        static int W = 32;
        private static uint RightShift(uint value, int shift)
        {
            return (value >> shift) | (value << (W - shift));
        }

        private static uint LeftShift(uint value, int shift)
        {
            return (value << shift) | (value >> (W - shift));
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace Toolbox
{
    public partial class HashCalculatorForm : STForm
    {
        private bool IsHex => chkUseHex.Checked;

        private bool IsLittleEndian => chkLittleEndian.Checked;

        public HashCalculatorForm()
        {
            InitializeComponent();

            hashTypeCB.Items.Add("NLG_Hash");
            hashTypeCB.Items.Add("FNV64A1");
            hashTypeCB.Items.Add("CRC32");
            hashTypeCB.Items.Add("BCSV");
            hashTypeCB.Items.Add("SARC");
            hashTypeCB.Items.Add("MMH3");

            hashTypeCB.SelectedIndex = 0;

            maxLengthUD.Value = 3;
        }

        private void stTextBox1_TextChanged(object sender, EventArgs e) {
            UpdateHash();
        }

        private void UpdateHash()
        {
            dynamic Hash = CalculateHash(hashTypeCB.GetSelectedText(), stringTB.Text);
            if (IsHex)
                resultTB.Text = IsLittleEndian ? LittleEndian(Hash) : Hash.ToString("X");
            else
                resultTB.Text = Hash.ToString();
        }

        static string LittleEndian(dynamic number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        private void chkUseHex_CheckedChanged(object sender, EventArgs e) {
            UpdateHash();
        }

        public static dynamic CalculateHash(string type, string text)
        {
            if (type == "NLG_Hash")
                return StringToHash(text);
            else if (type == "FNV64A1")
                return FNV64A1.Calculate(text);
            else if (type == "CRC32")
                return Toolbox.Library.Security.Cryptography.Crc32.Compute(text);
            else if (type == "BCSV")
                return stringToHash(text);
            else if (type == "SARC")
                return NameHash(text);
            else if (type == "MMH3")
                return Toolbox.Library.Security.Cryptography.MurMurHash3.Hash(text);
            return 0;
        }

        public static uint stringToHash(string name)
        {
            int hash = 0;
            for (int i = 0; i < name.Length; i++)
            {
                hash *= 0x1F;
                hash += name[i];
            }

            return (uint)hash;
        }

        static uint NameHash(string name)
        {
            uint result = 0;
            for (int i = 0; i < name.Length; i++)
            {
                result = name[i] + result * 0x00000065;
            }
            return result;
        }

        public static uint StringToHash(string name, bool caseSensative = false)
        {
            //From (Works as tested comparing hashbin strings/hashes
            //https://gist.github.com/RoadrunnerWMC/f4253ef38c8f51869674a46ee73eaa9f
            byte[] data = Encoding.Default.GetBytes(name);

            int h = -1;
            for (int i = 0; i < data.Length; i++)
            {
                int c = (int)data[i];
                if (caseSensative && ((c - 65) & 0xFFFFFFFF) <= 0x19)
                    c |= 0x20;

                h = (int)((h * 33 + c) & 0xFFFFFFFF);
            }

            return (uint)h;
        }

        private bool CancelOperation = false;

        private bool UseSpeialcase => true;
        private bool UseNumbered => chkSearchNumbered.Checked;
        private bool UseLowercase => true;
        private bool UseUppercase => searchUppercase.Checked;

        private char[] SetupCharacters()
        {
            List<char> chars = new List<char>();
            if (UseLowercase)
                chars.AddRange(CharacterSetLower);
        //    if (UseUppercase)
        //        chars.AddRange(CharacterSetUpper);
         //   if (UseSpeialcase)
         //       chars.AddRange(Special);
            return chars.ToArray();
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            STProgressBar progressBar = new STProgressBar();
            progressBar.FormClosed += OnProgressBarExist;
            progressBar.Show();
            progressBar.Task = $"Searching characters";

            string hashType = hashTypeCB.GetSelectedText();

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                BruteForceHashes(progressBar, hashType);
            }));
            Thread.Start();
        }

        private void BruteForceHashes(STProgressBar progressBar, string hashType)
        {
            if (bruteForceHashTB.Text.Length == 0)
                return;

            var characterSet = SetupCharacters();

            CancelOperation = false;

            List<ulong> hashes = new List<ulong>();
            foreach (var line in bruteForceHashTB.Lines)
            {
                ulong hash = 0;
                ulong.TryParse(line, out hash);
                if (hash == 0) continue;

                hashes.Add(hash);
            }

            if (hashes.Count == 0) return;



            int maxLength = (int)maxLengthUD.Value;

            char lastChar = 'z';
            var firstChar = characterSet.FirstOrDefault();

            int charIndex = 0;
            bool useSpace = true;
            for (int length = 1; length <= maxLength; ++length)
            {
                UpdateProgressbar(progressBar, length, maxLength);

                StringBuilder Sb = new StringBuilder(new String(' ', length));
                while (true && CancelOperation == false && hashes.Count > 0)
                {
                    String value = Sb.ToString();

                    ulong calculatedHash = CalculateHash(hashType, $"{characterStartTB.Text}{value}");
                    if (hashes.Contains(calculatedHash))
                    {
                        UpdateTextbox($"{characterStartTB.Text}{value}");
                        hashes.Remove(calculatedHash);

                        if (hashes.Count == 0)
                        {
                            if (progressBar.InvokeRequired)
                            {
                                progressBar.Invoke((MethodInvoker)delegate {
                                    progressBar.Close();
                                });
                            }
                            return;
                        }
                    }

                    foreach (var line in characterStartTB.Lines)
                    {
                        ulong calculatedHash2 = CalculateHash(hashType, $"{line}{value}");
                        if (hashes.Contains(calculatedHash2))
                        {
                            UpdateTextbox($"{line}{value}");
                            hashes.Remove(calculatedHash2);

                            if (hashes.Count == 0)
                            {
                                if (progressBar.InvokeRequired)
                                {
                                    progressBar.Invoke((MethodInvoker)delegate {
                                        progressBar.Close();
                                    });
                                }
                                return;
                            }
                        }
                    }

                    if (value.All(item => item == lastChar))
                        break;

                    // Add one: aaa -> aab -> ... aaz -> aba -> ... -> zzz
                    for (int i = length - 1; i >= 0; --i)
                        if (Sb[i] == ' ')
                        {
                            Sb[i] = '/';
                            break;
                        }
                        else if (Sb[i] == '/')
                        {
                            if (UseNumbered)
                                Sb[i] = '0';
                            else
                            {
                                if (UseUppercase)
                                    Sb[i] = 'A';
                                else
                                    Sb[i] = 'a';
                            }
                            break;
                        }
                        else if (Sb[i] == '0') { Sb[i] = '1'; break; }
                        else if (Sb[i] == '1') { Sb[i] = '2'; break; }
                        else if (Sb[i] == '2') { Sb[i] = '3'; break; }
                        else if (Sb[i] == '3') { Sb[i] = '4'; break; }
                        else if (Sb[i] == '4') { Sb[i] = '5'; break; }
                        else if (Sb[i] == '5') { Sb[i] = '6'; break; }
                        else if (Sb[i] == '6') { Sb[i] = '7'; break; }
                        else if (Sb[i] == '7') { Sb[i] = '8'; break; }
                        else if (Sb[i] == '8') { Sb[i] = '9'; break; }
                        else if (Sb[i] == '9')
                        {
                            if (UseUppercase)
                                Sb[i] = 'A';
                            else
                                Sb[i] = 'a';
                            break;
                        }
                        else if (Sb[i] == 'Z')
                        {
                            Sb[i] = 'a';
                            break;
                        }
                        else if (Sb[i] != lastChar)
                        {
                            Sb[i] = (Char)(Sb[i] + 1);
                            break;
                        }
                        else
                        {
                            Sb[i] = ' ';
                        }
                }
            }

            progressBar.Close();
        }

        private void UpdateProgressbar(STProgressBar progressBar, int length, int maxLength)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((MethodInvoker)delegate {
                    progressBar.Task = $"Searching characters {length} of max {maxLength}";
                    progressBar.Value = ((length * 100) / maxLength);
                    progressBar.Refresh();
                });
            }
        }

        private void UpdateTextbox(string text)
        {
            if (bruteForceStringTB.InvokeRequired)
            {
                bruteForceStringTB.Invoke((MethodInvoker)delegate {
                    if (bruteForceStringTB.Text.Length == 0)
                        bruteForceStringTB.Text = text;
                    else
                        bruteForceStringTB.AppendText("\r\n" + text);
                });
            }
            else
                bruteForceStringTB.AppendText(text);
        }

        private char[] Special = new char[]
        {  ' ', };

        private char[] Digits = new char[]
        {  '0','1','2','3','4','5','6','7','8','9' };

        private char[] CharacterSetLower = new char[]
        { 'a','b','c','d','e','f','g','h','i','j',
          'k','l','m','n','o','p','q','r','s',
          't','u','v','w', 'x', 'y', 'z'};

        private char[] CharacterSetUpper = new char[]
        { 'A','B','C','D','E','F','G','H','I','J',
          'K','L','M','N','O','P','Q','R','S',
          'T','U','V','W', 'X', 'Y', 'Z'};

        private void OnProgressBarExist(object sender, EventArgs e)
        {
            CancelOperation = true;
        }

        private void hashTypeCB_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateHash();
        }

        private void chkLittleEndian_CheckedChanged(object sender, EventArgs e) {
            UpdateHash();
        }
    }
}

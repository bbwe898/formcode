using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Xml;
using System.Collections;

namespace formCode
{
    public partial class Form1 : Form
    {
        private XmlDocument xmlDoc;
        private XmlNode rootNode;
        Configuration config;
        private ConfigSectionDate regDate1;
        private ConfigSectionDate regDate2;
        private ConfigSectionDate regDate3;

        [DllImport("user32.dll")]
        public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk); //API 
        [DllImport("user32.dll")]
        public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 vk); //API 

        public Form1()
        {
            InitializeComponent();
            try
            {
                RegisterHotKey(this.Handle, 247696412, 2, (UInt32)Keys.D1); //加一行
                RegisterHotKey(this.Handle, 247696413, 2, (UInt32)Keys.D2); //行数变两倍
                RegisterHotKey(this.Handle, 247696414, 2, (UInt32)Keys.D3); //行数变三倍

                RegisterHotKey(this.Handle, 247696419, 2, (UInt32)Keys.D4); //串为单行
                RegisterHotKey(this.Handle, 247696420, 2, (UInt32)Keys.D5); //串为多组

                RegisterHotKey(this.Handle, 247696421, 2, (UInt32)Keys.D6); //拆解多组
                RegisterHotKey(this.Handle, 247696422, 2, (UInt32)Keys.D7); //拆解单行

                RegisterHotKey(this.Handle, 247696416, 2, (UInt32)Keys.D8); //复制
                RegisterHotKey(this.Handle, 247696415, 2, (UInt32)Keys.D9); //粘贴
                RegisterHotKey(this.Handle, 247696417, 2, (UInt32)Keys.F12); //HtmlEncode()
                RegisterHotKey(this.Handle, 247696423, 2, (UInt32)Keys.F11); //HtmlDecode()
                RegisterHotKey(this.Handle, 247696418, 2, (UInt32)Keys.D0); //正则替换
                RegisterHotKey(this.Handle, 247696424, 2, (UInt32)Keys.F7); //小写
                RegisterHotKey(this.Handle, 247696425, 2, (UInt32)Keys.F8); //大写
                RegisterHotKey(this.Handle, 247696426, 2, (UInt32)Keys.F9); //首字母大写

                RegisterHotKey(this.Handle, 247696427, 2, (UInt32)Keys.F4); //复制生成字串

                RegisterHotKey(this.Handle, 247696428, 2, (UInt32)Keys.F6); //生成txt表格

                skinEngine1.SkinFile = "DiamondBlue.ssk";
                xmlDoc = new XmlDocument();
                xmlDoc.Load("regex.xml");
                rootNode = xmlDoc.SelectSingleNode("/regex");
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                regDate1 = (ConfigSectionDate)config.Sections.Get("regex1");
                regDate2 = (ConfigSectionDate)config.Sections.Get("regex2");
                regDate3 = (ConfigSectionDate)config.Sections.Get("regex3");
                if (null != regDate1)
                {
                    comboBox1.Text = regDate1.Regex;
                    richTextBox2.Text = regDate1.Replace;
                }
                if (null != regDate2)
                {
                    comboBox2.Text = regDate2.Regex;
                    richTextBox3.Text = regDate2.Replace;
                }
                if (null != regDate3)
                {
                    comboBox3.Text = regDate3.Regex;
                    richTextBox4.Text = regDate3.Replace;
                }

                foreach (XmlElement i in rootNode.ChildNodes)
                {
                    comboBox1.Items.Add(i.GetAttribute("regex"));
                    comboBox2.Items.Add(i.GetAttribute("regex"));
                    comboBox3.Items.Add(i.GetAttribute("regex"));
                }
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, (UInt32)Keys.D1); //加一行
            UnregisterHotKey(this.Handle, (UInt32)Keys.D2); //行数变两倍
            UnregisterHotKey(this.Handle, (UInt32)Keys.D3); //行数变三倍

            UnregisterHotKey(this.Handle, (UInt32)Keys.D4); //串为单行
            UnregisterHotKey(this.Handle, (UInt32)Keys.D5); //串为多组

            UnregisterHotKey(this.Handle, (UInt32)Keys.D6); //拆解多组
            UnregisterHotKey(this.Handle, (UInt32)Keys.D7); //拆解单行

            UnregisterHotKey(this.Handle, (UInt32)Keys.D8); //复制
            UnregisterHotKey(this.Handle, (UInt32)Keys.D9); //粘贴
            UnregisterHotKey(this.Handle, (UInt32)Keys.F12); //HtmlEncode()
            UnregisterHotKey(this.Handle, (UInt32)Keys.F11); //HtmlDecode()
            UnregisterHotKey(this.Handle, (UInt32)Keys.D0); //正则替换

            UnregisterHotKey(this.Handle, (UInt32)Keys.F4); //复制生成字串

            UnregisterHotKey(this.Handle, (UInt32)Keys.F6); //生成txt表格
        }

        //重写消息循环 
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312; // m.WParam.ToInt32() 要和 注册热键时的第2个参数一样
            try
            {
                //复制
                if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696416)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                    }
                }
                //正则替换
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696418)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        button1_Click(this, new EventArgs());
                        SendKeys.SendWait("^v");
                    }
                }
                //生成txt表格
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696428)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        string[] seps = new string[] { "\t", "\n" };
                        string[] arrStr = richTextBox1.Text.Split(seps, StringSplitOptions.None);
                        int colNum = 1;
                        for (int i = 0; !richTextBox1.Text[i].Equals('\n'); i++)
                        {
                            if (richTextBox1.Text[i].Equals('\t'))
                            {
                                colNum++;
                            }
                        }
                        int rowNum = arrStr.Length / colNum;
                        int[] lenOfCol = new int[colNum];
                        int[] lenOfCell = new int[arrStr.Length];
                        for (int i = 0; i < rowNum; i++)
                        {
                            for (int j = 0; j < colNum - 1; j++)
                            {
                                for (int k = 0; k < arrStr[(i * colNum) + j].Length; k++)
                                {
                                    lenOfCell[(i * colNum) + j]++;
                                    if (127 < arrStr[(i * colNum) + j][k])
                                    {
                                        lenOfCell[(i * colNum) + j]++;
                                    }
                                }
                                if (lenOfCell[(i * colNum) + j] > lenOfCol[j])
                                {
                                    lenOfCol[j] = lenOfCell[(i * colNum) + j];
                                }
                            }
                        }

                        StringBuilder resStr = new StringBuilder();
                        for (int i = 0; i < rowNum; i++)
                        {
                            for (int j = 0; j < colNum - 1; j++)
                            {
                                resStr.Append(arrStr[(i * colNum) + j]);
                                for (int k = 0; k < ((lenOfCol[j] - lenOfCell[(i * colNum) + j]) / 2) + 1; k++)
                                {
                                    resStr.Append("　");
                                }
                                if (0 != (lenOfCol[j] - lenOfCell[(i * colNum) + j]) % 2)
                                {
                                    resStr.Append(" ");
                                }
                            }
                            resStr.Append(arrStr[(i * colNum) + (colNum - 1)] + "\n");
                        }
                        richTextBox1.Text = resStr.ToString();
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //HtmlEncode()
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696417)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        richTextBox1.Text = HttpUtility.HtmlEncode(richTextBox1.Text);
                        if (checkBox1.Checked)
                        {
                            richTextBox1.Text = richTextBox1.Text.Replace(" ", "&#160;");
                            richTextBox1.Text = richTextBox1.Text.Replace("\t", "&#160;&#160;&#160;");
                        }
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //HtmlDecode()
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696423)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        richTextBox1.Text = richTextBox1.Text.Replace("&#160;", " ");
                        richTextBox1.Text = HttpUtility.HtmlDecode(richTextBox1.Text);
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //小写
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696424)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        richTextBox1.Text = richTextBox1.Text.ToLower();
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //大写
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696425)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        richTextBox1.Text = richTextBox1.Text.ToUpper();
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //首字母大写
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696426)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);

                        if (!richTextBox1.Text.Equals(""))
                        {
                            char[] sep = new char[] { '\n' };
                            string[] ress = richTextBox1.Text.Split(sep, StringSplitOptions.None);

                            richTextBox1.Text = "";
                            Regex r = new Regex("^\\s*\\w{1}");
                            Match mat = null;
                            foreach (string str in ress)
                            {
                                mat = r.Match(str);
                                if (mat.Success)
                                {
                                    richTextBox1.Text += mat.Value.ToUpper() + str.Substring(mat.Index + mat.Length) + "\n";
                                }
                                else
                                {
                                    richTextBox1.Text += str + "\n";
                                }
                            }
                            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                            Clipboard.SetDataObject(richTextBox1.Text, true);
                            SendKeys.SendWait("^v");
                        }
                    }
                }
                //粘贴
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696415)
                {
                    if (this.richTextBox1.Text != "")
                    {
                        // SetDataObject(Object obj,bool copy)方法将数据放置在剪贴板上
                        // 参数obj指要放置的数据对象
                        // 参数copy指当程序退出时数据是否仍然保存在剪贴板上
                        Clipboard.SetDataObject(richTextBox1.Text, true);
                        SendKeys.SendWait("^v");
                    }
                }
                //加一行
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696412)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            char[] sep = new char[] { '\n' };
                            string[] ress = richTextBox1.Text.Split(sep);

                            string pre = "";
                            richTextBox1.Text = "";
                            for (int i = 0; i < ress.Length; i++)
                            {
                                if (ress.Length - 1 == i)
                                {
                                    if (!pre.Equals(ress[i]) && 0 != i)
                                        richTextBox1.Text += pre + "\n";
                                    richTextBox1.Text += ress[i] + "\n";
                                }
                                else if (0 == i)
                                {
                                    pre = ress[i];
                                }
                                else if (!pre.Equals(ress[i]))
                                {
                                    richTextBox1.Text += pre + "\n";
                                    pre = ress[i];
                                }
                                richTextBox1.Text += ress[i] + "\n";
                            }
                            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                            Clipboard.SetDataObject(richTextBox1.Text, true);
                            SendKeys.SendWait("^v");
                        }
                    }
                }
                //行数变两倍
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696413)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            char[] sep = new char[] { '\n' };
                            string[] ress = richTextBox1.Text.Split(sep);
                            richTextBox1.Text = "";
                            foreach (string s in ress)
                            {
                                richTextBox1.Text += s + "\n" + s + "\n"; ;
                            }
                            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                            Clipboard.SetDataObject(richTextBox1.Text, true);
                            SendKeys.SendWait("^v");
                        }
                    }
                }
                //行数变三倍
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696414)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        //res
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            char[] sep = new char[] { '\n' };
                            string[] ress = richTextBox1.Text.Split(sep);
                            richTextBox1.Text = "";
                            foreach (string s in ress)
                            {
                                richTextBox1.Text += s + "\n" + s + "\n" + s + "\n"; ;
                            }
                            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                            Clipboard.SetDataObject(richTextBox1.Text, true);
                            SendKeys.SendWait("^v");
                        }
                    }
                }
                //串为单行
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696419)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            try
                            {
                                char[] sep = new char[] { '\n' };
                                string[] ress = richTextBox1.Text.Split(sep);

                                int sameLineNum = 1;
                                double[] douarr = new double[ress.Length];
                                richTextBox1.Text = "";
                                for (int i = 0; i < ress.Length; i++)
                                {
                                    douarr[i] = lcs(ress[0], ress[i]);
                                }
                                double total = 0;
                                foreach (double d in douarr)
                                    total += d;
                                double avg = total / douarr.Length;
                                for (int i = 0; i < ress.Length; i++)
                                {
                                    if (douarr[i] < avg)
                                    {
                                        sameLineNum = i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < sameLineNum; i++)
                                {
                                    for (int j = 0; i + sameLineNum * j < ress.Length; j++)
                                        richTextBox1.Text += ress[i + sameLineNum * j];
                                    richTextBox1.Text += "\n";
                                }
                                richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                                Clipboard.SetDataObject(richTextBox1.Text, true);
                                SendKeys.SendWait("^v");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.StackTrace);
                            }
                        }
                    }
                }

                //串为单行
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696420)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            try
                            {
                                char[] sep = new char[] { '\n' };
                                string[] ress = richTextBox1.Text.Split(sep);

                                int sameLineNum = 1;
                                double[] douarr = new double[ress.Length];
                                richTextBox1.Text = "";
                                for (int i = 0; i < ress.Length; i++)
                                {
                                    douarr[i] = lcs(ress[0], ress[i]);
                                }
                                double total = 0;
                                foreach (double d in douarr)
                                    total += d;
                                double avg = total / douarr.Length;
                                for (int i = 0; i < ress.Length; i++)
                                {
                                    if (douarr[i] < avg)
                                    {
                                        sameLineNum = i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < sameLineNum; i++)
                                {
                                    for (int j = 0; i + sameLineNum * j < ress.Length; j++)
                                        richTextBox1.Text += ress[i + sameLineNum * j] + "\n";
                                    //richTextBox1.Text += "\n";
                                }
                                richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                                Clipboard.SetDataObject(richTextBox1.Text, true);
                                SendKeys.SendWait("^v");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.StackTrace);
                            }
                        }
                    }
                }
                //拆解多组
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696421)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            try
                            {
                                char[] sep = new char[] { '\n' };
                                string[] ress = richTextBox1.Text.Split(sep);

                                int sameLineNum = 1;
                                double[] douarr = new double[ress.Length];
                                richTextBox1.Text = "";
                                for (int i = 0; i < ress.Length; i++)
                                {
                                    douarr[i] = lcs(ress[0], ress[i]);
                                }
                                double total = 0;
                                foreach (double d in douarr)
                                    total += d;
                                double avg = total / douarr.Length;
                                for (int i = 1; i < ress.Length; i++)
                                {
                                    if (douarr[i] > avg)
                                    {
                                        sameLineNum = i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < sameLineNum; i++)
                                {
                                    for (int j = 0; i + sameLineNum * j < ress.Length; j++)
                                        richTextBox1.Text += ress[i + sameLineNum * j] + "\n";
                                    //richTextBox1.Text += "\n";
                                }
                                richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                                Clipboard.SetDataObject(richTextBox1.Text, true);
                                SendKeys.SendWait("^v");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.StackTrace);
                            }
                        }
                    }
                }
                //拆解单行
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696422)
                {
                    SendKeys.SendWait("^c");
                    // GetDataObject检索当前剪贴板上的数据
                    IDataObject iData = Clipboard.GetDataObject();

                    // 将数据与指定的格式进行匹配，返回bool
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        // GetData检索数据并指定一个格式
                        richTextBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        if (!richTextBox1.Text.Equals(""))
                        {
                            try
                            {
                                char[] sep = new char[] { '\n' };
                                string[] ress = richTextBox1.Text.Split(sep);

                                string comStr = "";
                                lcs(ress[0], ress[1], ref comStr);
                                if (comStr.Equals(""))
                                {
                                    throw new Exception("没有相同字串");
                                }
                                richTextBox1.Text = "";
                                foreach (string s in ress)
                                {
                                    richTextBox1.Text += s.Replace(comStr, "") + "\n";
                                }
                                richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
                                Clipboard.SetDataObject(richTextBox1.Text, true);
                                SendKeys.SendWait("^v");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                        }
                    }
                }
                //复制生成字串
                else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696427)
                {
                    button5_Click(this, new EventArgs());
                    SendKeys.SendWait("^v");
                }
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }

        //返回最长子串的比例
        private double lcs(string str1, string str2, ref string comStr)
        {
            if ((String.IsNullOrEmpty(str1)) || (String.IsNullOrEmpty(str2)))
            {
                return 0;
            }

            int[] maxtix = new int[str1.Length];
            int maxLength = 0;
            int end = 0;

            for (int i = 0; i < str2.Length; i++)
            {
                for (int j = str1.Length - 1; j >= 0; j--)
                {
                    if (str2[i] == str1[j])
                    {
                        if ((i == 0) || (j == 0))
                        {
                            maxtix[j] = 1;
                        }
                        else
                        {
                            maxtix[j] = maxtix[j - 1] + 1;
                        }
                    }
                    else
                    {
                        maxtix[j] = 0;
                    }
                    if (maxtix[j] > maxLength)
                    {
                        maxLength = maxtix[j];
                        end = j;
                    }
                }
            }
            if (maxLength == 0)
            {
                return 0;
            }
            if (null != comStr)
                comStr = str1.Substring(end - maxLength + 1, maxLength);
            return (double)maxLength / (double)str1.Length * 100;
        }
        //返回最长子串的比例
        private double lcs(string str1, string str2)
        {
            if ((String.IsNullOrEmpty(str1)) || (String.IsNullOrEmpty(str2)))
            {
                return 0;
            }

            int[] maxtix = new int[str1.Length];
            int maxLength = 0;

            for (int i = 0; i < str2.Length; i++)
            {
                for (int j = str1.Length - 1; j >= 0; j--)
                {
                    if (str2[i] == str1[j])
                    {
                        if ((i == 0) || (j == 0))
                        {
                            maxtix[j] = 1;
                        }
                        else
                        {
                            maxtix[j] = maxtix[j - 1] + 1;
                        }
                    }
                    else
                    {
                        maxtix[j] = 0;
                    }
                    if (maxtix[j] > maxLength)
                    {
                        maxLength = maxtix[j];
                    }
                }
            }
            if (maxLength == 0)
            {
                return 0;
            }
            return (double)maxLength / (double)str1.Length * 100;
        }

        private void attempAdd(string regex, string replace)
        {
            bool notHave = true;
            foreach (XmlElement i in rootNode.ChildNodes)
            {
                if (i.GetAttribute("regex").Equals(regex))
                {
                    notHave = false;
                    break;
                }
            }
            if (notHave)
            {
                XmlElement item = xmlDoc.CreateElement("item");
                item.SetAttribute("regex", regex);
                item.SetAttribute("replace", replace);
                rootNode.AppendChild(item);
                xmlDoc.Save("regex.xml");
                comboBox1.Items.Add(regex);
                comboBox2.Items.Add(regex);
                comboBox3.Items.Add(regex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!comboBox1.Text.ToString().Equals(""))
                {
                    Regex r = new Regex(comboBox1.Text);
                    if (!checkBox3.Checked)
                    {
                        richTextBox1.Text = r.Replace(richTextBox1.Text, richTextBox2.Text);
                        if (!comboBox1.Text.ToString().Equals(regDate1.Regex))
                        {
                            attempAdd(comboBox1.Text, richTextBox2.Text);
                        }
                    }
                    else
                    {
                        int rel = 0;
                        foreach (Match m in r.Matches(richTextBox1.Text))
                        {
                            if (checkBox4.Checked)
                            {
                                richTextBox1.Text = richTextBox1.Text.Insert(m.Index + m.Length + rel, "\n");
                            }
                            else
                            {
                                richTextBox1.Text = richTextBox1.Text.Insert(m.Index + rel, "\n");
                            }
                            rel++;
                        }
                    }
                }
                if (!comboBox2.Text.ToString().Equals(""))
                {
                    Regex r = new Regex(comboBox2.Text);
                    richTextBox1.Text = r.Replace(richTextBox1.Text, richTextBox3.Text);
                    if (!comboBox2.Text.ToString().Equals(regDate2.Regex))
                    {
                        attempAdd(comboBox2.Text, richTextBox3.Text);
                    }
                }
                if (!comboBox3.Text.ToString().Equals(""))
                {
                    Regex r = new Regex(comboBox3.Text);
                    if (!checkBox2.Checked)
                    {
                        richTextBox1.Text = r.Replace(richTextBox1.Text, richTextBox4.Text);
                        if (!comboBox3.Text.ToString().Equals(regDate3.Regex))
                        {
                            attempAdd(comboBox3.Text, richTextBox4.Text);
                        }
                    }
                    else
                    {
                        int rel = 0;
                        char[] sp = new char[] {'\n'};
                        string [] strs = richTextBox4.Text.Split(sp);
                        IEnumerator en = strs.GetEnumerator();
                        MatchCollection mc = r.Matches(richTextBox1.Text);
                        foreach (Match m in mc)
                        {
                            if(!en.MoveNext())
                            {
                                break;
                            }
                            richTextBox1.Text = richTextBox1.Text.Remove(m.Index + rel, m.Length);
                            richTextBox1.Text = richTextBox1.Text.Insert(m.Index + rel, (string)en.Current);
                            rel += ((string)en.Current).Length - m.Length;
                        }
                    }
                }
                Clipboard.SetDataObject(richTextBox1.Text, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //设为默认
        private void button2_Click(object sender, EventArgs e)
        {
            if (!comboBox1.Text.Equals(""))
            {
                ConfigSectionDate date = new ConfigSectionDate();
                date.Regex = comboBox1.Text;
                date.Replace = richTextBox2.Text;
                regDate1 = date;

                config.Sections.Remove("regex1");
                config.Sections.Add("regex1", date);
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!comboBox2.Text.Equals(""))
            {
                ConfigSectionDate date = new ConfigSectionDate();
                date.Regex = comboBox2.Text;
                date.Replace = richTextBox3.Text;
                regDate2 = date;

                config.Sections.Remove("regex2");
                config.Sections.Add("regex2", date);
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!comboBox3.Text.Equals(""))
            {
                ConfigSectionDate date = new ConfigSectionDate();
                date.Regex = comboBox3.Text;
                date.Replace = richTextBox4.Text;
                regDate3 = date;

                config.Sections.Remove("regex3");
                config.Sections.Add("regex3", date);
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }
        //选择历史记录
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                XmlElement xe = (XmlElement)rootNode.ChildNodes[((ComboBox)sender).SelectedIndex];
                //textBox1.Text = xe.GetAttribute("regex");
                richTextBox2.Text = xe.GetAttribute("replace");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                XmlElement xe = (XmlElement)rootNode.ChildNodes[((ComboBox)sender).SelectedIndex];
                //textBox2.Text = xe.GetAttribute("regex");
                richTextBox3.Text = xe.GetAttribute("replace");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                XmlElement xe = (XmlElement)rootNode.ChildNodes[((ComboBox)sender).SelectedIndex];
                //textBox3.Text = xe.GetAttribute("regex");
                richTextBox4.Text = xe.GetAttribute("replace");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        //清空历史记录
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("regex.xml");
                XmlNode rootNode = xmlDoc.SelectSingleNode("/regex");
                rootNode.RemoveAll();
                xmlDoc.Save("regex.xml");
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        //重置
        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox1.Text, true);
            richTextBox1.Text = "";
            comboBox1.Text = "";
            richTextBox2.Text = "";
            comboBox2.Text = "";
            richTextBox3.Text = "";
            comboBox3.Text = "";
            richTextBox4.Text = "";
            richTextBox1.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StringBuilder str = new StringBuilder("");
            for (int i = int.Parse(textBox1.Text); i <= int.Parse(textBox2.Text); i++)
            {
                str.Append("\n" + i);
            }
            Clipboard.SetDataObject(str.ToString(), true);
        }
    }
}

class ConfigSectionDate : ConfigurationSection
{
    [ConfigurationProperty("regex")]
    public string Regex
    {
        get { return (string)this["regex"]; }
        set { this["regex"] = value; }
    }

    [ConfigurationProperty("replace")]
    public string Replace
    {
        get { return (string)this["replace"]; }
        set { this["replace"] = value; }
    }
}
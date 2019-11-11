using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Microsoft.International.Converters.PinYinConverter;
using DotNetSpeech;
using System.Collections.ObjectModel;

namespace WinProcedure
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private String getInputText()
        //{

        //}

        private void PinYinBtn_Click(object sender, RoutedEventArgs e)
        {           
            String chinese = inputText.Text.Trim();
            if(chinese == null || chinese == "")
            {
                MessageBox.Show("请输入汉字");
            }
            else
            {
                //清除原有的拼音
                ouputList.Items.Clear();
                foreach (char c in chinese)
                {
                    if(ChineseChar.IsValidChar(c))
                    {
                        ChineseChar chChar = new ChineseChar(c);
                        ReadOnlyCollection<string> pinyins = chChar.Pinyins;
                        for(int i = 0;i < pinyins.Count;i++)
                        {
                            if(pinyins[i] != null && pinyins[i] != "") 
                                ouputList.Items.Add(pinyins[i]);
                        }
                    }
                    //换字的时候换行
                    ouputList.Items.Add(null);
                }
            }
        }

        private void Jian2fanBtn_Click(object sender, RoutedEventArgs e)
        {
            String text = inputText.Text.Trim();
            if (text == null || text == "")
            {
                MessageBox.Show("请输入汉字（简体）");
            }
            else
            {
                String simplified = ChineseConverter.Convert(text,ChineseConversionDirection.SimplifiedToTraditional);
                ouputList.Items.Clear();
                ouputList.Items.Add(simplified);
            }
        }

        private void Fan2jianBtn_Click(object sender, RoutedEventArgs e)
        {
            String text = inputText.Text.Trim();
            if (text == null || text == "")
            {
                MessageBox.Show("请输入汉字（繁体）");
            }
            else
            {
                String traditional = ChineseConverter.Convert(text, ChineseConversionDirection.TraditionalToSimplified);
                ouputList.Items.Clear();
                ouputList.Items.Add(traditional);
            }
        }

        private void Text2speechBtn_Click(object sender, RoutedEventArgs e)
        {
            String text = inputText.Text.Trim();
            if (text == null || text == "")
            {
                MessageBox.Show("请输入要转为语音的汉字");
            }
            else
            {
                SpeechVoiceSpeakFlags spFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                SpVoice voice = new SpVoice();
                voice.Speak(text, spFlags);
            }
        }

    }
}

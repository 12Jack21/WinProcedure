using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

using Microsoft.Office.Interop.Excel;
using MsWord = Microsoft.Office.Interop.Word;
using System.IO;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MsWord.Application oWordApplic = null;
            MsWord.Document odoc;
            try
            {
                //创建操作Word文档的项目
                oWordApplic = new MsWord.Application();
                object missing = Missing.Value;
                String filedir = Directory.GetCurrentDirectory(); // debug目录
                Console.WriteLine("当前目录： " + filedir);

                #region 创建Word文档的小节
                MsWord.Range curRange;
                object curTxt;
                int curSectionNum = 1;
                odoc = oWordApplic.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                odoc.Activate();
                Console.WriteLine("正在生成文档小节");
                object section_nextPage = MsWord.WdBreakType.wdSectionBreakNextPage;
                object page_break = MsWord.WdBreakType.wdPageBreak;
                //添加4个分节符，共5个小节，小节的类型是下一页
                for(int si = 0;si < 4; si++)
                {
                    //注意：Paragraphs的下标从 1 开始而不是 0
                    odoc.Paragraphs[1].Range.InsertParagraphAfter();
                    odoc.Paragraphs[1].Range.InsertBreak(ref section_nextPage);
                }
                #endregion

                #region 插入摘要文本并设置文本格式
                //从文件 abstract.txt中读取文本内容作为摘要部分内容，Word文档的内容都是具有格式的。
                Console.WriteLine("正在插入摘要内容");
                curSectionNum = 1;
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curRange.Select();
                string one_str, key_word;
                //摘要的文本来自 abstract.txt文件
                StreamReader file_abstract = new StreamReader("../../../COM_source/abstract.txt");
                oWordApplic.Options.Overtype = false; //overtype改写模式
                MsWord.Selection curSelection = oWordApplic.Selection;

                if(curSelection.Type == MsWord.WdSelectionType.wdSelectionNormal)
                {
                    one_str = file_abstract.ReadLine(); //读入题目
                    curSelection.TypeText(one_str);
                    curSelection.TypeParagraph(); //添加段落标记
                    curSelection.TypeText(" 摘要");
                    curSelection.TypeParagraph();//添加段落标志，进入下一段
                    key_word = file_abstract.ReadLine(); //读入关键字
                    one_str = file_abstract.ReadLine(); //读入段落文本（例子有 3段）
                    while(one_str != null)
                    {
                        curSelection.TypeText(one_str);
                        curSelection.TypeParagraph(); //读入一个段落就写一段
                        one_str = file_abstract.ReadLine();
                    }
                    curSelection.TypeText("关键字： ");
                    curSelection.TypeText(key_word); ///写关键字
                    curSelection.TypeParagraph();
                }
                file_abstract.Close();

                //下面开始设置摘要的格式（上面是内容的读取）
                //摘要的标题
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curTxt = curRange.Paragraphs[1].Range.Text;
                curRange.Font.Name = "宋体";
                curRange.Font.Size = 22;
                curRange.Paragraphs[1].Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter; //中间对齐

                //“摘要” 二字
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[2].Range; //第二段
                curRange.Select(); //成为选择区（在其上进行后续的操作）
                curRange.Paragraphs[1].Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                curRange.Font.Name = "黑体";
                curRange.Font.Size = 16;

                //摘要正文
                //自己用一个变量存起来这个常用的数据
                MsWord.Range curSectionRange = odoc.Sections[curSectionNum].Range;

                odoc.Sections[curSectionNum].Range.Paragraphs[3].Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                for(int i = 3; i < odoc.Sections[curSectionNum].Range.Paragraphs.Count;i++)
                {
                    curRange = odoc.Sections[curSectionNum].Range.Paragraphs[i].Range;
                    curTxt = curRange.Paragraphs[1].Range.Text;
                    curRange.Select();

                    curRange.Font.Name = "宋体";
                    curRange.Font.Size = 12;
                    odoc.Sections[curSectionNum].Range.Paragraphs[i].LineSpacingRule = MsWord.WdLineSpacing.wdLineSpaceMultiple;
                    //多倍行距，1.25倍，这里的浮点值是以point为单位的，不是行距倍数 ？？？？？-----
                    odoc.Sections[curSectionNum].Range.Paragraphs[i].LineSpacing = 15f;
                    curSectionRange.Paragraphs[i].IndentFirstLineCharWidth(2); //段落开头的缩进
                }
                //设置“关键字：”为黑体
                curRange = curRange.Paragraphs[curRange.Paragraphs.Count].Range;//最后一段的 range
                curTxt = curRange.Paragraphs[1].Range.Text; //能不能直接用 curRange.Text ??????---------
                object range_start, range_end;
                range_start = curRange.Start;
                range_end = curRange.Start + 4;// ??
                curRange = odoc.Range(ref range_start, ref range_end);
                curTxt = curRange.Text;
                curRange.Select();
                curRange.Font.Bold = 1;
                curRange.Font.Name = "黑体"; //应该加上

                #endregion

                #region 插入目录并设置目录格式

                #endregion

                #region Word文档保存
                Console.WriteLine("正在保存文档");
                object fileName = filedir + "/My_doc.doc";
                odoc.SaveAs2(fileName);
                odoc.Close();
                //Word文档任务完成后，需要释放Document对象和Application对象
                Console.WriteLine("正在释放 COM 资源");
                //释放COM资源
                Marshal.ReleaseComObject(odoc);
                odoc = null;
                oWordApplic.Quit();
                Marshal.ReleaseComObject(oWordApplic);
                oWordApplic = null;
                #endregion

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                #region 终止Word进程
                Console.WriteLine("正在结束Word进程");
                //关闭Word进程
                System.Diagnostics.Process[] allProcess = System.Diagnostics.Process.GetProcesses();
                for(int i = 0;i < allProcess.Length; i++)
                {
                    string procName = allProcess[i].ProcessName;
                    if(String.Compare(procName,"WINWORD") == 0)
                    {
                        if(allProcess[i].Responding && !allProcess[i].HasExited)
                        {
                            allProcess[i].Kill();
                        }
                    }
                }
                #endregion
            }
            Console.ReadKey();
        }


        //调用 COM组件
        private void CallCOM()
        {

        }



        //托管方式调用dll
        private void DllManaged()
        {
            Assembly a = Assembly.LoadFrom("../../../related_dll/OperationDLL.dll");
            foreach (Type t in a.GetTypes())
            {
                Console.WriteLine("Type's Name :" + t.Name);
                if (t.IsClass && !t.IsAbstract)
                {
                    if (t.Name == "Calculation")
                    {
                        MethodInfo[] methodInfos = t.GetMethods();
                        foreach (MethodInfo m in methodInfos)
                        {
                            Console.WriteLine(m.Name + "    ");
                            ParameterInfo[] methodParameter = m.GetParameters();

                            if (m.Name == "Add")
                            {
                                object[] parameters = new object[2];
                                parameters[0] = 2.4;
                                parameters[1] = 5.6;
                                object result = m.Invoke(null, parameters);

                                Console.WriteLine("Final Result From Managed = " + result.ToString());

                                double re = Add(2, 6);

                                Console.WriteLine("Final Result From Unmanaged = " + re);

                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }

        // att: 此方式一般适用于 C++ 生成的 DLL文件，不用通过在类中写方法
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Add(double src1, double src2);
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Multiply(double src1, double src2);


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

using MsExcel = Microsoft.Office.Interop.Excel;
using MsWord = Microsoft.Office.Interop.Word;
using System.IO;
using System.Diagnostics;

using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Process cmdP = new Process();
            //cmdP.StartInfo.FileName = "cmd.exe";
            //cmdP.StartInfo.CreateNoWindow = true;
            //cmdP.StartInfo.UseShellExecute = true;

            //// 重定向输入、输出和错误(IO)，即不在cmd窗口显示;需要把 UseShellExecute 设置为 false
            //cmdP.StartInfo.RedirectStandardInput = true;

            //Process.Start("chrome.exe", "http://www.baidu.com");

            //cmdP.Start();

            Program program = new Program();
            program.CallWord();
            //program.CallExcel();

            Console.ReadKey();
        }

        public static void GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            ManagementClass mc;
            try
            {
                mc = new ManagementClass("Win32_NetworkAdapterCOnfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                        macAddress += mo["MacAddress"].ToString() + "\n";
                }
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {

                }
                String outPut = String.Format("MAC地址：{0}", macAddress);
                Console.WriteLine(outPut);
            }
            catch (Exception ex)
            {
                //这里写异常的处理
                Console.WriteLine(ex.Message);
            }
        }

        //调用 COM组件
        private void CallWord()
        {
            MsWord.Application oWordApplic;
            MsWord.Document odoc;
            try
            {
                oWordApplic = new MsWord.Application();
                object missing = Missing.Value;
                String filedir = "";
                Console.WriteLine("当前文件目录： " + filedir);

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
                for (int si = 0; si < 4; si++)
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

                if (curSelection.Type == MsWord.WdSelectionType.wdSelectionNormal)
                {
                    one_str = file_abstract.ReadLine(); //读入题目
                    curSelection.TypeText(one_str);
                    curSelection.TypeParagraph(); //添加段落标记
                    curSelection.TypeText(" 摘要");
                    curSelection.TypeParagraph();//添加段落标志，进入下一段
                    key_word = file_abstract.ReadLine(); //读入关键字
                    one_str = file_abstract.ReadLine(); //读入段落文本（例子有 3段）
                    while (one_str != null)
                    {
                        curSelection.TypeText(one_str);
                        curSelection.TypeParagraph(); //读入一个段落就写一段
                        one_str = file_abstract.ReadLine();

                        curSelection.TypeText("关键字： ");
                        curSelection.TypeText(key_word); ///写关键字
                        curSelection.TypeParagraph();
                    }
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
                for (int i = 3; i < odoc.Sections[curSectionNum].Range.Paragraphs.Count; i++)
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
                Console.WriteLine("正在插入目录");
                curSectionNum = 2; //转到第二小节了
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curRange.Select();//变成选择区

                //插入目录时指定的参数
                object useheading_styles = true; //使用内置的目录标题样式
                object upperheading_level = 1; //最高的标题级别 （大纲视图的选择）
                object lowerheading_level = 3; //最低标题级别
                object useelds = 1; //true表示创建的是目录 ？？？？？
                object tableid = 1;
                object RightAlignPageNumbers = true;//右边距对齐的页码
                object IncludePageNumbers = true; //目录中包含页码

                curSelection = oWordApplic.Selection;
                curSelection.TypeText("目录"); //第一段写标题
                curSelection.TypeParagraph();
                curSelection.Select();

                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[2].Range; //转到第二段
                                                                                   //插入的表格会替代当前range
                                                                                   //range为非折叠时，TablesOfContents会代替range，引起小节数减少
                curRange.Collapse();
                //在这里添加了目录,记得在最后需要更新目录
                MsWord.TableOfContents tablesOfContents = odoc.TablesOfContents.Add(curRange, ref useheading_styles, ref upperheading_level, ref lowerheading_level, ref useelds, ref tableid, ref RightAlignPageNumbers,
                    ref IncludePageNumbers, ref missing, ref missing, ref missing, ref missing);


                odoc.Sections[curSectionNum].Range.Paragraphs[1].Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter; //设置"目录"两个字的样式
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Range.Font.Bold = 1;
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Range.Font.Name = "黑体";
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Range.Font.Size = 16;
                #endregion

                #region 插入第一章正文并设置正文格式
                curSectionNum = 3;
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Range.Select();
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                Console.WriteLine("正在设置标题样式");
                object wdFontSizeIndex;
                //此序号在Word中的编号是格式->显示格式->样式和格式->显示所有样式的序号
                //14是标题一，一级标题：三号黑体
                wdFontSizeIndex = 14;
                oWordApplic.ActiveDocument.Styles.get_Item(ref wdFontSizeIndex).ParagraphFormat.Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                oWordApplic.ActiveDocument.Styles.get_Item(ref wdFontSizeIndex).Font.Name = "黑体";
                oWordApplic.ActiveDocument.Styles.get_Item(ref wdFontSizeIndex).Font.Size = 16; //三号
                wdFontSizeIndex = 15; //15是标题二，二级标题：小三号黑体
                oWordApplic.ActiveDocument.Styles.get_Item(ref wdFontSizeIndex).Font.Name = "黑体";
                oWordApplic.ActiveDocument.Styles.get_Item(ref wdFontSizeIndex).Font.Size = 15; //小三号

                //用指定的标题来设置文本格式
                object Style1 = MsWord.WdBuiltinStyle.wdStyleHeading1; //一级标题：三号黑体
                object Style2 = MsWord.WdBuiltinStyle.wdStyleHeading2; //二级标题：小三号黑体

                odoc.Sections[curSectionNum].Range.Select();
                curSelection = oWordApplic.Selection;
                //读入第一章文本信息
                StreamReader file_content = new StreamReader("../../../COM_source/content.txt");
                one_str = file_content.ReadLine(); // 一级标题
                curSelection.TypeText(one_str);
                curSelection.TypeParagraph();
                one_str = file_content.ReadLine(); //二级标题
                curSelection.TypeText(one_str);
                curSelection.TypeParagraph();
                one_str = file_content.ReadLine(); //正文
                while (one_str != null)
                {
                    curSelection.TypeText(one_str);
                    curSelection.TypeParagraph();
                    one_str = file_content.ReadLine(); //正文

                }
                file_content.Close();
                //段落的对齐方式
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curRange.set_Style(ref Style1);
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[2].Range;
                curRange.set_Style(ref Style2);
                //第一章正文文本格式
                for (int i = 3; i < odoc.Sections[curSectionNum].Range.Paragraphs.Count; i++)
                {
                    curRange = odoc.Sections[curSectionNum].Range.Paragraphs[i].Range;
                    curRange.Select();
                    curRange.Font.Name = "宋体";
                    curRange.Font.Size = 12;
                    odoc.Sections[curSectionNum].Range.Paragraphs[i].LineSpacingRule = MsWord.WdLineSpacing.wdLineSpaceMultiple;
                    //多倍行距，1.25倍
                    odoc.Sections[curSectionNum].Range.Paragraphs[i].LineSpacing = 15f;
                    odoc.Sections[curSectionNum].Range.Paragraphs[i].IndentFirstLineCharWidth(2);
                }
                #endregion

                #region 插入表格并设置表格格式
                Console.WriteLine("正在插入第二章内容-表格");
                curSectionNum = 4;
                odoc.Sections[curSectionNum].Range.Select(); //调整了 oWordApplic.Selection
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curSelection = oWordApplic.Selection;
                curSelection.TypeText("2 表格");
                curSelection.TypeParagraph();
                curSelection.TypeText("表格示例");
                curSelection.TypeParagraph();
                curSelection.TypeParagraph();
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[3].Range;
                odoc.Sections[curSectionNum].Range.Paragraphs[3].Range.Select();
                curSelection = oWordApplic.Selection;
                MsWord.Table oTable;
                oTable = curRange.Tables.Add(curRange, 5, 3, ref missing, ref missing); //添加表格
                oTable.Range.ParagraphFormat.Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                oTable.Range.Rows.Alignment = MsWord.WdRowAlignment.wdAlignRowCenter;
                oTable.Columns[1].Width = 80;
                oTable.Columns[2].Width = 180;
                oTable.Columns[3].Width = 80;
                oTable.Cell(1, 1).Range.Text = " 字段";
                oTable.Cell(1, 2).Range.Text = " 描述";
                oTable.Cell(1, 3).Range.Text = " 数据类型";
                oTable.Cell(2, 1).Range.Text = "ProductID";
                oTable.Cell(2, 2).Range.Text = " 产品标识";
                oTable.Cell(2, 3).Range.Text = " 字符串";
                oTable.Borders.InsideLineStyle = MsWord.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = MsWord.WdLineStyle.wdLineStyleSingle;
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curRange.set_Style(ref Style1);
                curRange.ParagraphFormat.Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;

                #endregion

                #region 插入图片
                Console.WriteLine("正在插入第三章内容-插入图片");
                curSectionNum = 5;
                odoc.Sections[curSectionNum].Range.Paragraphs[1].Range.Select();  // understand what Select() means for ----------------------------- dif with oWordApplic.Selection
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curSelection = oWordApplic.Selection;
                curSelection.TypeText("3 图片");
                curSelection.TypeParagraph();
                curSelection.TypeText("图片示例");
                curSelection.TypeParagraph();
                curSelection.InlineShapes.AddPicture(@"F:\VS2017 code\WinProcedure\COM_source\whu.png", ref missing, ref missing, ref missing); //TODO:插入图片,图片名无法根据相对路径来找到
                curRange = odoc.Sections[curSectionNum].Range.Paragraphs[1].Range;
                curRange.set_Style(ref Style1);
                curRange.ParagraphFormat.Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;

                #endregion

                #region 设置各小节的页眉页脚
                Console.WriteLine("正在设置第1节摘要的页眉内容");
                //设置页脚 section 1 摘要
                curSectionNum = 1;
                odoc.Sections[curSectionNum].Range.Select();
                //进入页脚视图
                oWordApplic.ActiveWindow.View.SeekView = MsWord.WdSeekView.wdSeekCurrentPageFooter;
                odoc.Sections[curSectionNum].Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.
                    Borders[MsWord.WdBorderType.wdBorderBottom].LineStyle = MsWord.WdLineStyle.wdLineStyleNone;
                oWordApplic.Selection.HeaderFooter.PageNumbers.RestartNumberingAtSection = true;
                oWordApplic.Selection.HeaderFooter.PageNumbers.NumberStyle = MsWord.WdPageNumberStyle.wdPageNumberStyleUppercaseRoman;
                oWordApplic.Selection.HeaderFooter.PageNumbers.StartingNumber = 1;
                //切换到文档
                oWordApplic.ActiveWindow.ActivePane.View.SeekView = MsWord.WdSeekView.wdSeekMainDocument;
                Console.WriteLine("正在设置第 2节目录页眉内容");
                //设置页脚section 2目录
                curSectionNum = 2;
                odoc.Sections[curSectionNum].Range.Select();
                //进入页脚视图
                oWordApplic.ActiveWindow.ActivePane.View.SeekView = MsWord.WdSeekView.wdSeekCurrentPageFooter;
                odoc.Sections[curSectionNum].Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Borders[MsWord.WdBorderType.wdBorderBottom].LineStyle = MsWord.WdLineStyle.wdLineStyleNone;
                oWordApplic.Selection.HeaderFooter.PageNumbers.RestartNumberingAtSection = false;
                oWordApplic.Selection.HeaderFooter.PageNumbers.NumberStyle = MsWord.WdPageNumberStyle.wdPageNumberStyleUppercaseRoman;

                //切换到文档
                oWordApplic.ActiveWindow.ActivePane.View.SeekView = MsWord.WdSeekView.wdSeekMainDocument;
                //第 1章页眉页码设置
                curSectionNum = 3;
                odoc.Sections[curSectionNum].Range.Select();
                //切换进入页脚视图
                oWordApplic.ActiveWindow.ActivePane.View.SeekView = MsWord.WdSeekView.wdSeekCurrentPageFooter;
                curSelection = oWordApplic.Selection;
                curRange = curSelection.Range;
                //本节页码不续上节
                oWordApplic.Selection.HeaderFooter.PageNumbers.RestartNumberingAtSection = true;
                //页码格式为阿拉伯数字
                oWordApplic.Selection.HeaderFooter.PageNumbers.NumberStyle = MsWord.WdPageNumberStyle.wdPageNumberStyleArabic;
                //起始页码为 1
                oWordApplic.Selection.HeaderFooter.PageNumbers.StartingNumber = 1;
                //添加页码域
                object fieldpage = MsWord.WdFieldType.wdFieldPage;
                oWordApplic.Selection.Fields.Add(oWordApplic.Selection.Range, ref fieldpage, ref missing, ref missing);
                //居中对齐
                oWordApplic.Selection.ParagraphFormat.Alignment = MsWord.WdParagraphAlignment.wdAlignParagraphCenter;
                //本小节不链接到上一节
                odoc.Sections[curSectionNum].Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
                //切换进入正文视图
                oWordApplic.ActiveWindow.ActivePane.View.SeekView = MsWord.WdSeekView.wdSeekMainDocument;

                #endregion

                //更新目录
                Console.WriteLine("正在更新目录");
                tablesOfContents.Update();
                //tablesOfContents.UpdatePageNumbers();

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
                for (int i = 0; i < allProcess.Length; i++)
                {
                    string procName = allProcess[i].ProcessName;
                    if (String.Compare(procName, "WINWORD") == 0)
                    {
                        if (allProcess[i].Responding && !allProcess[i].HasExited)
                        {
                            allProcess[i].Kill();
                        }
                    }
                }
                #endregion
            }
        }
        private void CallExcel()
        {
            MsExcel.Application oExcApp = null;
            MsExcel.Workbook oExcBook;
            try
            {
                oExcApp = new MsExcel.Application();
                //创建一个Excel文档
                oExcBook = oExcApp.Workbooks.Add(true);

                #region 读取文本内容到 Excel 表格
                Console.WriteLine("正在读取文本内容到 Excel 表格");
                MsExcel.Worksheet worksheet1 = (MsExcel.Worksheet)oExcBook.Worksheets["sheet1"];
                worksheet1.Activate();
                oExcApp.Visible = false;
                oExcApp.DisplayAlerts = false;
                MsExcel.Range range1 = worksheet1.get_Range("B1", "H2");
                range1.Columns.ColumnWidth = 8;
                range1.Columns.RowHeight = 20;
                range1.Merge(false);
                //设置垂直居中和水平居中
                range1.VerticalAlignment = MsExcel.XlVAlign.xlVAlignCenter;
                range1.HorizontalAlignment = MsExcel.XlHAlign.xlHAlignCenter;
                //range1.Font.Color = System.Drawing.ColorTranslator.ToOle
                range1.Font.Size = 20;
                range1.Font.Bold = true;
                worksheet1.Cells[1, 2] = "学生成绩单";
                worksheet1.Cells[3, 1] = "学号";
                worksheet1.Cells[3, 2] = "姓名";
                worksheet1.Columns[1].ColumnWidth = 12;
                StreamReader sw = new StreamReader("../../../COM_source/list.csv");
                string a_str;
                string[] str_list;
                int i = 4;
                a_str = sw.ReadLine();
                while (a_str != null)
                {
                    str_list = a_str.Split(",".ToCharArray());
                    worksheet1.Cells[i, 1] = str_list[0];
                    worksheet1.Cells[i, 2] = str_list[1];
                    i++;
                    a_str = sw.ReadLine();
                }
                sw.Close();

                #endregion

                #region 向工作表添加图表
                Console.WriteLine("正在向工作表添加图表");
                //通过随机函数构造源数据
                for (int i1 = 0; i1 < 5; i1++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        worksheet1.Cells[i1 + 18, j + 3].Value2 = "= CEILING.MATH(RAND() * 100)";
                        worksheet1.Cells[i1 + 4, j + 3].Value2 = worksheet1.Cells[i1 + 18, j + 3].Value;
                    }
                }
                //添加图表
                MsExcel.Shape theShape = worksheet1.Shapes.AddChart2(Type.Missing, MsExcel.XlChartType.xl3DColumn, 120, 130, 380, 250, Type.Missing);
                //设置图表标题文本
                theShape.Chart.ChartTitle.Caption = "学生成绩";
                worksheet1.Cells[3, 3].Value2 = "美术";
                worksheet1.Cells[3, 4].Value2 = "物理";
                worksheet1.Cells[3, 5].Value2 = "政治";
                worksheet1.Cells[3, 6].Value2 = "化学";
                worksheet1.Cells[3, 7].Value2 = "体育";
                worksheet1.Cells[3, 8].Value2 = "英语";
                worksheet1.Cells[3, 9].Value2 = "数学";
                worksheet1.Cells[3, 10].Value2 = "历史";
                //设定图表的数据区域
                MsExcel.Range range = worksheet1.get_Range("b3:j8");
                theShape.Chart.SetSourceData(range, Type.Missing);
                //设置单元格边框线型
                range1 = worksheet1.get_Range("a3", "j8");
                range1.Borders.LineStyle = MsExcel.XlLineStyle.xlContinuous;

                #endregion

                Console.WriteLine("正在将结果保存为Excel文档");
                //将结果保存为Excel文档
                object file_name = Directory.GetCurrentDirectory() + @"/one.xlsx";
                oExcBook.Close(true, file_name, null);

            }
            catch (Exception e2)
            {
                Console.WriteLine(e2.Message);
            }
            finally
            {
                //释放相应的 COM对象资源
                oExcApp.Quit();
                Marshal.ReleaseComObject(oExcApp);
                oExcApp = null;
                System.GC.Collect();
            }
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


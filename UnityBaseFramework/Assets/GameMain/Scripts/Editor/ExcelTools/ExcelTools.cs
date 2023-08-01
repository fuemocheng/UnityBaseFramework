using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using NPOI.SS.UserModel;
using BaseFramework;

namespace XGame.Editor.Tools
{
    public sealed class ExcelTools
    {
        [MenuItem("Tools/Excel to TXT")]
        private static void GenerateTXT()
        {
            // Excel 表路径
            string excelDirPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/../Common/Excel");
            excelDirPath = excelDirPath.Replace("\\", "/");

            // Excel 所有表格
            string[] excelAllFiles = Directory.GetFiles(excelDirPath, "*.xlsx");

            // 输出路径
            string outputDirPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/Assets/GameMain/DataTables");
            outputDirPath = outputDirPath.Replace("\\", "/");

            // 遍历处理每个 excel 文件
            foreach (var excelFile in excelAllFiles)
            {
                // 规范路径
                string excelFilePath = excelFile.Replace("\\", "/");

                // excel 文件名，去掉后缀
                string excelFileName = excelFilePath.Substring(excelFilePath.LastIndexOf("/") + 1);
                excelFileName = excelFileName.Substring(0, excelFileName.Length - 5);

                // 输出路径名
                string outputFileName = Utility.Text.Format("{0}/{1}.txt", outputDirPath, excelFileName);

                try
                {
                    using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        // NPOI读取并解析excel的sheet数据
                        IWorkbook workbook = WorkbookFactory.Create(stream);
                        ISheet sheet = workbook.GetSheetAt(0);

                        // 遍历sheet数据，转换成项目需要的txt格式
                        string txt = Excel2Txt(sheet);

                        using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                            {
                                sw.Write(txt);
                            }
                        }
                    }

                    Debug.Log(Utility.Text.Format("Generate '{0}.txt' success.", excelFileName));
                }
                catch (Exception exception)
                {
                    Debug.LogError(Utility.Text.Format("Generate '{0}.txt' failure, exception is '{1}'.", excelFileName, exception));
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Excel 转 txt
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <returns></returns>
        public static string Excel2Txt(ISheet sheet)
        {
            // 表格定义
            // 第一行为表头，第二行为字段名，第三行为类型

            // 获取表格有多少列 
            int columns = sheet.GetRow(3).LastCellNum;

            // 获取表格有多少行 
            int rows = sheet.LastRowNum + 1;

            StringBuilder sb = new StringBuilder();

            // 第一行 TableName
            sb.Append("#");
            for (int j = 0; j < columns; j++)
            {
                var cell = sheet.GetRow(0)?.GetCell(j);
                if (cell != null)
                    sb.Append("\t" + cell);
            }

            // 第三行字段名，FiledName
            sb.Append("\n#");
            for (int j = 0; j < columns; j++)
            {
                var cell = sheet.GetRow(2)?.GetCell(j);
                if (cell != null)
                    sb.Append("\t" + cell);
            }

            // 第四行字段类型，FiledType
            sb.Append("\n#");
            for (int j = 0; j < columns; j++)
            {
                sb.Append("\t");
                var cell = sheet.GetRow(3)?.GetCell(j);
                if (cell != null)
                    sb.Append(cell);
            }

            // 第二行字段描述，FiledDesc
            sb.Append("\n#");
            for (int j = 0; j < columns; j++)
            {
                sb.Append("\t");
                var cell = sheet.GetRow(1)?.GetCell(j);
                if (cell != null)
                {
                    sb.Append(cell);
                }
            }

            // 第4行后的表数据
            for (int i = 4; i < rows; i++)
            {
                var first = sheet.GetRow(i)?.GetCell(0);
                if (first == null) continue;

                sb.Append("\n");
                for (int j = 0; j < columns; j++)
                {
                    sb.Append("\t");
                    var cell = sheet.GetRow(i)?.GetCell(j);
                    if (cell != null)
                    {
                        sb.Append(cell);
                    }
                }
            }
            return sb.ToString();
        }
    }
}

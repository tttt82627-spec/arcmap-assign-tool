using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FZFZ
{
    public static class IniHelper
    {
        // 固定INI路径
        private static readonly string IniPath = @"C:\ArcMapAssignConfig.ini";
        private const string Section = "AssignValues";

        /// <summary>
        /// 初始化配置文件，不存在则创建默认内容(UTF-8)
        /// </summary>
        public static void InitConfig()
        {
            if (!File.Exists(IniPath))
            {
                CreateDefaultConfigFile();
            }
        }

        /// <summary>
        /// 创建默认INI配置（UTF-8）
        /// </summary>
        private static void CreateDefaultConfigFile()
        {
            string[] defaultLines =
            {
                $"[{Section}]",
                "0311 阔叶林=0311",
                "0312 针叶林=0312",
                "0370 初值树木=0370",
                "0391 高覆盖草=0391",
                "03B1 荒地草被=03B1",
                "03B2 工地草被=03B2",
                "0718 碾压踩踏=0718",
                "0719 其他硬化=0719",
                "0819 其他采掘=0819",
                "0890 其他堆掘地=0890"
            };
            // 强制UTF-8写入
            File.WriteAllLines(IniPath, defaultLines, Encoding.UTF8);
        }

        /// <summary>
        /// 解析INI，获取所有按钮名称和对应赋值
        /// 纯文本解析，兼容UTF8、中文、空格
        /// </summary>
        public static Dictionary<string, string> GetAllAssignItems()
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            InitConfig();

            try
            {
                // 以UTF-8读取，和写入编码保持一致
                string[] allLines = File.ReadAllLines(IniPath, Encoding.UTF8);
                bool inTargetSection = false;

                foreach (string line in allLines)
                {
                    string trimLine = line.Trim();
                    // 跳过空行、注释行(;开头)
                    if (string.IsNullOrEmpty(trimLine) || trimLine.StartsWith(";"))
                        continue;

                    // 识别区块 [Section]
                    if (trimLine.StartsWith("[") && trimLine.EndsWith("]"))
                    {
                        string secName = trimLine.Trim('[', ']').Trim();
                        inTargetSection = secName.Equals(Section, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    // 只解析目标区块内的 键=值
                    if (!inTargetSection) continue;

                    int equalIndex = trimLine.IndexOf('=');
                    if (equalIndex <= 0) continue;

                    string key = trimLine.Substring(0, equalIndex).Trim();
                    string value = trimLine.Substring(equalIndex + 1).Trim();

                    if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }
            catch
            {
                // 读取异常时走默认数据
            }

            // 兜底：解析失败/文件为空，加载内置默认项
            if (result.Count == 0)
            {
                result["0311 阔叶林"] = "0311";
                result["0312 针叶林"] = "0312";
                result["0370 初值树木"] = "0370";
                result["0391 高覆盖草"] = "0391";
                result["03B1 荒地草被"] = "03B1";
                result["03B2 工地草被"] = "03B2";
                result["0718 碾压踩踏"] = "0718";
                result["0719 其他硬化"] = "0719";
                result["0819 其他采掘"] = "0819";
                result["0890 其他堆掘地"] = "0890";
            }

            return result;
        }

        /// <summary>
        /// 用记事本打开配置文件
        /// </summary>
        public static void OpenConfigFile()
        {
            if (File.Exists(IniPath))
            {
                System.Diagnostics.Process.Start("notepad.exe", IniPath);
            }
        }
    }
}
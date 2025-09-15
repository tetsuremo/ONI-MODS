using System.Collections.Generic;
using System.IO;

namespace TetsuRemo.OniFontPatchZhCN
{
    public static class PoFileReader
    {
        /// <summary>
        /// 从文件读取并解析 .po 文件，返回 msgid->msgstr 字典
        /// </summary>
        public static Dictionary<string, string> LoadPoFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("PO 文件不存在", filePath);

            var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
            return ParseLines(lines);
        }

        /// <summary>
        /// 从字符串数组解析 .po 内容
        /// </summary>
        public static Dictionary<string, string> ParseLines(string[] lines)
        {
            var dict = new Dictionary<string, string>();
            string currentMsgId = null;
            string currentMsgStr = null;
            bool readingMsgId = false;
            bool readingMsgStr = false;

            foreach (var raw in lines)
            {
                string line = raw.Trim();

                if (line.StartsWith("msgid "))
                {
                    readingMsgId = true;
                    readingMsgStr = false;
                    currentMsgId = ExtractQuoted(line.Substring(5));
                    currentMsgStr = "";
                }
                else if (line.StartsWith("msgstr "))
                {
                    readingMsgStr = true;
                    readingMsgId = false;
                    currentMsgStr = ExtractQuoted(line.Substring(6));
                }
                else if (line.StartsWith("\""))
                {
                    string content = ExtractQuoted(line);
                    if (readingMsgId) currentMsgId += content;
                    else if (readingMsgStr) currentMsgStr += content;
                }
                else if (string.IsNullOrEmpty(line))
                {
                    AddEntry(dict, currentMsgId, currentMsgStr);

                    currentMsgId = null;
                    currentMsgStr = null;
                    readingMsgId = false;
                    readingMsgStr = false;
                }
            }

            // 最后一条
            AddEntry(dict, currentMsgId, currentMsgStr);

            return dict;
        }

        private static void AddEntry(Dictionary<string, string> dict, string msgId, string msgStr)
        {
            if (!string.IsNullOrEmpty(msgId))
            {
                if (dict.ContainsKey(msgId))
                    dict[msgId] = msgStr; // 覆盖旧值
                else
                    dict.Add(msgId, msgStr);
            }
        }

        /// <summary>
        /// 去掉引号并处理换行
        /// </summary>
        private static string ExtractQuoted(string str)
        {
            str = str.Trim();
            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                string inner = str.Substring(1, str.Length - 2);
                return inner.Replace("\\n", "\n").Replace("\\\"", "\"");
            }
            return str;
        }
    }
}

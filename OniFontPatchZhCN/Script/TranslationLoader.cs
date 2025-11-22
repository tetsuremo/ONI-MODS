using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OniFontPatchZhCN
{
    public static class TranslationLoader
    {
        public static Dictionary<string, string> LoadPoFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("PO 文件不存在", filePath);

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            return ParsePoFile(lines);
        }

        private static Dictionary<string, string> ParsePoFile(string[] lines)
        {
            var dict = new Dictionary<string, string>();
            PoEntry currentEntry = new PoEntry();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                {
                    if (currentEntry.IsValid)
                    {
                        dict[currentEntry.MsgId] = currentEntry.MsgStr;
                    }
                    currentEntry = new PoEntry();
                    continue;
                }

                if (line.StartsWith("msgctxt "))
                {
                    currentEntry.MsgCtxt = ExtractQuotedValue(line.Substring(7).Trim());
                }
                else if (line.StartsWith("msgid "))
                {
                    currentEntry.MsgId = ExtractQuotedValue(line.Substring(5).Trim());
                }
                else if (line.StartsWith("msgstr "))
                {
                    currentEntry.MsgStr = ExtractQuotedValue(line.Substring(6).Trim());
                }
                else if (line.StartsWith("\""))
                {
                    // 多行字符串继续
                    string content = ExtractQuotedValue(line);
                    if (!string.IsNullOrEmpty(currentEntry.MsgId)
                        && string.IsNullOrEmpty(currentEntry.MsgStr))
                    {
                        currentEntry.MsgId += content;
                    }
                    else if (!string.IsNullOrEmpty(currentEntry.MsgStr))
                    {
                        currentEntry.MsgStr += content;
                    }
                }
            }

            // 添加最后一个条目
            if (currentEntry.IsValid)
            {
                dict[currentEntry.MsgId] = currentEntry.MsgStr;
            }

            return dict;
        }

        private static string ExtractQuotedValue(string str)
        {
            str = str.Trim();
            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                string inner = str.Substring(1, str.Length - 2);
                return inner.Replace("\\n", "\n").Replace("\\\"", "\"");
            }
            return str;
        }

        private struct PoEntry
        {
            public string MsgCtxt;
            public string MsgId;
            public string MsgStr;

            public bool IsValid => !string.IsNullOrEmpty(MsgId) && !string.IsNullOrEmpty(MsgStr);
        }
    }
}

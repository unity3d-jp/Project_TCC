using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    // CSVやタブスペースで区切ったテキストファイルを読み込む簡易読み込み関数
    public class TextRecordParsing
    {
        public const string DefaultSeparators = "\t,";
        public static IEnumerable<string> DefaultCommentPrefixes
        {
            get { return new string[] { "//", "#", ";" }; }
        }

        public class Record
        {
            public Record(IEnumerable<string> initialItems)
            {
                items = initialItems.ToList();
            }

            public int Count { get { return items.Count; } }
            public IEnumerable<string> Items { get { return items; } }

            public string GetString(int index)
            {
                return TextRecordParsing.GetString(items, index);
            }

            public bool GetBool(int index)
            {
                return TextRecordParsing.GetBool(items, index);
            }

            public bool TryGetInt(int index, ref int output)
            {
                return TextRecordParsing.GetInt(items, index, ref output);
            }

            public bool TryGetFloat(int index, ref float output)
            {
                return TextRecordParsing.GetFloat(items, index, ref output);
            }

            public bool TryGetVector3(int startIndex, ref Vector3 output)
            {
                return TextRecordParsing.GetVector3(items, startIndex, ref output);
            }

            public Queue<string> ToQueue()
            {
                return new Queue<string>(items);
            }

            private List<string> items = new List<string>();
        }

        // レコードのアイテムを取得。存在しない場合は空の文字列を返す。
        public static string GetString(List<string> items, int index)
        {
            return (index >= 0 && index < items.Count) ? items[index] : "";
        }

        // レコードの数字を取得します。アイテムが空・false・0の場合はfalseを返します
        public static bool GetBool(List<string> items, int index)
        {
            var falseItems = new List<string> { "0", "false" };
            var itemString = GetString(items, index).Trim().ToLowerInvariant();
            return itemString.Length > 0 && !falseItems.Contains(itemString);
        }

        // レコードの数字を取得します。できなかった場合はfalseを返す。
        public static bool GetInt(List<string> items, int index, ref int output)
        {
            var item = GetString(items, index);
            int newValue;
            var succeeded = int.TryParse(item, out newValue);
            if (succeeded)
            {
                output = newValue;
            }
            return succeeded;
        }

        // レコードの数字を取得します。できなかった場合はfalseを返す。
        public static bool GetFloat(List<string> items, int index, ref float output)
        {
            var item = GetString(items, index);
            float newValue;
            var succeeded = float.TryParse(item, out newValue);
            if (succeeded)
            {
                output = newValue;
            }
            return succeeded;
        }

        // ベクトルを取得。できなかったらfalseを返す。
        public static bool GetVector3(List<string> items, int startIndex, ref Vector3 output)
        {
            var succeeded = false;
            if (startIndex >= 0 && startIndex + 2 < items.Count)
            {
                float x = 0f;
                float y = 0f;
                float z = 0f;
                succeeded = float.TryParse(items[startIndex], out x)
                    && float.TryParse(items[startIndex + 1], out y)
                    && float.TryParse(items[startIndex + 2], out z);
                if (succeeded)
                {
                    output.Set(x, y, z);
                }
            }
            return succeeded;
        }

        public static List<Record> ParseRecordsFromReader
        (
            System.IO.TextReader reader,
            string entrySeparators = DefaultSeparators,
            IEnumerable<string> commentPrefixes = null
        )
        {
            commentPrefixes = commentPrefixes ?? DefaultCommentPrefixes;
            var records = new List<Record>();
            while (reader.Peek() != -1)
            {
                var nextRow = CSVUtilities.ReadNextCSVRow(reader, entrySeparators)
                    .Select(item => item.Trim());
                if (nextRow.Any() && !LineIsCommentedOut(nextRow.First(), commentPrefixes))
                {
                    records.Add(new Record(nextRow));
                }
            }
            return records;
        }

        public static List<Record> ParseRecordsFromFile
        (
            string sourcePath,
            System.Text.Encoding encoding,
            string entrySeparators = DefaultSeparators,
            IEnumerable<string> commentPrefixes = null
        )
        {
            List<Record> newRecords = null;
            try
            {
                using (var reader = new System.IO.StreamReader(sourcePath, encoding))
                {
                    newRecords = ParseRecordsFromReader(reader, entrySeparators, commentPrefixes);
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogError("TextRecordParsing 読み込みエラー: " + sourcePath + "\n" + exception.ToString());
                newRecords = new List<Record>();
            }
            return newRecords;
        }

        public static List<Record> ParseRecordsFromText
        (
            string sourceText,
            string entrySeparators = DefaultSeparators,
            IEnumerable<string> commentPrefixes = null
        )
        {
            List<Record> newRecords = null;
            try
            {
                using (var reader = new System.IO.StringReader(sourceText))
                {
                    newRecords = ParseRecordsFromReader(reader, entrySeparators, commentPrefixes);
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogError("TextRecordParsing 読み込みエラー\n" + exception.ToString());
                newRecords = new List<Record>();
            }
            return newRecords;
        }

        // Gets records within a given ini-style section (enclosed in square braces)
        // An empty or null section name will retrieve the unnamed section (all rows before the first named section)
        public static List<Record> GetSectionRecords
        (
            List<Record> sourceRecords,
            string sectionName
        )
        {
            // First find the start of the section
            var sectionStartIndex = 0;
            if (!string.IsNullOrEmpty(sectionName))
            {
                sectionName = "[" + sectionName.ToLowerInvariant() + "]";
                sectionStartIndex = sourceRecords.FindIndex(
                    item => item.GetString(0).Trim().ToLowerInvariant() == sectionName);
                if (sectionStartIndex == -1)
                {
                    return new List<Record>(0);
                }
                sectionStartIndex += 1;
            }

            return sourceRecords.Skip(sectionStartIndex)
                .TakeWhile(item => !item.GetString(0).Trim().StartsWith("["))
                .ToList();
        }

        // private

        private static bool LineIsCommentedOut(string trimmedLine, IEnumerable<string> commentPrefixes)
        {
            return commentPrefixes.Any(prefix => prefix.Length > 0 
                && trimmedLine.StartsWith(prefix));
        }
    }
}
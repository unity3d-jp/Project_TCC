using System.Collections.Generic;
using System.Linq;

namespace UTJ
{
    public static class CSVUtilities
    {
        public static string BuildCSVItem(string source)
        {
            return BuildCSVItem(source, DefaultSeparators);
        }

        public static string BuildCSVItem(string source, string separatorCharacters)
        {
            var tokensRequiringQuote = ("\"\r\n" + separatorCharacters).ToCharArray();
            if (tokensRequiringQuote.Any(token => source.Contains(token)))
            {
                return "\"" + source.Replace("\"", "\"\"") + "\"";
            }
            return source;
        }

        public static IList<string> ReadNextCSVRow(System.IO.TextReader reader)
        {
            return ReadNextCSVRow(reader, DefaultSeparators);
        }

        public static IList<string> ReadNextCSVRow(System.IO.TextReader reader, string separatorCharacters)
        {
            const int EndOfStreamValue = -1;
            if (reader.Peek() == EndOfStreamValue)
            {
                return new List<string>();
            }

            var rowItems = new List<string>();
            var currentItem = new System.Text.StringBuilder();
            var insideQuotes = false;
            var isEndOfRow = false;
            while (!isEndOfRow)
            {
                var isEndOfStream = reader.Peek() == EndOfStreamValue;
                isEndOfRow = isEndOfStream;
                var isEndOfItem = isEndOfRow;
                if (!isEndOfRow)
                {
                    var nextItem = (char)reader.Read();
                    if (!insideQuotes)
                    {
                        isEndOfRow = nextItem == '\r' || nextItem == '\n';
                        // Eat \r\n
                        if (nextItem == '\r' && (char)reader.Peek() == '\n')
                        {
                            reader.Read();
                        }

                        isEndOfItem = isEndOfRow || separatorCharacters.Contains(nextItem);
                        if (!isEndOfItem)
                        {
                            if (nextItem == '"')
                            {
                                insideQuotes = true;
                            }
                            else
                            {
                                currentItem.Append(nextItem);
                            }
                        }
                    }
                    else
                    {
                        if (nextItem == '"')
                        {
                            var peekedValue = reader.Peek();
                            if (peekedValue != EndOfStreamValue 
                                && (char)peekedValue == '"')
                            {
                                currentItem.Append('"');
                                reader.Read();
                            }
                            else
                            {
                                insideQuotes = false;
                            }
                        }
                        else
                        {
                            currentItem.Append(nextItem);
                        }
                    }
                }

                var isEmptyRow = isEndOfRow 
                    && rowItems.Count == 0 
                    && currentItem.Length == 0;
                if (isEndOfItem && !isEmptyRow)
                {
                    rowItems.Add(currentItem.ToString());
                    currentItem.Length = 0;
                }
            }
            return rowItems;
        }

        private const string DefaultSeparators = ",\t";
    }
}
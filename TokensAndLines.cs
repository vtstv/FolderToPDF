using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolderToPDF
{
    public static class TokensAndLines
    {
        public static (int TotalLines, int TotalTokens) Calculate(List<(string FilePath, string FileName, string Content)> contents)
        {
            int totalLines = 0;
            int totalTokens = 0;
            foreach (var (_, _, content) in contents)
            {
                totalLines += CountLines(content);
                totalTokens += CountTokens(content);
            }
            return (totalLines, totalTokens);
        }

        private static int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static int CountTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            var delimiters = new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '-', '*', '^' };
            var tokens = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Length;
        }
    }
}

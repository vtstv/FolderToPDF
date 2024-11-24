using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpToken;

namespace FolderToPDF
{
    public static class TokensAndLines
    {
        private static readonly GptEncoding _encoder;

        static TokensAndLines()
        {
            // Initialize the encoder for GPT-3.5-turbo model
            // You can change this to other models like "gpt4" or "text-davinci-003"
            _encoder = GptEncoding.GetEncoding("cl100k_base");
        }

        public static (int TotalLines, int TotalTokens) Calculate(List<(string FilePath, string FileName, string Content)> contents)
        {
            int totalLines = 0;
            int totalTokens = 0;

            foreach (var (_, _, content) in contents)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    totalLines += CountLines(content);
                    totalTokens += CountTokens(content);
                }
            }

            return (totalLines, totalTokens);
        }

        private static int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            return lines.Length;
        }


        private static int CountTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                // Use Tiktoken to get accurate token count
                return _encoder.Encode(text).Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting tokens: {ex.Message}");
                return 0;
            }
        }

        // Helper method to analyze token distribution and potentially large tokens
        public static TokenAnalysis AnalyzeTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new TokenAnalysis(0, new List<string>(), 0);

            var encoded = _encoder.Encode(text);
            var decoded = _encoder.Decode(encoded); // Assuming this returns a `List<char>` or similar
            var largeTokens = new List<string>();
            int maxTokenLength = 0;

            for (int i = 0; i < encoded.Count; i++)
            {
                string token = decoded[i].ToString(); // Convert `char` to `string`
                if (token.Length > 10) // Threshold for "large" tokens
                {
                    largeTokens.Add(token);
                }
                maxTokenLength = Math.Max(maxTokenLength, token.Length);
            }

            return new TokenAnalysis(encoded.Count, largeTokens, maxTokenLength);
        }

    }

    public class TokenAnalysis
    {
        public int TotalTokens { get; }
        public List<string> LargeTokens { get; }
        public int MaxTokenLength { get; }

        public TokenAnalysis(int totalTokens, List<string> largeTokens, int maxTokenLength)
        {
            TotalTokens = totalTokens;
            LargeTokens = largeTokens;
            MaxTokenLength = maxTokenLength;
        }
    }
}
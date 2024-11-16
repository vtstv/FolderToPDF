using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FolderToPDF
{
    public class FileCleaner
    {

        public static string RemoveCommentsAndWhitespace(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // Remove single-line comments (//)
            content = Regex.Replace(content, @"//.*", string.Empty);

            // Remove multi-line comments (/* */)
            content = Regex.Replace(content, @"/\*[\s\S]*?\*/", string.Empty);

            // Remove extra blank lines and trim leading/trailing spaces
            content = Regex.Replace(content, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline);
            content = content.Trim();

            return content;
        }


        public static string ReplaceSensitiveInformation(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // Replace potential login credentials
            content = Regex.Replace(content, @"(?i)(username|user|login)\s*=\s*[""']?[\w@.]+[""']?", @"$1=""<USERNAME>""");

            // Replace passwords
            content = Regex.Replace(content, @"(?i)(password|pass|pwd)\s*=\s*[""']?[\w@#$%^&*!]+[""']?", @"$1=""<PASSWORD>""");

            // Replace tokens
            content = Regex.Replace(content, @"(?i)(token|api_key|secret)\s*=\s*[""']?[\w-]+[""']?", @"$1=""<TOKEN>""");

            // Replace email addresses
            content = Regex.Replace(content, @"\b[\w.-]+@[\w.-]+\.\w+\b", "<EMAIL>");

            // Replace IP addresses
            content = Regex.Replace(content, @"\b\d{1,3}(\.\d{1,3}){3}\b", "<IP_ADDRESS>");

            return content;
        }


        public static string CleanContent(string content, bool removeComments, bool replaceSensitiveInfo)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            if (removeComments)
            {
                content = RemoveCommentsAndWhitespace(content);
            }

            if (replaceSensitiveInfo)
            {
                content = ReplaceSensitiveInformation(content);
            }

            return content;
        }
    }
}
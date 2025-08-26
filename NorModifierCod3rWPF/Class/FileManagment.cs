using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NorModifierCod3rWPF.Class
{
    class FileManagment
    {
        public static string GetFileSize(string filePath)
        {
            // Check if file exists incase something goes wrong.

            if (!File.Exists(filePath))
                return $"Cannot locate file from path {filePath}. The application should still function normally.";

            // Get the file size in bytes and convert it to a human readable format.

            FileInfo fileInfo = new FileInfo(filePath);
            long sizeInBytes = fileInfo.Length;

            // Round decimal place by 2 and convert to MB.
            double sizeInMB = sizeInBytes / (1024.0 * 1024.0);

            return $"{Math.Round(sizeInMB, 2)} MB";
        }

        private static string BytesToReadable(long byteCount)
        {
            // Convert bytes to a human readable format with this function.
            string[] suf = { "B", "KB", "MB" };
            if (byteCount == 0) return "0" + suf[0];
            int place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
            double num = Math.Round(byteCount / Math.Pow(1024, place), 2);
            return $"{num} {suf[place]}";
        }
    }
}

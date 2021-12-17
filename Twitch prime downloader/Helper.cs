using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Twitch_prime_downloader
{
    public static class Helper
    {
        public static string ToText(this IEnumerable<string> collection)
        {
            string res = string.Empty;
            foreach (string str in collection)
            {
                res += str + "\n";
            }
            return res;
        }

        public static void LoadFromFile(this ComboBox comboBox, string fileName)
        {
            try
            {
                using (StreamReader file = new StreamReader(fileName))
                {
                    string s;
                    while ((s = file.ReadLine()) != null)
                    {
                        comboBox.Items.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public static void SaveToFile(this ComboBox comboBox, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                try
                {
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        for (int i = 0; i < comboBox.Items.Count; i++)
                        {
                            sw.WriteLine(comboBox.Items[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        public static void SaveToFile(this IEnumerable<string> collection, string fileName)
        {
            string t = collection.ToText();
            File.WriteAllText(fileName, t);
        }

        public static bool SaveToFile(this Stream stream, string fileName, bool fromOrigin = true)
        {
            Stream fileStream = File.OpenWrite(fileName);
            if (fromOrigin)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            bool res = MultiThreadedDownloader.AppendStream(stream, fileStream);
            fileStream.Dispose();
            return res;
        }

    }
}

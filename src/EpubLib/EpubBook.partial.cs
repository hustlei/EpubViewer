using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Lei.Common
{
    partial class EpubBook
    {
        private string GetTempDirectory()
        {
            string text = Path.GetRandomFileName();
            text = Path.Combine(Path.GetTempPath(), text);
            Directory.CreateDirectory(text);
            return text;
        }
        private bool equalsFileURL(string url1, string url2)
        {
            bool result;
            try
            {
                url1 = url1.Trim().ToLower();
                url2 = url2.Trim().ToLower();
                if (url1 == url2)
                {
                    result = true;
                }
                else
                {
                    url1 = this.getFileName(url1);
                    url2 = this.getFileName(url2);
                    if (url1 == url2)
                    {
                        result = true;
                    }
                    else
                    {
                        url1 = Uri.UnescapeDataString(url1);
                        url2 = Uri.UnescapeDataString(url2);
                        if (url1 == url2)
                        {
                            result = true;
                        }
                        else
                        {
                            url1 = this.unescapeFileName(url1);
                            url2 = this.unescapeFileName(url2);
                            if (url1 == url2)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        private string unescapeFileName(string url)
        {
            try
            {
                url = Uri.UnescapeDataString(url);
                if (url.IndexOf("%") < 0)
                {
                    string result = url;
                    return result;
                }
                string[] array = new string[]
				{
					"%21",
					"%27",
					"%28",
					"%29",
					"%5F",
					"%2A",
					"%2D",
					"%2E",
					"%7E"
				};
                string[] array2 = new string[]
				{
					"!",
					"'",
					"(",
					")",
					"_",
					"*",
					"-",
					".",
					"~"
				};
                for (int i = 0; i < array.Length; i++)
                {
                    url = url.Replace(array[i], array2[i]);
                }
                if (url.IndexOf("%") < 0)
                {
                    string result = url;
                    return result;
                }
                for (char c = 'A'; c <= 'Z'; c += '\u0001')
                {
                    url = url.Replace("%" + ((byte)(c - 'A' + ')')).ToString(), c.ToString());
                }
                if (url.IndexOf("%") < 0)
                {
                    string result = url;
                    return result;
                }
                for (char c2 = 'a'; c2 <= 'z'; c2 += '\u0001')
                {
                    url = url.Replace("%" + ((byte)(c2 - 'a' + '=')).ToString(), c2.ToString());
                }
                if (url.IndexOf("%") < 0)
                {
                    string result = url;
                    return result;
                }
                for (char c3 = '0'; c3 <= '9'; c3 += '\u0001')
                {
                    url = url.Replace("%" + ((byte)(c3 - '0' + '\u001e')).ToString(), c3.ToString());
                }
            }
            catch
            {
            }
            return url;
        }
        private string getFileName(string url)
        {
            try
            {
                int num = url.LastIndexOf('#');
                if (num != -1)
                {
                    url = url.Substring(0, num);
                }
            }
            catch
            {
            }
            return url;
        }
        private string getFileURL(string contant_Path, string href)
        {
            return string.Format("file:///{0}/" + href, contant_Path).Replace("\\", "/");
        }
        private string getFileURL(string href)
        {
            return ("file:///" + href).Replace("\\", "/");
        }
        private string getFilePath(string url)
        {
            return this.getFileName(url).Replace("file:///", "").Replace("/", "\\");
        }
        private void formatCoverURL(string cover_url)
        {
            this.Cover_Content = "";
            this.Cover_URL = "";
            if (string.IsNullOrEmpty(cover_url))
            {
                return;
            }
            cover_url = this.getFilePath(cover_url);
            if (!File.Exists(cover_url))
            {
                return;
            }
            try
            {
                string text = File.ReadAllText(cover_url);
                if (!string.IsNullOrEmpty(text))
                {
                    int num = text.IndexOf("<svg ");
                    if (num >= 0)
                    {
                        int num2 = text.IndexOf("</svg>", num);
                        if (num2 >= 0)
                        {
                            string text2 = text.Substring(num, num2 - num);
                            string text3 = text2.Trim().ToLower();
                            if (!string.IsNullOrEmpty(text3))
                            {
                                int num3 = text3.IndexOf("viewbox=\"");
                                if (num3 >= 0)
                                {
                                    int num4 = text3.IndexOf("\"", num3 + 9);
                                    if (num4 >= 0)
                                    {
                                        string text4 = text3.Substring(num3 + 9, num4 - num3 - 9).Trim();
                                        if (!string.IsNullOrEmpty(text4))
                                        {
                                            string[] array = text4.TrimStart(new char[]
											{
												'"'
											}).TrimEnd(new char[]
											{
												'"'
											}).Split(new char[]
											{
												' '
											});
                                            if (array.Length == 4)
                                            {
                                                int num5 = 0;
                                                if (int.TryParse(array[2], out num5))
                                                {
                                                    if (num5 > 0)
                                                    {
                                                        int num6 = 0;
                                                        if (int.TryParse(array[3], out num6))
                                                        {
                                                            if (num6 > 0)
                                                            {
                                                                string text5 = text2;
                                                                text5 = Regex.Replace(text5, "height=\"100%\"", "height=\"" + num6.ToString() + "\"", RegexOptions.IgnoreCase);
                                                                text5 = Regex.Replace(text5, "width=\"100%\"", "width=\"" + num5.ToString() + "\"", RegexOptions.IgnoreCase);
                                                                string text6 = text;
                                                                text6 = text6.Replace(text2, text5);
                                                                text6 = Regex.Replace(text6, "</html>", string.Concat(new string[]
																{
																	"<style type=\"text/css\">svg {width: ",
																	num5.ToString(),
																	"px; height: ",
																	num6.ToString(),
																	"px;}</style></html>"
																}), RegexOptions.IgnoreCase);
                                                                if (text6 != text)
                                                                {
                                                                    this.writeFileContent(cover_url, text6);
                                                                    this.Cover_Content = text;
                                                                    this.Cover_URL = cover_url;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                this.Cover_Content = "";
                this.Cover_URL = "";
            }
        }
        private void writeFileContent(string file, string content)
        {
            Encoding encoding = null;
            try
            {
                if (File.Exists(file))
                {
                    StreamReader streamReader = new StreamReader(file);
                    encoding = streamReader.CurrentEncoding;
                    streamReader.Close();
                }
            }
            catch
            {
            }
            if (encoding == null)
            {
                File.WriteAllText(file, content);
                return;
            }
            File.WriteAllText(file, content, encoding);
        }
    }
}

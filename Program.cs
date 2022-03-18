using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace EDI_HL_Sort
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EDI HL Sort");

            try
            {
                string inputFolderLocation = args[0];
                string outputFolderLocation = args[1];

                string[] files = Directory.GetFiles(inputFolderLocation);

                foreach (string f in files)
                {
                    SortedList<int, string> HLSegments = new SortedList<int, string>();

                    string testfileLocation = f;
                    string testFileName = Path.GetFileNameWithoutExtension(f);
                    string outboundFile = outputFolderLocation + "\\" + testFileName + "_parsed.edi";

                    string line;
                    string[] parts;
                    string hlgroup = "";
                    int current_hlid = 0;
                    int new_hlid;

                    System.IO.StreamReader file = new System.IO.StreamReader(testfileLocation);
                    while ((line = file.ReadLine()) != null)
                    {
                        parts = line.Split('*');
                        string type = parts[0];

                        if (type == "HL") //This line starts a new HL Grouping
                        {

                            new_hlid = Convert.ToInt32(parts[1]);

                            //hlgroup += line;

                            if (new_hlid != current_hlid) //This case means we are in a new HL Group
                            {
                                HLSegments.Add(current_hlid, hlgroup);

                                current_hlid = new_hlid;
                                hlgroup = "";
                            }
                        }

                        hlgroup += line.Replace("\n", "").Replace("\r", "").Replace("~", "") + "\n";
                    }

                    HLSegments.Add(current_hlid, hlgroup);

                    using (StreamWriter writer = new StreamWriter(outboundFile))
                    {
                        foreach (KeyValuePair<int, string> kvp in HLSegments)
                        {
                            Console.Write(kvp.Value);
                            writer.Write(kvp.Value);
                        }
                    }

                    file.Close();
                    file.Dispose();

                    Thread.Sleep(1000);

                    File.Delete(testfileLocation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            //Console.Read();

        }  
    }
}

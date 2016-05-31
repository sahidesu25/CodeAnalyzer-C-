///////////////////////////////////////////////////////////////////////
//FileMgr.cs - Get the Files based on the path,useroption, pattern   //
//and write the output in an XML File                                 //
// ver 1.3                                                           //
// Language:    C#, 2008, .Net Framework 3.5                         //
// Platform:   DellInspiron 15 5000 series                           //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
// Author:      SahtihiDesu, Syracuse Universiy 
//               sldesu@syr.edu
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 *fetches alll the files from the current directory or all the directories present in the current directory.
/* 
 * 
 * Maintenance History:
 * --------------------
 * 
 * ver 1.0 : 28 Aug 2011
 * - first release
************************************************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{
    public class FileMgr
    {
        public List<string> cSharpFiles = new List<string>();
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        string[] newFiles ;
        string[] dirs;
        private bool recurse = false;

        public void findFiles(string path)
        {
          //  if (patterns.Count == 0)
            //    addPattern("*.*");
            foreach (string pattern in patterns)
            {
                try
                {
                    newFiles = Directory.GetFiles(path, pattern);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("The error message is {0}", e.Message);
                }

                //    string[] newFiles = Directory.GetFiles(path, pattern,SearchOption.AllDirectories);
                for (int i = 0; i < newFiles.Length; ++i)
                    try
                    {
                        newFiles[i] = Path.GetFullPath(newFiles[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("the error message is {0}", e.Message);
                    }
                files.AddRange(newFiles);
            }
            
            
                if (recurse)
                {

                    try
                    {
                        dirs = Directory.GetDirectories(path);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("the error message is {0}", e.Message);
                    }

                    foreach (string dir in dirs)
                    {
                        try
                        {
                            findFiles(dir);
                        }
                        catch (Exception es)
                        {
                            Console.WriteLine("/n The error message is:  {0}", es.Message);
                        }
                    }
                }
            }
        

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }
        public void setBool(bool change)
        {
            this.recurse = change;
        }

        public List<string> getFiles()
        {
            return files;
        }
        public string[] getCSharpFile()
        {

            return cSharpFiles.ToArray();
        }

#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            //int flag1 = 0;
            int flag = 0;
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");
            string path=".";
            string pattern = "";
            FileMgr fm = new FileMgr();
            for(int i=0;i<args.Length;i++)
            {
                if (args[i] == "*.cs" || args[i] == "*.txt" || args[i] == "*.java" || args[i] == "*.cpp" || args[i] == "*.c")
                {
                     flag=1;
                    fm.addPattern(args[i]);
                }
                else if (args[i]== "/S"||args[i]=="/s")
                {
                    fm.recurse = true;
                }

                else
                {
                    int pos = args[i].LastIndexOf('\\');
                    if (pos == -1)
                    {
                        pos = args[i].LastIndexOf('/');


                    }

                    if (pos > -1)
                    {
                        ++pos;
                        path = args[i].Remove(pos, args[i].Length - pos);
                        pattern = args[i].Remove(0, pos);
                    }
                    else
                    {
                        path = ".";
                       // flag1 = 1;
                    }
                }

            }
            

            if (flag == 0)
            {
                fm.addPattern("*.*");
            }


                try
                {
                    fm.findFiles(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine("error {0} \n ", e.Message);
                }
                List<string> files = fm.getFiles();
                if (files.Count == 0)
                {
                    Console.WriteLine("\n no files are present \n");

                }
                else
                {
                    if(path==".")
                        
                    {
                        Console.WriteLine("Path is considered as current directory as no path is specified\n");
                    }
                    foreach (string file in files)
                    {
                        Console.Write("\n  {0}", file);
                        if (Path.GetExtension(file) == ".cs" || Path.GetExtension(file) == ".txt")
                  {
                      fm.cSharpFiles.Add(file) ;
                  }
                    
                    }
                }



                    
            }
            
            
        }
#endif
    }


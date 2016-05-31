////////////////////////////////////////////////////////////////////////
//CommandLine.cs - Parses the command line arguments fiven by the     //
//                   executive.                                       //
//and write the output in an XML File                                 //
// ver 1.0                                                            //
// Language:    C#, 2008, .Net Framework 3.5                          //
// Platform:   DellInspiron 15 5000 series                            //
// Application: Demonstration for CSE681, Project #2, Fall 2014       //
// Source:      Jim Fawcett, CST 4-187, Syracuse University           //
//              (315) 443-3948, jfawcett@twcny.rr.com                 //
// Author:      SahtihiDesu, Syracuse Universiy                       //
//               sldesu@syr.edu                                       //
///////////////////////////////////////////////////////////////////////
//*
 //* Module Operations:
// * ------------------
 //*Parses all the commangline arguments and will give them back to executive
//* 
 //* 
 ////* Maintenance History:
// * -----------------
// * 
// * ver 1.0 : Oct 1 2014
// * - first release
//************************************************************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalysis;
namespace CodeAnalysis
{
    public class CommandLine
    {
        string path = ".";
        string pattern = " ";
        public void setpath(string path)
        {
            this.path = path;

        }
        public void setpattern(string pattern)
        {
            this.pattern = pattern;
        }
        public string getpath()
        {
            return this.path;
        }
        public string getpattern()
        {
            return this.pattern;
        }

        public List<string> processcmd(string[] args)
        {
             int flag = 0;
            int pathf = 0;
         
            int count = 0;
            CodeAnalysis.FileMgr fmgr = new CodeAnalysis.FileMgr();
            CodeAnalysis.display ds = new CodeAnalysis.display();
          
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "*.cs" || args[i] == "*.txt" || args[i] == "*.java" || args[i] == "*.cpp" || args[i] == "*.c")
                {
                  
                    fmgr.addPattern(args[i]);
                    count++;

                }
                else if (args[i] == "/S" || args[i] == "/s")
                {
                    fmgr.setBool(true);
                    Console.WriteLine("\n Displaying all the files in the directory  \n");
                }
                else if (args[i] == "/r" || args[i] == "/R")
                {
                    ds.SetBoolrelations(true);


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
                        if (pathf == 0)
                        {
                            ++pos;
                            string temp="";
                            string s;
                            s = args[i].Remove(0, pos);
                          
                            if (s.Contains(".cs") || s.Contains(".txt") || s.Contains(".java") || s.Contains(".cpp") || s.Contains(".c"))
                            {
                                setpattern(args[i].Remove(0, pos));
                                fmgr.addPattern(args[i].Remove(0, pos));
                                setpath(args[i].Remove(pos, args[i].Length - pos));
                               
                                
                                temp = s;
                            }
                            else
                            {
                               
                                setpath(args[i]);

                            }
                            pathf = 1;
                            if((s==temp))
                            flag = 1;
                        }
                    }
                    else
                    {
                        if ((pathf == 0)&&(path!="."))
                        {
                            Console.WriteLine("Path is not specified \n");
                            setpath(".");


                        }
                          
                    }
                }

            }


            if ((count==0)&&(flag==0))
            {
                fmgr.addPattern("*.*");
             }


            try
            {
                fmgr.findFiles(getpath());
            }
            catch (Exception e)
            {
                Console.WriteLine("error {0} \n ", e.Message);
            }
            List<string> files = fmgr.getFiles();

            return files;
        }
#if(TEST_COMMANDLINE)
        public static void Main(string[] args)
        {
            CodeAnalysis.CommandLine cmd = new CommandLine();

            string[] arguments = new String[args.Length];
            arguments = args;
            List<string> files = new List<string>();
            files = cmd.processcmd(arguments);

            for (int i = 0; i < args.Length; i++)
            {

                if (args[i] == "/r" || args[i] == "/R")
                {
                    Console.WriteLine("\n '/R' Command Detected ");

                }
                else if (args[i] == "/S" || args[i] == "/s")
                {
                    Console.WriteLine("\n '/S' Command Detected ");


                }
                else if (args[i] == "/X" || args[i] == "/x")
                {
                    Console.WriteLine("\n '/X' Command Detected ");


                }

            }
        }
        
#endif
    }
}

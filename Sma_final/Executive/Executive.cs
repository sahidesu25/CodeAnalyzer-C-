
/////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////
//Executive.cs - Main Entry for the project                          //
// ver 1.0                                                           //
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
 * Creates an instance of display, Commandline Processing, FileManager, Analyzer.
 * -Passes the arguments to commandline 
 * -get the files list from 
/* Required Files:
 *   Analyzer.cs,FieMgr.cs,Display.cs
 * 
 * Maintenance History:
 * --------------------
 * 
 * ver 1.0 : Oct 1 2014
 * - first release
************************************************************************************************************************** */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalysis;
using System.IO;
namespace Executive
{
    public class Executive
    {

        static void Main(string[] args)
        {

            Console.WriteLine("             \t\t   CODE ANALYZER");
            Console.WriteLine("\n ------------------------------------------------------------------------");


            display ds = new display();
            FileMgr fm = new FileMgr();
            CodeAnalysis.CommandLine cmd = new CommandLine();

            string[] arguments = new String[args.Length];
            arguments = args;
            List<string> files = new List<string>();
            files = cmd.processcmd(arguments);

            for (int i = 0; i < args.Length;i++ )
            {

                if (args[i] == "/r" || args[i] == "/R")
                {
                    ds.SetBoolrelations(true);
                    Console.WriteLine(" /R command executed ");
                }
                else if (args[i] == "/S" || args[i] == "/s")
                {
                    Console.WriteLine(" /S command executed ");
                    fm.setBool(true);
                    
                }
                else if (args[i] == "/X" || args[i] == "/x")
                {
                    Console.WriteLine(" /X command executed ");
                    ds.SetBoolXml(true);
                   
                }
                
            }



               
                if (files.Count == 0)
                {
                    Console.WriteLine("\n No Files are Present \n");

                }
                else
                {
                   
                    foreach (string file in files)
                    {
                        
                        if (Path.GetExtension(file) == ".cs" || Path.GetExtension(file) == ".txt")
                        {
                            fm.cSharpFiles.Add(file);
                        }

                    }
                }
                          
            
            Analyzer.doAnalysis(fm.getCSharpFile());

                Console.WriteLine("\n Summary of all Types across all the files\n\n");
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine();
            ds.displayTypeSummary();
            if (ds.GetBoolrelations())
            {
                Console.WriteLine("\n-----------------------------------------------------------");
                Console.WriteLine("\nRelationship Summary between all the files                       ");
                Console.WriteLine("-----------------------------------------------------------");
                ds.SetBoolrelations(true);
                ds.displayRelationTable();

            }
            if (ds.GetBoolXml() == true)
            {
                Console.WriteLine("\n-----------------------------------------------------------");
                Console.WriteLine("  XML file has been created\n");
                Console.WriteLine("\n-----------------------------------------------------------");
                ds.writeXMLFile();
                Console.ReadKey();
            }
            }

        }
    }



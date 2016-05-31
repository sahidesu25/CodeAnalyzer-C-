//////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////
//Analyzer.cs - Manages Code Analysis                                //
//and write the output in an XML File                                //
// ver 1.3                                                           //
// Language:    C#, 2008, .Net Framework 3.5                         //
// Platform:   DellInspiron 15 5000 series                           //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
// Author:      SahtihiDesu, Syracuse Universiy                      //
//               sldesu@syr.edu                                      //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 *  Invokes Semi Expression  
 *  parses semi expression obtained.
 * - Passes the arguments to commandline  
 *
 * 
 * Maintenance History:
 * --------------------
 * 
 * ver 1.0 : Oct 2 2014
 * - first release
************************************************************************************************************************** */






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Analyzer
    {
        public static List<Elem> AllTypesTable=new List<Elem>();
        public static List<Elem2> AllClassRelationTable = new List<Elem2>();
        static public string[] getFiles(string path, List<string> patterns)
        {
            FileMgr fm = new FileMgr();
            foreach (string pattern in patterns)
                fm.addPattern(pattern);
            fm.findFiles(path);
            return fm.getFiles().ToArray();
        }

       public static void doAnalysis(string[] files)
        {
            
            
               

                List<Elem> UserTypeTable = new List<Elem>();
                List<Elem2> classrelationtable = new List<Elem2>();
                List<Elem2> RelationPrint = new List<Elem2>();

                foreach (object file in files)
                {
                    Console.Write("\nProcessing file(s) : \n {0}\n\n ", file as string);

                    CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                    semi.GetType();
                    semi.displayNewLines = false;
                    if (!semi.open(file as string))
                    {
                        Console.Write("\n  Can't open {0}\n\n", file);
                        return;
                    }

                    Console.Write("\n  Type and Function Analysis");
                    Console.Write("\n ----------------------------\n");

                    BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                    Parser parser = builder.build();

                    try
                    {
                        while (semi.getSemi())
                            parser.parse(semi);
                       Console.Write("\n\n  locations table contains : \n ");
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }
                    Repository rep = Repository.getInstance();
                    List<Elem> table = rep.locations;
                    
                    List<Elem> classtable = rep.classLocations;
                    AllTypesTable.AddRange(table);

                    UserTypeTable.AddRange(classtable);
                    foreach (Elem e in table)
                    {
                        Console.Write("\n  {0,10}, {1,25}, {2,5}, {3,5}", e.type, e.name, e.begin, e.end);
                    }
                   
                  
                  Console.WriteLine();
                   
                    semi.close();
                }


                //Console.WriteLine("*********NEW PARSING**********");
                foreach (object file in files)
                {
                   // Console.Write("\n  Processing file {0}\n", file as string);

                    CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                    semi.GetType();
                    semi.displayNewLines = false;
                    if (!semi.open(file as string))
                    {
                        Console.Write("\n  Can't open {0}\n\n", file);
                       return;
                    }

                   
                    Console.Write("\n ----------------------------------------------------\n");

                    Repository rep1 = Repository.getInstance();
                    Repository.SetList = UserTypeTable;


                    BuildCodeAnalyzer1 builder1 = new BuildCodeAnalyzer1(semi);
                    Parser parser1 = builder1.build();

                    try
                    {
                        while (semi.getSemi())
                            parser1.parse(semi);
                      //  Console.Write("\n\n  Displaying relationships:");
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }
                    Repository rep2 = Repository.getInstance();
                    foreach (Elem2 e in rep2.RelationsList)
                    {
                        
                            classrelationtable.Add(e);
                        
                    }

                  IEnumerable<Elem2> distinctElem2 = classrelationtable.Distinct(new Elem2Comparer());

                   AllClassRelationTable.AddRange(distinctElem2);


                 /*  foreach (Elem2 e in distinctElem2)
                    {
                        if (e.aggregation == true)
                            Console.Write("\n  {0}, aggregates, {1}", e.classFrom, e.classTo);

                    }

                   foreach (Elem2 e in distinctElem2)
                    {
                        if (e.composition == true)
                            Console.Write("\n  {0}, is composing , {1}", e.classFrom, e.classTo);

                    }

                   foreach (Elem2 e in distinctElem2)
                    {
                        if (e.inheritance == true)
                            Console.Write("\n  {0}, is inheriting , {1}", e.classFrom, e.classTo);

                    }

                   foreach (Elem2 e in distinctElem2)
                    {
                        if (e.usingType == true)
                            Console.Write("\n  {0}, is using , {1}", e.classFrom, e.classTo);

                    }*/
        
                }
            }


        
#if(TEST_ANALYZER)
        static void Main(string[] args)
        {
            string path=" ";
            List<string> patterns = new List<string>();
            int pos = args[0].LastIndexOf('\\');
            if (pos == -1)
            {
                pos = args[0].LastIndexOf('/');

            }
            else if (pos > -1)
            {
                path = args[0];
            }
            else
            {
                path = ".";

            }
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "*.cs" || args[i] == "*.txt")

                    patterns.Add(args[i]);
                else
                    Console.WriteLine("Please provide a valid pattern, only .cs and .txt files are accepted");
            }
           
            string[] files = Analyzer.getFiles(path, patterns);
            doAnalysis(files); 

            //string path = "../../";
            //List<string> patterns = new List<string>();
            //patterns.Add("*.cs");
            //
            //doAnalysis(files);
        }
  
    }
#endif
}

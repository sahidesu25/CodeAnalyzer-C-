///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 3.5                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
//                                                                   //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com
//Author:       Sahithi Desu, Syracuse University
//               sldesu@syr.edu    
//
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   -Detect Enum Rule
 *   -Detect anonymous scopes
 *   
 * 
 * It also defines the following set of rules which are used in the second parse
 * -Detect Aggregation 
 * -Detect Composition
 * -Detect Inheritance
 * -Detect Using
 * 
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 *   -Store relations in repository
 *   
 * During the Second parse the enum and the function were taken as the anonymous scopes along with the others.
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * Ver 2.3 : Oct 2 2014
 * -Added new Build Code analyzer class which configures the parser for the new set of rules 
 *    written for the second parse.
 * -Added actions in the Pushstack class for performing actions when the any relation ship rule is detected
 * -Added actions in the Pop stack to calculate the complexity for a function.
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis
{
    public class Elem // holds scope information
    {

        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public int complexity { get; set; }//****************************
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }
    public class Elem2 
    {
       
        public string classFrom { get; set; }
        public string classTo { get; set; }
        public bool inheritance { get; set; }
        public bool composition { get; set; }
        public bool usingType { get; set; }
        public bool aggregation { get; set; }


        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", classFrom));
            temp.Append(String.Format("{0,-10}", classTo));
            temp.Append(String.Format("{0,-5}", inheritance.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", composition.ToString()));
            temp.Append(String.Format("{0,-5}", aggregation.ToString()));
            temp.Append(String.Format("{0,-5}", usingType.ToString()));// line of scope end
            temp.Append("}");
            return temp.ToString();
        }
   

    }
    public class Elem2Comparer : IEqualityComparer<Elem2>
    {
        public bool Equals(Elem2 x, Elem2 y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return (x.classFrom == y.classFrom && x.classTo == y.classTo
                               && x.composition == y.composition && x.aggregation == y.aggregation
                               && x.inheritance == y.inheritance && y.usingType == y.usingType);



        }

        public int GetHashCode(Elem2 elem2)
        {
            if (Object.ReferenceEquals(elem2, null)) return 0;
            int hashClassFrom = elem2.classFrom == null ? 0 : elem2.classFrom.GetHashCode();
            int hashClassTo = elem2.classTo.GetHashCode();
            int hashinheritance = elem2.inheritance.GetHashCode();
            int hashcomposition = elem2.composition.GetHashCode();
            int hashusingType = elem2.usingType.GetHashCode();
            int hashaggregation = elem2.aggregation.GetHashCode();
            return hashClassFrom ^ hashClassTo ^ hashaggregation ^ hashcomposition ^ hashusingType ^ hashinheritance;
        }
    }


    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>();
        List<Elem> ClassList = new List<Elem>();
        public List<Elem2> RelationsList = new List<Elem2>();
        public static List<Elem> SetList = new List<Elem>();
        public int complexityCount = 0;
        static Repository instance;
       



        public Repository()
        {
            instance = this;
        }
        // To access the Previous complete UserTypeList obtained in the first List
        public void setList(List<Elem> setList)
        {
            // this.SetList= setList;
            
        }

        // To get the Previous complete UserTypeList obtained in the first List
        public List<Elem> getList()
        {
            return SetList;
        }


        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
        }

        public List<Elem> classLocations
        {
            get { return ClassList; }
        }
    }

    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            elem.complexity = -1;
            repo_.stack.push(elem);
            if (elem.type == "control" || elem.name == "anonymous")
                return;
            repo_.locations.Add(elem);
            if ((elem.type == "class") || (elem.type == "struct") || (elem.type == "enum") || (elem.type=="interface"))
                repo_.classLocations.Add(elem);


            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            

            Elem elem;
            try
            {
                elem = repo_.stack.pop();
                if (elem.type == "control")
                {
                    repo_.complexityCount = repo_.complexityCount + 1;

                }
                else if (elem.type == "function")
                {
                    elem.complexity = repo_.complexityCount;
                    repo_.complexityCount = 0;
                    if(AAction.displaySemi)
                    Console.WriteLine("The Function Complexity is {0}", elem.complexity);

                }
                // if(elem.type == "")
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                
                
                    Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                    Console.Write("leaving  ");
                    string indent = new string(' ', 2 * (repo_.stack.count + 1));
                    Console.Write("{0}", indent);
                    this.display(local); // defined in abstract action
                    
                
            }
        }
    }
    /// <summary>
    /// ////Action for second parse./############################///////////////////////////////////////////////////////
    /// </summary>
    public class PushStack1 : AAction
    {

        Repository repo_;

        public PushStack1(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            // int f=0;
            Elem elem = new Elem();
           // string colon = "";
            string baseClassName = "";
            string baseClassType = "";
            List<Elem> LocalUserTypeTable = Repository.SetList;
            elem.type = semi[0];  // expects type
            elem.name = semi[1];
            


          //  if(semi.count==4)
           // {
              ////  colon = semi[2].ToString();
               // baseClassName = semi[3].ToString();
           // }


            /////////////////////@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@/////////////////////////////////////
            if (elem.type == "class" || elem.type == "new")
            {
                int flag = 0;
                foreach (Elem e in LocalUserTypeTable)
                {
                    if (elem.name == e.name)
                    {
                        repo_.stack.push(elem);
                        flag = 1;
                        break;
                    }

                }
                if (elem.type == "new" && flag == 1)
                {
                    for (int i = repo_.stack.count - 1; i > 0; i--)
                    {
                        if (repo_.stack[i].type == "class")
                        {
                            Elem2 el = new Elem2();
                            string classname = repo_.stack[i].type.ToString() + " " + repo_.stack[i].name.ToString();
                            el.classFrom = classname;
                            el.classTo = "class " + elem.name.ToString();
                            el.aggregation = true;
                            repo_.RelationsList.Add(el);
                            //break;

                        }
                    }
                }
            }
            else if ((elem.type == "enum") || (elem.type == "struct"))
            {
                int flag = 0;
                foreach (Elem e in LocalUserTypeTable)
                {
                    if (elem.name == e.name)
                    {
                        repo_.stack.push(elem);
                        flag = 1;
                        break;
                    }

                }

                /////////@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@///////////////////////////////////////////////////////////////////////////
                if (flag == 1)
                {
                    for (int i = repo_.stack.count - 1; i > 0; i--)
                    {
                        if (repo_.stack[i].type == "class")
                        {
                            Elem2 el = new Elem2();
                            string classname = repo_.stack[i].type.ToString() + " " + repo_.stack[i].name.ToString();
                            el.classFrom = classname;
                            el.classTo =" "+ elem.type + " " + elem.name.ToString();
                            el.composition = true;
                            repo_.RelationsList.Add(el);
                            break;
                        }
                    }
                }

            }
            else if(semi[1] == ":" )
            {

                string[] split = semi[0].Split(new char[] { ',' });

                int flag1 = 0;
                int flag2 = 0;
                foreach (Elem e in LocalUserTypeTable)
                {
                    if (split[0] == e.name)
                    {
                        
                        flag1 = 1;
                        break;
                    }

                }
                foreach (Elem e in LocalUserTypeTable)
                {
                    if (split[1] == e.name)
                    {

                        flag2 = 1;
                        baseClassType = e.type;
                        break;
                    }

                }
                if(flag1==1 && flag2==1)
                {
                    Elem2 el = new Elem2();
                   // string classname = .type.ToString() + " " + repo_.stack[i].name.ToString();
                    el.classFrom = "class" + " " + split[0];
                    el.classTo = " " +baseClassType+" "+split[1]+ " " ;
                    el.inheritance = true;
                    repo_.RelationsList.Add(el);
                    elem.type = "class";
                    elem.name = split[0];
                    repo_.stack.push(elem);
                    
                }



            }
            else if ( semi[0] =="function" && semi[semi.count-1]==")")
            {
                string classname = "";
               
                
                for (int i = repo_.stack.count - 1; i > 0; i--)
                    {
                        if (repo_.stack[i].type == "class")
                        {
                             classname = repo_.stack[i].type.ToString() + " " + repo_.stack[i].name.ToString();
                            break;
                        }
                }
                for(int i=2;i<semi.count-1;i++)
                        {
                            Elem2 el = new Elem2();
                            el.classFrom = classname;
                            el.classTo =" "+"class" + " " + semi[i].ToString();
                            el.usingType = true;
                            repo_.RelationsList.Add(el);
                            
            
                }
                repo_.stack.push(elem);
            }



            else
            {
                repo_.stack.push(elem);
            }

            //  if (elem.type == "control" || elem.name == "anonymous")
            // return;
            // repo_.locations.Add(elem);
            // if ((elem.type == "class") || (elem.type == "struct") || (elem.type == "enum"))
            // repo_.classLocations.Add(elem);


            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }


    }
    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
           int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }


    public class DetectEnum : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("enum");



            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int indexf = semi.FindFirst("(");
            int indexl = semi.FindLast(")");

            if (indexf > 0 && !isSpecialToken(semi[indexf - 1]))
            {
                String functionarg = "";
                for (int i = indexf; i <= indexl; i++)
                {
                    functionarg = functionarg + semi[i];
                }
                semi[indexf - 1] = semi[indexf - 1] + functionarg;
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[indexf - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {


            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            else
            {
                string[] SpecialToken = { "if", "for", "else", "foreach", "while" };
                foreach (string stoken in SpecialToken)
                {
                    int temp = semi.Contains(stoken);
                    if (temp != -1)
                    {
                        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        // create local semiExp with tokens for type and name
                        local.displayNewLines = false;
                        local.Add("control").Add("anonymous");
                        doActions(local);
                        return true;
                    }
                }

            }
            return false;
        }
    }


    /// <summary>
    /// //////////detecting aggregation rule///////////////////////////////////////////
    /// </summary>
    public class DetectAgg : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("new");



            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// ///Detecting Composition rule/////////////////////////////////////////////////
    /// </summary>

    public class DetectComposition : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            List<Elem> LookupList = new List<Elem>();
            LookupList = Repository.SetList;
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            //look = semi;
            foreach (Elem el in LookupList)
            {
                int index = semi.FindFirst(el.name);
                int index1 = semi.FindLast("{");///////confirm and take action /////it doesnt allow struct now;
                if (index != -1 && index1 == -1)
                {
                    local.displayNewLines = false;
                    local.Add(el.type).Add(semi[index]);
                    doActions(local);
                    return true;
                }

            }
            return false;


        }
    }
    /// <summary>
    /// //detecting inheritance rule//////////////////////////
    /// </summary>
    /// 



    public class DetectInheritance: ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains(":");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index - 1] + "," + semi[index + 1]).Add(semi[index]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    ///////////////////Detecting Using Rule///////////////////
    public class DetectUsing : ARule
    {
        public static bool isSpecialToken(string token)

        {
           
        
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            List<Elem> LocalUserTypeTable = Repository.SetList;
            //bool returnVal = false;
           // String argList = "";
            string functionarg = "";
            int flag = 0;
           // functionarg = "";
            if (semi[semi.count - 1] != "{")
                return false;

            int indexf = semi.FindFirst("(");
            int indexl = semi.FindLast(")");
            

            if (indexf > 0 && !isSpecialToken(semi[indexf - 1]))
            {
                local.Add("function");
                
                local.Add(semi[indexf-1]);
               
                for (int i = indexf+1; i <= indexl; i=i+1)
                {
                    
                    
                    if (semi[i+1]=="," || semi[i+1]==")")
                    {
                       
                    }
                    else
                    {
                        if (semi[i]==")")
                        {

                        }
                        else
                        {
                            functionarg = functionarg + semi[i];
                        }
                    }

                }
                string[] functionArgSplit = functionarg.Split(',');

                foreach (Elem e in LocalUserTypeTable)
                {
                    for (int i = 0; i < functionArgSplit.Length; i++)
                    {
                        if (functionArgSplit[i] == e.name)
                        {

                            local.Add(e.name);
                           // returnVal = true;
                            flag = 1;
                        }


                    }
                }

                local.Add(")");
                if(flag==1)
                {
                doActions(local);
                return true;
                }
                



            }
            return false;
        }
        
    }
        /////////////////////////////////////////////////////////
        // detect leaving scope

        public class DetectLeavingScope : ARule
        {
            public override bool test(CSsemi.CSemiExp semi)
            {
                int index = semi.Contains("}");
                if (index != -1)
                {
                    doActions(semi);
                    return true;
                }
                return false;
            }
        }
        public class BuildCodeAnalyzer
        {
            Repository repo = new Repository();

            public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
            {
                repo.semi = semi;
            }
            public virtual Parser build()
            {
                Parser parser = new Parser();

                // decide what to show
                AAction.displaySemi = true;
                AAction.displayStack = false;  // this is default so redundant

                // action used for namespaces, classes, and functions
                PushStack push = new PushStack(repo);

                // capture namespace info
                DetectNamespace detectNS = new DetectNamespace();
                detectNS.add(push);
                parser.add(detectNS);

                // capture class info
                DetectClass detectCl = new DetectClass();
                detectCl.add(push);
                parser.add(detectCl);
                //capture enum info
                DetectEnum detectEN = new DetectEnum();
                detectEN.add(push);
                parser.add(detectEN);

                // capture function info
                DetectFunction detectFN = new DetectFunction();
                detectFN.add(push);
                parser.add(detectFN);

                // handle entering anonymous scopes, e.g., if, while, etc.
                DetectAnonymousScope anon = new DetectAnonymousScope();
                anon.add(push);
                parser.add(anon);

                // handle leaving scopes
                DetectLeavingScope leave = new DetectLeavingScope();
                PopStack pop = new PopStack(repo);
                leave.add(pop);
                parser.add(leave);

                // parser configured
                return parser;
            }
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////
        /// For second parse
        /// //////////////////////////////////////////////////////////////////
        /// </summary>
        public class BuildCodeAnalyzer1
        {
            Repository repo = new Repository();

            public BuildCodeAnalyzer1(CSsemi.CSemiExp semi)
            {
                repo.semi = semi;
            }
            public virtual Parser build()
            {
                Parser parser = new Parser();

                // decide what to show
                AAction.displaySemi = false;
                AAction.displayStack = false;  // this is default so redundant

                // action used for namespaces, classes, and functions
                PushStack1 push = new PushStack1(repo);

                // capture namespace info
                DetectNamespace detectNS = new DetectNamespace();
                detectNS.add(push);
                parser.add(detectNS);

                DetectInheritance inher = new DetectInheritance();
                inher.add(push);
                parser.add(inher);

                // capture class info
                DetectClass detectCl = new DetectClass();
                detectCl.add(push);
                parser.add(detectCl);
                
                //Detection Aggregation
                DetectAgg agg = new DetectAgg();
                agg.add(push);
                parser.add(agg);

                //Detection Composition
                DetectComposition com = new DetectComposition();
                com.add(push);
                parser.add(com);

                //Detect Using
                DetectUsing usin = new DetectUsing();
                usin.add(push);
                parser.add(usin);

                //Detect anon
                DetectAnonymousScope anon = new DetectAnonymousScope();
                anon.add(push);
                parser.add(anon);

                //Handle leaving scopes
                DetectLeavingScope leave = new DetectLeavingScope();
                PopStack pop = new PopStack(repo);
                leave.add(pop);
                parser.add(leave);

                // parser configured
                return parser;
            }
        }
    }

    
    
    



//////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////
//Display.cs - Displays output on the console                        //
//and write the output in an XML File                                //
// ver 1.0                                                           //
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
 *Display all the types and the relations from the list created in the repository
 *Creates an XML file and writes all the relations to the file
/* 
 * 
 * Maintenance History:
 * --------------------
 * 
 * ver 1.0 : Oct 4 2014
 * - first release
************************************************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalysis;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace CodeAnalysis
{
    public class display
    {
        bool xmldisplay = false;
        bool relations = false;
        XmlWriter writer;
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode;

        public void SetBoolXml(bool option)
        {
            this.xmldisplay = option;
        }
        public bool GetBoolXml()
        {
            return (this.xmldisplay);
        }
        public void SetBoolrelations(bool option)
        {
            this.relations = option;
        }
        public bool GetBoolrelations()
        {
            return (this.relations);
        }

        //---------------------------------DISPLAYING TYPE SUMMARY------------------------------------------

        public void displayTypeSummary()
        {
            List<Elem> displaytypes = new List<Elem>();
            displaytypes = CodeAnalysis.Analyzer.AllTypesTable;
            foreach (Elem e in displaytypes)
            {
                Console.Write("\n  {0,10}, {1,25}", e.type, e.name);
            }
        }

        //---------------------------------WRITING THE XML FILE-----------------------------------------------

        public void writeXMLFile()
        {
            //------------------------XML FOR TYPES SUMMARY--------------------------------
            List<Elem> displaytypes = new List<Elem>();
            displaytypes = CodeAnalysis.Analyzer.AllTypesTable;

            rootNode = xmlDoc.CreateElement("Output");
            xmlDoc.AppendChild(rootNode);
            XmlNode root1 = xmlDoc.CreateElement("Types_Output");
            rootNode.AppendChild(root1);
            xmlDoc.Save("Output.xml");

            XmlNode node, node1;
            int counter = 0;
            foreach (Elem e in displaytypes)
            {
                counter++;
                node = xmlDoc.CreateElement("TypeExample", counter.ToString());
                root1.AppendChild(node);

                node1 = xmlDoc.CreateElement("Type");
                node1.InnerXml = e.type.ToString();
                node.AppendChild(node1);
                node1 = xmlDoc.CreateElement("Name");
                node1.InnerXml = e.name.ToString();
                node.AppendChild(node1);

            }
            xmlDoc.Save("Output.xml");

            //------------------------XML FOR RELATION TABLE--------------------------------

            List<Elem2> displayrelations = new List<Elem2>();
            displayrelations = CodeAnalysis.Analyzer.AllClassRelationTable;
            IEnumerable<Elem2> distinctrelations = displayrelations.Distinct(new Elem2Comparer());

            XmlNode root2 = xmlDoc.CreateElement("Relationships_Output");
            rootNode.AppendChild(root2);

            xmlDoc.Save("Output.xml");

            XmlNode node_, node1_;
            int counter_ = 0;

            //--------------------------------------------------------------------------------------------
            foreach (Elem2 e in distinctrelations)
            {
                if (e.aggregation == true)
                {
                    counter_++;

                    node_ = xmlDoc.CreateElement("Relationships_Example", counter_.ToString());
                    root2.AppendChild(node_);

                    node1_ = xmlDoc.CreateElement("Type_From");
                    xmlDoc.DocumentElement.RemoveAttribute("xmlns");
                    node1_.InnerXml = e.classFrom.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Type_To");
                    node1_.InnerXml = e.classTo.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Relation");
                    node1_.InnerXml = "Aggregation";
                    node_.AppendChild(node1_);
                }

            }
            xmlDoc.DocumentElement.RemoveAttribute("xmlns");

            xmlDoc.Save("Output.xml");

            foreach (Elem2 e in distinctrelations)
            {
                if (e.composition == true)
                {
                    counter_++;

                    node_ = xmlDoc.CreateElement("Relationships_Example", counter_.ToString());
                    root2.AppendChild(node_);

                    node1_ = xmlDoc.CreateElement("Type_From");
                    node1_.InnerXml = e.classFrom.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Type_To");
                    node1_.InnerXml = e.classTo.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Relation");
                    node1_.InnerXml = "Composition";
                    node_.AppendChild(node1_);
                }

            }
            xmlDoc.Save("Output.xml");

            foreach (Elem2 e in distinctrelations)
            {
                if (e.inheritance == true)
                {
                    counter_++;

                    node_ = xmlDoc.CreateElement("Relationships_Example", counter_.ToString());
                    root2.AppendChild(node_);

                    node1_ = xmlDoc.CreateElement("Type_From");
                    node1_.InnerXml = e.classFrom.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Type_To");
                    node1_.InnerXml = e.classTo.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Relation");
                    node1_.InnerXml = "Inheritance";
                    node_.AppendChild(node1_);
                }

            }
            xmlDoc.Save("Output.xml");

            foreach (Elem2 e in distinctrelations)
            {
                if (e.usingType == true)
                {
                    counter_++;
                    node_ = xmlDoc.CreateElement("Relationships_Example", counter_.ToString());
                    root2.AppendChild(node_);

                    node1_ = xmlDoc.CreateElement("Type_From");
                    node1_.InnerXml = e.classFrom.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Type_To");
                    node1_.InnerXml = e.classTo.ToString();
                    node_.AppendChild(node1_);
                    node1_ = xmlDoc.CreateElement("Relation");
                    node1_.InnerXml = "Using";
                    node_.AppendChild(node1_);
                }

            }
            xmlDoc.Save("Output.xml");
        }


        //----------------------------------------DISPLAYING THE RELATIONSHIP TABLE------------------------------------

        public void displayRelationTable()
        {

            if (GetBoolrelations())
            {

                List<Elem2> displayrelations = new List<Elem2>();
                displayrelations = CodeAnalysis.Analyzer.AllClassRelationTable;
                IEnumerable<Elem2> distinctrelations = displayrelations.Distinct(new Elem2Comparer());

                //------------------------------------------------------------------------------------------  


                using (writer)
                {


                    //--------------------------------------------------------------------------------------------
                    foreach (Elem2 e in distinctrelations)
                    {
                        //writer.WriteStartElement("Aggregation Group");

                        if (e.aggregation == true)
                        {
                            Console.Write("\n  {0}, Aggregates, {1}", e.classFrom, e.classTo);
                        }

                    }


                    foreach (Elem2 e in distinctrelations)
                    {
                        if (e.composition == true)
                        {
                            Console.Write("\n  {0}, is composing , {1}", e.classFrom, e.classTo);
                        }

                    }

                    foreach (Elem2 e in distinctrelations)
                    {
                        if (e.inheritance == true)
                        {
                            Console.Write("\n  {0}, is inheriting , {1}", e.classFrom, e.classTo);
                        }

                    }

                    foreach (Elem2 e in distinctrelations)
                    {
                        if (e.usingType == true)
                        {
                            Console.Write("\n  {0}, is using , {1}", e.classFrom, e.classTo);
                        }

                    }

                }

            }


        }


    }
}


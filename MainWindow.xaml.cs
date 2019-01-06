using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LLPARSER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            
        }



        public static Grammar Grammar { get; set; }
        public static DataTable table;
        public static string[] stack;

        private void GrammarClick(object sender, RoutedEventArgs e)
        {

            string text = System.IO.File.ReadAllText(@"C:\Users\Teodora\Documents\LLParser\text.txt");
            Reguli.AppendText("P={\n");
            Reguli.AppendText(text);
            Reguli.AppendText("\n}");


            string[] lines =System.IO.File.ReadAllLines(@"C:\Users\Teodora\Documents\LLParser\text.txt");

            List<string> mylines = lines.ToList();


            string S = lines[0];

            string[] nt = lines[1].Split(' ');

            string[] t = lines[2].Split(' ');

            List<string> terminals = new List<string>();
            
            terminals.AddRange(t);

            List<string> nterminals = new List<string>();
            nterminals.AddRange(nt);




            List<string[]> productRules = new List<string[]>();


            // Regulile de PRODUCTIE incep de pe pozitia 4
            for (int i = 4;i <(int.Parse(lines[3])+3);i++ )
            {

                productRules.Add(lines[i].Split());
           
            }
            

            
            Grammar = new Grammar(S,int.Parse(lines[3]),terminals,nterminals,productRules);
            Grammar.VerifyAmbiguity();
            Grammar.VerifyRecursivity();


            



            productRules = Grammar.ProductRules;

            Reguli.AppendText("\n\n Dupa rezolvarea ambiguitatilor: " +
                "\nP={\n");
            int j = 0;
             while( j < productRules.Count)
                {
                if(productRules[j].Length!=0)
                    Reguli.AppendText(string.Join(" ",productRules[j]) + "\n");
                    j++;
                }
            
            Reguli.AppendText("\n }");

        



            //Generare tabel
           //table= GetTable();

        }


        //Returneaza terminalele si nt pt care exista nt-> nt sau t
        // 
        public static string[] GetList(string nt)
        {
            string[] list = new string[20];
            int nr = 0;

            int nrRule = 0;

            foreach (string[] r in Grammar.Directory)
            {
                nrRule++;
                if (r[0]=="first ")
                {
                    
                    int i = 5;
                    if(nt==r[1].Substring(0,1) || nt==r[1].Substring(0,2))
                    {
                        if (r[i] == null)
                            break;
                        while (i!=r.Length)
                        {
                            if (r[i] == null)
                                break;

                            if (r[i] == "\\")
                                break;

                            list[nr] = r[i];
                            list[nr + 1] = nrRule.ToString();
                            i++;
                            nr=nr+2;
                        }
                    }

                }
                else if(r[0]=="follow")
                {
                    if(nt==r[2])
                    {
                        int i = 5;
                        while(i!=r.Length)
                        {
                            if (r[i] == null)
                                break;
                            if (r[i] == "\\")
                                break;

                            list[nr] = r[i];
                            list[nr + 1] = nrRule.ToString();
                            i++;
                            nr=nr+2;

                        }
                        
                    }
                }
            }

            return list;
        }
        private static DataTable GetTable()
        {
            DataTable table = new DataTable();

            DataRow row=null;

            int c = 1;

            DataColumn coloana = new DataColumn();
            coloana.ColumnName = "*";
            table.Columns.Add(coloana);
                
            foreach (string t in Grammar.Terminals)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = t;
                table.Columns.Add(col);


                c++;

            }
            coloana = new DataColumn();
            coloana.ColumnName = "$";
            table.Columns.Add(coloana);



            foreach (string nt in Grammar.Nterminals)
            {
                row = table.NewRow();
                row[0] = nt;


                string[] l = GetList(nt);


                int j = 0;

                foreach (DataColumn item in table.Columns)
                {
                    int i = 0;

                    string x = l[i];
                    while (x != null)
                    {
                        if (x == item.ToString())
                        {

                            row[j] = "R" + l[i + 1];
                            break;
                        }
                        i++;
                        x = l[i];
                    }
                    j++;

                }

                for (int it = 1; it < row.ItemArray.Count(); it++)
                {
                    if (row[it].ToString() == "")
                    {
                        row[it] = "Error";
                    }

                  
                }


               


                table.Rows.Add(row);

            }
            
            foreach (string t in Grammar.Terminals)
            {
                row = table.NewRow();
                row[0] = t;

                foreach(DataColumn col in table.Columns)
                {
                    if (col.ToString() == t)
                        row[col] = "Match";
                }
                table.Rows.Add(row);
            }

            row = table.NewRow();
            row[0] = "$";
            table.Rows.Add(row);

            foreach (DataRow r in table.Rows)
            {

                if (r.ItemArray[0].ToString() == "$") {
                    for (int i = 1; i < r.ItemArray.Count()-2; i++)
                    {
                        r.ItemArray[i] = "Error";
                    }

                    r.ItemArray[r.ItemArray.Count() - 1] = "Accept";
                }
                    
            }


            return table;
        }

        private void TableClick(object sender, RoutedEventArgs e)
        {
            DataSet1 dataset = new DataSet1();
            dataset.Tables.Add(GetTable());
        }



        private void DirectoryClick(object sender, RoutedEventArgs e)
        {
            List<string[]> directories = Grammar.FindDirectories();
            bool v = Grammar.VerifyDis(directories);

            if (v == false)
            {
                TableBlock.AppendText("\n Gramatica nu respecte conditia necesara pentru a fi GRAMATICA LL1 \n");

            }

            foreach (string[] dir in Grammar.Directory)
            {
                if (dir[0] == "first ")
                {
                    TableBlock.AppendText("\n");
                    TableBlock.AppendText(dir[0].ToUpper() + " " + dir[1].Substring(0, 1) + " : { ");
                    //   TableBlock.Text = s;

                }
                else
                {
                    TableBlock.AppendText("\n");
                    TableBlock.AppendText(dir[0].ToUpper() + " " + dir[2] + " : { ");

                }

                int i = 5;


                while (i < dir.Length)
                {
                    if (dir[i] == null)
                    {
                        TableBlock.AppendText(" }\n");

                        break;
                    }


                    TableBlock.AppendText(" " + dir[i]);
                    i++;
                }
            }

            TableBlock.AppendText(" }\n");


            table = GetTable();
        }
        
        

        private void SentenceClick(object sender, RoutedEventArgs e)
        {
            Stack.Text = " Propozitia de analizat:\t";
            
            string senten = Sentence.Text;


            Stack.AppendText(senten);
            sentence = Tokenize(senten);

            AnalyseSentence(sentence);


        }

        private string[] Tokenize(string s)
        {
            return s.Split(' ');
        }

        public bool VerifyFirstMatch(string x)
        {
            foreach(string[] s in Grammar.Directory)
            {
                if(s[0]=="first " && s[1].Substring(0,1)==Grammar.StartSimbol )
                {

                    for (int i = 2; i < s.Length; i++)
                    {
                        if (s[i] == x)
                            return true;
                    }
                }
            }

            return false;
        }


        int numberOfMatches = 0;
        public static string[] sentence;

        public string[] ReturnedRule(string NT, string tok )
        {
            string[] rule;

            int nrRows = 0;
            foreach (DataRow r in table.Rows)
            {
                if (r[0].ToString() == NT)
                {
                    if (r[tok].ToString() != "Error")
                    {
                        foreach (DataColumn cl in table.Columns)
                        {
                            if (cl.ColumnName == tok)
                            {
                                var item = table.Rows[nrRows][tok];
                                string it = item.ToString();
                                int index = int.Parse(it.Substring(1, 1));
                                if (item != "Error")
                                {
                                    rule = new string[Grammar.ProductRules[index - 1].Length - 4];
                                    for (int i = 4; i < Grammar.ProductRules[index - 1].Length; i++)
                                    {
                                        rule[i - 4] = Grammar.ProductRules[index - 1][i];
                                        if (Grammar.ProductRules[index - 1][i] == null)
                                            break;
                                    }
                                    return rule;
                                }
                            }
                        }


                    }
                }

                nrRows++;


            }
            return null;
        }





        public bool AnalyseSentence(string[] sentence)
        {
            stack = new string[100];
            stack.Initialize();
            stack[0]= Grammar.StartSimbol;
            int index = 0;


            if(VerifyFirstMatch(sentence[0])==false)
            {
                // Propozitia NU apartine limbajului 
            }




           if (ReturnedRule(Grammar.StartSimbol, sentence[0]) == null)  { ; }       
              

           stack[0] = String.Empty;
           stack = ReturnedRule(Grammar.StartSimbol, sentence[0]);

            Stack.AppendText("\n");
        //    Stack.AppendText(stack.ToString());

           if(stack[0]==sentence[0])
           {
               numberOfMatches++;
               stack=stack.Skip(1).ToArray();
               index++;

                Stack.AppendText("\n");
               // Stack.AppendText(stack);

                // caz in care urmeaza terminal si caz in care urmeaza nt
                if (stack[index] == "")
                    index++;

                if(Grammar.verifyTerminal(stack[index]))
                {
                    string[] newstack = VerifyMatch(stack, sentence[numberOfMatches]);
                   
                    if(newstack!=null)
                    {
                        index++;
                    }
                    else
                    {

                        //Propozitua NU apartine limbajului 
                        Stack.AppendText ( "\n Propozitia NU apartine LIMBAJULUI definit de gramatica G");
                    }
                    
                }
                else
                {
                    ContinueAnalyse(stack, sentence[index]);
                }
               

           }
           else
           {
                //Propozitua NU apartine limbajului 
                Stack.AppendText ( "\n Propozitia NU apartine LIMBAJULUI definit de gramatica G");
            }

            return true;
        }
        public string[] VerifyMatch(string[] stack,string token)
        
        {
            if (stack[0] == token)
            {
                numberOfMatches++;

                return stack.Skip(1).ToArray();
            }

            return null;
        }
        public bool VSpace(string x)
        {
            if (x == "" || x == " " || x == "epsilon")
                return true;

            return false;
        }
        public void ContinueAnalyse(string[] st, string sent)
        {

            if (numberOfMatches == sentence.Length)
            {
                //Propozitia apartine limbajului definit de GRAMATICA
                return;
            }


            string[] ret;
            if (VSpace(st[0]))
            {
                st = st.Skip(1).ToArray();

                while(VSpace(st[0]))
                    st = st.Skip(1).ToArray();

                while(Grammar.verifyTerminal(st[0]))
                {
                    if(st[0]==sent)
                    {
                        numberOfMatches++;

                        if (numberOfMatches >= sentence.Length)
                        {
                            //PROPOZITIE CORECTA;
                            Stack.AppendText("\n Propozitia analizata apartine limbajului definit de gramatica.");

                        }
                        else {

                            st = st.Skip(1).ToArray();
                            sent = sentence[numberOfMatches];
                                }
                    }

                }

                ret = ReturnedRule(st[0], sent);
                
            }
            else
            {
                ret = ReturnedRule(st[0], sent);

            }

            if(ret==null)
            {
                //Propozitia NU apartine limbajului
                Stack.AppendText("\nPropozitia analizata nu apartine limbajului.");
            }

            else if (ret[0] != null)
            {
                st = st.Skip(1).ToArray(); // INLOCUIM NT
                string[] newstack = new string[30];
                ret.CopyTo(newstack, 0);

                int i = 0;
                while (newstack[i] != null)
                    i++;

                for (int k = i; k < st.Length+3; k++)
                {
                    if (st[k - i] == null)
                        break;

                    newstack[k] = st[k - i];
                }

                if (newstack[0] == sent)
                {
                    numberOfMatches++;

                    if (numberOfMatches == sentence.Length)
                    {
                        //PROPOZITIA APARTINE LIMBAJULUI
                        Stack.AppendText("Propozitia "+ " apartine limbajului definit de gramatica.");

                    }
                    else
                    {
                        newstack = newstack.Skip(1).ToArray();

                        stack = newstack;
                        ContinueAnalyse(stack, sentence[numberOfMatches]);
                    }
                }
                else
                {
                    stack = newstack;

                    if (stack[0] == "")
                        stack = stack.Skip(1).ToArray();

                    if (Grammar.verifyTerminal(stack[0]))
                    {

                        stack = VerifyMatch(stack, sentence[numberOfMatches]);
                        ContinueAnalyse(stack, sentence[numberOfMatches]);


                    }
                    else
                    {
                        ContinueAnalyse(stack, sentence[numberOfMatches]);
                    }



                }

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}

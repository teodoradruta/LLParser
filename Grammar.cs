using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LLPARSER
{
    public class Grammar
    {

        public string StartSimbol;
        public int numberOfrules { get; set; }


        private List<string> terminals;

        public List<string> Terminals
        {
            get => terminals;
            set
            {
                terminals = value;

            }
        }

        public List<string> Nterminals { get; set; }

        private List<string[]> productRules = new List<string[]>();
        public List<string[]> ProductRules
        {
            get { return productRules; }
            set { productRules = value; }
        }

        private List<string[]> directory = new List<string[]>();
        public List<string[]> Directory
        {
            get { return directory; }
            set { directory = value; }
        }


        public Grammar(string S,int nr, List<string> t, List<string> nt, List<string[]> rules)
        {
            StartSimbol = S;
            numberOfrules = nr;
            Terminals = t;
            Nterminals = nt;
            productRules = rules;


        }
        /*
         A---gb
         A---gc
         
         A-gA'
         A'-b|c

         */
        public void VerifyRecursivity()
        {

            int aux = numberOfrules;
            List<string[]> mylist = new List<string[]>(productRules);
            mylist = productRules.ToList();




            for (int i = 0; i < numberOfrules - 1; i++)
            {
                string[] rule = mylist[i];

                //Verific recursivitate stanga
                if (rule[0] == rule[4])
                {
                    string nt = rule[0] + "1";
                    if (verifyNt(nt))
                        Nterminals.Add(nt);
                    else if (verifyNt(nt = rule[0] + "2"))
                        Nterminals.Add(nt);
                    else if (verifyNt(nt = rule[0] + "3"))
                        Nterminals.Add(nt);
                    else
                        Nterminals.Add(nt = rule[0] + "4");



                    /*
                                            string[] newstr = new string[] { };
                                            productRules.Add(newstr);
                                            string[] newstr1 = new string[] { };
                                            productRules.Add(newstr1);
                                            string[] newstr2 = new string[] { };
                                            productRules.Add(newstr2);

                    */
                    string[] epsrule = new string[6];
                    epsrule[0] = nt;
                    epsrule[1] = " ";
                    epsrule[2] = ":";
                    epsrule[3] = " ";
                    epsrule[4] = "epsilon";
                    epsrule[5] = "";


                    for (int j = 0; j < numberOfrules - 1; j++)
                    {
                        // In caz ca exista o regula pt acelasi neterminal
                        if (mylist[j][0] == rule[0] && i != j)
                        {
                            string[] brule = new string[mylist[j].Length + 2];
                            mylist[j].CopyTo(brule, 0);



                            brule[mylist[j].Length] = nt;
                            brule[mylist[j].Length + 1] = "";

                            string[] newrule = new string[rule.Length + 1];
                            newrule[0] = nt;
                            for (int k = 1; k < 3; k++)
                            {
                                newrule[k] = rule[k];
                            }
                            for (int z = 4; z < rule.Length - 1; z++)
                            {
                                newrule[z - 1] = rule[z + 1];
                            }
                            newrule[rule.Length - 2] = " ";
                            newrule[rule.Length - 1] = nt;
                            newrule[rule.Length] = "";








                            productRules.RemoveAt(i);
                            productRules.RemoveAt(j);



                            productRules.Insert(aux - 3, brule);
                            productRules.Insert(aux - 2, newrule);
                            productRules.Insert(aux - 1, epsrule);

                            aux++;
                        }

                        // Daca nu a mai gasit o alta regula
                        /*  else if(i!=j)
                          {
                              string[] crule = new string[6];
                              crule[0] = rule[0];
                              crule[1] = rule[1];
                              crule[2] = rule[2];
                              crule[3] = rule[3];
                              crule[4] = nt;

                              string[] drule = new string[rule.Length + 1];
                              drule[0] = nt;
                              drule[1] = " ";
                              drule[2] = ":";
                              drule[3] = " ";
                              for (int k = 4; k < rule.Length - 1; k++)
                              {
                                  drule[k] = rule[k + 1];
                              }
                              drule[rule.Length - 1] = nt;
                              drule[rule.Length] = " ";

                              productRules.RemoveAt(i);

                              productRules.Insert(aux - 3, crule);
                              productRules.Insert(aux - 2, drule);
                              productRules.Insert(aux - 1, epsrule);

                              aux++;

                          }
                          */
                    }
                }
            }
            numberOfrules = aux;
        }

        public void VerifyAmbiguity()
        {
            int aux = numberOfrules;
            List<string[]> auxlist = new List<string[]>(productRules);
            auxlist = productRules.ToList();

            int depl = 0;

            for (int i = 0; i < numberOfrules - 2; i++)
            {
                for (int j = i + 1; j < numberOfrules - 1; j++)
                {

                    string[] rule = auxlist[i];
                    string[] brule = auxlist[j];

                    // Regula incepe cu acelasi NETERMINAL
                    if (rule[0] == brule[0]) // verificam partea dreapta 
                    {

                        // Indicele din regula de la care difera
                        int ind = VerifyRIGHT(rule.Skip(4).ToArray(), brule.Skip(4).ToArray());
                        //ind = ind 2;


                        if (ind == 0)
                            break;

                        string addedNT = String.Empty;

                        if (verifyNt("K"))
                            addedNT = "K";
                        else if (verifyNt("K1"))
                            addedNT = "K1";
                        else if (verifyNt("K2"))
                            addedNT = "K2";
                        else if (verifyNt("K3"))
                            addedNT = "K3";
                        else
                            addedNT = "K4";

                        Nterminals.Add(addedNT);

                        string[] ruleAux = new string[ind + 6];

                        for (int k = 0; k < ind+4; k++)
                        {
                            ruleAux.SetValue(auxlist[j].GetValue(k), k);
                        }
                        if (ruleAux[ind + 3] == "")
                            ruleAux.SetValue(addedNT, ind + 4);
                        else
                        {
                            ruleAux.SetValue(" ", ind + 4);
                            ruleAux.SetValue(addedNT, ind + 5);
                        }
                        ind = ind + 4; // adaugam pt inceput[0]

                        string[] ruleAux1 = new string[20];
                        string[] ruleAux2 = new string[20];

                        // Verific ce urmeaza dupa partea comuna fiecarei reguli de productie
                        if (ind < rule.Length)
                        {

                            ruleAux1[0] = addedNT;
                            ruleAux1[1] = " ";
                            ruleAux1[2] = ":";
                            ruleAux1[3] = " ";
                            int v = 0;
                            int x = 0;

                            while (x < rule.Length - ind)
                            {
                                if (rule[ind] == "" && v == 0)
                                {
                                    ruleAux1[4 + v] = rule[ind + v + 1];
                                    v++;
                                    x = x + 3;
                                }

                                ruleAux1[4 + v] = rule[ind + x];
                                ruleAux1[4 + v + 1] = " ";
                                x++;
                                v = v + 1;
                            }
                        }
                        else
                        {
                            ruleAux1[0] = addedNT;
                            ruleAux1[1] = " ";
                            ruleAux1[2] = ":";
                            ruleAux1[3] = " ";
                            ruleAux1[4] = "epsilon";
                            //ruleAux1[5] = "";

                        }
                        if (ind < brule.Length)
                        {
                            ruleAux2[0] = addedNT;
                            ruleAux2[1] = " ";
                            ruleAux2[2] = ":";
                            ruleAux2[3] = " ";
                            int v = 0;
                            int x = 0;

                            while (x < brule.Length - ind)
                            {
                                if(brule[ind]=="" && v==0)
                                {
                                    ruleAux2[4 + v] = brule[ind + v + 1];
                                    v++;
                                    x=x+2;
                                }
                                
                                ruleAux2[4 + v] = brule[ind + x];
                                ruleAux2[4 + v+1] = " ";
                                x++;
                                v=v+1;
                            }
                        }
                        else
                        {
                            ruleAux2[0] = addedNT;
                            ruleAux2[1] = " ";
                            ruleAux2[2] = ":";
                            ruleAux2[3] = " ";
                            ruleAux2[4] = "epsilon";
                            ruleAux2[5] = "";
                        }





                        productRules.RemoveAt(i - depl);
                        productRules.RemoveAt(j - depl - 1);

                        string[] newstr = new string[] { };
                        productRules.Add(newstr);
                        string[] newstr1 = new string[] { };
                        productRules.Add(newstr1);
                        string[] newstr2 = new string[] { };
                        productRules.Add(newstr2);

                        productRules.Insert(aux - 3, ruleAux);
                        productRules.Insert(aux - 2, ruleAux1);
                        productRules.Insert(aux - 1, ruleAux2);
                        aux++;
                        index = 0;
                        depl = depl + 2;
                    }

                }
            }
            numberOfrules = aux;
        }
        int index = 0;

        private int VerifyRIGHT(string[] r1, string[] r2)
        {

            if (r1.Length == 0 | r2.Length == 0)
                return index;
            if (r1[0] != r2[0])
            {
                return index;
            }
            else
            {
                index++;
                VerifyRIGHT(r1.Skip(1).ToArray(), r2.Skip(1).ToArray());
            }

            return index;
        }

        public bool verifyNt(string c)
        {
            foreach (string x in Nterminals)
                if (x == c)
                    return false;

            return true;
        }





        public bool verifyTerminal(string t)
        {
            foreach (string x in Terminals)
                if (x == t)
                    return true;

            return false;
        }



        ///  FIRST AND FOLLOW

        private List<string[]> first;
        public List<string[]> First { get; set; }

        private List<string[]> follow;
        public List<string[]> Follow { get; set; }



        public List<string[]> FindDirectories()
        {
            foreach (string[] rule in productRules)
            {
                if (rule.Length == 0)
                    return directory; // returneaza lista de stringuri 

                if (rule[4] == "epsilon")
                {
                    FindFollow(rule[0]);
                }
                else
                    FindFirst(rule);

            }
            return directory;

        }

       
      
        public void FindFirst(string[] rule)
        {
            string[] element = new string[20];

            element[0] = "first "; // pt a stii pt ce neterminal calculez
            element[1] = String.Join(" ",rule);
            element[2] = rule[4];
            element[3] = ":";
            element[4] = " ";

            string x = rule[4]; // terminal sau neterminal

            //terminal
            foreach (string term in terminals)
                if (x == term)
                {
                    element[5] = x;
                    directory.Add(element);
                    return;
                }


            // neterminal  -- caut regula nt--> t si daca nu exista 
                                       // nt--> nt2 si caut first(nt2)   
                                       


            string[] found = Find(rule[4]);
            for (int k = 0; k < found.Length; k++)
            {
                if (found[k] == null)
                    break;

                element[k + 5] = found[k];
            }
            element[found.Length + 6] = "//";
               directory.Add(element);
                return;
            
        }
        //Tratare cazuri FIRST
        public string[] Find(string nterm)
        {

            if (nterm == "epsilon")
            {
                // Follow
            }
            string[] t = new string[10];

            foreach(string[] d in directory)
            {
                if(d[0]=="first "&& d[1].Substring(0,1)==nterm)
                {
                    for (int i = 0; i < d.Length-5; i++)
                    {
                        if (t[i] == null)
                            break;
                        t[i] = d[i + 5];

                    }
                    return t;
                           
                }
            }
            int nr = 0;
            foreach (string[] r in productRules)
            {
                if (r.Length == 0)
                    break;

                if (r[0] == nterm)
                {

                    // First(neterm-->epsilon) --> caz in care cautam neterminalul si facem follow

                    if (r[4] == "epsilon")
                    {
                        //implementeaza metoda de FOLLOW pt return
                       // string[] f = FindFollow(r[0]);
                        break;
                    }

                    bool verify = false;
                    foreach (string s in terminals)
                        if (r[4] == s)
                        {
                            t[nr] = s;
                            nr++;
                            verify = true; ; // trebuie sa ies pana la foreach care schimba regula
                        }

                    if (verify == true)
                        continue;
                    else
                    {
                        string[] aux = Find(r[4]);
                        foreach (string element in aux)
                        {
                            t[nr] = element;
                            nr++;
                        }
                    }
                    

                }


            }
            return t;


        }

        public string[] RecursiveFollow(string nterm,string[] viewedNt)
        {
            //Verificare
            foreach(string nt in viewedNt)
            {
                if (nt != null)
                {
                    if (nt == nterm)
                        return new string[0];
                }
                else
                    break
                        ;
            }
            
            string[] terms = new string[10];
            string[] auxterms = new string[10];

          

            foreach (string[] s in directory)
            {
                if (s[0] == "follow" && s[2] == nterm)
                {
                    terms = new string[s.Length];
                    for (int i = 0; i < s.Length - 5; i++)
                    {
                        terms[i] = s[5 + i];
                    }
                    return terms;
                }
            }


            int n = 0;
            foreach(string[] frule in productRules)
            {
                if (frule.Length == 0)
                    break;

                for (int i = 4; i < frule.Length; i++)
                {
                    if(frule[i]==nterm && frule[i]!=null) // am gasit nt in partea dr a regulilor de productie
                    {
                       

                       // 1 . nt la final

                        int count = 0;
                        // doua cazuri pt neterminal la final
                        bool it = false;
                        if(i+2>=frule.Length)
                        {
                           
                            auxterms = RecursiveFollow(frule[0],viewedNt);
                            it = true;
                            if (auxterms.Length == 0)
                                break;
                            

                            
                        }
                        if (it == false)
                        {
                            if (frule[i + 2] == null)
                            {
                                auxterms = RecursiveFollow(frule[0], viewedNt);
                                it = true;
                                if (auxterms.Length == 0)
                                {
                                    break;
                                }

                            }
                        }




                        //2. dupa nt urmeaza terminal


                        if (it == false)
                        {
                            foreach (string t in terminals)
                            {
                                if (frule[i + 2] == t && i + 2 <= frule.Length)
                                {
                                    it = true;
                                    terms[n] = t;
                                    n++;
                                    count++;
                                    break;
                                }
                            }
                        }
                        // 3. dupa nt urmeaza tot nt ==> first(nt)
                        if (count == 0 && it==false)
                        {
                         auxterms=Find(frule[i + 2]);
                        }



                        if (auxterms.Length != 0)
                        {
                            if (auxterms[0] != null)
                                foreach (string x in auxterms)
                                {
                                    if (x == null)
                                        break;

                                    terms[n] = x;
                                    n++;
                                }

                        }



                    }
                }

            }

            return terms;
            
        }

        public void FindFollow(string nterm)
        {
            string[] follow = new string[10];
            follow[0] = "follow";
            follow[1] = " ";
            follow[2] = nterm;
            follow[3] = ":";
            follow[4] = " ";

            string[] viewed = new string[10];
            viewed[0] = nterm;
            int countNT = 1;

            // pt fiecare regula de productie : 4+
            // daca gasesc neterm -- 
            //-- terminal ==> gasit
            // neterminal ==> first
            // nimic ==> follow (0)


            foreach (string[] r in productRules)
            {
                int i = 4;
                while (i!=r.Length)
                {
                    if (r.Length == 0)
                        break;
                    if (r[i] == null)
                        break;

                    if (r[i] == nterm) // am gasit neterminalul in partea dreapta a unei reguli
                    {
                        // cautam ce urmeaza dupa el : TREI CAUZURI

                       



                        // neterminal pe pozitia penultima , ultima e " " mereu 
                        // calculam FOLLOW(r[0])
                        if (i + 1 == r.Length) 
                        {
                            string[] rules = RecursiveFollow(r[0], viewed);
                            viewed[countNT++] = r[0];

                            int count = 0;

                            if (rules.Length == 0)
                            {
                                break;
                            }

                            while (count != rules.Length && rules[count] != null)
                            {
                                follow[5 + count] = rules[count];
                                count++;

                            }
                            follow[5 + count] = "\\";

                            directory.Add(follow);

                            return;

                        }

                        // dupa neterminal este un terminal
                        foreach (string term in terminals)
                            if (r[i + 1] == term)
                            {
                                follow[5] = term;
                                follow[6] = "\\";

                                directory.Add(follow);

                                return;
                            }



                        //CAZUL 2:

                        foreach (string nt in Nterminals)
                            if (r[i + 1] == nt)
                            {
                                string[] aux = RecursiveFollow(nt, viewed);
                                int j = 0;
                                while (j < aux.Length)
                                {
                                    follow[5 + j] = aux[j];
                                    j++;
                                }
                                follow[5 + j] = "\\";

                                directory.Add(follow);

                                return;
                            }

                        //CAZUL 3
                        string[] find = RecursiveFollow(r[0], viewed);


                    }
                    i++;

                }

            }
        }


        // Verificare multimi disjuncte 
        public bool VerifyDis(List<string[]> dir)
        {

            for (int i = 0; i < dir.Count()-1; i++)
            {
                if (dir == null)
                    break;

                string nt1, nt2;
                if (dir[i][0] == "first ")
                    nt1 = dir[i][1].Substring(0, 1);
                else
                    nt1 = dir[i][2];

                for (int j = i+1; j < dir.Count(); j++)
                {
                    if (dir == null)
                        break;
                    if (dir[j][0] == "first ")
                        nt2 = dir[j][1].Substring(0, 1);
                    else
                        nt2 = dir[j][2];


                    if(nt1==nt2)
                    {

                        int k=5,l=5;
                        while(dir[i][k]!=null)
                        {
                            l = 5;
                            while (String.IsNullOrEmpty(dir[j][l])!=true)
                            {
                                if (dir[i][k] == dir[j][l])
                                    return false;
                                l = l + 1;
                            }
                            k = k + 1;
                             ;
                        }
                    }



                }
            }

            return true;
        }

    }
}


            

        
      
    
   

    
    


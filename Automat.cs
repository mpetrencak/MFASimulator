using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Multiset_finite_automata_simulator
{
    [Serializable()]
    public class Automat : ISerializable
    {
        public Dictionary<string, int> inputMultiset { get; private set; }  //Vstupni multimnožina
        public List<State> states { get; private set; } //množina stavů
        public List<Transition> function { get; private set; }  //Přechodová funcke delta

        //DKA
        public string actualSymbol { get; private set; }
        public State actualState { get; private set; }
        public State nextState { get; private set; }
        public Transition actualFunction { get; private set; }
        public Dictionary<string, int> unreadMultiset { get; set; }

        //NKA
        private bool zpracovaniRetezce;
        public int pocetPermutaci;
        public List<string> poradiVstupu { get; set; }
        public List<State> nextStates;
        public List<State> actualStates;
        public string oneInput { get; set; }
        public bool uspesnyVypocet { get; set; }
        public bool neprectenyRetezec{ get; set; }

        //detekce
        public bool detekcePrechod;
        public bool detekcePrazdnaM;


        public Automat(Dictionary<string, int> multiset, List<State> states, List<Transition> functions)
        {
            inputMultiset = multiset;
            unreadMultiset = new Dictionary<string, int>(inputMultiset);
            this.states = states;
            this.function = functions;

            SetInitialState();

            nextState = null;
            actualSymbol = null;
            actualFunction = null;
            oneInput = String.Empty;

            uspesnyVypocet = false;
            detekcePrazdnaM = false;
            detekcePrechod = false;
            neprectenyRetezec = false;
        }


        #region DKA

        public bool DoFirstStepDKA()
        {
            foreach (Transition function in function)//prohledavam funkce
            {
                if (function.actualState.Equals(actualState))//testuju funkci, ktera vyhovuje aktualnimu stavu
                {
                    foreach (var symbol in unreadMultiset)//pro tuto funci projdu vsechny symboly v multimnozine
                    {
                        if (symbol.Key == function.symbol && symbol.Value > 0)// pokud jsem nasel shodu
                        {
                            nextState = function.nextState;
                            actualFunction = function;
                            actualSymbol = symbol.Key;
                            return true;
                        }
                    }
                }

                if (function.actualState.Equals(actualState))
                {
                    foreach (var symbol in unreadMultiset)
                    {
                        if(symbol.Key.ToUpper() == function.symbol && symbol.Value == 0)//podminka k detekci
                        {
                            nextState = function.nextState;
                            actualFunction = function;
                            actualSymbol = symbol.Key;
                            return true;

                        }
                    }

                }
            }
            return false;
        }

        public void DoSecondStepDKA()
        {
            if (unreadMultiset[actualSymbol] > 0) 
            {
                unreadMultiset[actualSymbol]--;
            }
            actualState = nextState;
            nextState = null;
            actualSymbol = null;
            actualFunction = null;

        }
        #endregion



        #region NKA
        public bool DoFirstStepNKA()
        {
            
            if (poradiVstupu.Count == 0 && zpracovaniRetezce == false) 
            {
                return false;
            }

            if (String.IsNullOrEmpty(oneInput) && poradiVstupu.Count >= 1) 
            {
                oneInput = poradiVstupu.Last();
                unreadMultiset = new Dictionary<string, int>(inputMultiset);
                poradiVstupu.RemoveAt(poradiVstupu.Count - 1);
                zpracovaniRetezce = true;

            }

            nextStates = new List<State>();
            actualSymbol = oneInput.Last().ToString();
           
            
            foreach (Transition function in function)
            {
                foreach (State actualState in actualStates)
                {
                    if (function.actualState.Equals(actualState) && function.symbol == actualSymbol && unreadMultiset[actualSymbol] != 0) 
                    {
                        nextStates.Add(function.nextState);
                    }
                    //detekce
                    foreach (var v in unreadMultiset)
                    {
                        if(v.Value==0)
                        {
                            if (function.actualState.Equals(actualState) && function.symbol == v.Key.ToUpper())
                            {
                                nextStates.Clear();
                                nextStates.Add(function.nextState);
                                actualSymbol = v.Key.ToUpper();
                                detekcePrechod = true;

                            }
                        }
                    }
                    if(detekcePrechod)
                    {
                        break;
                    }
                }
            }            

            if (nextStates.Count > 0)
            {
                return true;
            }

            return false;
        }

        public void DoSecondStepNKA()
        {
            if(detekcePrazdnaM)
            {
                oneInput = String.Empty;
                detekcePrazdnaM = false;
                zpracovaniRetezce = false;
            }
            if(detekcePrechod)
            {
                detekcePrechod = false;
                actualStates = nextStates;
                nextStates = null;
                actualSymbol = null;
                actualFunction = null;
                return;

            }
            if (oneInput.Length == 1)
            {
                unreadMultiset[actualSymbol]--;
                if (CheckDetekce())             //kontrola detekce po precteni posledniho symbolu
                {
                }
                else
                {
                    oneInput = String.Empty;
                    zpracovaniRetezce = false;
                }


            }

            if (oneInput.Length > 1)
            {
                unreadMultiset[actualSymbol]--;
                oneInput = oneInput.Remove(oneInput.Length - 1);
            }

            actualStates = nextStates;
            nextStates = null;
            actualSymbol = null;
            actualFunction = null;

        }


        private bool CheckDetekce()
        {
            foreach(Transition function in function)
            {
                foreach(State actState in actualStates)
                {
                    foreach (var v in unreadMultiset)
                    {
                        if (function.actualState.Equals(actState) && function.symbol == v.Key.ToUpper() && v.Value == 0) 
                        {
                            detekcePrazdnaM = true; //Promena pro prechod detekcniho automatu pri prazdne vstupni moltimnozine
                            return true;                          
                        }
                    }
                }
            }
            return false;
        }

        #endregion


        public void ResetAutomat()
        {
            unreadMultiset = new Dictionary<string, int>(inputMultiset);
            SetInitialState();

            nextState = null;
            actualSymbol = null;
            actualFunction = null;


            oneInput = String.Empty;
            detekcePrazdnaM = false;
            detekcePrechod = false;
            zpracovaniRetezce = false;
            

        }

        public void SetInitialState()
        {
            actualStates = new List<State>();
            foreach (State state in states)
            {
                if (state.IsInitial())
                {
                    actualStates.Add(state);
                    actualState = state;
                }
            }
        }










        public void MakePossibleInputs()
        {
            int pocetPrvku = inputMultiset.Values.Sum();

            char[] array = new char[pocetPrvku];
            int index = 0;
            foreach(var v in inputMultiset)
            {
                for (int i = 0; i < v.Value; i++)
                {
                    array[index++] = Convert.ToChar(v.Key);
                }
            }

            IEnumerable<IEnumerable<int>> result = GetPermutations(Enumerable.Range(1, pocetPrvku), pocetPrvku);
            poradiVstupu = new List<string>();

            foreach (var list in result)
            {
                bool nalezeno = false;
                string item = String.Empty;
                foreach (var vr in list)
                {
                    item += array[vr - 1];
                }

                if (poradiVstupu.Count >= 1)
                {
                    foreach (var vr in poradiVstupu)
                    {
                        if (vr == item)
                        {
                            nalezeno = true;
                            break;
                        }
                    }
                    if (!nalezeno)
                    {
                        poradiVstupu.Add(item);
                    }
                }

                if (poradiVstupu.Count == 0)
                {
                    poradiVstupu.Add(item);
                }
            }
            pocetPermutaci = poradiVstupu.Count;
        }


        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("inputMultiset", inputMultiset);
            info.AddValue("states", states);
            info.AddValue("functions", function);
        }

        public Automat(SerializationInfo info, StreamingContext context)
        {
            inputMultiset = (Dictionary<string, int>)info.GetValue("inputMultiset", typeof(Dictionary<string, int>));
            states = (List<State>)info.GetValue("states", typeof(List<State>));
            function = (List<Transition>)info.GetValue("functions", typeof(List<Transition>));

        }


    }


}

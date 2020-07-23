using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Multiset_finite_automata_simulator
{


    public partial class Form1 : Form
    {
        #region Gloval variables
        //globalni promenne pro zadavani automatu
        private Dictionary<string, int> Multiset = new Dictionary<string, int>();
        private List<State> States = new List<State>();
        private List<Transition> Functions = new List<Transition>();

        //globalni objekt automat
        Automat automat;

        //DKA index kroku
        int indexKrokuDKA;

        //integeer pro mazani prechodu
        int pocetPrechodu = 1;

        //NKA
        bool konecvypoctu = false;
        //int indexVetve;
        int pocetSlovVstupu = 1;
        #endregion

        public Form1()
        {
            InitializeComponent();         
            
        }

        #region Add Buttons
        private void buttonAddMultisetSymbol_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(comboBoxSymbol.Text))
            {
                MessageBox.Show("Nevybral jsi symbol!","Chyba!");
                return;
            }


            if (Multiset.ContainsKey(comboBoxSymbol.Text))
            {
                Multiset[comboBoxSymbol.Text] += Convert.ToInt32(numericUpDownSymbol.Value);
            }
            else
            {
                Multiset.Add(comboBoxSymbol.Text, Convert.ToInt32(numericUpDownSymbol.Value));
            }

            textBoxPrvky.Clear();
            comboBoxFunctionSymbol.Items.Clear();

            foreach (var v in Multiset)
            {
                textBoxPrvky.Text += v.Key + "\t" + "-" + "\t" + v.Value + Environment.NewLine;
                comboBoxFunctionSymbol.Items.Add(v.Key);


            }
        }


        private void buttonAddState_Click(object sender, EventArgs e)
        {
            foreach (State stat in States)
            {
                if (stat.number == Convert.ToInt32(numericUpDownStateNumber.Value))
                {
                    MessageBox.Show("Tento satav je již zadán", "Chyba!");
                    return;
                }
            }

            bool initialState = false;
            if (checkBoxInitialState.Checked)
            {
                initialState = true;
                checkBoxInitialState.Checked = false;
                checkBoxInitialState.Enabled = false;

            }

            bool endstate = false;
            if (checkBoxEndState.Checked)
            {
                endstate = true;
                checkBoxEndState.Checked = false;
            }



            State state = new State(Convert.ToInt32(numericUpDownStateNumber.Value), initialState, endstate);
            States.Add(state);

            comboBoxNextState.Items.Add(States[States.Count - 1].ToString());
            comboBoxActualState.Items.Add(States[States.Count - 1].ToString());


            string line = 'q' + Convert.ToString(numericUpDownStateNumber.Value) + "\t";

            if (initialState)
            {
                line += "POČÁTEČNÍ" + "\t";
            }
            else
            {
                line += "\t" + "\t";
            }

            if (endstate)
            {
                line += "KONCOVÝ";
            }

            textBoxStavy.Text += line + Environment.NewLine;

            numericUpDownStateNumber.Value++;

        }


        private void buttonAddFunction_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(comboBoxActualState.Text) || String.IsNullOrWhiteSpace(comboBoxNextState.Text) || String.IsNullOrWhiteSpace(comboBoxFunctionSymbol.Text))
            {
                MessageBox.Show("Nevybral jsi parametry!", "Chyba!");
                return;
            }

            State actualState = null;
            State nextState = null;
            foreach (State state in States)
            {
                if (String.Equals(comboBoxActualState.Text.Substring(1), Convert.ToString(state.number)))
                {
                    actualState = new State(state);

                }
                if (String.Equals(comboBoxNextState.Text.Substring(1), Convert.ToString(state.number)))
                {
                    nextState = new State(state);
                }
            }
            string symbol = comboBoxFunctionSymbol.Text;
            if(checkBoxDetekce.Checked == true)
            {
                symbol = symbol.ToUpper();

            }
            Transition function = new Transition(pocetPrechodu++, actualState, symbol, nextState);

            if (Functions.Count >= 1)
            {
                foreach (Transition func in Functions)
                {
                    if (func.Equals(function))
                    {
                        MessageBox.Show("Přechod již existuje!", "Chyba!");
                        pocetPrechodu--;
                        return;
                    }
                }
                Functions.Add(function);
            }


            if (Functions.Count == 0)
            {
                Functions.Add(function);
            }

            string line = String.Empty;

            //line = "q" + Convert.ToString(function.actualState.number) + "\t" + "+" + "\t" + function.symbol + "\t" + "-->" + "\t" + "q" + Convert.ToString(function.nextState.number);
            //line = function.number.ToString() + "." + "\t" + function.actualState.ToString() + " " + "+" + " " + function.symbol + "  " + "-->" + "  " + function.nextState.ToString() + "\t" + "\t";
            line = function.ToString() + "\t" + "\t";
            textBoxFunkce.Text += line;
            if (Functions.Count % 3 == 0 && Functions.Count != 0) 
            {
                textBoxFunkce.Text += Environment.NewLine;
            }



        }
        #endregion

        #region Reset Buttons
        private void buttonResetMultiset_Click(object sender, EventArgs e)
        {
            if (Functions.Count == 0) 
            {
                ResetMultiset();
            }
            else
            {

                switch (MessageBox.Show("Přejete si smazat i přechodovou funkci?", "Smazat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        ResetMultiset();
                        ResetFunction();
                        break;

                    case DialogResult.No:
                        ResetMultiset();
                        break;

                    case DialogResult.Cancel:
                        break;

                }

            }

            
        }
       

        private void buttonResetState_Click(object sender, EventArgs e)
        {
            if (Functions.Count == 0) 
            {
                ResetState();
            }
            else
            {
                switch (MessageBox.Show("Přejete si smazat i přechodovou funkci?", "Smazat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        ResetState();
                        ResetFunction();
                        break;

                    case DialogResult.No:
                        ResetState();
                        break;

                    case DialogResult.Cancel:
                        break;
                }

            }    



        }


        private void buttonResetFunction_Click(object sender, EventArgs e)
        {
            ResetFunction();
        }                                

        private void buttonSaveAutomat_Click(object sender, EventArgs e)
        {
            SaveAutomat();
        }

        private void buttonResetAutomat_Click(object sender, EventArgs e)
        {
            ResetMultiset();
            ResetState();
            ResetFunction();

        }


        private void buttonResetVizualization_Click(object sender, EventArgs e)
        {
            automat.ResetAutomat();

            ResetFullDKA();


            int radek = 0;
            foreach (State state in States)
            {
                if (state.IsInitial())
                {
                    dataGridViewStavy.Rows[radek].DefaultCellStyle.BackColor = Color.LimeGreen;
                }
                radek++;
            }

            toolStripStatusLabelAction.Text = String.Empty;
            toolStripStatusLabelStateLine.Text = String.Empty;
            indexKrokuDKA = 0;
        }


        private void buttonResetVizualizationNKA_Click(object sender, EventArgs e)
        {
            automat.ResetAutomat();
            automat.MakePossibleInputs();

            textBoxZpracovane.Clear();
            konecvypoctu = false;

            pocetSlovVstupu = 1;
            toolStripStatusLabelStateLine.Text = "Permutace: 0 z " + automat.pocetPermutaci;
            toolStripStatusLabelAction.Text = String.Empty;

            //Set White background color to all DGVs...
            ResetFullNKA();
        }

        #endregion

        #region Other buttons
        private void buttonDeletePrechod_Click(object sender, EventArgs e)
        {

            int pocetZaznamu = 0;
            textBoxFunkce.Clear();

            foreach (Transition function in Functions)
            {
                if (function.number == numericUpDownDeleteNumber.Value)
                {
                    Functions.Remove(function);
                    break;
                }
            }

            foreach (Transition function in Functions)
            {
                pocetZaznamu++;
                string line = String.Empty;

                //line = "q" + Convert.ToString(function.actualState.number) + "\t" + "+" + "\t" + function.symbol + "\t" + "-->" + "\t" + "q" + Convert.ToString(function.nextState.number);
                //line = function.number.ToString() + "." + "\t" + function.actualState.ToString() + " " + "+" + " " + function.symbol + "  " + "-->" + "  " + function.nextState.ToString() + "\t" + "\t";
                line = function.ToString() + "\t" + "\t";

                textBoxFunkce.Text += line;
                if (pocetZaznamu % 3 == 0 && pocetZaznamu != 0)
                {
                    textBoxFunkce.Text += Environment.NewLine;
                }



            }




        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripStatusLabelStateLine.Text = String.Empty;
            toolStripStatusLabelAction.Text = String.Empty;

            if(automat !=null)
            {
                automat.ResetAutomat();
                automat.MakePossibleInputs();
            }
            

            ResetFullDKA();
            ResetFullNKA();

        }

        private void buttonSaveToFile_Click(object sender, EventArgs e)
        {
            if (automat == null)
            {
                SaveAutomat();
                SaveAutomatToFile();

            }
            else
            {
                SaveAutomatToFile();
            }


        }

        private void buttonLoadFromFile_Click(object sender, EventArgs e)
        {
            string path = String.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "automatData Files (*.dat)|*.dat";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                return;
            }

            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            automat = (Automat)bf.Deserialize(stream);
            stream.Close();

            Multiset = automat.inputMultiset;
            States = automat.states;
            Functions = automat.function;

            textBoxFunkce.Clear();
            textBoxPrvky.Clear();
            textBoxStavy.Clear();

            foreach (var v in Multiset)
            {
                textBoxPrvky.Text += v.Key + "\t" + "-" + "\t" + v.Value + Environment.NewLine;
                comboBoxFunctionSymbol.Items.Add(v.Key);
            }
            foreach (State state in States)
            {
                textBoxStavy.Text += state.ToString() + "\t";
                if (state.IsInitial())
                {
                    textBoxStavy.Text += "POČÁTEČNÍ" + "\t";
                }
                else
                {
                    textBoxStavy.Text += "\t" + "\t";
                }

                if (state.IsEnd())
                {
                    textBoxStavy.Text += "KONCOVÝ" + "\t";
                }
                else
                {
                    textBoxStavy.Text += "\t" + "\t";
                }

                textBoxStavy.Text += Environment.NewLine;
                comboBoxActualState.Items.Add(state);
                comboBoxNextState.Items.Add(state);
            }

            int i = 0;
            int nejvyssi = 1;
            foreach (Transition function in Functions)
            {
                if (function.number > nejvyssi) 
                {
                    nejvyssi = function.number;
                }
                textBoxFunkce.Text += function.ToString() + "\t" + "\t";
                i++;
                if (i == 3)
                {
                    textBoxFunkce.Text += Environment.NewLine;

                    i = 0;
                }

            }
            pocetPrechodu = nejvyssi + 1;
            SaveAutomat();

        }

        #endregion


        private void buttonStepDKA_Click(object sender, EventArgs e)
        {

            if (automat == null)
            {
                MessageBox.Show("Automat neexistuje! Nezapomněli jste uložit?");
                return;
            }

            //Determinism test

            Dictionary<State, string> determinismTestDict = new Dictionary<State, string>();
            foreach(Transition function in automat.function)
            {
                if(Char.IsLower(Convert.ToChar(function.symbol)))
                {
                    try 
                    {
                        determinismTestDict.Add(function.actualState, function.symbol);
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("Tento automat není deterministický!");
                        return;
                    }
                }
            }            

            ClearFullDKA();
            HighLightActStateDKA();            

            //First computing step
            if (automat.DoFirstStepDKA())
            {
                toolStripStatusLabelStateLine.Text = "Krok " + ++indexKrokuDKA;
                HighLightFunctionsDKA();
                HighLightAndDecMultisetDKA();
                HighLightNextStateDKA();

                //Second computing step
                automat.DoSecondStepDKA();
            }
            else    //If first step fails... => no next state
            {
                if (automat.actualState.IsEnd())    //test if actual state is end
                {
                    if(automat.unreadMultiset.Values.Sum() == 0)
                    {
                        toolStripStatusLabelAction.Text = "Automat přijímá vstupní multimnožinu";
                        MessageBox.Show("Automat přijímá vstupní multimnožinu");
                    }
                    else
                    {
                        toolStripStatusLabelAction.Text = "Automat nepřijímá vstupní multimnožinu";
                        MessageBox.Show("Automat nepřijímá vstupní multimnožinu");
                    }

                }
                else
                {
                    toolStripStatusLabelAction.Text = "Automat nepřijímá vstupní multimnožinu";
                    MessageBox.Show("Automat nepřijímá vstupní multimnožinu");
                }

            }           

        }
           
        private void buttonStepNKA_Click(object sender, EventArgs e)
        {
            if(konecvypoctu == true)
            {
                return;
            }

            if (automat == null)
            {
                MessageBox.Show("Automat neexistuje! Nezapomněli jste uložit?");
                return;
            }

            if (automat.inputMultiset.Values.Sum() > 4) 
            {
                MessageBox.Show("Příliš mnoho symbolů ve vstupní multimnožině!");
                return;
            }

            if (automat.inputMultiset.Values.Sum() == 4 && automat.inputMultiset.Keys.Count > 3) 
            {
                MessageBox.Show("Příliš mnoho symbolů ve vstupní multimnožině!");
                return;

            }


            if (automat.poradiVstupu == null)
            {
                automat.MakePossibleInputs();
            }

            ClearFullNKA();
            HighLightActStatesNKA();
            if(automat.neprectenyRetezec == true)
            {
                ResetMultisetNKA();
                automat.neprectenyRetezec = false;
            }



            toolStripStatusLabelStateLine.Text = "Permutace: " + pocetSlovVstupu + " z " + automat.pocetPermutaci;

            //First computing step...
            if (automat.DoFirstStepNKA())
            {                
                if(automat.detekcePrechod)
                {
                    textBoxZpracovane.Text += automat.actualSymbol.ToUpper() + " ";
                }
                else
                {                   
                    textBoxZpracovane.Text += automat.oneInput.Last().ToString() + " ";
                }

                HighLighAndDecMultisetNKA();
                HighLightNextStatesNKA();
                HighLightFunctionsNKA();


                automat.DoSecondStepNKA();  //Second step

                if(String.IsNullOrEmpty(automat.oneInput))  //pokud je permutace prazdna
                {
                    textBoxZpracovane.Text += "\t";
                    pocetSlovVstupu++;

                    foreach (State actualState in automat.actualStates)
                    {
                        if(actualState.IsEnd())
                        {
                            automat.uspesnyVypocet = true;      //pokud je vstup prazdny a aktualni stav koncovy, nastavim na true      
                            ClearStatesNKA();

                            for (int i = 0; i < dataGridViewStavy2.Rows.Count; i++) 
                            {
                                if (actualState.ToString() == dataGridViewStavy2.Rows[i].Cells[0].Value.ToString())
                                {
                                    dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                                    dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                                }                          

                            }
                            if(automat.poradiVstupu.Count == 0)     //pokud je posleni permutace vypisu uspech a return
                            {
                                MessageBox.Show("Automat přijímá vstupní multimnožinu!");
                                return;
                            }
                            switch(MessageBox.Show("Automat přijímá vstupní multimnožinu! Přejete si pokračovat v demonstraci výpočtu?","",MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                case DialogResult.Yes:
                                {
                                        ResetMultisetNKA();
                                        ClearFunctionsNKA();

                                        //pokud jsou vsechny vstupy precteny...
                                        if (automat.poradiVstupu.Count == 0)
                                        {
                                            konecvypoctu = true;
                                            toolStripStatusLabelAction.Text = "Automat přijímá vstupní multimnožinu!";
                                            return;

                                        }

                                        //Znovuinicializace pocatecniho stavu
                                        automat.actualStates.Clear();
                                        automat.oneInput = String.Empty;
                                        automat.unreadMultiset = new Dictionary<string, int>(automat.inputMultiset);
                                        foreach (State state in automat.states)
                                        {
                                            if (state.IsInitial())
                                            {
                                                automat.actualStates.Add(state);
                                            }
                                        }
                                        ClearStatesNKA();
                                        HighLightActStatesNKA();
                                        return;
                                }

                                case DialogResult.No:
                                {
                                        automat.poradiVstupu.Clear();
                                        konecvypoctu = true;

                                        ClearMultisetsNKA();
                                        ClearFunctionsNKA();
                                        toolStripStatusLabelAction.Text = "Automat přijímá vstupní multimnožinu!";
                                        return;
                                }
                            }


                        }
                    }
                    
                    automat.SetInitialState();
                }

            }
            else //pokud Dofirststep neprojde
            {
                automat.neprectenyRetezec = true;

                while (automat.oneInput.Length > 0)
                {

                    textBoxZpracovane.Text += automat.oneInput.Last() + " ";
                    automat.oneInput = automat.oneInput.Remove(automat.oneInput.Length - 1);

                }
                for (int i = 0; i < dataGridViewMultiset2.Rows.Count; i++)
                {
                    dataGridViewMultiset2.Rows[i].Cells[1].Value = automat.unreadMultiset[dataGridViewMultiset2.Rows[i].Cells[0].Value.ToString()];
                }

                textBoxZpracovane.Text += "\t";
                pocetSlovVstupu++;
                automat.SetInitialState();
            }

            if (pocetSlovVstupu == automat.pocetPermutaci + 1) 
            {
                if(!automat.uspesnyVypocet)
                {
                    MessageBox.Show("Automat nepřijímá vstupní multimnožinu!");

                }
                konecvypoctu = true;
            }
        }


        #region FUNCTIONS        
        #region Clearing Dgvs...
        #region DKA
        private void ClearStatesDKA()
        {
            for (int i = 0; i < dataGridViewStavy.Rows.Count; i++)
            {
                dataGridViewStavy.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.White;
            }

        }

        private void ResetStatesDKA()
        {
            for (int i = 0; i < dataGridViewStavy.Rows.Count; i++)
            {
                dataGridViewStavy.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.White;

                foreach (State state in automat.states)
                {
                    if (state.IsInitial() && dataGridViewStavy.Rows[i].Cells[0].Value.ToString() == state.ToString())
                    {
                        dataGridViewStavy.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                        dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;

                    }
                }

            }

        }

        private void ClearMultisetDKA()
        {
            for (int i = 0; i < dataGridViewMultiset.Rows.Count; i++)
            {
                dataGridViewMultiset.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void ResetMultisetDKA()
        {
            for (int i = 0; i < dataGridViewMultiset.Rows.Count; i++)
            {
                dataGridViewMultiset.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewMultiset.Rows[i].Cells[1].Value = Multiset[dataGridViewMultiset.Rows[i].Cells[0].Value.ToString()];
            }

        }
        private void ClearFunctionsDKA()
        {
            for (int i = 0; i < dataGridViewFunkce.Rows.Count; i++)
            {
                dataGridViewFunkce.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewFunkce.Rows[i].Cells[2].Style.BackColor = Color.White;
            }
        }

        private void ClearFullDKA()
        {
            ClearMultisetDKA();
            ClearFunctionsDKA();
            ClearStatesDKA();


            dataGridViewFunkce.ClearSelection();
            dataGridViewStavy.ClearSelection();
            dataGridViewMultiset.ClearSelection();
        }

        private void ResetFullDKA()
        {
            ResetStatesDKA();
            ResetMultisetDKA();
            ClearFunctionsDKA();

            dataGridViewFunkce.ClearSelection();
            dataGridViewStavy.ClearSelection();
            dataGridViewMultiset.ClearSelection();

        }
        #endregion
        #region NKA

        private void ClearStatesNKA()
        {
            for (int i = 0; i < dataGridViewStavy2.Rows.Count; i++)
            {
                dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.White;
            }

        }

        /// <summary>
        /// Highlight initial state
        /// </summary>
        private void ResetStatesNKA()
        {
            for (int i = 0; i < dataGridViewStavy2.Rows.Count; i++)
            {
                dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.White;

                foreach(State state in automat.states)
                {
                    if (state.IsInitial() && dataGridViewStavy2.Rows[i].Cells[0].Value.ToString() == state.ToString()) 
                    {
                        dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                        dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;

                    }
                }

            }
        }

        /// <summary>
        /// Set values form unreadMultiset
        /// </summary>
        private void ClearMultisetsNKA()
        {
            for (int i = 0; i < dataGridViewMultiset2.Rows.Count; i++)
            {
                dataGridViewMultiset2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewMultiset2.Rows[i].Cells[1].Value = automat.unreadMultiset[dataGridViewMultiset2.Rows[i].Cells[0].Value.ToString()];
            }
        }

        /// <summary>
        /// Set value from inputMultiset
        /// </summary>
        private void ResetMultisetNKA()
        {
            for (int i = 0; i < dataGridViewMultiset.Rows.Count; i++)
            {
                dataGridViewMultiset2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewMultiset2.Rows[i].Cells[1].Value = automat.inputMultiset[dataGridViewMultiset.Rows[i].Cells[0].Value.ToString()];
            }

        }



        /// <summary>
        /// White all 
        /// </summary>
        private void ClearFunctionsNKA()
        {
            for (int i = 0; i < dataGridViewFunkce2.Rows.Count; i++)
            {
                dataGridViewFunkce2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                dataGridViewFunkce2.Rows[i].Cells[2].Style.BackColor = Color.White;
            }
        }

        
        /// <summary>
        /// White everything and set unreadMultiset
        /// </summary>
        private void ClearFullNKA()
        {
            ClearStatesNKA();
            ClearMultisetsNKA();
            ClearFunctionsNKA();

            dataGridViewFunkce2.ClearSelection();
            dataGridViewStavy2.ClearSelection();
            dataGridViewMultiset2.ClearSelection();
        }

        /// <summary>
        /// White everything and Highlight initial state and set inputMultiset
        /// </summary>
        private void ResetFullNKA()
        {
            ResetStatesNKA();
            ResetMultisetNKA();
            ClearFunctionsNKA();

            dataGridViewFunkce2.ClearSelection();
            dataGridViewStavy2.ClearSelection();
            dataGridViewMultiset2.ClearSelection();

            textBoxZpracovane.Clear();

        }
        #endregion
        #endregion


        #region Highlighting
        #region DKA

        /// <summary>
        /// Highlight actual state
        /// </summary>
        private void HighLightActStateDKA()
        {
            for (int i = 0; i < dataGridViewStavy.Rows.Count; i++)
            {
                if (dataGridViewStavy.Rows[i].Cells[0].Value.ToString() == automat.actualState.ToString())
                {
                    dataGridViewStavy.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                    dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;


                }
            }

        }
        /// <summary>
        /// Highlight next state
        /// </summary>
        private void HighLightNextStateDKA()
        {
            if(automat.actualState.Equals(automat.nextState))
            {
                for (int i = 0; i < dataGridViewStavy.Rows.Count; i++)
                {
                    if (dataGridViewStavy.Rows[i].Cells[0].Value.ToString() == automat.nextState.ToString())
                    {
                        dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.Cyan;
                    }
                }
            }
            else
            {
                for (int i = 0; i < dataGridViewStavy.Rows.Count; i++)
                {
                    if (dataGridViewStavy.Rows[i].Cells[0].Value.ToString() == automat.nextState.ToString())
                    {
                        dataGridViewStavy.Rows[i].DefaultCellStyle.BackColor = Color.Cyan;
                        dataGridViewStavy.Rows[i].Cells[1].Style.BackColor = Color.Cyan;
                    }
                }
            }
        }

        /// <summary>
        /// Highlight actual transition
        /// </summary>
        private void HighLightFunctionsDKA()
        {
            for (int i = 0; i < dataGridViewFunkce.Rows.Count; i++)
            {
                if (dataGridViewFunkce.Rows[i].Cells[0].Value.ToString() == automat.actualState.ToString())
                {
                    if (dataGridViewFunkce.Rows[i].Cells[1].Value.ToString() == automat.actualSymbol || dataGridViewFunkce.Rows[i].Cells[1].Value.ToString() == automat.actualSymbol.ToUpper())
                    {
                        if (dataGridViewFunkce.Rows[i].Cells[2].Value.ToString() == automat.nextState.ToString())
                        {
                            dataGridViewFunkce.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                            dataGridViewFunkce.Rows[i].Cells[2].Style.BackColor = Color.Cyan;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Highlight and decrease actual symbol, if detection symbol, only highlight
        /// </summary>
        private void HighLightAndDecMultisetDKA()
        {
            for (int i = 0; i < dataGridViewMultiset.Rows.Count; i++)
            {
                if (dataGridViewMultiset.Rows[i].Cells[0].Value.ToString() == automat.actualSymbol)
                {
                    dataGridViewMultiset.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                    if (Convert.ToInt32(dataGridViewMultiset.Rows[i].Cells[1].Value) > 0)
                    {
                        dataGridViewMultiset.Rows[i].Cells[1].Value = Convert.ToInt32(dataGridViewMultiset.Rows[i].Cells[1].Value) - 1;

                    }
                    break;
                }
            }

        }
        #endregion

        #region NKA
        /// <summary>
        /// Highlight actual states
        /// </summary>
        private void HighLightActStatesNKA()
        {
            for (int i = 0; i < dataGridViewStavy2.Rows.Count; i++)
            {
                foreach (State actualState in automat.actualStates)
                {
                    if (dataGridViewStavy2.Rows[i].Cells[0].Value.ToString() == actualState.ToString())
                    {
                        dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                        dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                    }
                }
            }
        }

        /// <summary>
        /// Highlight next sates
        /// </summary>
        private void HighLightNextStatesNKA()
        {
            foreach (State st in automat.nextStates)
            {
                for (int i = 0; i < dataGridViewStavy2.Rows.Count; i++)
                {
                    if (dataGridViewStavy2.Rows[i].Cells[0].Value.ToString() == st.ToString() && !automat.actualStates.Contains(st))
                    {
                        dataGridViewStavy2.Rows[i].DefaultCellStyle.BackColor = Color.Cyan;
                        dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.Cyan;
                    }

                    if (dataGridViewStavy2.Rows[i].Cells[0].Value.ToString() == st.ToString() && automat.actualStates.Contains(st))
                    {
                        dataGridViewStavy2.Rows[i].Cells[1].Style.BackColor = Color.Cyan;
                    }
                }
            }
        }

        /// <summary>
        /// Highlight and decrease actual symbol, if detection symbol, only highlight
        /// </summary>
        private void HighLighAndDecMultisetNKA()
        {
            for (int i = 0; i < dataGridViewMultiset2.Rows.Count; i++)
            {
                if (dataGridViewMultiset2.Rows[i].Cells[0].Value.ToString() == automat.actualSymbol)
                {
                    dataGridViewMultiset2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                    if (Convert.ToInt32(dataGridViewMultiset2.Rows[i].Cells[1].Value) > 0)
                    {
                        dataGridViewMultiset2.Rows[i].Cells[1].Value = Convert.ToInt32(dataGridViewMultiset2.Rows[i].Cells[1].Value) - 1;
                    }
                }
                if (dataGridViewMultiset2.Rows[i].Cells[0].Value.ToString().ToUpper() == automat.actualSymbol && automat.unreadMultiset[automat.actualSymbol.ToLower()] == 0)
                {
                    dataGridViewMultiset2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                }

            }
        }

        /// <summary>
        /// Highlight actual transition
        /// </summary>
        private void HighLightFunctionsNKA()
        {
            for (int i = 0; i < dataGridViewFunkce2.Rows.Count; i++)
            {
                foreach (State actualState in automat.actualStates)
                {
                    if (dataGridViewFunkce2.Rows[i].Cells[0].Value.ToString() == actualState.ToString() && dataGridViewFunkce2.Rows[i].Cells[1].Value.ToString() == automat.actualSymbol.ToString())
                    {
                        dataGridViewFunkce2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                        dataGridViewFunkce2.Rows[i].Cells[2].Style.BackColor = Color.Cyan;
                    }

                    foreach (var v in automat.unreadMultiset)
                    {
                        if (v.Value == 0)
                        {
                            if (dataGridViewFunkce2.Rows[i].Cells[0].Value.ToString() == actualState.ToString() && dataGridViewFunkce2.Rows[i].Cells[1].Value.ToString() == v.Key.ToUpper())
                            {
                                dataGridViewFunkce2.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                                dataGridViewFunkce2.Rows[i].Cells[2].Style.BackColor = Color.Cyan;
                            }

                        }
                    }

                }

            }
        }





        #endregion
        #endregion




        #region Button Actions

        /// <summary>
        /// Reset multiset at input page
        /// </summary>
        private void ResetMultiset()
        {
            Multiset.Clear();
            textBoxPrvky.Clear();
            numericUpDownSymbol.Value = 0;
            comboBoxSymbol.Text = "a";
            comboBoxFunctionSymbol.Items.Clear();
        }

        /// <summary>
        /// Reset state at input page
        /// </summary>
        private void ResetState()
        {
            checkBoxInitialState.Enabled = true;
            checkBoxEndState.Enabled = true;

            checkBoxInitialState.Checked = true;
            checkBoxEndState.Checked = false;

            numericUpDownStateNumber.Value = 0;

            comboBoxActualState.Items.Clear();
            comboBoxNextState.Items.Clear();

            textBoxStavy.Clear();
            States.Clear();
        }

        /// <summary>
        /// Reset function at input page
        /// </summary>
        private void ResetFunction()
        {
            Functions.Clear();
            textBoxFunkce.Clear();
            pocetPrechodu = 1;
        }



        /// <summary>
        /// Save automat from input page into object
        /// </summary>
        private void SaveAutomat()
        {
            dataGridViewFunkce.Rows.Clear();
            dataGridViewStavy.Rows.Clear();
            dataGridViewMultiset.Rows.Clear();
            dataGridViewFunkce2.Rows.Clear();
            dataGridViewStavy2.Rows.Clear();
            dataGridViewMultiset2.Rows.Clear();

            State initialState = null;
            State endState = null;

            foreach (State st in States)
            {
                if (st.IsInitial())
                {
                    initialState = st;
                }
                if (st.IsEnd())
                {
                    endState = st;
                }
            }

            if (initialState == null && endState == null)
            {
                MessageBox.Show("Automat nemá koncový, nebo počáteční stav");
                return;
            }

            if (Functions.Count == 0)
            {
                MessageBox.Show("Neni zadana zadna prechodova funkce");
                return;
            }
            automat = new Automat(Multiset, States, Functions);

            textBoxInputMultiset.Clear();
            textBoxInputMultiset2.Clear();
            int i = 0;
            foreach (var v in Multiset)
            {
                i++;
                textBoxInputMultiset.Text += v.Key + "  " + "-" + "  " + v.Value + "\t";
                textBoxInputMultiset2.Text += v.Key + "  " + "-" + "  " + v.Value + "\t";
                if (i == 10)
                {
                    textBoxInputMultiset.Text += Environment.NewLine;
                    textBoxInputMultiset2.Text += Environment.NewLine;
                    i = 0;
                }
                dataGridViewMultiset.Rows.Add(v.Key, v.Value);
                dataGridViewMultiset2.Rows.Add(v.Key, v.Value);
            }

            bool koncovy;
            int radek = 0;
            foreach (State state in States)
            {
                koncovy = false;
                if (state.IsEnd())
                {
                    koncovy = true;

                }
                dataGridViewStavy.Rows.Add(state.ToString(), koncovy);
                dataGridViewStavy2.Rows.Add(state.ToString(), koncovy);

                if (state.IsInitial())
                {
                    dataGridViewStavy.Rows[radek].DefaultCellStyle.BackColor = Color.LimeGreen;
                    dataGridViewStavy2.Rows[radek].DefaultCellStyle.BackColor = Color.LimeGreen;

                }
                radek++;
            }

            foreach (Transition function in Functions)
            {
                dataGridViewFunkce.Rows.Add(function.actualState.ToString(), function.symbol, function.nextState.ToString());
                dataGridViewFunkce2.Rows.Add(function.actualState.ToString(), function.symbol, function.nextState.ToString());
            }

            toolStripStatusLabelStateLine.Text = "Automat byl uložen!";

        }


        /// <summary>
        /// Save object into file
        /// </summary>
        private void SaveAutomatToFile()
        {
            string path = String.Empty;
            if (automat != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "automatData Files (*.dat)|*.dat";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    path = sfd.FileName;

                }
                else
                {
                    return;
                }


                Stream stream = File.Open(path, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(stream, automat);
                stream.Close();
            }

        }

        #endregion



        #endregion

    }

}

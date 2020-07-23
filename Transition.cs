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
    public class Transition : ISerializable
    {
        public int number { get; }
        public State actualState { get; }
        public string symbol { get; }
        public State nextState { get; }

        public Transition(int number,State actualState, string symbol, State nextState)
        {
            this.number = number;
            this.actualState = actualState;
            this.symbol = symbol;
            this.nextState = nextState;
        }

        public override bool Equals(object obj)
        {
            var myFunction = obj as Transition;

            if (myFunction != null)
            {
                return this.actualState.Equals(myFunction.actualState) && this.symbol == myFunction.symbol && this.nextState.Equals(myFunction.nextState);
            }

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return number+":   "+ actualState.ToString() + " + " + symbol + "  " + "-->" + "  " + nextState.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }



        #region Serialization Implementation

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("number", number);
            info.AddValue("actualState", actualState);
            info.AddValue("symbol", symbol);
            info.AddValue("nextState", nextState);
        }

        public Transition (SerializationInfo info, StreamingContext context)
        {
            number = (int)info.GetValue("number", typeof(int));
            actualState = (State)info.GetValue("actualState", typeof(State));
            symbol = (string)info.GetValue("symbol", typeof(string));
            nextState = (State)info.GetValue("nextState", typeof(State));

        }
        #endregion
    }
}

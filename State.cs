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
    public class State : ISerializable
    {
        public int number { get; }
        public bool initialState { get; }
        public bool endState { get; }

        public State(int number, bool initialState, bool endState)
        {
            this.number = number;
            this.initialState = initialState;
            this.endState = endState;
        }

        public State(State state)
        {
            number = state.number;
            initialState = state.initialState;
            endState = state.endState;
        }

        public bool IsInitial()
        {
            return initialState == true;
        }

        public bool IsEnd()
        {
            return endState == true;
        }

        public override bool Equals(object obj)
        {
            var myState = obj as State;

            if (myState != null)
            {
                return this.number == myState.number && this.initialState == myState.initialState && this.endState == myState.endState;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "q" + number;
        }

        public override int GetHashCode()
        {
            return number;
        }






        #region Serialization Implementation
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("number",number);
            info.AddValue("initialState", initialState);
            info.AddValue("endState", endState);
        }

        public State(SerializationInfo info, StreamingContext context)
        {
            number = (int)info.GetValue("number", typeof(int));
            initialState = (bool)info.GetValue("initialState", typeof(bool));
            endState = (bool)info.GetValue("endState", typeof(bool));
        }

        #endregion


    }
}

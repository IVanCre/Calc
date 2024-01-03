using System;
using System.Collections.Generic;

namespace calc
{
    public struct CalcSetInfo
    {
        public string name { get; set; }
        public char[] operators { get; set; }
        public char[] numbers { get; set; }
    }

    public abstract class OperatorDetector
    {
        protected Dictionary<char, Operator> operators;
        protected Dictionary<char, int> numbers;
        protected string detectorName;

        char[] GetAllOperators()
        {
            char[] allOperionsName = new char[operators.Count];
            operators.Keys.CopyTo(allOperionsName, 0);

            return allOperionsName;
        }
        char[] GetAllNumbers()
        {
            char[] allNumbers = new char[numbers.Count];
            numbers.Keys.CopyTo(allNumbers, 0);
            return allNumbers;
        }


        public CalcSetInfo GetDetctorInfo()
        {
            return new CalcSetInfo()
            {
                name = detectorName,
                operators = GetAllOperators(),
                numbers = GetAllNumbers()
            };
        }
            



        public Priority GetOperatorPriority(char firstOperatorSymbol)
        {
            return operators[firstOperatorSymbol].priority;
        }
        public bool IsOperator(char firstOperatorSymbol)
        {
            return operators.ContainsKey(firstOperatorSymbol);
        }

        protected double SearchValueInCompletedOperations(int indexOfBoardString, List<Operation> completedOpeartion)
        {
            foreach (Operation oper in completedOpeartion)
            {
                if (oper.lastLeftIndex <= indexOfBoardString && indexOfBoardString <= oper.lastRigthIndex)
                    return oper.rezult;
            }
            return Int32.MinValue;
        }

        //на основании типа оператора, определяем, как вычленить границы операции и какие аргументы будут использоваться
        public abstract double Calculate(ref string parsingString, int leftBoardIndex, int rigthBoardIndex, int indexOfOperator, List<Operation> completedOperations);
        public virtual string ConvertToSystemType(double val)
        {
            return val.ToString();
        }

    }
}

using System;
using System.Collections.Generic;


namespace calc
{
    public sealed class StandartDetector : OperatorDetector
    {
        public StandartDetector()
        {
            detectorName = "Standart";
            operators = new Dictionary<char, Operator>(){{ '*' ,new Multiplication()},
                                                         { '/', new Division()},
                                                         { '+', new Plus()},
                                                         { '-', new Minus()},
                                                         { 'c', new Cosinus()},
                                                         { 's', new Sinus()},
                                                         { 't', new Tangens()},
                                                         { 'f', new Factorial()},
                                                         { 'n', new Negative()},
                                                         { '^', new Power()}
                                                         };

            numbers = new Dictionary<char, int>() {
                {'1',1},
                {'2',2},
                {'3',3},
                {'4',4},
                {'5',5},
                {'6',6},
                {'7',7},
                {'8',8},
                {'9',9},
                {'0',0}};
        }


        public override double Calculate(ref string parsingString, int leftBoardIndex, int rigthBoardIndex, int indexOfOperator, List<Operation> completedOperations)
        {
            double val_1 = 0;
            double val_2 = 0;
            string valStr = "";//очищенное значение,из которого удалены скобки, которые могли остаться после вычленения операции из общей строки
            //проверяем, не участвовали ли искомые числа уже в других операциях

            int numOfArgs = operators[parsingString[indexOfOperator]].numArgs;

            if (numOfArgs == 1)//т.к. все операторы пишутся преед аргументами
            {
                val_1 = SearchValueInCompletedOperations(rigthBoardIndex, completedOperations);
                if (val_1 == Int32.MinValue)
                {
                    valStr = parsingString.Substring(indexOfOperator + 1, rigthBoardIndex - indexOfOperator);
                    valStr = valStr.Replace("(", "");
                    valStr = valStr.Replace(")", "");
                    if (valStr[0] == '-')
                    {
                        valStr = valStr.Replace("-", "");
                        val_1 = Convert.ToInt32(valStr) * -1;
                    }
                    else
                        val_1 = Convert.ToInt32(valStr);
                }
            }

            if (numOfArgs == 2)
            {
                val_1 = SearchValueInCompletedOperations(leftBoardIndex, completedOperations);
                if (val_1 == Int32.MinValue)
                {
                    valStr = parsingString.Substring(leftBoardIndex, indexOfOperator - leftBoardIndex);
                    valStr = valStr.Replace("(", "");
                    valStr = valStr.Replace(")", "");

                    val_1 = Convert.ToInt32(valStr);
                }

                val_2 = SearchValueInCompletedOperations(rigthBoardIndex, completedOperations);
                if (val_2 == Int32.MinValue)
                {
                    valStr = parsingString.Substring(indexOfOperator + 1, rigthBoardIndex - indexOfOperator);
                    valStr = valStr.Replace("(", "");
                    valStr = valStr.Replace(")", "");

                    val_2 = Convert.ToInt32(valStr);
                }
            }

            //передаем нужное количество аргументов конкретному оператору для вычислений
            return operators[parsingString[indexOfOperator]].calc(val_1, val_2);
        }
    }
}

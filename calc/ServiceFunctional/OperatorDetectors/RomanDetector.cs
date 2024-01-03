using System;
using System.Collections.Generic;

namespace calc
{
    public sealed class RomanDetector : OperatorDetector
    {
        public RomanDetector()
        {
            detectorName = "Roman";
            operators = new Dictionary<char, Operator>(){{ '*' ,new Multiplication()},
                                                         { '/', new Division()},
                                                         { '+', new Plus()},
                                                         { '-', new Minus()}};
            numbers = new Dictionary<char, int>(){{'I',1 },
                                                  {'V',5 },
                                                  {'X',10},
                                                  {'L',50},
                                                  {'C',100},
                                                  {'D',500},
                                                  {'M',1000}};
        }

        public override double Calculate(ref string parsingString, int leftBoardIndex, int rigthBoardIndex, int indexOfOperator, List<Operation> completedOperations)
        {
            double val_1 = 0;
            double val_2 = 0;
            string valStr = "";


            val_1 = SearchValueInCompletedOperations(leftBoardIndex, completedOperations);
            if (val_1 == Int32.MinValue)
            {
                valStr = parsingString.Substring(leftBoardIndex, indexOfOperator - leftBoardIndex);

                val_1 = ConvertRomanToDouble(valStr);
            }

            val_2 = SearchValueInCompletedOperations(rigthBoardIndex, completedOperations);
            if (val_2 == Int32.MinValue)
            {
                valStr = parsingString.Substring(indexOfOperator + 1, rigthBoardIndex - indexOfOperator);

                val_2 = ConvertRomanToDouble(valStr);
            }
            return operators[parsingString[indexOfOperator]].calc(val_1, val_2);
        }


        public double ConvertRomanToDouble(string str)
        {
            double buffer = numbers[str[str.Length - 1]];

            for (int i = str.Length - 1; i > 0; i--)//от меньшего разряда к старшему, так проще
            {
                if (numbers[str[i - 1]] < numbers[str[i]])
                {
                    buffer -= numbers[str[i - 1]];
                }
                else
                {
                    buffer += numbers[str[i - 1]];
                }
            }
            return buffer;
        }
        public static string DoubleToRoman(double val)
        {
            string[,] values ={{"I","V","X"},
                               {"X","L","C"},
                               {"C","D","M"},
                               {"M","M","M"}};
            string str = "";
            char[] val_1 = val.ToString().ToCharArray();
            Array.Reverse(val_1);//чтобы порядок формирования числа шел с единиц 

            for (int i = 0; i < val_1.Length; i++)
            {
                switch (val_1[i])
                {
                    case '0'://у римлян не было нормального обозначения для нуля как единичной цифры
                        break;
                    case '1':
                        str = values[i, 0] + str;
                        break;
                    case '2':
                        str = values[i, 0] + values[i, 0] + str;
                        break;
                    case '3':
                        str = values[i, 0] + values[i, 0] + values[i, 0] + str;
                        break;
                    case '4':
                        str = values[i, 0] + values[i, 1] + str;
                        break;
                    case '5':
                        str = values[i, 1] + str;
                        break;
                    case '6':
                        str = values[i, 1] + values[i, 0] + str;
                        break;
                    case '7':
                        str = values[i, 1] + values[i, 0] + values[i, 0] + str;
                        break;
                    case '8':
                        str = values[i, 1] + values[i, 0] + values[i, 0] + values[i, 0] + str;
                        break;
                    case '9':
                        str = values[i, 0] + values[i, 2] + str;
                        break;
                }
            }
            return str;
        }


        string SelectOstatok(double val)
        {
            double[] uncio = {0.083333,//1
                         0.166666,//2
                         0.25,    //3
                         0.333333,//4
                         0.416666,//5
                         0.5,     //6
                         0.583333,//7
                         0.666666,//8
                         0.75,    //9
                         0.833333,//10
                         0.916666,//11
                         1};

            if (val > 0)
            {
                for (int i = 0; i < uncio.Length; i++)
                {
                    if (uncio[i] > val)
                    {
                        return $" и ~ {DoubleToRoman(i)} унций";
                    }
                    if (uncio[i] == val)
                    {
                        return $" и {DoubleToRoman(i + 1)} унций";
                    }
                }
            }
            return "";
        }
        public override string ConvertToSystemType(double val)
        {
            double part = val - Math.Truncate(val);
            return DoubleToRoman(Math.Truncate(val))    +SelectOstatok(part)+ $"  ({val})";
        }
    }
}

using System;
using System.Collections.Generic;

namespace calc
{
    public struct Operation//хранит результат операции и крайние индексы символов в исходной строке, которые в ней участвовали
    {
        public double rezult;
        public int lastLeftIndex;
        public int lastRigthIndex;
        public Operation(double rez, int left, int rigth)
        {
            rezult = rez;
            lastLeftIndex = left;//индексы показывают, где начинается и где заканчивается операция в общей строке. Операция - это 2 участника и оператор между ними(стандартная запись)
            lastRigthIndex = rigth;
        }

        public void Clear()
        {
            rezult = 0;
            lastLeftIndex = -1;
            lastRigthIndex = -1;
        }
    }


    public enum Priority : byte
    {
        max,
        higth,
        middle,
        low,
        none
    }
        public struct RequestForParse
        {
            public string type { get; set; }
            public bool expln { get; set; }
            public string dataForParse { get; set; }
        }


    public class OperationParser
    {
        enum BraceType : byte
        {
            open,
            close
        }
        struct Brace
        {
            public int indexInStr;
            public int priorLevel;
            public BraceType type;

            public Brace(int index, int priorLvl, BraceType _type)
            {
                indexInStr = index;
                priorLevel = priorLvl;
                this.type = _type;
            }
        }


        public struct OperatorProp
        {
            public Priority priority { get; }
            public int operatorIndex { get; }
            public int innerLevelPriority { get; }

            public OperatorProp(int innerPriority, Priority _priority, int index)
            {
                priority = _priority;
                operatorIndex = index;
                innerLevelPriority = innerPriority;
            }

            public static int Compare(OperatorProp a, OperatorProp b)//левый больше =1, правый больше =-1, одинаковые =0
            {
                if (a.innerLevelPriority == b.innerLevelPriority)
                    if (a.priority == b.priority)
                    {
                        if (a.operatorIndex == b.operatorIndex)
                        {
                            return 0;
                        }
                        else
                        {
                            if (a.operatorIndex < b.operatorIndex)
                                return 1;
                            else
                                return -1;
                        }
                    }

                if (a.innerLevelPriority == b.innerLevelPriority)
                {
                    if (a.priority < b.priority)
                        return 1;//у левого приоритет выше
                    else
                        return -1;
                }

                if (a.innerLevelPriority > b.innerLevelPriority)
                    return 1;
                else
                    return -1;
            }
        }

        string systemType;
        bool isUseExplains = false;
        List<Brace> innerPriorityList = new List<Brace>();//хранит индексы каждой скобки и присваивает ей(скобке) внутренний приоритет выполнения
        List<OperatorProp> priorityList = new List<OperatorProp>();//порядок выполнения операторов
        List<Operation> completedOperation = new List<Operation>();//буфер хранения уже выполненных операций
        List<Operation> operationIndexToDelete = new List<Operation>();//отслеживает индексы выполненных операций, которые уже были использованы в других операциях
        string parsingString;

        Operation newOper = new Operation(0, -1, -1);
        public OperatorDetector operatorDetector;// в зависимости от выбранной системы счисления выбирает способ парсинга операций/операторов и доступные операторы



        public void Set(RequestForParse set)
        {
            systemType = set.type;
            isUseExplains = set.expln;

            switch (set.type)
            {
                case "Standart":
                    {
                        operatorDetector = new StandartDetector();
                        break;
                    }
                case "Roman":
                    {
                        operatorDetector = new RomanDetector();
                        break;
                    }
            }
        }
    



        Operation ConvertToOperation(int indexOfOperator, List<Operation> completedOpeartion)//преобразует строку в Операцию
        {
            //ищем индексы в строке, где начинается и заканчивается операция с указанным оператором(индексом)
            int leftBoardIndex = -1;
            int rigthBoardIndex = -1;
            for (int i = indexOfOperator - 1; i > -1; i--)
            {
                if (operatorDetector.IsOperator(parsingString[i]))
                {
                    leftBoardIndex = i + 1;
                    break;
                }
            }
            if (leftBoardIndex == -1)
                leftBoardIndex = 0;

            for (int i = indexOfOperator + 1; i < parsingString.Length; i++)
            {
                if (operatorDetector.IsOperator(parsingString[i]) && !operatorDetector.IsOperator(parsingString[i-1]))// символ-оператор
                {
                    rigthBoardIndex = i - 1;
                    break;
                }
                else if(operatorDetector.IsOperator(parsingString[i]) && operatorDetector.IsOperator(parsingString[i - 1]))//оператор-оператор(значит второй оператор- оператор с 1 аргументом, типа синуса)
                {
                    rigthBoardIndex = i;
                    break;
                }
            }
            if (rigthBoardIndex == -1)
            {
                rigthBoardIndex = parsingString.Length - 1;
            }


            newOper.Clear();

            double rezult = operatorDetector.Calculate(ref parsingString, leftBoardIndex, rigthBoardIndex, indexOfOperator, completedOpeartion);

            newOper = new Operation(rezult, leftBoardIndex, rigthBoardIndex);

            if(isUseExplains)
            Console.WriteLine($"Создана новая операция: rezult={newOper.rezult} / границы операции(в строке) [{newOper.lastLeftIndex}_{newOper.lastRigthIndex}]");

            return newOper;
        }
        void AddToCompleted(Operation newOper, List<Operation> completedOperation)
        {
            for (int i = 0; i < completedOperation.Count; i++)
            {
                if (completedOperation[i].lastLeftIndex <= newOper.lastLeftIndex && newOper.lastLeftIndex <= completedOperation[i].lastRigthIndex)
                {
                    newOper.lastLeftIndex = completedOperation[i].lastLeftIndex;
                    operationIndexToDelete.Add(completedOperation[i]);
                }


                else if (completedOperation[i].lastLeftIndex <= newOper.lastRigthIndex && newOper.lastRigthIndex <= completedOperation[i].lastRigthIndex)
                {
                    newOper.lastRigthIndex = completedOperation[i].lastRigthIndex;
                    operationIndexToDelete.Add(completedOperation[i]);
                }


                if (operationIndexToDelete.Count == 2)//если в текущей опреации участвовали сразу 2 опреации, то удалять надо сразу обе
                    break;
            }

            foreach (Operation op in operationIndexToDelete)
                completedOperation.Remove(op);

            operationIndexToDelete.Clear();

            completedOperation.Add(newOper);

            if (isUseExplains)
            {
                Console.WriteLine("буфер вычисленных операций после добавления новой операции:");
                foreach (Operation op in completedOperation)
                    Console.WriteLine($" rez={op.rezult} / boards[{op.lastLeftIndex}_{op.lastRigthIndex}]");
            }
        }

        void CalcOperation( int indexOperation, List<Operation> completedOperation)
        {
            Operation oper = ConvertToOperation(indexOperation, completedOperation);
            AddToCompleted(oper, completedOperation);
        }



        void CheckSyntaxOfPriorityNodes()
        {
            if (innerPriorityList.Count > 0)
            {
                if (innerPriorityList[0].type == BraceType.close)
                    throw new Exception("innerLevel list cant  start from ')' symbol !");

                if (innerPriorityList[innerPriorityList.Count - 1].type == BraceType.open)
                    throw new Exception("innerLevel list cant end to '(' symbol !");


                int differentBetwenSymbols = 0;
                foreach (Brace node in innerPriorityList)
                {
                    if (node.type == BraceType.open)
                        differentBetwenSymbols += 1;
                    else
                        differentBetwenSymbols -= 1;
                }
                if (differentBetwenSymbols != 0)
                    throw new Exception("different betwen open symbols and close is not zero!");
            }
        }
        void BracePriority()//присваиваем каждой скобке ее приоритет, все операции в этой скобке будут иметь ее приоритет
        {
            int innerLevel = 0;

            for (int i = 0; i < parsingString.Length; i++)
            {
                if (parsingString[i] == '(')
                {
                    if (systemType == "Standart")
                    {
                        innerLevel += 1;
                        innerPriorityList.Add(new Brace(i, innerLevel, BraceType.open));
                    }
                    else
                        throw new Exception("в данной системе скобки не поддерживаются!");
                }
                else if (parsingString[i] == ')')
                {
                    if (systemType == "Standart")
                    {
                        innerPriorityList.Add(new Brace(i, innerLevel, BraceType.close));
                        innerLevel -= 1;//уменьшаем приоритет, т.к. скобка закрылась
                    }
                    else
                        throw new Exception("в данной системе скобки не поддерживаются!");
                }
            }

            CheckSyntaxOfPriorityNodes();

            if (isUseExplains)
            {
                Console.WriteLine("\n индексы скобокок(в строке) и их приоритет вложенности:");
                foreach (Brace elem in innerPriorityList)
                    Console.WriteLine($"inner prior: indexChar={elem.indexInStr} prior={elem.priorLevel} type={elem.type}");
            }
        }
        int SearchInnerPriority(int indexOfOperationInStr)//на основании индекса оператора, под которым он стоит в строке, мы сопоставляем ему приоритет вложенности
        {
            int minInnerLevel = 0;
            for(int i=1;i<innerPriorityList.Count;i++)
            {
                if(innerPriorityList[i-1].indexInStr<indexOfOperationInStr && indexOfOperationInStr<innerPriorityList[i].indexInStr)
                {
                    if (parsingString[innerPriorityList[i - 1].indexInStr] == ')' && parsingString[innerPriorityList[i].indexInStr] == '(')
                        minInnerLevel = Math.Min(innerPriorityList[i - 1].priorLevel, innerPriorityList[i].priorLevel) - 1;//операция между двумя равными приоритетными скобками всегда ниже их по приоритету
                    else
                        minInnerLevel = Math.Min(innerPriorityList[i - 1].priorLevel, innerPriorityList[i].priorLevel);
                }
            }
            return minInnerLevel;
        }
        void CreatePriority()//выстраиваем порядок выполнения всех операций(учитывая приоритет вложенности и обычный приоритет операций) 
        {
            BracePriority();

            int innerLevelPriority = 0;

            for (int i = 0; i < parsingString.Length; i++)
            {
                if(operatorDetector.IsOperator(parsingString[i]))
                {
                    innerLevelPriority = SearchInnerPriority(i);
                    priorityList.Add(new OperatorProp(innerLevelPriority,operatorDetector.GetOperatorPriority(parsingString[i]), i));
                }
            }
            innerPriorityList.Clear();

            priorityList.Sort(OperatorProp.Compare);
            priorityList.Reverse();//от большего к меньшему

            if (isUseExplains)
            {
                Console.WriteLine("\n итоговый порядок выполнения операторов:");
                foreach (OperatorProp prior in priorityList)
                    Console.WriteLine($"innerLVL={prior.innerLevelPriority} prior={prior.priority}  operator(indx={prior.operatorIndex}): {parsingString[prior.operatorIndex]}  ");
            }
        }






        public string Parse(RequestForParse set, ILogger logger)
        {
            Set(set);

            string rezult = "";
            try
            {
                parsingString = set.dataForParse;

                CreatePriority();

                foreach (OperatorProp node in priorityList)
                    CalcOperation( node.operatorIndex, completedOperation);

                if (completedOperation.Count > 0)
                    rezult="Response="+ operatorDetector.ConvertToSystemType(completedOperation[0].rezult);//все операции, в итоге, сольются в одну операцию
                else
                    rezult="ОШИБКА! Cписок операций пуст!";
            }
            catch (Exception e)
            {
                logger.Save("Parser exception! ="+e.ToString());
            }
            Clear();
            return rezult;
        }

        void Clear()
        {
            innerPriorityList.Clear();
            completedOperation.Clear();
            operationIndexToDelete.Clear();
            priorityList.Clear();
        }
    }
}

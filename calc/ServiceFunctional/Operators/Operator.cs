using System;

namespace calc
{
    public abstract class Operator
    {
        public string name;
        public Priority priority;
        public int numArgs;

        public virtual double calc(double val_1, double val_2)
        {
            return 0;
        }
    }



    sealed class Plus : Operator
    {
        public Plus()
        {
            name = "+";
            priority = Priority.low;
            numArgs = 2;
        }

        public override double calc(double val_1, double val_2)
        {
            return val_1 + val_2;
        }
    }
    sealed class Minus : Operator
    {
        public Minus()
        {
            name = "-";
            priority = Priority.low;
            numArgs = 2;
        }
        public override double calc(double val_1, double val_2)
        {
            return val_1 - val_2;
        }
    }
    sealed class Multiplication : Operator
    {
        public Multiplication()
        {
            name = "*";
            priority = Priority.middle;
            numArgs = 2;
        }
        public override double calc(double val_1, double val_2)
        {
            return val_1 * val_2;
        }
    }
    sealed class Division : Operator
    {
        public Division()
        {
            name = "/";
            priority = Priority.middle;
            numArgs = 2;
        }
        public override double calc(double val_1, double val_2)
        {
            return val_1 / val_2;
        }
    }

    sealed class Negative:Operator
    {
        public Negative()
        {
            name = "negative";
            priority = Priority.max;
            numArgs = 1;
        }
        public override double calc(double val_1, double val_2 = 0)
        {
            return val_1 *(-1);
        }
    }


    sealed class Sinus:Operator
    {
        public Sinus()
        {
            name = "sin";
            priority = Priority.higth;
            numArgs = 1;
        }
        public override double calc(double val_1, double val_2=0)
        {
            return Math.Sin(val_1);
        }
    }
    sealed class Cosinus : Operator
    {
        public Cosinus()
        {
            name = "cos";
            priority = Priority.higth;
            numArgs = 1;
        }
        public override double calc(double val_1, double val_2=0)
        {
            return Math.Cos(val_1);
        }
    }
    sealed class Tangens : Operator
    {
        public Tangens()
        {
            name = "tan";
            priority = Priority.higth;
            numArgs = 1;
        }
        public override double calc(double val_1, double val_2 = 0)
        {
            return Math.Tan(val_1);
        }
    }

    sealed class Factorial:Operator
    {
        public Factorial()
        {
            name = "factorial";
            priority = Priority.higth;
            numArgs = 1;
        }
        public override double calc(double val_1, double val_2 = 0)
        {
            int buffer = 1;
            for (int i = 2; i < val_1 + 1; i++)
                buffer = buffer * i;
            
            return buffer;
        }
    }
    sealed class Power : Operator
    {
        public Power()
        {
            name = "^";
            priority = Priority.higth;
            numArgs = 2;
        }
        public override double calc(double val_1, double val_2)
        {
            return Math.Pow(val_1,val_2);
        }
    }
}

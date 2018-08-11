using System;

namespace Sample
{
    public enum Access
    {
        Delete = 1,
        Publish = 2,
        Submit = 4,
        Comment = 8,
        Modify = 16,
        Writer = Submit | Modify,
        Editor = Delete | Publish | Comment,
        Owner = Writer | Editor

    }

    public class TextInput
    {
        private string text;

        public TextInput()
        {
            text = string.Empty;
        }

        public virtual void Add(char ch)
        {
            text += ch;
        }

        public virtual string GetValue()
        {
            return text;
        }
    }

    public class NumericInput : TextInput
    {
        public override void Add(char ch)
        {
            if (Char.IsDigit(ch))
            {
                base.Add(ch);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TextInput input = new NumericInput();

            input.Add('1');
            input.Add('a');
            input.Add('0');

            Console.WriteLine(input.GetValue());

            Console.ReadKey();

        }
    }
}

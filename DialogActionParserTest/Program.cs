using System;

namespace DialogActionParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var testActions = "let a = 81 \nlet b = 'Zanzibar' \nshow_browser 'http://www.google.com' \nlet blah='O\\'Really?'\n let t =true";
            var actions = DialogActionParser.Parse(testActions);

            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }
    }
}

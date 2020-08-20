using System;
using System.Collections.Generic;

namespace DialogActionParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var testActions = "let a = 81 \nlet b = 'Zanzibar' \nshow_browser 'http://www.google.com' \nlet blah='O\\'Really?'\n let t =true";
            var actions = DialogActionParser.Parse(testActions);

            var symbols = new Dictionary<string, DialogActionValue>();

            foreach (var action in actions)
            {
                Console.WriteLine(action);
                // We'll only handle "let" actions in this example
                if (action.action == "let" && action.args.Count == 3 && action.args[0].type == DialogActionValue.Type.Identifier && action.args[1].type == DialogActionValue.Type.Operator && action.args[1].Operator == "=")
                {
                    symbols[action.args[0].Identifier] = action.args[2];
                }
            }

            Console.WriteLine();
            Console.WriteLine("Symbols:");
            foreach(var s in symbols)
            {
                Console.WriteLine($"{s.Key} = {s.Value}");
            }
        }
    }
}

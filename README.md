# A very simple scripting language parser for C#

This is an ad-hoc parser for a small scripting language in C#. It's not a complete language - it only deals with syntax, you need to provide the semantics yourself.

The language looks like this:

```
let a = 42
let b = "hello, world"
let c = true
perform_action a b "some argument"
do_something_else
```

The language is newline-delimited, each action is a separate line. Each action must start with an identifier (you can think of it as a command) which can be followed by arbitrary arguments.

At the very simplest case, it's not a Turing complete language. The output of the parser is a list of parsed actions (aka commands, statements), 
and the caller will walk through the list and perform whatever action is needed. That includes almost everything relating to the syntax, maintaining the symbol table, 
validating the arguments are correct, etc.

The parser only knows about 3 data types:

* String
* Number
* Bool

and there are two special token types:

* Operator
* Identifier

## Examples

A real-world-ish example might be:

```
using System;
using System.Collections.Generic;

namespace DialogActionParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var testProgram = "let a = 81 \nlet b = 'Zanzibar' \nshow_browser 'http://www.google.com' \nlet blah='O\\'Really?'\n let t =true";
            var actions = DialogActionParser.Parse(testProgram);

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
```

The main thing to grok is the `DialogActionValue` class, which implements a sort of variable data type. Depending on its `type` field, the data encapsulated in the instance can be
accessed by one of the read-only properties: `Identifier`, `String`, `Bool`, `Number` or `Operator`.

## Notes

* The design of this parser will not win any awards in efficiency or performance, you really shouldn't use it for anything complex.
* This same design is also implemented as a Go module `minigoscript` at https://github.com/ivoras/minigoscript

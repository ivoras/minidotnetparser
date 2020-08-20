# A very simple scripting language parser for C#

This is an ad-hoc parser for a small scripting language in C#. It's not a complete language - it only deals with syntax, you need to provide the semantics yourself.

The language looks like this:

```
let a = 42
let b = "hello, world"
let c = true
perform_action a b "some argument"
```

At the very simplest case, it's not a Turing complete language. The output of the parser is a list of parsed actions (aka commands, statements), 
and the caller will walk through the list and perform whatever action is needed. That includes maintaing the symbol table, validating the arguments are correct, etc.

The parser knows only 3 data types:

* String
* Number
* Bool

and there are two special token types:

* Operator
* Identifier


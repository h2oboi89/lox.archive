# [lox](https://github.com/h2oboi89/lox.archive)

## Summary

Lox interpreter written in C# (.NET Framework 4.8)

This was my first attempt at Crafting Interpreters that sputtered out at some point.
Doing it all over again in C# .NET 7 here: https://github.com/h2oboi89/lox

Based on Bob Nystrom's Lox book
 - website: <a href="http://craftinginterpreters.com/">Crafting Interpreters</a>
 - github:  <a href="https://github.com/munificent/craftinginterpreters">munificent/craftinginterpreters</a>

## Significant Deviations

* `print` keyword converted into built-in function

* redeclaring variables outside of prompt throws an exception

* added % operator (modulo)

## Challenges

* added `break` and `continue` statements

# [lox](https://github.com/h2oboi89/lox)

## Summary

Lox interpreter written in C# (.NET Framework 4.8)

Based on Bob Nystrom's Lox book
 - website: <a href="http://craftinginterpreters.com/">Crafting Interpreters</a>
 - github:  <a href="https://github.com/munificent/craftinginterpreters">munificent/craftinginterpreters</a>

## Significant Deviations

* `print` keyword converted into built-in function

* redeclaring variables outside of prompt throws an exception

## Challenges

* added `break` and `continue` statements
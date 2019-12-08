﻿namespace LoxVM.Compiling
{
    enum Precedence
    {
        NONE,
        ASSIGNMENT,
        OR,
        AND,
        EQUALITY,
        COMPARISON,
        TERM,
        FACTOR,
        UNARY,
        CALL,
        PRIMARY
    }
}
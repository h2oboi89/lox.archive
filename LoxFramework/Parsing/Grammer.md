# Lox Grammar

## Grammer
| Rule					| Goes to													|
|-----------------------|-----------------------------------------------------------|
| program				| declaration\* EOF											|
|						|															|
| declaration			| classDeclaration											|
|						| functionDeclaration										|
|						| variableDeclaration										|
|						| statement													|
|						|															|
| classDeclaration		| "class" IDENTIFIER "{" function* "}"						|
|						|															|
| functionDeclaration	| "fun" function											|
|						|															|
| function				| IDENTIFIER "(" parameters? ")" block						|
|						|															|
| parameters			| IDENTIFIER ( "," IDENTIFIER )*							|
|						|															|
| variableDeclaration	| "var" IDENTIFIER ( "=" expression )? ";"					|
|						|															|
| statement				| expressionStatement										|
|						| forStatement												|
|						| ifStatement												|
|						| printStatement											|
|						| returnStatement											|
|						| whileStatement											|
|						| breakStatement											|
|						| block														|
|						|															|
| returnStatement		| "return" expression? ";"									|
|						|															|
| expressionStatement	| expression ";"											|
|						|															|
| forStatement			| "for" "(" ( variableDeclaration \| expressionStatement \| ";" ) expression? ";" expression? ")" statement |
|						|															|
| ifStatement			| "if" "(" expression ")" statement ( "else" statement )?	|
|						|															|
| printStatement		| "print" expression ";"									|
|						|															|
| whileStatement		| "while" "(" expression ")" statement						|
|						|															|
| breakStatement		| "break" ";"												|
|						|															|
| block					| "{" declaration\* "}"										|
|						|															|
| expression			| assignment												|
|						|															|
| assignment			| ( call "." )? IDENTIFIER "=" assignment					|
|						| logic_or													|
|						|															|
| logic_or				| logic_and ( "or" logic_and )*								|
|						|															|
| logic_and				| equality ( "and" equality )*								|
|						|															|
| equality				| comparison ( ( "!=" \| "==" ) comparison )*				|
|						|															|
| comparison			| addition ( ( ">" \| ">=" \| "<" \| "<=" ) addition )*		|
|						|															|
| addition				| multiplication ( ( "-" \| "+" ) multiplication )*			|
|						|															|
| multiplication		| unary ( ( "/" \| "\*" ) unary )*							|
|						|															|
| unary					| ( "!" \| "-" ) unary \| call								|
|						|															|
| call					| primary ( "(" arguments? ")" | "." IDENTIFIER )*			|
|						|															|
| arguments				| expression ( "," expression )*							|
|						|															|
| primary				| "true" \| "false" \| "nil"								|
|						| NUMBER \| STRING											|
|						| "(" expression ")"										|
|						| IDENTIFIER												|

## Rules
| Grammar notation	| Code representation				|
|-------------------|-----------------------------------|
| Terminal			| Code to match and consume a token	|
| Nonterminal		| Call to that rule’s function		|
| \|				| If or switch statement			|
| \* or +			| While or for loop					|
| ?					| If statement						|

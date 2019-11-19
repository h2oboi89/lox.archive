# Lox Grammar

## Grammer
| Rule					| Goes to												|
|-----------------------|-------------------------------------------------------|
| program				| declaration\* EOF										|
| declaration			| variableDeclaration									|
|						| statement												|
| variableDeclaration	| "var" IDENTIFIER ( "=" expression )? ";"				|
| statement				| expressionStatement									|
|						| printStatement										|
| expressionStatement	| expression ";"										|			
| printStatement		| "print" expression ";"								|
| expression			| equality												|
| equality				| comparison ( ( "!=" \| "==" ) comparison )*			|
| comparison			| addition ( ( ">" \| ">=" \| "<" \| "<=" ) addition )* |
| addition				| multiplication ( ( "-" \| "+" ) multiplication )*		|
| multiplication		| unary ( ( "/" \| "\*" ) unary )*						|
| unary					| ( "!" \| "-" ) unary									|
|						| primary												|
| primary				| "true" \| "false" \| "nil"							|
|						| NUMBER \| STRING										|
|						| "(" expression ")"									|
|						| IDENTIFIER											|

## Rules
| Grammar notation	| Code representation				|
|-------------------|-----------------------------------|
| Terminal			| Code to match and consume a token	|
| Nonterminal		| Call to that rule’s function		|
| \|				| If or switch statement			|
| \* or +			| While or for loop					|
| ?					| If statement						|

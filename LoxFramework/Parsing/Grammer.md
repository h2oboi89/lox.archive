# Lox Grammar

## Grammer
| Rule					| Goes to												|
|-----------------------|-------------------------------------------------------|
| program				| statement\* EOF										|
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
| primary				| NUMBER \| STRING \| "false" \| "true" \| "nil"		|
|						| "(" expression ")"									|

## Rules
| Grammar notation	| Code representation				|
|-------------------|-----------------------------------|
| Terminal			| Code to match and consume a token	|
| Nonterminal		| Call to that rule’s function		|
| \|				| If or switch statement			|
| \* or +			| While or for loop					|
| ?					| If statement						|

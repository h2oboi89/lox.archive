# Lox Grammar

## Grammer
| Rule					| Goes to													|
|-----------------------|-----------------------------------------------------------|
| program				| declaration\* EOF											|
|						|															|
| declaration			| variableDeclaration										|
|						| statement													|
|						|															|
| variableDeclaration	| "var" IDENTIFIER ( "=" expression )? ";"					|
|						|															|
| statement				| expressionStatement										|
|						| forStatement												|
|						| ifStatement												|
|						| printStatement											|
|						| whileStatement											|
|						| block														|
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
| block					| "{" declaration\* "}"										|
|						|															|
| expression			| assignment												|
|						|															|
| assignment			| IDENTIFIER "=" assignment									|
|						| logc_or													|
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
| unary					| ( "!" \| "-" ) unary										|
|						| primary													|
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

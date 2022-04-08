lexer grammar BeamLexer;

channels {
	ERROR
}

options {
	superClass = BeamLexerBase;
}

Comment: '#' ~[\r\n]* -> channel(HIDDEN);

User: 'user';
Satellite: 'sat';
Interferer: 'interferer';

Decimal: DECIMAL;
Float: FLOAT;

WhiteSpacesToken: [ \t]+ -> channel(HIDDEN);
LineTerminatorToken: [\r\n] -> channel(HIDDEN);
UnexpectedCharacterToken: . -> channel(ERROR);

fragment DECIMAL: '0' | [1-9] [0-9]*;
fragment FLOAT: [-+]?[0-9]*[.]?[0-9]+([eE][-+]?[0-9]+)?;

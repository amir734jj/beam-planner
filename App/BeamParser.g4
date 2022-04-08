parser grammar BeamParser;

options {
	tokenVocab = BeamLexer;
	superClass = BeamParserBase;
}

coordinate  : number number number
            ;

id          : Decimal
            ;

instruction : User id coordinate
            | Satellite id coordinate
            | Interferer id coordinate
            ;

subroutine  : instruction* EOF
            ;
            
number      : Decimal
            | Float
            ;
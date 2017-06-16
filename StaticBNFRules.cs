//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	public class TestLALR1 : Translate
	{
		public TestLALR1()
		{
			m_ParseType = ParseType.LALR1;
			m_StartSign = "S";
			m_BNF =		"S->C,C."+
						"C->'c',C|'d'.";
		}
	}

	public class TestLR1_a : Translate
	{
		public TestLR1_a()
		{
			m_ParseType = ParseType.LR1;
			m_StartSign = "S";
			m_BNF =		"S->E,'=',E | 'id'."+
						"E->E,'+','id'|'id'.";
		}
	}

	public class TestLR1_b : Translate
	{
		public TestLR1_b()
		{
			m_ParseType = ParseType.LR1;
			m_StartSign = "S";
			m_BNF =		"S->C,C."+
						"C->'c',C|'d'.";
		}
	}

	public class TestLR1 : Translate
	{
		public TestLR1()
		{
			m_ParseType = ParseType.LR1;
			m_StartSign = "E";
			m_BNF =	"E->E,'+',T|T."+
				"T->T,'*',F|F."+
				"F->'(',E,')'|'id'.";
		}
	}

	public class LR1_FirstTry : Translate
	{
		public LR1_FirstTry()
		{
			m_ParseType = ParseType.LR1;
			m_StartSign = "program";
			m_BNF =	"program->'begin',statement_sequence,'end'."+
					"statement_sequence->satement."+
					"";
			}
	}

//	1. <system goal> ? <program> EOF
//	2. <program> ? begin <statement sequence> end .
//	3. <statement sequence> ? <statement> {<statement>}
//	4. <statement> ? ID := <expression> ;
//	5. <statement> ? readln ( <identifier list> ) ;
//	6. <statement> ? writeln ( <expression list> ) ;
//	7. <identifier list> ? ID {, ID}
//	8. <expression list> ? <expression> {, <expression>}
//	9. <expression> ? <factor> <addition operator> <factor>
//	10. <factor> ? ( <expression> )
//	11. <factor> ? ID
//	12. <factor> ? INTLITERAL
//	13. <addition operator> ? PLUS
//	14. <addition operator> ? MINUS


	public class TestSLR1 : Translate
	{
		public TestSLR1()
		{
			m_ParseType = ParseType.SLR1;
			m_StartSign = "E";
			m_BNF =	"E->E,'+',T|T."+
					"T->T,'*',F|F."+
					"F->'(',E,')'|'id'.";
		}
	}

	public class TestLL1 : Translate
	{
		public TestLL1()
		{
			m_ParseType = ParseType.LL1;
			m_StartSign = "E";
			m_BNF =	"E->T,E_."+
					"E_->'+',T,E_|."+
					"T->F,T_."+
					"T_->'*',F,T_|."+
					"F->'(',E,')'|'id'.";
		}
	}

	public class LanguageLL1 : Translate
	{
		public LanguageLL1()
		{
			m_ParseType = ParseType.LL1;
			m_StartSign = "stmt";
			m_BNF =	"stmt->assignment | cond | loop ."+
					"assignment->'id',':=', expr ."+
					"cond->'if',boolexpr,'then',stmt,cond_rest."+
					"cond_rest->'fi' | 'else',stmt,'fi'."+
					"loop->'while', boolexpr, 'do', stmt, 'od'."+
					"expr-> numexpr, bool_rest."+
					"bool_rest->'cop', numexpr | ."+
					"boolexpr->numexpr,'cop',numexpr."+
					"numexpr->term, numexpr_ ."+
					"numexpr_->'+',term,numexpr_ |."+
					"term-> factor, term_ ."+
					"term_-> '*', factor, term_ | . "+
					"factor->'id' | const | '(',numexpr+')'.";
		}
	}

	public class TranslateTestLL1 : Translate
	{
		public TranslateTestLL1()
		{
			m_StartSign = "program";
			m_ParseType = ParseType.LL1;
			m_BNF =	"program->block."+
					"block->variable,spaces,block|function,spaces,block|."+

					"variable->'VAR',any,spaces,name,spaces,VarAdd,'.'."+					
					"VarAdd->'[',spaces,integer,spaces,']'|'=',spaces,integer|."+
				
					"function->'FUNC',any,spaces,name,spaces,'(',spaces,func_dec,')',spaces,'{',func_body,'}'."+
					"func_dec->name,spaces,func_dec2|."+
					"func_dec2->',',spaces,name,spaces,func_dec2|."+

					"func_body->spaces."+

					"spaces->any,spaces|."+
					"alphanum->letter|digit."+
					
					"integer->digit,integer_1."+
					"integer_1->digit,integer_1|."+
					
					"name->letter,name_1."+
					"name_1->alphanum,name_1|."+

					"letter->'A'|'B'|$C|$D|$E|$F|$G|$H|$I|$J|$K|$L|$M|$N|$O|$P|$Q|$R|$S|$T|$U|$V|$W|$X|$Y|$Z|$a|$b|$c|$d|$e|$f|$g|$h|$i|$j|$k|$l|$m|$n|$o|$p|$q|$r|$s|$t|$u|$v|$w|$x|$y|$z."+
					"digit->$0|$1|$2|$3|$4|$5|$6|$7|$8|$9."+
					"any->' '|'\t'|'\n'|'\r'."+
	
					"";
		}


		protected override void ExecuteMethod(string MethodName,bool Terminal)
		{
			if(Terminal)
			{
			}
		}
	}

}

//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for Translate.
	/// </summary>
	public class Translate
	{
		public enum ParseType
		{
			LL1,
			SLR1,
			LR1,
			LALR1
		}

		ASTElement m_astElm = null;
		public string m_StartSign;
		public string m_BNF = "";
		public string m_Filename = "";
		protected ParseType m_ParseType;
		public ParseType ParsingType
		{
			get{return m_ParseType;}
		}

		public Translate()
		{
		}

		public void Convert(ASTElement astElm)
		{
			m_astElm = astElm;
			TraverseTree(m_astElm);
		}

		private void TraverseTree(ASTElement Node)
		{
			if(Node!=null)
			{
				RuleElement re = Node.rlElement;
				string Token = re.GetToken();
				bool Terminal = re.IsTerminal();
				ExecuteMethod(Token,Terminal);

				ASTElement tmpAst = null;
				int NodeNr = 0;
				do
				{
					tmpAst = Node.GetNextNode(NodeNr);
					TraverseTree(tmpAst);
					NodeNr++;
				}
				while(tmpAst!=null);
			}
		}

		protected virtual void ExecuteMethod(string MethodName,bool Terminal)
		{
		}
	}
}

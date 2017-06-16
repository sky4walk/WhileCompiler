// André Betz
// http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for ButtomUpParser.
	/// </summary>
	public class ButtomUpParser
	{
		private ButtomUpParseTabelle m_pt = null;
		private ButtomUpParseStack m_ps = null;
		private string m_EofSign = "";
		private string m_ParseVarlauf = "";
		private SLR1ParseTable m_ParseTable = null;
		private MyArrayList m_ParseVerlaufArray = null;

		public ButtomUpParser(SLR1ParseTable ParseTable)
		{
			m_ParseTable = ParseTable;
			m_pt = ParseTable.SLR1Table;
			m_EofSign = ParseTable.EofTerminal;
		}

		public string ParseVerlauf
		{
			get{return m_ParseVarlauf;}
		}

		public MyArrayList ParseVerlaufListe
		{
			get{return m_ParseVerlaufArray;}
		}

		public bool Init()
		{
			if(m_pt!=null)
			{
				m_ParseVarlauf = "";
				m_ps = new ButtomUpParseStack();
				m_ParseVerlaufArray = new MyArrayList();
				return true;
			}
			return false;
		}

		public ASTElement Parse(string Input)
		{
			m_ParseVarlauf = "";

			Input += m_EofSign;
			TerminalMatcher tm = new TerminalMatcher(Input,m_pt.Terminals);

			RuleTerminal ReadTerminal = tm.GetNextTerminal();	
			bool bAccept = false;

			while(!bAccept && ReadTerminal!=null && !m_ps.IsEmpty && !ReadTerminal.Equals(m_EofSign))
			{
				string tmpVerlauf = ReadTerminal.GetToken() + " ; "+ m_ps.StackValues ;
				m_ParseVerlaufArray.Add(m_ps.StackArray);

				ButtomUpParseStack.buStackElement stkElm = m_ps.Look();

				if(stkElm!=null)
				{
					if(stkElm.IsRuleElement)
					{
						tmpVerlauf += "err";
					}
					else
					{
						int State = stkElm.GetState;
						ButtomUpParseTabelle.ActionEntry ae = m_pt.Get(ReadTerminal,State);
						if(ae!=null)
						{
							if(ae.GetAction==ButtomUpParseTabelle.Actions.SHIFT)
							{
								tmpVerlauf += "; s "+ae.NextState.ToString();
								m_ps.PushShift(ReadTerminal,ae.NextState);
								ReadTerminal = tm.GetNextTerminal();
							}
							else if(ae.GetAction==ButtomUpParseTabelle.Actions.REDUCE)
							{
								int sm = ae.NextState;
								tmpVerlauf += "; r "+ae.NextState.ToString();

								if(sm<m_ParseTable.AllRules.Count)
								{
									BNFRule rl = (BNFRule)m_ParseTable.AllRules[sm];
									int RlLen = rl.RuleLength();
									for(int i=0;i<RlLen;i++)
									{
										m_ps.Pop2();
									}
									ButtomUpParseStack.buStackElement stkLook = m_ps.Look();
									if(stkLook!=null)
									{
										int nxtState = m_ParseTable.GetGotoState(rl.GetStart(),stkLook.GetState);
										m_ps.PushReduce(rl.GetStart(),nxtState);
										tmpVerlauf += "; " + rl.GetRuleString();
									}
								}
							}
							else if(ae.GetAction==ButtomUpParseTabelle.Actions.JUMP)
							{
								tmpVerlauf += "; err";
							}
							else if(ae.GetAction==ButtomUpParseTabelle.Actions.ACCEPT)
							{
								tmpVerlauf += "; acc";
								bAccept = true;
							}
						}
					}
				}
				m_ParseVarlauf += tmpVerlauf+"\n";
			}

			return m_ps.ASTRoot;
		}

		public static string ParseVerlaufArray2String(MyArrayList parseList)
		{
			string parseverlaufStr = "";
			if(parseList!=null)
			{
				for(int i=0;i<parseList.Count;i++)
				{
					MyArrayList actStack = (MyArrayList)parseList[i];
					if(actStack!=null)
					{
						for(int j=0;j<actStack.Count;j++)
						{
							ButtomUpParseStack.buStackElement stkElm = (ButtomUpParseStack.buStackElement) actStack[j];
							if(stkElm.IsRuleElement)
							{
								RuleElement rl = stkElm.GetRule;
								if(rl.IsTerminal())
								{
									parseverlaufStr += "'"+rl.GetToken()+"'";
								}
								else
								{
									parseverlaufStr += rl.GetToken();
								}
								parseverlaufStr+=" ";
							}
							else
							{
								// parseverlaufStr += stkElm.GetState.ToString();
								// parseverlaufStr+=" ";
							}
							
						}
						parseverlaufStr+="\n";
					}
				}
			}
			return parseverlaufStr;
		}
	}
}

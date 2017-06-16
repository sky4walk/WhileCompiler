//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for TopDownParser.
	/// </summary>
	public class TopDownParser
	{
		private TopDownParseTabelle m_pt = null;
		private TopDownParseStack m_ps = null;
		private RuleStart m_rs = null;
		private string m_ParseVarlauf = "";
		private string m_EofSign = "";
		private MyArrayList m_ParseVerlaufArray = null;

		public string ParseVerlauf
		{
			get{return m_ParseVarlauf;}
		}

		public MyArrayList ParseVerlaufListe
		{
			get{return m_ParseVerlaufArray;}
		}

		public TopDownParser(TopDownParseTabelle pt,RuleStart rs, string EOFSign)
		{
			m_pt = pt;
			m_rs = rs;
			m_ParseVerlaufArray = new MyArrayList();
			m_EofSign = EOFSign;
		}

		public bool Init()
		{
			if(m_rs!=null&&m_pt!=null)
			{
				m_ParseVarlauf = "";
				m_ps = new TopDownParseStack(m_rs);
				return true;
			}
			return false;
		}

		public ASTElement Parse(string Input)
		{
			Input += m_EofSign;
			TerminalMatcher tm = new TerminalMatcher(Input,m_pt.Terminals);

			RuleTerminal ReadTerminal = tm.GetNextTerminal();		

			while(ReadTerminal!=null&&!m_ps.IsEmpty&&!ReadTerminal.Equals(m_EofSign))
			{
				string tmpVerlauf = ReadTerminal.GetToken() + " : "+ m_ps.StackValues +"\n";
				m_ParseVerlaufArray.Add(m_ps.StackArray);
				m_ParseVarlauf += tmpVerlauf;

				RuleElement re = m_ps.Pop();
				if(re!=null)
				{
					if(re.IsTerminal())
					{
						if(ReadTerminal.GetToken().Equals(re.GetToken()))
						{
						}
						else
						{
							return null;
						}
						ReadTerminal = tm.GetNextTerminal();		
					}
					else if(re.GetToken().Length>0&&ReadTerminal!=null&&ReadTerminal.GetToken().Length>0)
					{
						RuleStart rs = m_pt.Get(ReadTerminal.GetToken(),re.GetToken());
						if(rs==null)
						{
							return null;
						}
						m_ps.Push(rs);
					}
				}
				else
				{
					break;
				}
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
							RuleElement rl = (RuleElement)actStack[j];
							if(rl.IsTerminal())
							{
								parseverlaufStr += "'"+rl.GetToken()+"'";
							}
							else
							{
								parseverlaufStr += rl.GetToken();
							}
							parseverlaufStr += " ";
						}
					}
					parseverlaufStr += "\n";
				}
			}
			return parseverlaufStr;
		}
	}
}

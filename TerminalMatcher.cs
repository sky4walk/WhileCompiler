//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for TerminalMatcher.
	/// </summary>
	public class TerminalMatcher
	{
		MyArrayList m_Terminals = null;
		char[] m_Input = null;
		int m_InpPos = 0;

		public TerminalMatcher(string Input,MyArrayList Terminals)
		{
			m_Terminals = Terminals;
			m_Input = Input.ToCharArray();
		}

		public RuleTerminal GetNextTerminal()
		{
			int WordLen = 0;
			string Buf = "";
			string retBuf = "";
			while(m_InpPos+WordLen<m_Input.Length)
			{
				Buf += m_Input[m_InpPos+WordLen];
				if(!IsTerminalPrefix(Buf))
				{
					break;
				}
				retBuf = Buf;
				WordLen++;
			}
			m_InpPos += WordLen;

			return FindTerminal(retBuf);
		}

		private RuleTerminal FindTerminal(string Buf)
		{
			for(int i=0;i<m_Terminals.Count;i++)
			{
				RuleTerminal rt = (RuleTerminal)m_Terminals[i];
				if(rt.GetToken().Equals(Buf))
				{
					return rt;
				}
			}
			return null;
		}
		private bool IsTerminalPrefix(string Buf)
		{
			for(int i=0;i<m_Terminals.Count;i++)
			{
				RuleTerminal rt = (RuleTerminal)m_Terminals[i];
				if(IsPrefix(Buf,rt.GetToken()))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsPrefix(string Buf,string Terminal)
		{
			if(Terminal.Length>=Buf.Length)
			{
				for(int i=0;i<Buf.Length;i++)
				{
					if((Buf.ToCharArray()[i])!=(Terminal.ToCharArray()[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}

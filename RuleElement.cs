//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for RuleElement.
	/// </summary>

	public class RuleElement
	{
		protected RuleElement m_Next;
		protected RuleStart m_Begin;
		protected string m_Token;

		public RuleElement(string Token,RuleStart re)
		{
			m_Token = Token;
			m_Begin = re;
			m_Next = null;
		}
		public void SetBegin(RuleStart re)
		{
			m_Begin = re;
		}
		public RuleStart GetBegin()
		{
			return m_Begin;
		}
		public void SetNext(RuleElement re)
		{
			m_Next = re;
		}
		public RuleElement GetNext()
		{
			return m_Next;
		}
		public string GetToken()
		{
			return m_Token;
		}
		public void SetToken(string Token)
		{
			m_Token=Token;
		}
		public virtual bool IsTerminal()
		{
			return false;
		}
	}
}

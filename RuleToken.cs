//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for RuleToken.
	/// </summary>
	public class RuleToken : RuleElement
	{
		protected RuleStart m_Connected = null;
	
		public RuleToken(string Token,RuleStart re):base(Token,re)
		{
			m_Connected = null;
		}
	
		public void SetConnected(RuleStart rs)
		{
			m_Connected = rs;
		}
		public RuleStart GetConnected()
		{
			return m_Connected;
		}
	}
	/// <summary>
	/// Terminal Symbol
	/// </summary>
	public class RuleTerminal : RuleElement
	{
		public RuleTerminal(string Token,RuleStart re):base(Token,re)
		{
		}
		public override bool IsTerminal()
		{
			return true;
		}
	}
	/// <summary>
	/// Terminal Symbol
	/// </summary>
	public class RuleEpsilon : RuleElement
	{
		public RuleEpsilon(RuleStart re):base("",re)
		{
		}
	}
}

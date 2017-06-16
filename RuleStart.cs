//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for RuleStart.
	/// </summary>
	public class RuleStart : RuleToken
	{
		private RuleStart AlternateRule;
		private RuleStart AlternateRuleBack;
		private BNFRule RuleConnect;
	
		public RuleStart(String Token,RuleStart re,BNFRule back):base(Token,re)
		{
			AlternateRule = null;
			AlternateRuleBack = null;
			RuleConnect = back;
		}
		public RuleStart GetAlternate()
		{
			return AlternateRule;
		}
		public void SetAlternate(RuleStart alternate)
		{
			AlternateRule = alternate;
			if(alternate!=null)
			{
				alternate.SetAlternateBack(this);
			}
		}
		public RuleStart GetAlternateBack()
		{
			return AlternateRuleBack;
		}
		public void SetAlternateBack(RuleStart alternate)
		{
			AlternateRuleBack = alternate;
		}
		public BNFRule GetRuleConnect()
		{
			return RuleConnect;
		}
		public MyArrayList GetAllTokens()
		{
			MyArrayList tokens = new MyArrayList();
			RuleElement re = m_Next;
			while(re!=null)
			{
				if(!re.IsTerminal()&&!re.GetToken().Equals(""))
				{
					tokens.Add(re);
				}
				re = re.GetNext();
			}
			return tokens;
		}
		public string GetAllElements()
		{
			RuleElement re = m_Next;
			string Elements = "";
			while(re!=null)
			{
				if(re.GetToken().Length>0)
				{
					if(re.IsTerminal())
					{
						Elements += "\'"+re.GetToken()+"\',";
					}
					else
					{
						Elements += re.GetToken()+",";
					}
				}
				else
				{
					Elements += " ,";
				}
				re = re.GetNext();
			}
			return Elements;
		}
	}
}

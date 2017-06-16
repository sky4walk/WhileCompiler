//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	class BNFReader
	{
		private MyArrayList m_Rules = new MyArrayList();	
		string m_BNF = null;

		public BNFReader(string BNF)
		{
			m_BNF = BNF;
		}
		
		public MyArrayList Rules
		{
			get{return m_Rules;}
		}
		
		public bool Init()
		{
			m_Rules.Clear();
			if(!ParseBNF(m_BNF))
			{
				return false;
			}
			return true;
		}
		
		private bool ParseBNF(string BNF)
		{
			bool bTerminal = false;
			bool bDelNoSigns = true;
			int automstate = 0;
			int len = BNF.Length;
			int spos = 0;
			char sign;
			int failureState = 100;
			char[] DelSigns = new char[]{' ','\n','\r','\t'};
			BNFRule actRule = null;
			while(spos<len)
			{
				if(bDelNoSigns)
					spos = ParseHelper.DelNoSigns(BNF,spos,DelSigns);
				if(spos<len) 
				{
					sign = BNF.ToCharArray()[spos];
					switch(automstate)
					{
						case 0:
						{
							actRule = null;
							if (sign=='#')
							{
								spos = ParseHelper.DelComment(BNF,spos);
							}
							else 
							{
								automstate = 1;
							}
							break;
						}
						case 1:
						{
							char[] EndSigns = new char[]{'-'};
							ScannerErg scnErg = ParseHelper.GetWord(BNF,spos,EndSigns,DelSigns);
							if(scnErg.getTermSignNr()==0)
							{
								actRule = new BNFRule(m_Rules.Count);
								actRule.SetStart(scnErg.getWord());
								m_Rules.Add(actRule);
								automstate = 2;	
							}
							else
							{
								automstate = failureState;
							}
							spos = scnErg.getSpos();
							break;
						}
						case 2:
						{
							  if(spos<len && BNF.ToCharArray()[spos]=='>')
							  {
								  automstate = 3;
								  spos++;
							  }
							  else
							  {
								  automstate = failureState;
							  }
							  break;
						  }
						case 3:
						{
							char[] EndSigns = new char[]{',','\'','|','.','$'};
							ScannerErg scnErg = ParseHelper.GetWord(BNF,spos,EndSigns,DelSigns);
							if(scnErg.getTermSignNr()==0)
							{
								actRule.SetNextToken(scnErg.getWord(),bTerminal);
								automstate = 3;
								bTerminal = false;
							}
							else if(scnErg.getTermSignNr()==1)
							{
								bTerminal = true;
								bDelNoSigns = false;
								automstate = 4;								
							}
							else if(scnErg.getTermSignNr()==2)
							{
								if(scnErg.getWord().Length!=0)
								{
									actRule.SetNextToken(scnErg.getWord(),bTerminal);
								}
								else
								{
									actRule.SetNextToken(scnErg.getWord(),true);
								}
								string StartSign = actRule.GetStart().GetToken();
								actRule = new BNFRule(m_Rules.Count);
								actRule.SetStart(StartSign);
								m_Rules.Add(actRule);
								automstate = 3;
								bTerminal = false;								
							}
							else if(scnErg.getTermSignNr()==3)
							{
								if(scnErg.getWord().Length!=0)
								{
									actRule.SetNextToken(scnErg.getWord(),bTerminal);
								}
								else
								{
									actRule.SetNextToken(scnErg.getWord(),true);
								}
								automstate = 0;
								bTerminal = false;															
							}
							else if(scnErg.getTermSignNr()==4)
							{
								automstate = 3;
								bTerminal = true;															
							}
							else
							{
								automstate = failureState;
							}
							spos = scnErg.getSpos();
							break;
						}
						case 4:
						{
							char[] EndSigns = new char[]{'\''};
							ScannerErg scnErg = ParseHelper.GetWord(BNF,spos,EndSigns,null);
							if(scnErg.getTermSignNr()==0)
							{
								actRule.SetNextToken(scnErg.getWord(),bTerminal);
								automstate = 5;
								bTerminal = false;
							}
							else
							{
								automstate = failureState;
							}
							bDelNoSigns = true;
							spos = scnErg.getSpos();
							break;
						}
						case 5:
						{
							char[] EndSigns = new char[]{',','|','.'};
							ScannerErg scnErg = ParseHelper.GetWord(BNF,spos,EndSigns,DelSigns);
							if(scnErg.getTermSignNr()==0)
							{
								automstate = 3;
								bTerminal = false;
							}
							else if(scnErg.getTermSignNr()==1)
							{
								string StartSign = actRule.GetStart().GetToken();
								actRule = new BNFRule(m_Rules.Count);
								actRule.SetStart(StartSign);
								m_Rules.Add(actRule);
								automstate = 3;
								bTerminal = false;								
							}
							else if(scnErg.getTermSignNr()==2)
							{
								automstate = 0;
								bTerminal = false;															
							}						
							else
							{
								automstate = failureState;
							}
							spos = scnErg.getSpos();
							break;
						}
						default:
						{
						   return false;
						}
					}
				}
			}
			return true;
		}

		public RuleStart GetStartRule(string Token)
		{
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				RuleStart rs = rl.GetStart();
				if(Token.Equals(rs.GetToken()))
				{
					return rs;
				}
			}
			return null;
		}

		public string Print()
		{
			string res = "";
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				res += i+")\t"+rl.GetAllInfos()+"\n";
			}
			return res;
		}
	}
}
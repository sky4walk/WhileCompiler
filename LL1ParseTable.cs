//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for GenerateLL1ParseTable.
	/// </summary>
	public class LL1ParseTable
	{
		protected MyArrayList m_Rules = null;
		private TopDownParseTabelle m_LL1ParseTabelle = null;
		protected string m_EofTerminal = "";
		protected string m_StartSign;
		
		public LL1ParseTable(MyArrayList Rules, string StartSign)
		{
			m_StartSign = StartSign;
			m_Rules = Rules;
			m_EofTerminal = GetEofTerminal();
		}

		public MyArrayList AllRules
		{
			get{return m_Rules;}
		}

		public string EofTerminal
		{
			get{return m_EofTerminal;}
		}

		public string StartRule
		{
			get{ return m_StartSign;}
		}

		public virtual bool Init()
		{
			ConnectAlternateStartRules();
			ConnectTokens();
			CheckEpsilonInRules();
			GenerateAllFirstSets();
			GenerateAllFollowSets();
			if(!GenerateParseTable())
			{
				return false;
			}
			return true;
		}

		public TopDownParseTabelle LL1ParseTabelle
		{
			get{return m_LL1ParseTabelle;}
		}

		protected MyArrayList GetStartRulesOneToken(string Token)
		{
			MyArrayList StartRuleList = new MyArrayList();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rule = (BNFRule)m_Rules[i];
				if(Token.Equals(rule.GetStart().GetToken()))
				{
					StartRuleList.Add(rule.GetStart());
				}
			}
			return StartRuleList;
		}

		protected MyArrayList GetDifStartTokens()
		{
			MyArrayList TokenList = new MyArrayList();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rule = (BNFRule)m_Rules[i];
				RuleStart Strt = rule.GetStart();
				if(!ParseHelper.IsTokenInTokenList(TokenList,Strt))
				{
					TokenList.Add(Strt);
				}
			}
			return TokenList;
		}

		private void ConnectAlternateStartRules(MyArrayList StartRules)
		{
			if(StartRules.Count==1)
			{
				RuleStart sr1 = (RuleStart)StartRules[0];
				sr1.SetAlternate(null);
				sr1.SetAlternateBack(null);
			}
			for(int i=0;i<StartRules.Count-1;i++)
			{
				RuleStart sr1 = (RuleStart)StartRules[i];
				RuleStart sr2 = (RuleStart)StartRules[i+1];
				if(sr2!=null)
				{
					sr2.SetAlternate(null);
				}
				sr1.SetAlternateBack(null);
				sr1.SetAlternate(sr2);
			}
		}

		private RuleStart FindStartTokenList(MyArrayList StartTokens, RuleToken rlTkn)
		{
			RuleStart rlStrt = null;
			for(int i=0;i<StartTokens.Count;i++)
			{
				RuleStart rlStartTkn = (RuleStart)StartTokens[i];
				if(rlTkn.GetToken().Equals(rlStartTkn.GetToken()))
				{
					rlStrt = rlStartTkn;
					break;
				}
			}
			return rlStrt;
		}

		private void ConnectToken2Start(MyArrayList StartTokens, MyArrayList AllRuleTokens)
		{
			for(int i=0;i<AllRuleTokens.Count;i++)
			{
				RuleToken rlTkn = (RuleToken)AllRuleTokens[i];
				RuleStart rlStartTkn = FindStartTokenList(StartTokens,rlTkn);
				if(rlStartTkn!=null)
				{
					rlStartTkn = GoToFirstStartRule(rlStartTkn);
					rlTkn.SetConnected(rlStartTkn);
				}
			}
		}

		protected void ConnectAlternateStartRules()
		{
			MyArrayList StartTokens = GetDifStartTokens();
			for(int i=0;i<StartTokens.Count;i++)
			{
				RuleStart strt = (RuleStart)StartTokens[i];
				MyArrayList OneTokenRules = GetStartRulesOneToken(strt.GetToken());
				ConnectAlternateStartRules(OneTokenRules);
			}
		}
		protected void ConnectTokens()
		{
			MyArrayList StartTokens = GetDifStartTokens();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rule = (BNFRule)m_Rules[i];
				RuleStart strt = rule.GetStart();
				MyArrayList AllTkns = strt.GetAllTokens();
				ConnectToken2Start(StartTokens,AllTkns);
			}
		}

		private bool IsRuleLeftRekursiv(RuleStart rs)
		{
			RuleElement re = rs.GetNext();
			if(re!=null)
			{
				string StartToken = rs.GetToken();
				string FirstToken = re.GetToken();
				if(StartToken.Equals(FirstToken))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsLeftRecursionInStartToken(RuleStart rs)
		{
			while(rs!=null)
			{
				if(IsRuleLeftRekursiv(rs))
				{
					return true;
				}
				rs = rs.GetAlternate();
			}
			return false;
		}

		protected RuleStart GoToFirstStartRule(RuleStart rs)
		{
			if(rs!=null)
			{
				while(rs.GetAlternateBack()!=null)
				{
					rs = rs.GetAlternateBack();
				}
			}
			return rs;
		}

		private bool IsTokenInStartRule(string Token)
		{
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule bnfRl = (BNFRule)m_Rules[i];
				if(Token.Equals(bnfRl.GetStart().GetToken()))
				{
					return true;
				}
			}
			return false;
		}

		protected string FindUnusedToken(string oldToken)
		{
			string newToken = oldToken;
			if(newToken!=null)
			{
				newToken += "_";
				while(IsTokenInStartRule(newToken))
				{
					newToken += "_";
				}
			}
			return newToken;
		}

		protected string GetEofTerminal()
		{
			string EOFTerminal = "$";
			MyArrayList Terminals = GetAllTerminals();

			string newToken = EOFTerminal;
			if(newToken!=null)
			{
				while(IsTokenInTokenList(Terminals,newToken))
				{
					newToken += "_";
				}
			}
			return newToken;
		}

		private bool IsTokenInTokenList(MyArrayList RuleElementList,string Token)
		{
			if(RuleElementList!=null && Token!=null)
			{
				for(int i=0;i<RuleElementList.Count;i++)
				{
					RuleElement re = (RuleElement)RuleElementList[i];
					if(re!=null && re.GetToken().Equals(Token))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void EleminateDirectLeftRecursion(RuleStart strt)
		{
			string ElemStartTkn = "";
			if(strt!=null)
			{
				ElemStartTkn = FindUnusedToken(strt.GetToken());
			}
			while(strt!=null)
			{
				if(IsRuleLeftRekursiv(strt))
				{
					RuleElement re = strt.GetNext();
					if(re!=null)
					{
						strt.SetNext(re.GetNext());
						re = GotoLastRule(strt);
						re.SetNext(new RuleToken(ElemStartTkn,strt));
						strt.SetToken(ElemStartTkn);
					}
					BNFRule bnfRl = new BNFRule(m_Rules.Count);
					bnfRl.SetStart(ElemStartTkn);
					bnfRl.GetStart().SetNext(new RuleEpsilon(bnfRl.GetStart()));
					m_Rules.Add(bnfRl);
				}
				else
				{
					RuleElement re = strt.GetNext();
					if(re!=null)
					{
						re = GotoLastRule(strt);
						re.SetNext(new RuleToken(ElemStartTkn,strt));
					}				
				}
				strt = strt.GetAlternate();
			}
		}

		private RuleElement GotoLastRule(RuleElement re)
		{
			while(re.GetNext()!=null)
			{
				re = re.GetNext();
			}
			return re;
		}

		private void DeleteStartRule(RuleStart rs)
		{
			BNFRule bnfRl = rs.GetRuleConnect();
			RuleStart AltNext = rs.GetAlternate();
			RuleStart AltBack = rs.GetAlternateBack();
			if(AltBack!=null)
			{
				AltBack.SetAlternate(AltNext);
			}
			if(AltNext!=null)
			{
				AltNext.SetAlternateBack(AltBack);
			}
			int index = bnfRl.Index;
			m_Rules.RemoveAt(index);
		}

		private void EleminateAllLeftRecursion()
		{
			MyArrayList StartTokens = GetDifStartTokens();
			for(int i=0;i<StartTokens.Count;i++)
			{
				RuleStart strt = (RuleStart)StartTokens[i];
				strt = GoToFirstStartRule(strt);
				if(IsLeftRecursionInStartToken(strt))
				{
					EleminateDirectLeftRecursion(strt);
				}
			}
			ConnectAlternateStartRules();
		}

		protected virtual bool RuleIsEpsilonFree(RuleStart rs)
		{
			RuleElement re = null;
			if(rs!=null)
				re = rs.GetNext();
			else
				return false;
			while(re!=null)
			{
				if(re.IsTerminal())
				{
					rs.GetRuleConnect().EpsilonFree = true;
					return true;
				}
				else
				{
					if(re.GetToken().Equals(""))
					{
						rs.GetRuleConnect().EpsilonFree = false;
						return false;
					}
					else
					{
						RuleToken rt = (RuleToken)re;
						bool res = RuleIsEpsilonFree(rt.GetConnected());
						if(res)
						{
							rs.GetRuleConnect().EpsilonFree = res;
							return res;
						}
						else
						{
							re = re.GetNext();
						}
					}
				}
				
			}
			rs.GetRuleConnect().EpsilonFree = false;
			return false;
		}

//		protected virtual void GenerateAllFirstSets()
//		{
//			for(int i=0;i<m_Rules.Count;i++)
//			{
//				BNFRule bnfRl = (BNFRule)m_Rules[i];
//				RuleStart strt = bnfRl.GetStart();
//				strt.GetRuleConnect().FirstSet = GenerateFirstSet(strt);
//			}
//		}

//		protected virtual void CheckEpsilonInRules()
//		{
//			for(int i=0;i<m_Rules.Count;i++)
//			{
//				BNFRule bnfRl = (BNFRule)m_Rules[i];
//				RuleStart strt = bnfRl.GetStart();
//				RuleIsEpsilonFree(strt);
//			}
//		}

		protected bool TotalRuleEpsFree(RuleStart rs)
		{
			rs = GoToFirstStartRule(rs);
			while(rs!=null)
			{
				if(!rs.GetRuleConnect().EpsilonFree)
					return false;
				rs = rs.GetAlternate();
			}
			return true;
		}

		private MyArrayList TotalRuleFirstSet(RuleStart rs)
		{
			MyArrayList res = new MyArrayList();
			rs = GoToFirstStartRule(rs);
			while(rs!=null)
			{
				res = ParseHelper.UnionSet(res,rs.GetRuleConnect().FirstSet);
				rs = rs.GetAlternate();
			}
			return res;
		}

		private MyArrayList TotalRuleFollowSet(RuleStart rs)
		{
			MyArrayList res = new MyArrayList();
			rs = GoToFirstStartRule(rs);
			while(rs!=null)
			{
				res = ParseHelper.UnionSet(res,rs.GetRuleConnect().FollowSet);
				rs = rs.GetAlternate();
			}
			return res;
		}

		protected virtual MyArrayList GenerateFirstSet(RuleStart rs)
		{
			MyArrayList res = new MyArrayList();
			RuleElement re = null;
			if(rs!=null)
				re = rs.GetNext();
			else
				return null;

			while(re!=null)
			{
				if(re.GetToken().Equals(""))
				{
					re = re.GetNext();
				}
				else
				{
					if(re.IsTerminal())
					{
						res.Add(re);
						return res;
					}
					else
					{
						RuleToken rt = (RuleToken)re;
						RuleStart conStr = GoToFirstStartRule(rt.GetConnected());
						MyArrayList res2 = null;
						while(conStr!=null)
						{
							res2 = GenerateFirstSet(conStr);
							res = ParseHelper.UnionSet(res,res2);
							conStr = conStr.GetAlternate();
						}
						if(TotalRuleEpsFree(rt.GetConnected()))
						{
							return res;
						}
						else
						{
							re = re.GetNext();
						}						
					}
				}
			}
			return res;
		}

		protected void CheckEpsilonInRules()
		{
			bool bChanged = true;
			while(bChanged)
			{
				bChanged =false;
				for(int i=0;i<m_Rules.Count;i++)
				{
					bool EpsFree = false;
					BNFRule bnfRl = (BNFRule)m_Rules[i];
					RuleStart strt = bnfRl.GetStart();
					if(strt!=null)
					{
						RuleElement re = strt.GetNext();
						EpsFree = BetaTotalRuleEpsFree(re);
					}
					if(strt.GetRuleConnect().EpsilonFree!=EpsFree)
						bChanged = true;
					strt.GetRuleConnect().EpsilonFree=EpsFree;
				}
			}
		}

		protected void GenerateAllFirstSets()
		{
			bool bChanged = true;
			while(bChanged)
			{
				bChanged = false;
				for(int i=0;i<m_Rules.Count;i++)
				{
					MyArrayList FirstSet = new MyArrayList();
					BNFRule bnfRl = (BNFRule)m_Rules[i];
					RuleStart strt = bnfRl.GetStart();
					if(strt!=null)
					{
						RuleElement re = strt.GetNext();
						FirstSet = BetaTotalRuleFirstSet(re);
					}
					int FrstSize = 0;
					if(strt.GetRuleConnect().FirstSet!=null)
					{
						FrstSize = strt.GetRuleConnect().FirstSet.Count;
					}
					strt.GetRuleConnect().FirstSet = ParseHelper.UnionSet(strt.GetRuleConnect().FirstSet,FirstSet);
					FrstSize = strt.GetRuleConnect().FirstSet.Count-FrstSize;
					if(FrstSize!=0)
						bChanged = true;
				}
			}
		}

		private MyArrayList GetNonTermsFrom(string NonTerm)
		{
			MyArrayList AllNonTerms = null;
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rule = (BNFRule)m_Rules[i];
				MyArrayList NonTrms = rule.FindToken(NonTerm);
				AllNonTerms = ParseHelper.AddList(AllNonTerms,NonTrms);
			}
			return AllNonTerms;
		}

		private MyArrayList GenerateFollowList(MyArrayList NonTerms)
		{
			MyArrayList Follows = new MyArrayList();
			for(int i=0;i<NonTerms.Count;i++)
			{
				RuleElement re = (RuleElement)NonTerms[i];
				RuleStart rs = re.GetBegin();
				BNFRule rl = rs.GetRuleConnect();
				RuleElement nextRe = re.GetNext();
				if(nextRe==null)
				{
					Follows = ParseHelper.UnionSet(Follows,rl.FollowSet);
				}
				else
				{
					if(nextRe.IsTerminal())
					{
						Follows.Add(nextRe);
					}
					else
					{
						RuleToken rt = (RuleToken)nextRe;
						MyArrayList AllFirstSet = BetaTotalRuleFirstSet(rt);
						Follows = ParseHelper.UnionSet(Follows,AllFirstSet);
						if(!BetaTotalRuleEpsFree(rt))
						{
							Follows = ParseHelper.UnionSet(Follows,rl.FollowSet);
						}
					}
				}
			}
			return Follows;
		}

		protected MyArrayList BetaTotalRuleFirstSet(RuleElement re)
		{
			MyArrayList BetaFirst = new MyArrayList();
			while(re!=null)
			{
				if(re.IsTerminal())
				{
					BetaFirst.Add(re);
					return BetaFirst;
				}
				else
				{
					if(re.GetToken().Length>0)
					{
						RuleToken rt = (RuleToken)re;
						if(rt!=null)
						{
							RuleStart conStr = GoToFirstStartRule(rt.GetConnected());
							MyArrayList FirstSet = TotalRuleFirstSet(conStr);
							BetaFirst = ParseHelper.UnionSet(BetaFirst,FirstSet);
							if(TotalRuleEpsFree(rt.GetConnected()))
							{
								return BetaFirst;
							}
						}
					}
				}
				re = re.GetNext();
			}
			return BetaFirst;
		}

		protected bool BetaTotalRuleEpsFree(RuleElement re)
		{
			while(re!=null)
			{
				if(re.IsTerminal())
				{
					return true;
				}
				else
				{
					if(re.GetToken().Length>0)
					{
						
						RuleToken rt = (RuleToken)re;
						if(TotalRuleEpsFree(rt.GetConnected()))
						{
							return true;
						}
					}
					else
					{
						return false;
					}
				}
				re = re.GetNext();
			}
			return false;
		}

		private bool SetAllFollows(RuleStart rs,MyArrayList Follows)
		{
			bool changed = false;
			rs = GoToFirstStartRule(rs);
			while(rs!=null)
			{
				int FlwSize = 0;
				if(rs.GetRuleConnect().FollowSet!=null)
				{
					FlwSize = rs.GetRuleConnect().FollowSet.Count;
				}
				rs.GetRuleConnect().FollowSet = ParseHelper.UnionSet(rs.GetRuleConnect().FollowSet,Follows);
				FlwSize = rs.GetRuleConnect().FollowSet.Count-FlwSize;
				if(FlwSize!=0)
					changed = true;
				rs = rs.GetAlternate();
			}
			return changed;
		}

		protected void GenerateAllFollowSets()
		{
			MyArrayList StartTokens = GetDifStartTokens();
			bool changed = false;
			do
			{
				changed = false;
				for(int i=0;i<StartTokens.Count;i++)
				{
					RuleStart rs = (RuleStart)StartTokens[i];
					MyArrayList AllNonTerms = GetNonTermsFrom(rs.GetToken());
					MyArrayList Follows = GenerateFollowList(AllNonTerms);	
					if(rs.GetToken().Equals(m_StartSign))
					{
						Follows.Add(new RuleTerminal(m_EofTerminal,null));
					}
					if(SetAllFollows(rs,Follows))
						changed = true;
				}
			}while(changed);
		}

		private MyArrayList GetAllFirst()
		{
			MyArrayList Terminals = new MyArrayList();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				Terminals = ParseHelper.UnionSet(Terminals,rl.FirstSet);
			}
			return Terminals;
		}

		private MyArrayList GetAllFollows()
		{
			MyArrayList Terminals = new MyArrayList();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				Terminals = ParseHelper.UnionSet(Terminals,rl.FollowSet);
			}
			return Terminals;
		}

		protected MyArrayList GetAllTerminals()
		{
			MyArrayList Terminals = new MyArrayList();
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				Terminals = ParseHelper.UnionSet(Terminals,rl.GetAllTerminals());
			}
			return Terminals;
		}

		private string[] Rule2String(RuleStart rs)
		{
			MyArrayList elements = new MyArrayList();
			if(rs!=null)
			{
				RuleElement re = rs.GetNext();
				while(re!=null)
				{
					elements.Add(re.GetToken());
					re = re.GetNext();
				}
			}
			return (string[])elements.ToArray(typeof(string));
		}

		protected virtual bool  GenerateParseTable()
		{
			MyArrayList Terminals = GetAllTerminals();
			Terminals.Add(new RuleTerminal(m_EofTerminal,null));
			MyArrayList StartTokens = GetDifStartTokens();

			m_LL1ParseTabelle = new TopDownParseTabelle(Terminals,StartTokens);

			bool dblEntry = false;
			for(int i=0;i<m_Rules.Count;i++)
			{
				BNFRule rl = (BNFRule)m_Rules[i];
				RuleStart rs = rl.GetStart();
				int len = rs.GetRuleConnect().FirstSet.Count;
				for(int j=0;j<len;j++)
				{
					RuleElement re = (RuleElement)rs.GetRuleConnect().FirstSet[j];
					string colSign = re.GetToken();
					string rowSign = rs.GetToken();
					if(!m_LL1ParseTabelle.Add(colSign,rowSign,rs))
						dblEntry = true;
				}

				if(!rl.EpsilonFree)
				{
					len = rs.GetRuleConnect().FollowSet.Count;
					for(int j=0;j<len;j++)
					{
						RuleElement re = (RuleElement)rs.GetRuleConnect().FollowSet[j];
						string colSign = re.GetToken();
						string rowSign = rs.GetToken();

						if(!m_LL1ParseTabelle.Add(colSign,rowSign,rs))
							dblEntry = true;
					}
				}
			}
			return !dblEntry;
		}
	}
}

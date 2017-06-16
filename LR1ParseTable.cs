// written by André Betz
// http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for LR1Parsetable.
	/// </summary>
	public class LR1ParseTable : SLR1ParseTable
	{
		private int m_LH = 1;

		#region LR1Element klasse
		protected class LR1Element : LRElement
		{
			internal LR1Element(int RlPos,int HlNr,int Nr) : base(RlPos,HlNr,Nr)
			{
			}
			internal LR1Element(LR1Element LreElm,int HlNr,int Nr) : base(LreElm,HlNr,Nr)
			{
				m_FirstSet = LreElm.FirstSet;
			}
			internal LR1Element(LR1Element LreElm) : base(LreElm)
			{
				m_FirstSet = LreElm.FirstSet;
			}
			private MyArrayList m_FirstSet = new MyArrayList();
			internal MyArrayList FirstSet
			{
				get{return m_FirstSet;}
				set{m_FirstSet = value;}
			}
		}
		#endregion

		public LR1ParseTable(MyArrayList Rules,string StartSign):base(Rules,StartSign)
		{
		}

		public ButtomUpParseTabelle LR1Table
		{
			get{return m_bupt;}
		}

		protected override void GenerateClosure()
		{
			MyArrayList FirstSet = new MyArrayList();
			FirstSet.Add(new RuleTerminal(m_EofTerminal,null));
			MyArrayList Huelle = new MyArrayList();

			if(FirstClosureFromRule(Huelle,m_StartSign,FirstSet))
			{
				Closure(Huelle);
				MyArrayList actHuellen = new MyArrayList();
				actHuellen.Add(Huelle);
				m_HuellenCounter++;
				while(!AreHuellenEmpty(actHuellen))
				{
					m_Huellen.AddArray(actHuellen);
					MyArrayList nextHuellen = CreateGotoFromLastHuellen(actHuellen);
					actHuellen = nextHuellen;
				}
			}
		}

		private bool FirstClosureFromRule(MyArrayList Huelle, string Token, MyArrayList FirstSet)
		{
			bool bChanged = false;
			MyArrayList TkRules = base.GetStartRulesOneToken(Token);
			if(TkRules!=null&&Huelle!=null)
			{
				for(int i=0;i<TkRules.Count;i++)
				{
					RuleStart rs = (RuleStart)TkRules[i];
					int RuleNr = rs.GetRuleConnect().Index;
					int HuelPos = RuleInsideHuelle(Huelle,RuleNr,1);
					if(HuelPos<0)
					{
						LR1Element LrElm = new LR1Element(rs.GetRuleConnect().Index,m_HuellenCounter,m_LRMECnter++);
						LrElm.FirstSet = FirstSet;
						Huelle.Add(LrElm);							
						bChanged = true;
					}
					else
					{
						LR1Element LrElm = (LR1Element)Huelle[HuelPos];
						int FrstSize = LrElm.FirstSet.Count;
						LrElm.FirstSet = ParseHelper.UnionSet(LrElm.FirstSet,FirstSet);
						if(FrstSize!=LrElm.FirstSet.Count)
						{
							bChanged = true;
						}
					}
				}
			}
			return bChanged;
		}

		protected override void Closure(MyArrayList Huelle)
		{
			bool bChanged = true;
			if(Huelle!=null)
			{
				while(bChanged)
				{
					bChanged = false;
					for(int i=0;i<Huelle.Count;i++)
					{
						LR1Element LrElm = (LR1Element)Huelle[i];
						RuleElement re = GetRuleElement(LrElm);
						if(re!=null&&!re.IsTerminal()&&re.GetToken().Length>0)
						{
							RuleElement reLH = GetRuleElement(LrElm,m_LH);
							MyArrayList FrstSet = null;
							if(reLH==null)
							{
								FrstSet = LrElm.FirstSet;
							}
							else
							{
								FrstSet = BetaTotalRuleFirstSet(reLH);
								if(!BetaTotalRuleEpsFree(reLH))
								{
									FrstSet = ParseHelper.UnionSet(FrstSet,LrElm.FirstSet);
								}
							}
							if(FirstClosureFromRule(Huelle,re.GetToken(),FrstSet))
							{
								bChanged = true;
							}
						}
					}
				}
			}
		}

		private int RuleInsideHuelle(MyArrayList Huelle,int RuleNr,int RuleInPos)
		{
			if(Huelle!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LRElement LrElm = (LRElement)Huelle[i];
					if(LrElm!=null)
					{
						if(LrElm.RulePos==RuleNr&&LrElm.PosInRule==RuleInPos)
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		protected override bool IsSameClosure(MyArrayList Closure1, MyArrayList Closure2)
		{
			if(Closure1!=null && Closure2!=null)
			{
				if(Closure1.Count==Closure2.Count)
				{
					for(int i=0;i<Closure1.Count;i++)
					{
						LR1Element lrm1 = (LR1Element)Closure1[i];
						if(!IsRuleInsideHuelle(Closure2,lrm1))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		private bool IsRuleInsideHuelle(MyArrayList Huelle,LR1Element lrm)
		{
			if(Huelle!=null && lrm!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LR1Element LrElm = (LR1Element)Huelle[i];
					if(LrElm!=null)
					{
						if(LrElm.RulePos==lrm.RulePos&&LrElm.PosInRule==lrm.PosInRule)
						{
							if(ParseHelper.SameRuleLists(LrElm.FirstSet,lrm.FirstSet))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private RuleElement GetRuleElement(LRElement LrElm, int LookAHead)
		{
			RuleElement re = null;
			if(LrElm!=null)
			{
				int RulePos = LrElm.RulePos;
				if(RulePos>=0 && RulePos<m_Rules.Count)
				{
					BNFRule rl = (BNFRule)m_Rules[RulePos];
					if(rl!=null)
					{
						re = rl[LrElm.PosInRule+LookAHead];
					}
				}
			}
			return re;
		}

		protected override MyArrayList GetLrElm(MyArrayList Huelle,string Token)
		{
			MyArrayList LreElmLst = new MyArrayList();
			if(Huelle!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LR1Element LrElm = (LR1Element)Huelle[i];
					RuleElement re = GetRuleElement(LrElm);
					if(re!=null)
					{
						if(re.GetToken().Equals(Token))
						{
							LR1Element nextLRElm = new LR1Element(LrElm,m_HuellenCounter,m_LRMECnter++);
							LreElmLst.Add(nextLRElm);
						}
					}
				}
			}
			return LreElmLst;
		}

		protected override bool GenerateParseTable()
		{
			MyArrayList Terminals = GetAllTerminals();
			Terminals.Add(new RuleTerminal(m_EofTerminal,null));
			MyArrayList Tokens = GetDifStartTokens();
			MyArrayList column = new MyArrayList();
			for(int i=0;i<Terminals.Count;i++)
			{
				RuleElement re = (RuleElement)Terminals[i];
				column.Add(re);
			}
			for(int i=0;i<Tokens.Count;i++)
			{
				RuleStart rs = (RuleStart)Tokens[i];
				column.Add(rs);
			}

			m_bupt = new ButtomUpParseTabelle(column,Terminals,m_Huellen.Count);
			
			bool dblEntry = false;
			for(int i=0;i<m_GotoTable.Count;i++)
			{
				GotoEntry ge = (GotoEntry)m_GotoTable[i];
				RuleElement re = ge.TokenSymbol;
				if(re.IsTerminal())
				{
					ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.SHIFT,ge.JumpToState);
					ButtomUpParseTabelle.ActionEntry buptAE2 = m_bupt.Get(re,ge.ThisState);
					if(buptAE2!=null)
					{
						if(buptAE2.GetAction==ButtomUpParseTabelle.Actions.REDUCE)
						{
							m_bupt.Add(re,ge.ThisState,buptAE);
						}
						else
						{
							dblEntry = true;
						}
					}
					else
					{
						m_bupt.Add(re,ge.ThisState,buptAE);
					}
				}
				else
				{
					ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.JUMP,ge.JumpToState);
					m_bupt.Add(re,ge.ThisState,buptAE);
				}
			}

			
			for(int i=0;i<m_Huellen.Count;i++)
			{
				MyArrayList ActHuelle = (MyArrayList)m_Huellen[i];
				for(int j=0;j<ActHuelle.Count;j++)
				{
					LR1Element lrm = (LR1Element)ActHuelle[j];
					RuleElement re = GetRuleElement(lrm);
					if(re==null)
					{
						MyArrayList Frst = lrm.FirstSet;
						for(int z=0;z<Frst.Count;z++)
						{
							RuleElement Frst_re = (RuleElement)Frst[z];
							ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.REDUCE,lrm.RulePos);
							ButtomUpParseTabelle.ActionEntry ae = m_bupt.Get(Frst_re,i);
							if(ae==null)
							{
								m_bupt.Add(Frst_re,i,buptAE);
							}
							else
							{
								if(ae.GetAction==ButtomUpParseTabelle.Actions.ACCEPT)
								{
									m_bupt.Add(Frst_re,i,buptAE);
								}
								else if(ae.GetAction==ButtomUpParseTabelle.Actions.REDUCE)
								{
									//leave it
								}
								else
								{
									dblEntry = true;
								}
							}
						}

						BNFRule rl = (BNFRule)m_Rules[lrm.RulePos];
						if(rl.GetStart().GetToken().Equals(m_StartSign))
						{
							ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.ACCEPT,lrm.RulePos);
							m_bupt.Add(m_EofTerminal,i,buptAE);
						}
					}
				}
			}

			return !dblEntry;
		}

		public override string PrintHuellen()
		{
			string res = "";
			if(m_Huellen!=null)
			{
				for(int i=0;i<m_Huellen.Count;i++)
				{
					res += "I"+i+":\n";
					MyArrayList Huelle = (MyArrayList)m_Huellen[i];
					if(Huelle!=null)
					{
						for(int j=0;j<Huelle.Count;j++)
						{
							LR1Element LrElm = (LR1Element)Huelle[j];
							BNFRule rl = (BNFRule)m_Rules[LrElm.RulePos];
							res += "("+LrElm.RulePos+","+LrElm.PosInRule+")"+"\t"+GetRuleString(rl.GetStart(),LrElm.PosInRule)+"\t"+ParseHelper.PrintRules(LrElm.FirstSet)+"\n";
						}
					}
					res += "\n\n";
				}
			}
			return res;
		}
	}
}

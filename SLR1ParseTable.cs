//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for SLR1ParseTable.
	/// </summary>
	public class SLR1ParseTable : LL1ParseTable
	{
		#region LRElement klasse
		protected class LRElement
		{
			private int m_Nr = 0;
			internal int Number
			{
				get{return m_Nr;}
			}
			private bool m_Kernel = false;
			internal bool IsKernel
			{
				get{return m_Kernel;}
			}
			private int m_RulePos = 0;
			internal int RulePos
			{
				get{ return m_RulePos;}
			}
			private int m_PosInRule = 0;
			internal int PosInRule
			{
				get{ return m_PosInRule;}
			}
			internal LRElement(int RlPos,int HlNr,int Nr)
			{
				m_Kernel = false;
				m_RulePos = RlPos;
				m_PosInRule = 1;
				m_HuellenNr = HlNr;
				m_Nr = Nr;
			}
			internal LRElement(LRElement lrmElm)
			{
				m_Kernel = lrmElm.IsKernel;
				m_RulePos = lrmElm.RulePos;
				m_PosInRule = lrmElm.PosInRule;
				m_HuellenNr = lrmElm.HuellenNr;
				m_Nr = lrmElm.Number;
			}
			internal LRElement(LRElement LreElm,int HlNr,int Nr)
			{
				m_Kernel = true;
				m_RulePos = LreElm.m_RulePos;
				m_PosInRule = LreElm.m_PosInRule+1;
				m_HuellenNr = HlNr;
				m_Nr = Nr;
			}
			private int m_HuellenNr = 0;
			internal int HuellenNr
			{
				get{return m_HuellenNr;}
			}
		}
		#endregion
		#region GotoEntry Klasse
		protected class GotoEntry
		{
			private RuleElement m_ReElm = null;
			internal RuleElement TokenSymbol
			{
				get{return m_ReElm;}
			}
			private int m_ActState = 0;
			internal int ThisState
			{
				get{return m_ActState;}
			}
			private int m_NextState = 0;
			internal int JumpToState
			{
				get{return m_NextState;}
				set{m_NextState=value;}
			}
			internal GotoEntry(RuleElement ReElm,int ActState,int NextState)
			{
				m_ReElm = ReElm;
				m_ActState = ActState;
				m_NextState = NextState;
			}
			internal GotoEntry(GotoEntry geOld)
			{
				m_ReElm = geOld.TokenSymbol;
				m_ActState = geOld.ThisState;
				m_NextState = geOld.JumpToState;
			}
			internal bool IsSame(RuleElement ReElm,int ActState)
			{
				if(ReElm!=null)
				{
					if(m_ReElm.GetToken().Equals(ReElm.GetToken()) && ActState==m_ActState)
					{
						return true;
					}
				}
				return false;
			}
			internal string Print()
			{
				return "goto["+m_ActState+","+m_ReElm.GetToken()+"]="+m_NextState;
			}
		}
		#endregion
		protected int m_LRMECnter = 0;
		protected MyArrayList m_Huellen = new MyArrayList();
		protected MyArrayList m_GotoTable = new MyArrayList();
		protected int m_HuellenCounter = 0;
		protected ButtomUpParseTabelle m_bupt = null;
		public ButtomUpParseTabelle SLR1Table
		{
			get{return m_bupt;}
		}

		public SLR1ParseTable(MyArrayList Rules,string StartSign) : base(Rules,StartSign)
		{
			m_StartSign = FindUnusedToken(m_StartSign);
			BNFRule bnfRl = new BNFRule(m_Rules.Count);
			bnfRl.SetStart(m_StartSign);
			bnfRl.GetStart().SetNext(new RuleToken(StartSign,bnfRl.GetStart()));
			m_Rules.Add(bnfRl);
		}

		public override bool Init()
		{
			m_HuellenCounter = 0;
			ConnectAlternateStartRules();
			ConnectTokens();
			CheckEpsilonInRules();
			GenerateAllFirstSets();
			GenerateAllFollowSets();
			GenerateClosure();
			if(!GenerateParseTable())
			{
				return false;
			}
			return true;
		}
	
		private bool FirstClosureFromRule(MyArrayList Huelle, string Token)
		{
			bool bChanged = false;
			MyArrayList TkRules = base.GetStartRulesOneToken(Token);
			if(TkRules!=null&&Huelle!=null)
			{
				for(int i=0;i<TkRules.Count;i++)
				{
					RuleStart rs = (RuleStart)TkRules[i];
					int RuleNr = rs.GetRuleConnect().Index;
					if(!IsRuleInsideHuelle(Huelle,RuleNr,1))
					{
						LRElement LrElm = new LRElement(rs.GetRuleConnect().Index,m_HuellenCounter,m_LRMECnter++);
						Huelle.Add(LrElm);							
						bChanged = true;
					}
				}
			}
			return bChanged;
		}

		protected virtual void Closure(MyArrayList Huelle)
		{
			bool bChanged = true;
			if(Huelle!=null)
			{
				while(bChanged)
				{
					bChanged = false;
					for(int i=0;i<Huelle.Count;i++)
					{
						LRElement LrElm = (LRElement)Huelle[i];
						RuleElement re = GetRuleElement(LrElm);
						if(re!=null&&!re.IsTerminal()&&re.GetToken().Length>0)
						{
							if(FirstClosureFromRule(Huelle,re.GetToken()))
							{
								bChanged = true;
							}
						}
					}
				}
			}
		}

		protected virtual void GenerateClosure()
		{
			MyArrayList Huelle = new MyArrayList();
			if(FirstClosureFromRule(Huelle,m_StartSign))
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

		public string PrintGotoTable()
		{
			string res = "";
			if(m_GotoTable!=null)
			{
				for(int i=0;i<m_GotoTable.Count;i++)
				{
					GotoEntry ge = (GotoEntry)m_GotoTable[i];
					res += ge.Print()+"\n";
				}
			}
			return res;
		}
		public virtual string PrintHuellen()
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
							LRElement LrElm = (LRElement)Huelle[j];
							BNFRule rl = (BNFRule)m_Rules[LrElm.RulePos];
							res += "("+LrElm.RulePos+","+LrElm.PosInRule+")"+"\t"+GetRuleString(rl.GetStart(),LrElm.PosInRule)+"\n";
						}
					}
					res += "\n\n";
				}
			}
			return res;
		}

		public string GetRuleString(RuleElement Start,int PntPos)
		{
			string res = Start.GetToken()+"->";
			RuleElement iterator = Start.GetNext();
			int Pos=1;
			while(iterator!=null)
			{
				if(PntPos==Pos)
				{
					res += ".";
				}
				if(iterator.IsTerminal())
				{
					res += "'"+iterator.GetToken()+"'";
				}
				else
				{
					res += iterator.GetToken();
				}
				iterator = iterator.GetNext();
				if(iterator!=null)
				{
					res += ",";
				}
				Pos++;
			}
			if(PntPos==Pos)
			{
				res += ".";
			}
			res += ";";
			return res;
		}

		protected MyArrayList CreateGotoFromLastHuellen(MyArrayList LastHuelle)
		{
			MyArrayList nextHuellen = new MyArrayList();
			if(LastHuelle!=null)
			{
				for(int i=0;i<LastHuelle.Count;i++)
				{
					MyArrayList Huelle = (MyArrayList)LastHuelle[i];
					if(Huelle!=null)
					{
						MyArrayList neueHuellen = CreateGoto(Huelle);
						nextHuellen.AddArray(neueHuellen);
					}
				}
			}
			return nextHuellen;
		}

		protected bool AreHuellenEmpty(MyArrayList Huellen)
		{
			if(Huellen!=null)
			{
				for(int i=0;i<Huellen.Count;i++)
				{
					MyArrayList actHuelle = (MyArrayList)Huellen[i];
					if(actHuelle!=null && actHuelle.Count>0)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected int GetHuellenNr(MyArrayList Huelle)
		{
			if(Huelle!=null && Huelle.Count>0)
			{
				LRElement lrm = (LRElement)Huelle[0];
				return lrm.HuellenNr;
			}
			return -1;
		}

		private MyArrayList CreateGoto(MyArrayList Huelle)
		{
			MyArrayList neueHuellen = new MyArrayList();
			if(Huelle!=null)
			{
				MyArrayList DifTokens = GetDifProduktionNrsFromClosure(Huelle);
				int actHuellenNr = GetHuellenNr(Huelle);
				for(int i=0;i<DifTokens.Count;i++)
				{
					RuleElement re = (RuleElement)DifTokens[i];
					MyArrayList NeueHuelle = GetLrElm(Huelle,re.GetToken());
					if(NeueHuelle.Count>0)
					{
						Closure(NeueHuelle);
						int ClosPos = FindSameClosure(NeueHuelle);
						if(ClosPos<0)
						{
							
							Closure(NeueHuelle);
							neueHuellen.Add(NeueHuelle);
							
							if(!IsInGototable(re,actHuellenNr))
							{
								m_GotoTable.Add(new GotoEntry(re,actHuellenNr,m_HuellenCounter));
							}
							m_HuellenCounter++;
						}
						else
						{
							if(!IsInGototable(re,actHuellenNr))
							{
								m_GotoTable.Add(new GotoEntry(re,actHuellenNr,ClosPos));
							}
						}
					}
				}
			}
			return neueHuellen;
		}

		private int FindSameClosure(MyArrayList Huelle)
		{
			if(Huelle!=null)
			{
				for(int i=0;i<m_Huellen.Count;i++)
				{
					MyArrayList tmpHuelle = (MyArrayList)m_Huellen[i];
					if(tmpHuelle!=null)
					{
						if(IsSameClosure(Huelle,tmpHuelle))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		protected virtual bool IsSameClosure(MyArrayList Closure1, MyArrayList Closure2)
		{
			if(Closure1!=null && Closure2!=null)
			{
				if(Closure1.Count==Closure2.Count)
				{
					for(int i=0;i<Closure1.Count;i++)
					{
						LRElement lrm1 = (LRElement)Closure1[i];
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

		protected virtual MyArrayList GetLrElm(MyArrayList Huelle,string Token)
		{
			MyArrayList LreElmLst = new MyArrayList();
			if(Huelle!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LRElement LrElm = (LRElement)Huelle[i];
					RuleElement re = GetRuleElement(LrElm);
					if(re!=null)
					{
						if(re.GetToken().Equals(Token))
						{
							LRElement nextLRElm = new LRElement(LrElm,m_HuellenCounter,m_LRMECnter++);
							LreElmLst.Add(nextLRElm);
						}
					}
				}
			}
			return LreElmLst;
		}

		private bool IsInGototable(RuleElement re,int HuellenNr)
		{
			if(re!=null)
			{
				for(int i=0;i<m_GotoTable.Count;i++)
				{
					GotoEntry ge = (GotoEntry)m_GotoTable[i];
					if(ge.IsSame(re,HuellenNr))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected int GetPosInGototable(RuleElement re,int HuellenNr)
		{
			if(re!=null)
			{
				for(int i=0;i<m_GotoTable.Count;i++)
				{
					GotoEntry ge = (GotoEntry)m_GotoTable[i];
					if(ge.IsSame(re,HuellenNr))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetGotoState(RuleElement re,int HuellenNr)
		{
			int GotoPos = GetPosInGototable(re,HuellenNr);
			if(GotoPos>=0)
			{
				GotoEntry ge = (GotoEntry)m_GotoTable[GotoPos];
				return ge.JumpToState;
			}
			return -1;
		}

		private MyArrayList GetDifProduktionNrsFromClosure(MyArrayList Huelle)
		{
			MyArrayList ProdTokens = new MyArrayList();
			if(Huelle!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LRElement LrElm = (LRElement)Huelle[i];
					RuleElement re = GetRuleElement(LrElm);
					if(re!=null)
					{
						if(!IsProdRuleInside(ProdTokens,re))
						{
							ProdTokens.Add(re);
						}
					}
				}
			}
			return ProdTokens;
		}

		protected RuleElement GetRuleElement(LRElement LrElm)
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
						re = rl[LrElm.PosInRule];
					}
				}
			}
			return re;
		}

		private bool IsProdRuleInside(MyArrayList ProdTokens,RuleElement re)
		{
			if(ProdTokens!=null&&re!=null)
			{
				for(int i=0;i<ProdTokens.Count;i++)
				{
					RuleElement ProdRe = (RuleElement)ProdTokens[i];

					if(ProdRe.GetToken().Equals(re.GetToken()))
					{
						return true;
					}
				}
			}
			return false;
		}

		private RuleElement GetElementFromHuelle(LRElement elm)
		{
			RuleElement re = null;
			if(elm.RulePos<m_Rules.Count)
			{
				BNFRule rl = (BNFRule)m_Rules[elm.RulePos];
				if(rl!=null)
				{
					re = rl[elm.PosInRule];
				}
			}
			return re;
		}

		private bool IsRuleInsideAllHuellen(MyArrayList AllHuellen,LRElement LrElm)
		{
			if(AllHuellen!=null&&LrElm!=null)
			{
				for(int i=0;i<AllHuellen.Count;i++)
				{
					MyArrayList Huelle = (MyArrayList)AllHuellen[i];
					if(Huelle!=null)
					{
						if(IsRuleInsideHuelle(Huelle,LrElm.RulePos,LrElm.PosInRule))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private int RuleInsideAllHuellen(MyArrayList AllHuellen,LRElement LrElm)
		{
			if(AllHuellen!=null&&LrElm!=null)
			{
				for(int i=0;i<AllHuellen.Count;i++)
				{
					MyArrayList Huelle = (MyArrayList)AllHuellen[i];
					if(Huelle!=null)
					{
						if(IsRuleInsideHuelle(Huelle,LrElm.RulePos,LrElm.PosInRule))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}


		private bool IsRuleInsideHuelle(MyArrayList Huelle,int RuleNr,int RuleInPos)
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
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsRuleInsideHuelle(MyArrayList Huelle,LRElement lrm)
		{
			if(Huelle!=null && lrm!=null)
			{
				for(int i=0;i<Huelle.Count;i++)
				{
					LRElement LrElm = (LRElement)Huelle[i];
					if(LrElm!=null)
					{
						if(LrElm.RulePos==lrm.RulePos&&LrElm.PosInRule==lrm.PosInRule)
						{
							return true;
						}
					}
				}
			}
			return false;
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

			for(int i=0;i<m_GotoTable.Count;i++)
			{
				GotoEntry ge = (GotoEntry)m_GotoTable[i];
				RuleElement re = ge.TokenSymbol;
				if(re.IsTerminal())
				{
					ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.SHIFT,ge.JumpToState);
					m_bupt.Add(re,ge.ThisState,buptAE);
				}
				else
				{
					ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.JUMP,ge.JumpToState);
					m_bupt.Add(re,ge.ThisState,buptAE);
				}
			}

			bool dblEntry = false;
			for(int i=0;i<m_Huellen.Count;i++)
			{
				MyArrayList ActHuelle = (MyArrayList)m_Huellen[i];
				for(int j=0;j<ActHuelle.Count;j++)
				{
					LRElement lrm = (LRElement)ActHuelle[j];
					RuleElement re = GetRuleElement(lrm);
					if(re==null)
					{
						BNFRule rl = (BNFRule)m_Rules[lrm.RulePos];
						MyArrayList flws = rl.FollowSet;
						if(flws!=null)
						{
							for(int z=0;z<flws.Count;z++)
							{
								RuleElement flw_re = (RuleElement)flws[z];
								ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.REDUCE,lrm.RulePos);
								ButtomUpParseTabelle.ActionEntry ae = m_bupt.Get(flw_re,i);
								if(ae==null)
								{
									m_bupt.Add(flw_re,i,buptAE);
								}
								else
								{
									if(ae.GetAction!=ButtomUpParseTabelle.Actions.ACCEPT)
									{
										dblEntry = true;
									}
								}
							}
							if(rl.GetStart().GetToken().Equals(m_StartSign))
							{
								ButtomUpParseTabelle.ActionEntry buptAE = new ButtomUpParseTabelle.ActionEntry(ButtomUpParseTabelle.Actions.ACCEPT,lrm.RulePos);
								m_bupt.Add(m_EofTerminal,i,buptAE);
							}
						}
					}
				}
			}

			return !dblEntry;
		}
	}
}

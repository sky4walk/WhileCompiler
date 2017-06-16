// written by André Betz
// http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for LALR1ParseTable.
	/// </summary>
	public class LALR1ParseTable : LR1ParseTable
	{
		protected ButtomUpParseTabelle m_LaLr1Table = null;
		protected MyArrayList m_HuellenNeu = new MyArrayList();
		protected MyArrayList m_GotoTableNeu = new MyArrayList();

		public LALR1ParseTable(MyArrayList Rules,string StartSign):base(Rules,StartSign)
		{
		}
		public ButtomUpParseTabelle LALR1Table
		{
			get{return m_LaLr1Table;}
		}

		protected override bool GenerateParseTable()
		{
			CopyGotoList();
			GenerateUnitedHuellen();
			m_Huellen = m_HuellenNeu;
			m_GotoTable = m_GotoTableNeu;
			return base.GenerateParseTable();
		}
		
		private void CopyGotoList()
		{
			for(int i=0;i<m_GotoTable.Count;i++)
			{
				GotoEntry ge = (GotoEntry)m_GotoTable[i];
				m_GotoTableNeu.Add(new GotoEntry(ge));
			}
		}

		private void GenerateUnitedHuellen()
		{
			for(int i=0;i<m_Huellen.Count;i++)
			{
				MyArrayList Huelle = (MyArrayList)m_Huellen[i];
				int Nr = FindSameClosure(m_HuellenNeu,Huelle);
				if(Nr<0)
				{
					MyArrayList newHuelle = CopyHuelle(Huelle);
					m_HuellenNeu.Add(newHuelle);

					MyArrayList fndHuellen = FindSameClosures(m_Huellen,Huelle,i);
					UniteFirstsets(fndHuellen,newHuelle);

					ChangeGotoStates(fndHuellen,i,m_HuellenNeu.Count-1);
				}
			}
		}

		private void ChangeGotoStates(MyArrayList fndHuellen,int OldStateNr,int NewStateNr)
		{
			for(int i=0;i<m_GotoTableNeu.Count;i++)
			{
				GotoEntry ge = (GotoEntry) m_GotoTableNeu[i];
				if(ge.JumpToState==OldStateNr)
				{
					ge.JumpToState  = NewStateNr; 
				}
				for(int j=0;j<fndHuellen.Count;j++)
				{
					int Nr = (int)fndHuellen[j];
					if(Nr==ge.JumpToState)
					{
						ge.JumpToState  = NewStateNr;
					}
					if(Nr==ge.ThisState)
					{
						m_GotoTableNeu.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private void ChangeGotoWithNextState(int Nr,int OldNr,int NewNr)
		{
			MyArrayList sameNextStates = new MyArrayList();
			for(int i=0;i<m_GotoTableNeu.Count;i++)
			{
				GotoEntry ge = (GotoEntry) m_GotoTableNeu[i];
				if(ge.JumpToState==OldNr)
				{
					ge.JumpToState  = NewNr; 
				}
				if(ge.ThisState==Nr)
				{
					m_GotoTableNeu.RemoveAt(i);
				}
			}
		}

		private MyArrayList CopyGotos(int HuellenNr)
		{
			MyArrayList cpGotos = null;
			if(m_GotoTable!=null)
			{
				cpGotos = new MyArrayList();
				for(int i=0;i<m_GotoTable.Count;i++)
				{
					GotoEntry ge = (GotoEntry)m_GotoTable[i];
					if(ge.ThisState==HuellenNr)
					{
						cpGotos.Add(new GotoEntry(ge));
					}
				}
			}
			return cpGotos;
		}

		private MyArrayList CopyHuellen(MyArrayList arrOld)
		{
			MyArrayList newHuellen = new MyArrayList();
			if(arrOld!=null)
			{
				for(int i=0;i<arrOld.Count;i++)
				{
					MyArrayList Huelle = (MyArrayList)arrOld[i];
					newHuellen.Add(CopyHuelle(Huelle));
				}
			}
			return newHuellen;
		}

		private MyArrayList CopyHuelle(MyArrayList arrOld)
		{
			MyArrayList newHuelle = null;
			if(arrOld!=null)
			{
				newHuelle = new MyArrayList();
				for(int i=0;i<arrOld.Count;i++)
				{
					LR1Element lrm1 = (LR1Element)arrOld[i];
					LR1Element lrm2 = new LR1Element(lrm1);
					newHuelle.Add(lrm2);
				}
			}
			return newHuelle;
		}

		private void UniteFirstsets(MyArrayList FndHuellen, MyArrayList Huelle)
		{
			if(FndHuellen!=null && Huelle != null)
			{
				for(int i=0;i<FndHuellen.Count;i++)
				{
					int fndHuellenNr = (int) FndHuellen[i];
					MyArrayList Huelle2 = (MyArrayList) m_Huellen[fndHuellenNr];
					for(int j=0;j<Huelle2.Count;j++)
					{
						LR1Element lrm1 = (LR1Element)Huelle2[j];
						int Pos = GetRuleInsideHuelle(Huelle,lrm1);
						if(Pos>=0)
						{
							LR1Element lrm2 = (LR1Element)Huelle[Pos];
							lrm2.FirstSet = ParseHelper.UnionSet(lrm2.FirstSet,lrm1.FirstSet);
						}
					}
				}
			}
		}

		private int FindSameClosure(MyArrayList HuellenListe,MyArrayList Huelle)
		{
			if(Huelle!=null&&HuellenListe!=null)
			{
				for(int i=0;i<HuellenListe.Count;i++)
				{
					MyArrayList tmpHuelle = (MyArrayList)HuellenListe[i];
					if(tmpHuelle!=null)
					{
						if(IsSameClosureWithoutFirst(Huelle,tmpHuelle))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		private MyArrayList FindSameClosures(MyArrayList HuellenListe,MyArrayList Huelle,int Nr)
		{
			MyArrayList CollectHuellen = new MyArrayList();
			if(Huelle!=null&&HuellenListe!=null)
			{
				for(int i=0;i<HuellenListe.Count;i++)
				{
					if(Nr!=i)
					{
						MyArrayList tmpHuelle = (MyArrayList)HuellenListe[i];
						if(tmpHuelle!=null)
						{
							if(IsSameClosureWithoutFirst(Huelle,tmpHuelle))
							{
								CollectHuellen.Add(i);
							}
						}
					}
				}
			}
			return CollectHuellen;
		}

		private bool IsSameClosureWithoutFirst(MyArrayList Closure1, MyArrayList Closure2)
		{
			if(Closure1!=null && Closure2!=null)
			{
				if(Closure1.Count==Closure2.Count)
				{
					for(int i=0;i<Closure1.Count;i++)
					{
						LRElement lrm1 = (LRElement)Closure1[i];
						if(GetRuleInsideHuelle(Closure2,lrm1)<0)
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		private int GetRuleInsideHuelle(MyArrayList Huelle,LRElement lrm)
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
							return i;
						}
					}
				}
			}
			return -1;
		}
	}
}

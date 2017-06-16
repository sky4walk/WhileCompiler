//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for Tabelle.
	/// </summary>
	public class Tabelle
	{
		/// <summary>
		/// Tabelle
		/// </summary>
		protected MyArrayList m_Tabelle = null;
		protected int m_cols = 0;
		protected int m_rows = 0;
		protected static string ms_Seperator = ";";

		public Tabelle(int col,int row)
		{
			m_cols = col;
			m_rows = row;
			m_Tabelle = new MyArrayList();
			for(int i=0;i<row;i++)
			{
				MyArrayList colArr = new MyArrayList();
				for(int j=0;j<col;j++)
				{
					colArr.Add(new object());
				}
				m_Tabelle.Add(colArr);
			}
		}

		public bool Add(int col,int row, object obj)
		{
			bool Empty = true;
			if(row>=0&&row<m_Tabelle.Count)
			{
				MyArrayList colArr = (MyArrayList)m_Tabelle[row];
				if(col>=0&&col<colArr.Count)
				{
					if(colArr[col].GetType()!=typeof(object))
						Empty = false;
					colArr[col] = obj;
				}
			}
			return Empty;
		}

		public void AddCol()
		{
			for(int i=0;i<m_rows;i++)
			{
				MyArrayList colArr = (MyArrayList)m_Tabelle[i];
				colArr.Add(new object());
			}
			m_cols++;
		}

		public void AddRow()
		{
			MyArrayList OneRow = new MyArrayList();
			for(int j=0;j<m_cols;j++)
			{
				OneRow.Add(new object());
			}
			m_Tabelle.Add(OneRow);
			m_rows++;
		}

		public object Get(int col,int row)
		{
			if(row>=0&&row<m_Tabelle.Count)
			{
				MyArrayList colArr = (MyArrayList)m_Tabelle[row];
				if(col>=0&&col<colArr.Count)
				{
					return colArr[col];
				}
			}
			return null;
		}
	}

	public class TopDownParseTabelle : Tabelle
	{
		MyArrayList m_Terminals = null;
		MyArrayList m_StartTerms = null;

		public MyArrayList Terminals
		{
			get{return m_Terminals;}
		}

		public TopDownParseTabelle(MyArrayList col,MyArrayList row) : base(col.Count,row.Count)
		{
			m_Terminals = col;
			m_StartTerms = row;
		}
		public bool Add(string col,string row, RuleStart Rule)
		{
			int iCol = GetColPos(col);
			int iRow = GetRowPos(row);
			return base.Add(iCol,iRow,Rule);
		}
		public RuleStart Get(string col,string row)
		{
			int iCol = GetColPos(col);
			int iRow = GetRowPos(row);
			object obj = base.Get(iCol,iRow);
			if(obj.GetType()==typeof(RuleStart))
			{
				return (RuleStart)obj;
			}
			return null;
		}
		private int GetColPos(string col)
		{
			for(int i=0;i<m_Terminals.Count;i++)
			{
				RuleTerminal rt = (RuleTerminal)m_Terminals[i];
				if(rt.GetToken().Equals(col))
				{
					return i;
				}
			}
			return -1;
		}
		private int GetRowPos(string row)
		{
			for(int i=0;i<m_StartTerms.Count;i++)
			{
				RuleElement re = (RuleElement)m_StartTerms[i];
				if(re.GetToken().Equals(row))
				{
					return i;
				}
			}
			return -1;
		}

		public string Print()
		{			
			string TableOut = ms_Seperator;
			for(int i=0;i<m_Terminals.Count;i++)
			{
				RuleTerminal rt = (RuleTerminal)m_Terminals[i];
				TableOut+=rt.GetToken()+ms_Seperator;
			}
			
			TableOut+="\n";

			for(int i=0;i<m_StartTerms.Count;i++)
			{
				RuleElement re = (RuleElement)m_StartTerms[i];
				TableOut+=re.GetToken()+ms_Seperator;
				for(int j=0;j<m_Terminals.Count;j++)
				{
					RuleTerminal rt = (RuleTerminal)m_Terminals[j];
					RuleStart rs = Get(rt.GetToken(),re.GetToken());
					if(rs==null)
					{
						TableOut+=ms_Seperator;
					}
					else
					{
						TableOut+=rs.GetAllElements()+ms_Seperator;
					}
				}
				TableOut+="\n";
			}
			return TableOut;
		}
	}
	public class ButtomUpParseTabelle : Tabelle
	{
		#region Table Entries
		public enum Actions
		{
			JUMP,
			SHIFT,
			REDUCE,
			ACCEPT
		}
		public class ActionEntry
		{
			private Actions m_Action;
			private int m_JumpState;
			public int NextState
			{
				get{return m_JumpState;}
			}
			public Actions GetAction
			{
				get{return m_Action;}
			}
			public ActionEntry(Actions Action,int JumpState)
			{
				m_Action = Action;
				m_JumpState = JumpState;
			}
		}
		#endregion

		private MyArrayList m_Signs = null;
		private MyArrayList m_Terminals = null;
		public MyArrayList Terminals
		{
			get{return m_Terminals;}
		}

		public ButtomUpParseTabelle(MyArrayList col,MyArrayList Terminals, int States) : base(col.Count,States)
		{
			m_Signs = col;
			m_Terminals = Terminals;
		}

		public bool Add(RuleElement col,int iRow, ActionEntry ae)
		{
			int iCol = GetColPos(col);
			return base.Add(iCol,iRow,ae);
		}

		public bool Add(string col,int iRow, ActionEntry ae)
		{
			int iCol = GetColPos(col);
			return base.Add(iCol,iRow,ae);
		}

		public ActionEntry Get(RuleElement col,int State)
		{
			int iCol = GetColPos(col);
//			if(iCol>=0)
			{
				object obj = base.Get(iCol,State);
				if(obj.GetType()==typeof(ActionEntry))
				{
					return (ActionEntry)obj;
				}
			}
			return null;
		}

		public ActionEntry Get(string col,int State)
		{
			int iCol = GetColPos(col);
			object obj = base.Get(iCol,State);
			if(obj.GetType()==typeof(ActionEntry))
			{
				return (ActionEntry)obj;
			}
			return null;
		}

		private int GetColPos(RuleElement col)
		{
			if(col!=null)
			{
				for(int i=0;i<m_Signs.Count;i++)
				{
					RuleElement re = (RuleElement)m_Signs[i];
					if(re!=null)
					{
						if(re.GetToken().Equals(col.GetToken()) && re.IsTerminal()==col.IsTerminal())
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		private int GetColPos(string col)
		{
			if(col!=null)
			{
				for(int i=0;i<m_Signs.Count;i++)
				{
					RuleElement re = (RuleElement)m_Signs[i];
					if(re!=null)
					{
						if(re.GetToken().Equals(col) && re.IsTerminal())
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public string Print()
		{			
			string TableOut = ms_Seperator;
			for(int i=0;i<m_Signs.Count;i++)
			{
				RuleElement re = (RuleElement)m_Signs[i];
				TableOut+=re.GetToken()+ms_Seperator;
			}
			
			TableOut+="\n";

			for(int i=0;i<m_rows;i++)
			{
				TableOut+=i+ms_Seperator;
				for(int j=0;j<m_Signs.Count;j++)
				{
					RuleElement re = (RuleElement)m_Signs[j];

					ActionEntry ae = Get(re,i);
					if(ae==null)
					{
						TableOut+=ms_Seperator;
					}
					else
					{
						if(ae.GetAction==Actions.SHIFT)
						{
							TableOut+="s "+ae.NextState+ms_Seperator;
						}
						else if(ae.GetAction==Actions.REDUCE)
						{
							TableOut+="r "+ae.NextState+ms_Seperator;
						}
						else if(ae.GetAction==Actions.JUMP)
						{
							TableOut+=ae.NextState+ms_Seperator;
						}
						else if(ae.GetAction==Actions.ACCEPT)
						{
							TableOut+="acc"+ms_Seperator;
						}
						else
						{
						}
					}
				}
				TableOut+="\n";
			}

			return TableOut;
		}
	}
}

//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for BNFRule.
	/// </summary>
	public class BNFRule
	{
		private RuleStart Start;
		private RuleElement act;
		private MyArrayList m_FirstSet;
		private MyArrayList m_FollowSet;
		private int m_Index = 0;
		private bool m_EpsilonFree; 

		public BNFRule(int index)
		{
			Start = null;
			act = null;
			m_Index = index;
			m_EpsilonFree = true;
		}

		public RuleElement this [int pos]
		{
			get
			{
				return this.GetRulePos(pos);
			}
		}

		public int Index
		{
			get{return m_Index;}
		}
		public MyArrayList FirstSet
		{
			get{return m_FirstSet;}
			set{m_FirstSet=value;}
		}
		public MyArrayList FollowSet
		{
			get{return m_FollowSet;}
			set{m_FollowSet=value;}
		}
		public bool EpsilonFree
		{
			get{return m_EpsilonFree;}
			set{m_EpsilonFree = value;}
		}
		public RuleStart GetStart()
		{
			return Start;
		}
		/// <summary>
		/// Find all Tokens inside Rule
		/// </summary>
		/// <param name="Token">Token String to find</param>
		/// <returns></returns>
		public MyArrayList FindToken(string Token)
		{
			MyArrayList sr = new MyArrayList();
			if(Start!=null)
			{
				RuleElement pos = Start.GetNext();
				while(pos!=null)
				{
					if(pos.GetToken().Equals(Token))
					{
						sr.Add(pos);
					}
					pos = pos.GetNext();
				}
			}
			return sr;
		}
		public MyArrayList GetAllTerminals()
		{
			MyArrayList ts = new MyArrayList();
			if(Start!=null)
			{
				RuleElement pos = Start.GetNext();
				while(pos!=null)
				{
					if(pos.IsTerminal())
					{
						ts.Add(pos);
					}
					pos = pos.GetNext();
				}
			}
			return ts;
		}
		public void SetStart(string Token)
		{
			Start = new RuleStart(Token,null,this);
			act = Start;
		}
		public void SetNextToken(String Token,bool Terminal)
		{
			if(Token==null||Token.Equals(""))
			{
				act.SetNext(new RuleEpsilon(Start));
			}
			else
			{
				if(Terminal)
				{
					act.SetNext(new RuleTerminal(Token,Start));
				}
				else
				{
					act.SetNext(new RuleToken(Token,Start));
				}
			}
			act = act.GetNext();
		}
		public RuleElement GetRulePos(int pos)
		{
			RuleElement re = GetStart();
			for(int i=0;i<pos;i++)
			{
				if(re!=null)
				{
					re = re.GetNext();
				}
				else
				{
					break;
				}
			}
			return re;
		}
		public string GetAllInfos()
		{
			return GetRuleString()+"\t"+PrintEpsFree()+"\t"+PrintFirst()+"\t"+PrintFollow();
		}

		public string GetRuleString()
		{
			string res = Start.GetToken()+"->";
			RuleElement iterator = Start.GetNext();
			while(iterator!=null)
			{
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
			}
			res += ".";
			return res;
		}
		private string PrintFollow()
		{
			string res = "Follow={";
			for(int i=0;i<m_FollowSet.Count;i++)
			{
				RuleElement re = (RuleElement)m_FollowSet[i];
				res+=re.GetToken();
				if(i+1!=m_FollowSet.Count)
				{
					res += ",";
				}
			}
			res += "}";
			return res;
		}
		private string PrintEpsFree()
		{
			string res = "Epsfree=";
			if(this.m_EpsilonFree)
			{
				res+="t";
			}
			else
			{
				res+="f";
			}
			return res;
		}
		private string PrintFirst()
		{
			string res = "First={";
			for(int i=0;i<m_FirstSet.Count;i++)
			{
				RuleElement re = (RuleElement)m_FirstSet[i];
				res+=re.GetToken();
				if(i+1!=m_FirstSet.Count)
				{
					res += ",";
				}
			}
			res += "}";
			return res;
		}
		public int RuleLength()
		{
			int rl_len = 0;
			RuleElement iterator = Start.GetNext();
			while(iterator!=null)
			{
				rl_len++;
				iterator = iterator.GetNext();
			}
			return rl_len;
		}
	}
}

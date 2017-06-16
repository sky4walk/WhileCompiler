//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for ParseStack.
	/// </summary>
	public class TopDownParseStack
	{
		/// <summary>
		/// Stack
		/// </summary>
		private MyStack m_Stack = null;
		private MyStack m_TreeStack = null;
		private ASTElement m_TreeRoot = null;
		private ASTElement m_ActRoot = null;

		public ASTElement ASTRoot
		{
			get{ return m_TreeRoot;}
		}

		public bool IsEmpty
		{
			get{return m_Stack.IsEmpty;}
		}

		public string StackValues
		{
			get
			{
				string StrStck = "";
				MyArrayList arr = m_Stack.GetStack;
				for(int i=0;i<arr.Count;i++)
				{
					RuleElement re = (RuleElement)arr[i];
					StrStck += re.GetToken() + " ";
				}
				return StrStck;
			}
		}

		public MyArrayList StackArray
		{
			get{return m_Stack.GetCopyStack;}
		}

		public TopDownParseStack(RuleStart rs)
		{
			m_Stack = new MyStack();
			m_TreeStack = new MyStack();
			if(rs!=null)
			{
				m_Stack.Push(rs);
				m_TreeRoot = new ASTElement(rs);
				m_ActRoot = m_TreeRoot;
				m_TreeStack.Push(m_TreeRoot);
			}
		}

		public void Push(RuleStart Transition)
		{
			MyArrayList TransRules = Rules2Array(Transition);
			for(int i=0;i<TransRules.Count;i++)
			{
				RuleElement re = (RuleElement)TransRules[TransRules.Count-i-1];
				m_Stack.Push(re);
				re = (RuleElement)TransRules[i];
				ASTElement astElm = m_ActRoot.AddNode(re);
				m_TreeStack.Push(astElm);
			}
		}

		private MyArrayList Rules2Array(RuleStart rs)
		{
			MyArrayList invRules = new MyArrayList();
			if(rs!=null)
			{
				RuleElement re = rs.GetNext();
				while(re!=null)
				{
					invRules.Add(re);
					re = re.GetNext();
				}
			}
			return invRules;
		}

		public RuleElement Pop()
		{
			RuleElement re = (RuleElement)m_Stack.Pop();
			m_ActRoot = (ASTElement)m_TreeStack.Pop();
			return re;
		}
	}

	public class ButtomUpParseStack
	{
		#region Stack Element
		public class buStackElement
		{
			private RuleElement m_re = null;
			public RuleElement GetRule
			{
				get{return m_re;}
			}
			private int m_State = 0;
			public int GetState
			{
				get{return m_State;}
			}
			private bool m_IsRuleElement = true;
			public bool IsRuleElement
			{
				get{return m_IsRuleElement;}
			}
			public buStackElement(RuleElement re)
			{
				m_IsRuleElement = true;
				m_re = re;
			}
			public buStackElement(int State)
			{
				m_IsRuleElement = false;
				m_State = State;
			}
		}
		#endregion
		private MyStack m_Stack = null;
		private MyStack m_TreeStack = null;
		private ASTElement m_TreeRoot = null;
		private MyArrayList m_actPopList = new MyArrayList();

		public ASTElement ASTRoot
		{
			get{ return m_TreeRoot;}
		}

		public bool IsEmpty
		{
			get{return m_Stack.IsEmpty;}
		}

		public ButtomUpParseStack()
		{
			m_Stack = new MyStack();
			m_TreeStack = new MyStack();

			buStackElement stkElm = new buStackElement(0);
			m_Stack.Push(stkElm);
		}
		public void PushShift(RuleElement re,int State)
		{
			m_Stack.Push(new buStackElement(re));
			m_Stack.Push(new buStackElement(State));
			
			ASTElement neuElm = new ASTElement(re);
			m_TreeStack.Push(neuElm);

			m_actPopList.Clear();
		}
		public void PushReduce(RuleElement re,int State)
		{
			m_Stack.Push(new buStackElement(re));
			m_Stack.Push(new buStackElement(State));
			
			m_TreeRoot = new ASTElement(re,m_actPopList);
			m_TreeStack.Push(m_TreeRoot);

			m_actPopList.Clear();
		}
		public buStackElement Pop()
		{
			buStackElement stkElm = (buStackElement)m_Stack.Pop();
			return stkElm;
		}

		public RuleElement Pop2()
		{
			buStackElement elm1 = (buStackElement)m_Stack.Pop();
			buStackElement elm2 = (buStackElement)m_Stack.Pop();

			m_TreeRoot = (ASTElement) m_TreeStack.Pop();
			m_actPopList.Add(m_TreeRoot);
			
			return elm2.GetRule;
		}

		public buStackElement Look()
		{
			buStackElement stkElm = (buStackElement)m_Stack.Look();
			return stkElm;
		}

		public string StackValues
		{
			get
			{
				string StrStck = "";
				MyArrayList arr = m_Stack.GetStack;
				for(int i=0;i<arr.Count;i++)
				{
					buStackElement stkElm = (buStackElement)arr[i];
					if(stkElm.IsRuleElement)
						StrStck += stkElm.GetRule.GetToken() + " ";
					else
						StrStck += stkElm.GetState.ToString() + " ";
				}
				return StrStck;
			}
		}

		public MyArrayList StackArray
		{
			get{return m_Stack.GetCopyStack;}
		}
	}
}

//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for MyStack.
	/// </summary>
	public class MyStack
	{
		/// <summary>
		/// Stack Array
		/// </summary>
		private MyArrayList m_stk = new MyArrayList();
		/// <summary>
		/// StackPosition
		/// </summary>
		private int m_stkPos = 0;
		/// <summary>
		/// Stack Hight
		/// </summary>
		public int Hight
		{
			get{return m_stkPos;}
		}
		/// <summary>
		/// constructor
		/// </summary>
		public MyStack()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsEmpty
		{
			get{return m_stkPos==0;}
		}
		/// <summary>
		/// get Stack
		/// </summary>
		public MyArrayList GetStack
		{
			get{return m_stk;}
		}
		/// <summary>
		/// Pop
		/// </summary>
		/// <returns>returns object from stack</returns>
		public object Pop()
		{
			object obj = null;
			if(m_stkPos>0)
			{
				m_stkPos--;
				obj = m_stk[m_stkPos];
				m_stk.RemoveAt(m_stkPos);
			}
			return obj;
		}

		/// <summary>
		/// returns a copy of the stack
		/// </summary>
		public MyArrayList GetCopyStack
		{
			get
			{
				MyArrayList copyArr = new MyArrayList();
				copyArr.AddArray(m_stk);
				return copyArr;
			}
		}
		/// <summary>
		/// Look at tho top of the Stack
		/// </summary>
		/// <returns>object from top of the stack</returns>
		public object Look()
		{
			return Look(0);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Look(int pos)
		{
			object obj = null;
			if(m_stkPos>0 && pos<m_stkPos && pos>=0)
			{
				obj = m_stk[m_stkPos-pos-1];
			}
			return obj;
		}
		/// <summary>
		/// Push
		/// </summary>
		/// <param name="obj">object to stack</param>
		public void Push(object obj)
		{
			m_stk.Add(obj);
			m_stkPos++;
		}
	}
}

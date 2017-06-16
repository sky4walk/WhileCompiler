//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for MyArrayList.
	/// </summary>
	public class MyArrayList
	{
		#region Klasse myArrayElement
		private class MyArrayElement
		{
			private MyArrayElement m_Next = null;
			private MyArrayElement m_Before = null;
			private object m_Content = null;

			internal object Content
			{
				get{return m_Content;}
				set{m_Content = value;}
			}
			internal MyArrayElement Next
			{
				get{return m_Next;}
				set{m_Next = value;}
			}
			internal MyArrayElement Before
			{
				get{return m_Before;}
				set{m_Before = value;}
			}

			internal MyArrayElement(MyArrayElement before,MyArrayElement next,object Cont)
			{
				m_Next = next;
				m_Before = before;
				m_Content = Cont;
			}
		}
		#endregion

		private MyArrayElement m_StartElement = null;
		private MyArrayElement m_LastElement = null;
		private int m_Count = 0;

		public MyArrayList()
		{
		}

		public int Count
		{
			get{return m_Count;}
		}

		/// <summary>
		/// indexer
		/// </summary>
		public object this [int pos]
		{
			get
			{
				return this.Get(pos);
			}
			set
			{
				this.Set(pos,value);
			}
		}

		public void Clear()
		{
			m_StartElement = null;
			m_LastElement = null;
			m_Count = 0;
		}

		public void Add(object Cont)
		{
			MyArrayElement neuCont = new MyArrayElement(m_LastElement,null,Cont);
			if(m_StartElement==null)
			{
				m_StartElement = neuCont;
				m_LastElement = neuCont;
			}
			else
			{
				m_LastElement.Next = neuCont;
				m_LastElement = neuCont;
			}
			m_Count++;
		}

		public void AddArray(MyArrayList ArrCont)
		{
			if(ArrCont!=null)
			{
				for(int i=0;i<ArrCont.Count;i++)
				{
					Add(ArrCont[i]);
				}
			}
		}

		private MyArrayElement GetElementAt(int pos)
		{
			MyArrayElement posElement = null;
			if(pos>=0&&pos<m_Count)
			{
				posElement = m_StartElement;
				for(int i=0;i<pos;i++)
				{
					posElement = posElement.Next;
				}
			}
			return posElement;
		}

		public object Get(int pos)
		{
			object retObj = null;
			MyArrayElement posElm = GetElementAt(pos);
			if(posElm!=null)
			{
				retObj = posElm.Content;
			}
			return retObj;
		}

		public object Set(int pos,object Cont)
		{
			object retObj = null;
			MyArrayElement posElm = GetElementAt(pos);
			if(posElm!=null)
			{
				posElm.Content = Cont;
			}
			return retObj;
		}

		public void RemoveAt(int pos)
		{
			MyArrayElement posElm = GetElementAt(pos);
			if(posElm!=null)
			{
				MyArrayElement before = posElm.Before;
				MyArrayElement next = posElm.Next;

				if(before==null)
				{
					m_StartElement = next;
				}
				else
				{
					before.Next = next;
				}
				if(next==null)
				{
					m_LastElement = before;
				}
				else
				{
					next.Before = before;
				}
				m_Count--;
			}
		}

		public object[] ToArray(Type objTyp)
		{
			object[] objArr = null;
			if(m_Count>0)
			{
				objArr = new object[m_Count];
				int cnt = 0;
				MyArrayElement posElement = m_StartElement;
				while(posElement!=null&&cnt<m_Count)
				{
					if(posElement.Content!=null)
					{
						objArr[cnt] = posElement.Content;
						cnt++;
					}
				}
			}
			return objArr;
		}
	}
}

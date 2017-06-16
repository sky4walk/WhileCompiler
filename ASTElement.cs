//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for ASTElement.
	/// </summary>
	public class ASTElement
	{
		/// <summary>
		/// RootNode
		/// </summary>
		private ASTElement m_Root = null;
		/// <summary>
		/// Nodes
		/// </summary>
		private MyArrayList m_NodeList = new MyArrayList();
		/// <summary>
		/// Rulelement in tress
		/// </summary>
		private RuleElement m_re = null;
		/// <summary>
		/// Shows if someone has visited this node
		/// </summary>
		private bool m_Visited = false;
		/// <summary>
		/// Tree depth
		/// </summary>
		private int m_Depth = 0;
		/// <summary>
		/// returns Treedepth
		/// </summary>
		public int TreeDepth
		{
			get{ return m_Depth;}
		}
		/// <summary>
		/// property
		/// </summary>
		public RuleElement rlElement
		{
			get{return m_re;}
		}
		/// <summary>
		/// is true if someone visited this node
		/// </summary>
		public bool IsVisited
		{
			get{ return m_Visited ;}
			set{ m_Visited = value;}
		}
		/// <summary>
		/// returns the root node
		/// </summary>
		public ASTElement GetRootNode
		{
			get{ return m_Root;}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="re">RuleElement</param>
		public ASTElement(RuleElement re)
		{
			m_re = re;
			m_Root = null;
			m_Depth = 0;
		}
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="re">RuleElement</param>
		public ASTElement(RuleElement re,ASTElement root)
		{
			m_re = re;
			m_Root = root;
			if(root!=null)
				m_Depth = root.TreeDepth+1;
			else
				m_Depth = 0;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="re"></param>
		/// <param name="Nodes"></param>
		public ASTElement(RuleElement re,MyArrayList Nodes)
		{
			m_re = re;
			m_Root = null;
			m_NodeList = new MyArrayList();
			if(Nodes!=null)
			{
				for(int i=0;i<Nodes.Count;i++)
				{
					m_NodeList.Add(Nodes[i]);
				}
			}
		}
		/// <summary>
		/// Add a rulelement to the root
		/// </summary>
		/// <param name="re"></param>
		/// <returns>Added ASTElement</returns>
		public ASTElement AddNode(RuleElement re)
		{
			ASTElement next = new ASTElement(re,this);
			m_NodeList.Add(next);
			return next;
		}
		/// <summary>
		/// Get a node
		/// </summary>
		/// <param name="Nr">Node Number</param>
		/// <returns>ASTElement</returns>
		public ASTElement GetNextNode(int Nr)
		{
			if(Nr<m_NodeList.Count&&Nr>=0)
			{
				return (ASTElement)m_NodeList[Nr];
			}
			return null;
		}
		/// <summary>
		/// creates an XML Tree from this AST to view it with XML Viewer eg XMLSpy
		/// </summary>
		/// <param name="astRoot">ASTTree Root</param>
		/// <returns>XML Tree</returns>
		public static string Tree2XML(ASTElement astRoot,int Depth)
		{
			string XmlTree = "";
			if(astRoot!=null)
			{
				
				for(int i=0;i<Depth;i++)
					XmlTree += "\t";
				XmlTree += "<Node ";
				XmlTree += "Token=\"" +astRoot.rlElement.GetToken()+"\" ";
				XmlTree += "Terminal=\""+(astRoot.rlElement.IsTerminal()?"true":"false")+"\" ";
				XmlTree += "Depth=\""+Depth+"\"";
				XmlTree += ">\n";

				ASTElement tmpAst = null;
				int NodeNr = 0;
				do
				{
					tmpAst = astRoot.GetNextNode(NodeNr);
					XmlTree += Tree2XML(tmpAst,Depth+1);
					NodeNr++;
				}
				while(tmpAst!=null);

				

				for(int i=0;i<Depth;i++)
					XmlTree += "\t";
				XmlTree += "</Node>\n";
			}
			return XmlTree;
		}
	}
}

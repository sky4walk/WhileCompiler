// André Betz
// http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for wc.
	/// </summary>
	public class wc
	{
		private TopDownParser m_tdParser = null;
		private ButtomUpParser m_buParser = null;
		private Translate m_tr = new TestLALR1();
		/// <summary>
		/// constructor
		/// </summary>
		public wc()
		{
			if(m_tr.m_Filename.Length>0)
			{
				TextLoader BnfLd = new TextLoader(m_tr.m_Filename);
				m_tr.m_BNF = BnfLd.Load();
			}
			BNFReader bnfReader = new BNFReader(m_tr.m_BNF);

			if(bnfReader.Init())
			{
				if(m_tr.ParsingType == Translate.ParseType.LL1)
				{
					LL1ParseTable ParseTable = new LL1ParseTable(bnfReader.Rules,m_tr.m_StartSign);
					if(ParseTable.Init())
					{
						string Sets = bnfReader.Print();
						TextLoader SetsSv = new TextLoader("Mengen.txt");
						SetsSv.Save(Sets);

						TopDownParseTabelle ptbl = ParseTable.LL1ParseTabelle;
						if(ptbl!=null)
						{
							string strTbl = ptbl.Print();
							TextLoader StrSv = new TextLoader("Tabelle.csv");
							StrSv.Save(strTbl);

							m_tdParser = new TopDownParser(ptbl,bnfReader.GetStartRule(ParseTable.StartRule),ParseTable.EofTerminal);
						}
					}
				}
				else if(m_tr.ParsingType == Translate.ParseType.SLR1)
				{
					SLR1ParseTable ParseTable = new SLR1ParseTable(bnfReader.Rules,m_tr.m_StartSign);
					if(ParseTable.Init())
					{
						string huellen = bnfReader.Print()+"\n"+ParseTable.PrintHuellen()+"\n"+ParseTable.PrintGotoTable();
						TextLoader StrSv = new TextLoader("Huellen.txt");
						StrSv.Save(huellen);

						ButtomUpParseTabelle ptbl = ParseTable.SLR1Table;
						if(ptbl!=null)
						{
							string strTbl = ptbl.Print();
							TextLoader TblSv = new TextLoader("Tabelle.csv");
							TblSv.Save(strTbl);

							m_buParser = new ButtomUpParser(ParseTable);
						}
					}
				}
				else if(m_tr.ParsingType == Translate.ParseType.LR1)
				{
					LR1ParseTable ParseTable = new LR1ParseTable(bnfReader.Rules,m_tr.m_StartSign);
					if(ParseTable.Init())
					{
						string huellen = bnfReader.Print()+"\n"+ParseTable.PrintHuellen()+"\n"+ParseTable.PrintGotoTable();
						TextLoader StrSv = new TextLoader("Huellen.txt");
						StrSv.Save(huellen);

						ButtomUpParseTabelle ptbl = ParseTable.LR1Table;
						if(ptbl!=null)
						{
							string strTbl = ptbl.Print();
							TextLoader TblSv = new TextLoader("Tabelle.csv");
							TblSv.Save(strTbl);

							m_buParser = new ButtomUpParser(ParseTable);
						}
					}
				}
				else if(m_tr.ParsingType == Translate.ParseType.LALR1)
				{
					LALR1ParseTable ParseTable = new LALR1ParseTable(bnfReader.Rules,m_tr.m_StartSign);
					if(ParseTable.Init())
					{
						string huellen = bnfReader.Print()+"\n"+ParseTable.PrintHuellen()+"\n"+ParseTable.PrintGotoTable();
						TextLoader StrSv = new TextLoader("Huellen.txt");
						StrSv.Save(huellen);

						ButtomUpParseTabelle ptbl = ParseTable.LR1Table;
						if(ptbl!=null)
						{
							string strTbl = ptbl.Print();
							TextLoader TblSv = new TextLoader("Tabelle.csv");
							TblSv.Save(strTbl);

							m_buParser = new ButtomUpParser(ParseTable);
						}
					}
				}
			}
		}

		public bool Compile(string FileName)
		{
			TextLoader StrLd = new TextLoader(FileName);
			string Datas = StrLd.Load();

			if(m_tr.ParsingType == Translate.ParseType.LL1)
			{
				if(m_tdParser.Init())
				{
					ASTElement astElm = m_tdParser.Parse(Datas);
					string verlauf = m_tdParser.ParseVerlauf;

					TextLoader StrSv = new TextLoader("parse.txt");
					StrSv.Save(verlauf);

					string RLtree = ASTElement.Tree2XML(astElm,0);
					TextLoader StrSv2 = new TextLoader("parsetree.xml");
					StrSv2.Save(RLtree);

					m_tr.Convert(astElm);
				}
			}
			else if(m_tr.ParsingType == Translate.ParseType.SLR1 || m_tr.ParsingType == Translate.ParseType.LR1 || m_tr.ParsingType == Translate.ParseType.LALR1)
			{
				if(m_buParser.Init())
				{
					ASTElement astElm = m_buParser.Parse(Datas);

					string verlauf = ButtomUpParser.ParseVerlaufArray2String(m_buParser.ParseVerlaufListe);

					TextLoader StrSv1 = new TextLoader("parse.txt");
					StrSv1.Save(verlauf);

					string RLtree = ASTElement.Tree2XML(astElm,0);
					TextLoader StrSv2 = new TextLoader("parsetree.xml");
					StrSv2.Save(RLtree);
				}
			}
			
			return true;
		}
	}
}

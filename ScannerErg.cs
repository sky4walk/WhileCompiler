//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for ScannerErg.
	/// </summary>
	public class ScannerErg
	{
		string Word;
		int spos;
		int TermSignNr;
		public int getSpos() 
		{
			return spos;
		}
		public int getTermSignNr() 
		{
			return TermSignNr;
		}
		public string getWord() 
		{
			return Word;
		}
		public void setSpos(int i) 
		{
			spos = i;
		}
		public void setTermSignNr(int i) 
		{
			TermSignNr = i;
		}
		public void setWord(string str) 
		{
			Word = str;
		}
	}
}

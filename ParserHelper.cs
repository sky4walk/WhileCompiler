//written by André Betz 
//http://www.andrebetz.de
using System;

namespace WC
{
	/// <summary>
	/// Summary description for ParserHelper.
	/// </summary>
	class ParseHelper
	{
		public static int DoesContain(char Sign, char[] SignList)
		{
			int count = 0;
			while(count<SignList.Length)
			{
				if(Sign == SignList[count])
				{
					return count;	
				}
				count++;
			}
			return -1;
		}
	
		public static int DoesContain(string Val, string[] ValList)
		{
			int count = 0;
			while(count<ValList.Length)
			{
				if(Val.Equals(ValList[count]))
				{
					return count;	
				}
				count++;
			}
			return -1;
		}
	
		public static bool IsNumerical(char Sign)
		{
			char[] a = new char[]{'0','1','2','3','4','5','6','7','8','9'};

			if(DoesContain(Sign,a)>=0)
			{
				return true;
			}
			return false;	
		}
	
		public static bool IsAlpha (char Sign)
		{
			char[] a = new char[]{	 'a','b','c','d','e','f','g','h','i','j','k','l','m',
									 'n','o','p','q','r','s','t','u','v','w','x','y','z',
									 'A','B','C','D','E','F','G','H','I','J','K','L','M',
									 'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
									 '_'};

			if(DoesContain(Sign,a)>=0)
			{
				return true;
			}
			return false;						
		}

		public static bool IsAlphaNumeric (char Sign)
		{
			if(IsAlpha(Sign)||IsNumerical(Sign))
			{
				return true;	
			}
			return false;
		}
	
		public static int DelNoSigns(string Input, int spos, char[] NoSigns) 
		{
			int newpos = spos;
			if(NoSigns==null)
			{
				return spos;
			}
			int slen = Input.Length;
			if(newpos<slen) 
			{
				char sign = Input.ToCharArray()[newpos];
				while((DoesContain(sign,NoSigns)>=0)&&(newpos<slen)) 
				{
					newpos++;
					if(newpos<slen) 
					{
						sign = Input.ToCharArray()[newpos];
					}
				}
			}
			return newpos;
		}
		public static int DelComment(string Input, int spos) 
		{
			int newpos = spos;
			int slen = Input.Length;
			char sign = Input.ToCharArray()[newpos];
			while((sign!='\n')&&(newpos<slen)) 
			{
				newpos++;
				if(newpos<slen) 
				{
					sign = Input.ToCharArray()[newpos];
				}
			}
			return newpos;  		
		}

		public static int GetWord(string Data, int spos,char[] Endsigns, String[] EndWords,char[] DelSigns)
		{
			ScannerErg scnErg;
			int res = -1;
			spos = DelNoSigns(Data,spos,DelSigns);		
			for(int i=0;i<EndWords.Length;i++)
			{
				scnErg = GetWord(Data,spos,Endsigns,null);
				if(EndWords[i].ToUpper().Equals(scnErg.getWord()))
				{
					res = i;
					break;
				}
			}
			return res;
		}
		
		public static ScannerErg GetWord(string Data, int spos, char[] Endsigns,char[] DelSigns)
		{
			char sign;
			char Change = '\\';
			ScannerErg scnErg = new ScannerErg();
			scnErg.setTermSignNr(-1);
			int endpos = Data.Length;
			String Word = "";
			bool NoEndNext = false;
			while(spos<endpos)
			{
				spos = DelNoSigns(Data,spos,DelSigns);
				sign = Data.ToCharArray()[spos];
				int nr = DoesContain(sign,Endsigns);
				if(NoEndNext==true)
				{
					NoEndNext = false;
				}
				else if(sign==Change)
				{
					NoEndNext = true;
				}
				if(nr>=0)
				{
					if(Endsigns[nr]==Change)
					{
						NoEndNext = false;
					}
					if(NoEndNext==false)
					{
						scnErg.setTermSignNr(nr);
						spos++;
						break;
					}
				}
				Word += sign; 		
				spos++;
			}
			scnErg.setSpos(spos);
			scnErg.setWord(Word);
			return scnErg;
		}
	
		// sucht nach str2 in einem grossen String-Array ab Position strPos1 
		public static int StringEqual(char[] str1, int strPos1, char[] str2,int len) 
		{
			int	k = 0;
			if(str1==null||str2==null)
			{
				return 0;
			}
			if(str1.Length<str2.Length)
			{
				return 0;
			}
			if(str2.Length<len)
			{
				return 0;
			}
			while(str2[k]!=len) 
			{
				if ((str1[strPos1+k] != str2[k]))
				{
					return 0;
				}
				k++;
			}
			return k;
		}
		public static int StringEqual(char[] str1, int strPos1, char[] str2) 
		{
			return StringEqual(str1,strPos1,str2,str2.Length);
		}
		public static int StringEqual(char[] str1, char[] str2) 
		{
			return StringEqual(str1,0,str2);
		}
	
		public static char GetChar(char[] Str,int Pos)
		{
			if(Str.Length<=Pos)
			{
				return (char)0;
			}
			return (char)(Str[Pos] & 127);
		}
		public static char GetNextChar(char[] Str,int Pos)
		{
			if(Str.Length<=(Pos+1))
			{
				return (char)0;
			}
			return (char)(Str[Pos+1] & 127);
		}

		public static MyArrayList ConcatList(MyArrayList Lst1,MyArrayList Lst2)
		{
			MyArrayList ConcatList = new MyArrayList();
			for(int i=0;i<Lst1.Count;i++)
			{
				ConcatList.Add(Lst1[i]);
			}
			for(int i=0;i<Lst2.Count;i++)
			{
				ConcatList.Add(Lst2[i]);
			}
			return ConcatList;
		}

		public static MyArrayList AddList(MyArrayList Lst1,MyArrayList Lst2)
		{
			MyArrayList Addlist = new MyArrayList();
			if(Lst1!=null)
			{
				for(int i=0;i<Lst1.Count;i++)
				{
					Addlist.Add(Lst1[i]);
				}
			}
			if(Lst2!=null)
			{
				for(int i=0;i<Lst2.Count;i++)
				{
					Addlist.Add(Lst2[i]);
				}
			}
			return Addlist;
		}

		public static bool IsTokenInTokenList(MyArrayList TokenLst,RuleElement Token)
		{
			if(TokenLst!=null)
			{
				for(int i=0;i<TokenLst.Count;i++)
				{
					RuleElement re = (RuleElement)TokenLst[i];
					if(Token.GetToken().Equals(re.GetToken()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsRuleInTokenList(MyArrayList RuleList,RuleElement re2)
		{
			if(RuleList!=null)
			{
				for(int i=0;i<RuleList.Count;i++)
				{
					RuleElement re1 = (RuleElement)RuleList[i];
					if(re1.GetToken().Equals(re2.GetToken()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static string PrintRules(MyArrayList RuleList)
		{
			string retStr = "{";
			if(RuleList!=null)
			{
				for(int i=0;i<RuleList.Count;i++)
				{
					RuleElement re = (RuleElement)RuleList[i];
					retStr += re.GetToken();
					if(i<RuleList.Count-1)
					{
						retStr += ",";
					}
				}
			}
			return retStr+"}";
		}

		public static MyArrayList UnionSet(MyArrayList Src1,MyArrayList Src2)
		{
			MyArrayList Union = null;
			Union = AddList(Union,Src1);

			if(Src2!=null)
			{
				for(int i=0;i<Src2.Count;i++)
				{
					RuleElement re1 = (RuleElement)Src2[i];
					if(!IsTokenInTokenList(Union,re1))
					{
						Union.Add(re1);
					}
				}
			}
			return Union;
		}

		public static bool SameRuleLists(MyArrayList List1,MyArrayList List2)
		{
			if(List1!=null && List2!=null)
			{
				if(List1.Count==List2.Count)
				{
					for(int i=0;i<List1.Count;i++)
					{
						RuleElement re1 = (RuleElement)List1[i];
						if(!IsRuleInTokenList(List2,re1))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}

//written by André Betz 
//http://www.andrebetz.de
using System;
using System.IO;

namespace WC
{
	/// <summary>
	/// Summary description for TextLoader.
	/// </summary>
	public class TextLoader
	{
		private string m_FileName = null;

		public TextLoader(string Filename)
		{
			m_FileName = Filename;
		}

		public string Load()
		{
			string FileData = "";
			StreamReader sr = null;


			try
			{
				if(!File.Exists(m_FileName))
					return FileData;

				FileStream fs = new FileStream(m_FileName,FileMode.Open,FileAccess.Read,FileShare.Read);
				sr = new StreamReader(fs);
				FileData = sr.ReadToEnd();
			}
			catch(Exception)
			{
				return FileData;
			}
			finally
			{
				if(sr!=null)
					sr.Close();
			}

			return FileData;
		}

		public bool Save(string FileData)
		{
			StreamWriter sw = null;

			try
			{
				if(File.Exists(m_FileName))
					File.Delete(m_FileName);
			}
			catch
			{
			}

			try
			{
				FileStream fs = new FileStream(m_FileName,FileMode.Create,FileAccess.Write,FileShare.Read);
				sw = new StreamWriter(fs);
				sw.Write(FileData);
			}
			catch(Exception)
			{
				return false;
			}
			finally
			{
				if(sw!=null)
					sw.Close();
			}

			return true;
		}
	}
}

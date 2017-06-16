// André Betz
// http://www.andrebetz.de
using System;
using System.Windows.Forms;

namespace WC
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class wcGui : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button Compile;
		private System.Windows.Forms.Button Help;
		private System.Windows.Forms.HelpProvider helpProvider1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// CompilerInstance
		/// </summary>
		private wc compiler = new wc();
		/// <summary>
		/// constructor
		/// </summary>
		public wcGui()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Compile = new System.Windows.Forms.Button();
			this.Help = new System.Windows.Forms.Button();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.SuspendLayout();
			// 
			// Compile
			// 
			this.Compile.Location = new System.Drawing.Point(24, 16);
			this.Compile.Name = "Compile";
			this.Compile.Size = new System.Drawing.Size(88, 23);
			this.Compile.TabIndex = 0;
			this.Compile.Text = "Compile";
			this.Compile.Click += new System.EventHandler(this.Compile_Click);
			// 
			// Help
			// 
			this.Help.Location = new System.Drawing.Point(144, 16);
			this.Help.Name = "Help";
			this.Help.Size = new System.Drawing.Size(80, 23);
			this.Help.TabIndex = 1;
			this.Help.Text = "Help";
			this.Help.Click += new System.EventHandler(this.Help_Click);
			// 
			// wcGui
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 53);
			this.Controls.Add(this.Help);
			this.Controls.Add(this.Compile);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "wcGui";
			this.Text = "WhileCompiler AndreBetz.de";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new wcGui());
	}

		private void Compile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog opfd = new OpenFileDialog();
			opfd.Filter = "While Script (*.wc)|*.wc" ;
			opfd.FilterIndex = 1 ;
			opfd.InitialDirectory = Application.StartupPath;
			opfd.RestoreDirectory = false ;
			opfd.Title = "Load While Script";
			DialogResult res = opfd.ShowDialog(this);
			if(res==DialogResult.OK)
			{
				compiler.Compile(opfd.FileName);
			}
		}
		/// <summary>
		/// help
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Help_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}

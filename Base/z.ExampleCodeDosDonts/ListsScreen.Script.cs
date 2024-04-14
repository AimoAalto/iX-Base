//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System.Windows.Forms;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using Neo.ApplicationFramework.Tools;
	using Neo.ApplicationFramework.Common.Graphics.Logic;
	using Neo.ApplicationFramework.Controls;
	using Neo.ApplicationFramework.Interfaces;
    
    
	public partial class ListsScreen
	{
		List<int> list = new List<int>();
		
		void Btn_AddToList_Click(System.Object sender, System.EventArgs e)
		{
			list.Add(list.Count+1);
		}
		
		void Btn_ReadList_Click(System.Object sender, System.EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			
			foreach (int i in list)
			{
		 		sb.AppendLine(i.ToString());
			}
			
			TextBox.Text = sb.ToString();
		}
		
		void Btn_RemoveFromList_Click(System.Object sender, System.EventArgs e)
		{
			// jos indeksi viittaa listan ulkopuolelle, aiheuttaa se Exceptionin
			// Unhandled Exception kaataa sovelluksen
			//list.RemoveAt(Globals.Tags.HMI_Index.Value);
			
			int index = Globals.Tags.HMI_Index.Value;
			// indeksi tulee tarkistaa että se on sallitulla alueella
			// list indeksi on 0-alkuinen, joten viimeinen indeksi on lukumäärä - 1
			if (list.Count == 0)
			{
				MessageBox.Show("lista on jo tyhjä");
			}
			else if (index > list.Count - 1)
			{
				MessageBox.Show("indeksi on liian suuri");
			}
			else if (index < 0)
			{
				MessageBox.Show("indeksi on liian pieni");
			}
			else
			{
				list.RemoveAt(index);
			}
		}
	}
}

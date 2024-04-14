//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Lavaus;
	using System.Windows.Forms;


	public partial class Template_Orfer
	{
		[DllImport("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private object lockme = new object();

		/// <summary>
		/// Käynnistää Windowsin Notepadin
		/// </summary>
		/// <param name="sender">Start_Notepad_btn</param>
		void Button_Notepad_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Process[] processes = Process.GetProcessesByName("notepad");

			if (processes.Length > 0)
			{
				SetForegroundWindow(processes[0].MainWindowHandle);
				ShowWindow(processes[0].MainWindowHandle, 9);
			}
			else
				Process.Start("notepad.exe", @"C:\Logbook.txt");
		}

		/// <summary>
		/// Käynnistää Windowsin laskimen.
		/// </summary>
		/// <param name="sender">Start_Calc_btn</param>
		void Button_Calc_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Process[] processes = Process.GetProcessesByName("calc");

			if (processes.Length > 0)
			{
				SetForegroundWindow(processes[0].MainWindowHandle);
				ShowWindow(processes[0].MainWindowHandle, 9);
			}
			else
				Process.Start("calc.exe");
		}

		void ChangeScreen(string btn_name)
		{
			try
			{
				string aux = "";

				// Erotetaan napin nimestä numero
				for (int i = 0; i < btn_name.Length; i++)
				{
					if (Char.IsDigit(btn_name[i]))
						aux += btn_name[i];
				}

				int num = Convert.ToInt16(aux);
				//if (Globals.Tags.Menu_MainMenu_Btn_Anim.Value.Int == num) return;

				// asetussivu on yhteinen
				int screenid;
				if (num == 7) screenid = (int)10000;
				else screenid = Globals.Tags.HMI_Settings_PanelNumber.Value * 10000;
				screenid += ((int)num * 100);
				screenid += 1;

				Globals.Tags.SystemTagNewScreenId.SetAnalog(screenid);

				Globals.Tags.ScreenChangePending.SetTag();
				Globals.Tags.Menu_MainMenu_Btn_Anim.SetAnalog(num);
				Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(1);
				Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(1);
			}
			catch (Exception x)
			{
				Globals.Tags.ScreenChangePending.ResetTag();
				Globals.Tags.Log(string.Format("ShowScreen button: {0}. Exception: {1}", btn_name, x.Message));
			}
		}
		
		/// <summary>
		/// Main menu button handler, get number part from button name and opens screen with calculated id
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Button_Menu_Click(System.Object sender, System.EventArgs e)
		{
			lock (lockme)
			{
				if (Globals.Tags.ScreenChangePending.Value.Bool == true)
				{
					Globals.Tags.Log("Screen change pending");
					return;
				}

				string btn_name = ((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name;
				if (Globals.Tags.TraceAll) Globals.Tags.Log(string.Format("ShowMainScreen button: {0}", btn_name));
				
				ChangeScreen(btn_name);
			}
		}

		/// <summary>
		/// Screen open
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Template_Orfer_Opened(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) Globals.Tags.Log(string.Format("Opened screen: {0}", Globals.Tags.SystemTagNewScreenId.Value));
			Globals.Tags.ScreenChangePending.ResetTag();
		}
		
		/// <summary>
		/// ButtonGroup visibility change down
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Button_Left_Click(System.Object sender, System.EventArgs e)
		{
			//Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(1);
			//ChangeScreen("1");
		}
		
		/// <summary>
		/// ButtonGroup visibility change up
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Button_Right_Click(System.Object sender, System.EventArgs e)
		{
			//Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(2);
			//ChangeScreen("8");
		}
	}
}

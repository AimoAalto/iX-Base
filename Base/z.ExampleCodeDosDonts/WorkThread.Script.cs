//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    //using System.Windows.Forms;
    using System;
    //using System.Drawing;
    //using Neo.ApplicationFramework.Tools;
    //using Neo.ApplicationFramework.Common.Graphics.Logic;
    //using Neo.ApplicationFramework.Controls;
    //using Neo.ApplicationFramework.Interfaces;
    

	public enum WorkThreadState
	{
		Stopped = 2,
		Running = 1,
		Aborted = 0
	}
	
	public partial class WorkThread
	{
		System.Threading.Thread localthread;
		int i = 0;

		public bool Loop { get; set; }
		public Action<WorkThreadState> State { get; set; }
		public Action IncCount { get; set; }

		public void Start()
		{
			try 
			{	        
				if (localthread == null)
				{
					localthread = new System.Threading.Thread(DoWork);
				}
				if (localthread.ThreadState == System.Threading.ThreadState.Running)
				{
					System.Windows.Forms.MessageBox.Show("Säie on jo käynnissä");
				}
				else
				{
					if (localthread.ThreadState == System.Threading.ThreadState.Suspended || localthread.ThreadState == System.Threading.ThreadState.Stopped)
					{
						localthread.Abort();
						bool bck = Loop;
						Loop = false;
						localthread.Join();
						localthread = null;
						localthread = new System.Threading.Thread(DoWork);
						Loop = bck;
					}

					localthread.Start();
				}
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}
		
		public void Abort()
		{
			if (localthread.ThreadState == System.Threading.ThreadState.Running)
			{
				System.Windows.Forms.MessageBox.Show("Säie ei ole käynnissä");
			}
			else
			{
				Loop = false;
				localthread.Abort();
				localthread.Join();
				localthread = null;
				if (State != null)
					State.Invoke(WorkThreadState.Aborted);
			}
		}

		void DoWork()
		{
			if (State != null) State.Invoke(WorkThreadState.Running);
			
			do
			{
				i++;
				if (i > 100)
				{
					i = 0;
					if (IncCount != null) IncCount.Invoke();
				}
				Globals.Tags.ExternalThreadMeterValue.Value = i;
				if (Loop) System.Threading.Thread.Sleep(250);
			}
			while (Loop);

			if (State != null) State.Invoke(WorkThreadState.Stopped);
		}
	}
}

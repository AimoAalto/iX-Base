//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System;
    /*using System.Windows.Forms;
    using System.Drawing;
    using Neo.ApplicationFramework.Tools;
    using Neo.ApplicationFramework.Common.Graphics.Logic;
    using Neo.ApplicationFramework.Controls;
    using Neo.ApplicationFramework.Interfaces;*/
	using IoT_TransferIx;
    
    public partial class IoT_Transfer
    {
		long NextRunTicks;
		long IntervalAsTicks;
		TimeSlices OEEData;
		RestService Tagquery;
		private Settings _settings;
		public Settings Settings { get { if (_settings == null) _settings = new Settings(); return _settings; } set { _settings = value; } }
		
		void IoT_Transfer_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("IoT_Transfer_Created (start)");
			
			Readsettings();

			TimeSpan ts = new DateTime().AddSeconds(1) - new DateTime();
			IntervalAsTicks = ts.Ticks;
			NextRunTicks = DateTime.Now.Ticks + IntervalAsTicks;

			OEEData = new TimeSlices(Settings);
			OEEData.TimerTickUpdate = TimerTickUpdate;
			OEEData.TimeSlicesUpdate = TimeSlicesUpdate;

			if (Settings.REST.Active)
				try
				{
					Tagquery = new RestService(Settings.REST.Port, TagQuery);
				}
				catch (System.Exception x)
				{
					//MessageBox.Show(x.Message);
					System.Diagnostics.Trace.WriteLine(x.Message);
				}
		}
		
		private void Readsettings()
		{
			try
			{
				Settings = (Settings)IoT_TransferIx.JsonUtils.ReadJson<Settings>(Settings.SettingsFileName);
			}
			catch (System.Exception x)
			{
				string s = string.Format("{0}\n{1}", x.Message, x.InnerException);
				//MessageBox.Show(s);
				System.Diagnostics.Trace.WriteLine(s);
			}
		}
		
		private void TagQuery(object sender, MIPAIoTEventArgs e) { }
		
		private void TimeSlicesUpdate(TimeSlice item)
		{
			if (item != null)
			{
				Globals.Tags.OEE_LastMsg.SetString(string.Format(
					"{0} : up: {1:00}, down: {2:00}, eff: {3:00}, ch: {5:00}",
					item.LogTime.ToString("yyyyMMdd HH:mm:ss"), (int)item.State.Uptime, (int)item.State.Downtime, (int)item.State.Efficient, (int)item.State.Errortime, (int)item.State.CheckSum));
				
				Globals.Tags.OEE_LastUptime.SetAnalog(OEEData.LastState.Uptime);
				Globals.Tags.OEE_LastDowntime.SetAnalog(OEEData.LastState.Downtime);
				//Globals.Tags.OEE_LastErrortime.SetAnalog(OEEData.LastState.Errortime);
				Globals.Tags.OEE_LastEfficient.SetAnalog(OEEData.LastState.Efficient);
				Globals.Tags.OEE_LastProduced1.SetAnalog(OEEData.LastCounters[0].Items);
				Globals.Tags.OEE_LastDefected1.SetAnalog(OEEData.LastCounters[0].Defected);
				Globals.Tags.OEE_LastProduced2.SetAnalog(OEEData.LastCounters[1].Items);
				Globals.Tags.OEE_LastDefected2.SetAnalog(OEEData.LastCounters[1].Defected);
				Globals.Tags.OEE_LastProduced3.SetAnalog(OEEData.LastCounters[2].Items);
				Globals.Tags.OEE_LastDefected3.SetAnalog(OEEData.LastCounters[2].Defected);
				Globals.Tags.OEE_LastProduced4.SetAnalog(OEEData.LastCounters[3].Items);
				Globals.Tags.OEE_LastDefected4.SetAnalog(OEEData.LastCounters[3].Defected);
				Globals.Tags.OEE_SendBufferCount.SetAnalog(OEEData.SendBufferCount);
			}
		}

		private void TimerTickUpdate()
		{
			Globals.Tags.OEE_NextSendCountDown.SetAnalog(OEEData.NextRunCountDown);

			if (DateTime.Now.Ticks >= NextRunTicks)
			{
				Globals.Tags.OEE_Connected.Value = OEEData.Connected;
				Globals.Tags.OEE_SendBufferCount.SetAnalog(OEEData.SendBufferCount);
				Globals.Tags.OEE_LastSend.SetString(OEEData.LastSend.ToString("dd.MM.yyyy HH:mm:ss"));
				NextRunTicks += IntervalAsTicks;
				OEEData.State.SetValues(
					Globals.Tags.OEE_Uptime.Value, 
					Globals.Tags.OEE_Downtime.Value, 
					0, //Globals.Tags.OEE_Errortime.Value, 
					Globals.Tags.OEE_Efficient.Value, 
					Globals.Tags.OEE_Checksum.Value);
				OEEData.Production.ItemId = Globals.Tags.OEE_ItemID.Value;
				OEEData.Production.ItemBatch = Globals.Tags.OEE_ItemBatch.Value;
				OEEData.Production.NominalSpeed = Globals.Tags.OEE_NominalSpeed.Value;
				OEEData.Production.MeasuredSpeed = Globals.Tags.OEE_MeasuredSpeed.Value;
				OEEData.Production.Capacity = Globals.Tags.OEE_Capacity.Value;
				OEEData.ProductionCounters[0].Items = Globals.Tags.OEE_Produced1.Value;
				OEEData.ProductionCounters[1].Items = Globals.Tags.OEE_Produced2.Value;
				OEEData.ProductionCounters[2].Items = Globals.Tags.OEE_Produced3.Value;
				OEEData.ProductionCounters[3].Items = Globals.Tags.OEE_Produced4.Value;
				OEEData.ProductionCounters[0].Defected = Globals.Tags.OEE_Defected1.Value;
				OEEData.ProductionCounters[1].Defected = Globals.Tags.OEE_Defected2.Value;
				OEEData.ProductionCounters[2].Defected = Globals.Tags.OEE_Defected3.Value;
				OEEData.ProductionCounters[3].Defected = Globals.Tags.OEE_Defected4.Value;
			}
		}
    }
}

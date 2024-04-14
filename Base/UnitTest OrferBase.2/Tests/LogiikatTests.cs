using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.ApplicationFramework.Generated.Tests
{
	[TestClass()]
	public class LogiikatTests
	{
		[TestMethod()]
		public void GenerateSSCCTest()
		{
			string s = new Logiikat(new Globals_()).GenerateSSCC("00101", "987654321");

			Assert.AreEqual("000101009876543213", s);
		}

		[TestMethod()]
		public void TarkistaTilaTest()
		{
			Logiikat l = new Logiikat(new Globals_());

			l.Globals.Tags.S7HMI_DB_ToHMI_WatchDog_1.Value = 0;
			l.Globals.Tags.S7HMI_DB_ToHMI_WatchDogOld_1.Value = 0;
			l.Globals.Tags.S7HMI_DB_ToPLC_WatchDog_1.Value = 0;

			l.TarkistaTila(1);

			Assert.AreEqual(false, (bool)l.Globals.Tags.HMI_CommFault_PLC1.Value);

			for (int i = 0; i < 7; i++) l.TarkistaTila(1);
			Assert.AreEqual(true, (bool)l.Globals.Tags.HMI_CommFault_PLC1.Value);

			l.Globals.Tags.S7HMI_DB_ToHMI_WatchDog_1.Value = 1;
			l.TarkistaTila(1);
			Assert.AreEqual(false, (bool)l.Globals.Tags.HMI_CommFault_PLC1.Value);
			Assert.AreEqual(1, (short)l.Globals.Tags.S7HMI_DB_ToHMI_WatchDogOld_1.Value);

			l.Globals.Tags.S7HMI_DB_ToHMI_WatchDog_1.Value = 2;
			l.TarkistaTila(1);
			Assert.AreEqual(false, (bool)l.Globals.Tags.HMI_CommFault_PLC1.Value);
			Assert.AreEqual(2, (short)l.Globals.Tags.S7HMI_DB_ToHMI_WatchDogOld_1.Value);

			for (int i = 0; i < 8; i++) l.TarkistaTila(1);
			Assert.AreEqual(true, (bool)l.Globals.Tags.HMI_CommFault_PLC1.Value);
		}
	}
}
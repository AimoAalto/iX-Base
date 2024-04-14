using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.ApplicationFramework.Generated.Tests
{
	[TestClass()]
	public class AjotiedotTests
	{
		private class Ajotiedot_ : Ajotiedot
		{
			public Ajotiedot_(Globals_ g) : base(g) { }
		}

		[TestMethod()]
		public void Line1_PLC_AloitettuX_ValueChangeTest()
		{
			Ajotiedot_ at = new Ajotiedot_(new Globals_());
			at.Globals.Tags.AppStart_Timer = 21;

			at.Globals.Tags.HMI_ProductionStarting.Value = 1;
			at.Line1_PLC_AloitettuX_ValueChange(at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdStartOK, null);
			Assert.AreEqual(1, (int)at.Globals.Tags.HMI_ProductionStarting.Value, "StartTest - StartOk = 0");

			at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdStartOK.Value = 1;
			at.Line1_PLC_AloitettuX_ValueChange(at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdStartOK, null);
			Assert.AreEqual(0, (int)at.Globals.Tags.HMI_ProductionStarting.Value, "StartTest");
		}

		[TestMethod()]
		public void Line1_PLC_LopetettuX_ValueChangeTest()
		{
			Ajotiedot_ at = new Ajotiedot_(new Globals_());
			at.Globals.Tags.AppStart_Timer = 21;

			at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdEndOK.SetTag();
			at.Globals.Tags.S7HMI_ToPLC_Line_1_CommBits_ProdEnd.SetAnalog(1);
			at.Line1_PLC_LopetettuX_ValueChange(at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdEndOK, null);

			Assert.AreEqual(0, (int)at.Globals.Tags.S7HMI_ToPLC_Line_1_CommBits_ProdStart.Value, "EndTest");
			Assert.AreEqual(0, (int)at.Globals.Tags.S7HMI_ToPLC_Line_1_CommBits_ProdEnd.Value, "EndTest");
		}

		[TestMethod()]
		public void HaeLavapaikanTuoteTest()
		{
			Ajotiedot_ at = new Ajotiedot_(new Globals_());

			at.Globals.Tags.HMI_TraceAll.SetTag();

			at.Globals._Konfiguraatio.CurrentConfig.Tuloradat.Add(1, 1);
			at.Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Add(1, 1);

			at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdStartOK.SetTag();
			at.Globals.Tags.Line1_PLC_Lavapaikka_TK1.SetAnalog(1);
			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(1);
			at.Globals.Tags.Line1_PLC_PalletPlaces1.SetAnalog(1);
			at.Globals.Tags.Line1_PLC_PalletPlaces1.Values = new Interfaces.VariantValue[] { "", "1" };

			string s = at.HaeLavapaikanTuote(1);

			Assert.AreEqual("1 - K1-K4 EUR Limppu 6x2 (500g)", s, "HaeLavapaikanTuote(1) [1]");

			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(20);

			s = at.HaeLavapaikanTuote(1);

			Assert.AreEqual("2 - K1-K4 QP (vaaka-pysty)(500g)", s, "HaeLavapaikanTuote(1) [20]");

			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(2);

			s = at.HaeLavapaikanTuote(1);

			Assert.AreEqual("0 - Unknown", s, "HaeLavapaikanTuote(1) [2]");

			//Assert.Fail("under Construction");
		}

		[TestMethod()]
		public void HaeTuloradanTuoteTest()
		{
			Ajotiedot_ at = new Ajotiedot_(new Globals_());

			at.Globals.Tags.HMI_TraceAll.SetTag();

			at.Globals._Konfiguraatio.CurrentConfig.Tuloradat.Add(1, 1);
			at.Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Add(1, 1);

			at.Globals.Tags.S7HMI_ToHMI_Line_1_CommBits_ProdStartOK.SetTag();
			at.Globals.Tags.Line1_PLC_Lavapaikka_TK1.SetAnalog(1);
			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(1);
			at.Globals.Tags.Line1_PLC_PalletPlaces1.SetAnalog(1);
			at.Globals.Tags.Line1_PLC_PalletPlaces1.Values = new Interfaces.VariantValue[] { "", "1" };

			string s = at.HaeTuloradanTuote(1);

			Assert.AreEqual("1 - K1-K4 EUR Limppu 6x2 (500g)", s, "HaeTuloradanTuote(1) [1]");

			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(20);

			s = at.HaeTuloradanTuote(1);

			Assert.AreEqual("2 - K1-K4 QP (vaaka-pysty)(500g)", s, "HaeTuloradanTuote(1) [20]");

			at.Globals.Tags.Line1_Rivinumero_TK1.SetAnalog(2);

			s = at.HaeTuloradanTuote(1);

			Assert.AreEqual("0 - Unknown", s, "HaeTuloradanTuote(1) [2]");

			//Assert.Fail("under Construction");
		}

		[TestMethod()]
		public void UpdateActiveDevicesTest()
		{
			//Assert.Fail();
		}
	}
}
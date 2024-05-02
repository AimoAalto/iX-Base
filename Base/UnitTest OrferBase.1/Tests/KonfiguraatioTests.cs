using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Neo.ApplicationFramework.Generated.Tests
{
	[TestClass()]
	public class KonfiguraatioTests
	{
		const string conf_filename = @"d:\tests\test_config.json";

		_Konfiguraatio CreateTestConfig()
		{
			// todo refacor
			Globals_ g = new Globals_();
			_Konfiguraatio test_conf = new _Konfiguraatio(g)
			{
				CurrentConfig = new Configuration__()
				{
					Aikavalit = new System.Collections.Generic.Dictionary<string, int>() { { "Time001", 1001 }, { "Time002", 2002 } },
					AllowedPatterns = new System.Collections.Generic.Dictionary<int, PatternInfo>() {
						{ 1, new PatternInfo() { 
							Lavapaikat = new System.Collections.Generic.List<int>() { 11,12,13,14 }, 
							Lavatyypit = new List<int> { 1 },
							Tuloradat = new List<int> { 11, 12, 13, 14, 21, 22, 23, 24 }
						} },
						{ 2, new PatternInfo() { 
							Lavapaikat = new System.Collections.Generic.List<int>() { 1 } 
						} }
					},
					Lavapaikat = new System.Collections.Generic.Dictionary<int, int>() { { 11, 11 }, { 12, 12 } },
					Lavatyypit = new System.Collections.Generic.Dictionary<int, string>() { { 1, "1" }, { 2, "2" } },
					Tuloradat = new System.Collections.Generic.Dictionary<int, int>() { { 1, 1 }, { 2, 2 } },
					NumberOfPLC = 1,
					PanelNo = 1,
					Robots = new System.Collections.Generic.Dictionary<int, RobotConf>() {
						{ 1, new RobotConf(1, 10, 0) { Lavapaikat = new List<int>() { 11, 12, 13, 14 }, Tuloradat = new List<int>{ 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } } },
						{ 2, new RobotConf(1, 1, 1) }
					}
				},
				ReadOk = true
			};
			return test_conf;
		}

		[TestMethod()]
		public void KonfiguraatioTest()
		{
			_Konfiguraatio conf = CreateTestConfig();
			conf.CurrentConfigFileName = conf_filename;

            conf.CurrentConfig.UseLocalUserCredentials = false;
            DefaultTests(conf, false);
		}

		[TestMethod()]
		public void ReadTest()
		{
			_Konfiguraatio conf = CreateTestConfig();
			conf.CurrentConfigFileName = conf_filename;

			conf.Save();
			conf.Read();

			Assert.AreEqual(true, conf.ReadOk, "ReadOk");

			conf.CurrentConfig.UseLocalUserCredentials = true;

			DefaultTests(conf, true);
		}

		private void DefaultTests(_Konfiguraatio conf, bool cred)
		{

			int time = conf.CurrentConfig.Aikavali("Time001");
			Assert.AreEqual(1001, time, $"Aikaväli 'Time001'");

			time = conf.CurrentConfig.Aikavali("Time000");
			Assert.AreEqual(1000, time, "Aikaväli 'Time000'");

			List<int> l = conf.CurrentConfig.AllowedInfeedTracks(1);
			Assert.AreEqual(10, l.Count, $"AllowedInfeedTracks");

			l = conf.CurrentConfig.AllowedPalletPlaces(1);
			Assert.AreEqual(4, l.Count, $"AllowedPalletPlaces");

			l = conf.CurrentConfig.AllowedPatternInfeedTracks(1);
			Assert.AreEqual(8, l.Count, $"AllowedPatternInfeedTracks");

			l = conf.CurrentConfig.AllowedPatternNumbers();
			Assert.AreEqual(2, l.Count, $"AllowedPatternNumbers");

			l = conf.CurrentConfig.AllowedPatternPalletPlaces(1);
			Assert.AreEqual(4, l.Count, $"AllowedPatternPalletPlaces");

			l = conf.CurrentConfig.AllowedPatternPalletTypes(1);
			Assert.AreEqual(1, l.Count, $"AllowedPatternPalletTypes");

			bool b = conf.CurrentConfig.GetRobotinLavapaikka(11, out int rno, out int lp);
			Assert.AreEqual(true, b, $"GetRobotinLavapaikka : UnKnown Pallet place");
			Assert.AreEqual(1, rno, $"GetRobotinLavapaikka : Robot");
			Assert.AreEqual(11, lp, $"GetRobotinLavapaikka : Pallet place");

			b = conf.CurrentConfig.GetRobotinTulorata(1, out rno, out int rtrno);
			Assert.AreEqual(true, b, $"GetRobotinLavapaikka : UnKnown Infeed track");
			Assert.AreEqual(1, rno, $"GetRobotinTulorata : Robot");
			Assert.AreEqual(1, rtrno, $"GetRobotinTulorata : Infeed track");

			Assert.AreEqual(1, conf.CurrentConfig.GetRobotNoByIndex(1), "");
			Assert.AreEqual(true, conf.CurrentConfig.IsAllowedInfeedTrack(1, 11), "IsAllowedInfeedTrack(1,11)");
			Assert.AreEqual(true, conf.CurrentConfig.IsAllowedPalletPlace(1, 11), "IsAllowedPalletPlace(1,11)");
			Assert.AreEqual(true, conf.CurrentConfig.IsAllowedPatternInfeedTrack(1, 11), "IsAllowedPatternInfeedTrack(1,11)");
			Assert.AreEqual(true, conf.CurrentConfig.IsAllowedPatternPalletPlace(1, 11), "IsAllowedPatternPalletPlace(1,11)");
			Assert.AreEqual(true, conf.CurrentConfig.IsAllowedPatternPalletType(1, 1), "IsAllowedPatternPalletType(1,1)");

			l = conf.CurrentConfig.PatternAllowedPalletPlaces(1, 1);
			Assert.AreEqual(4, l.Count, $"GetRobotinLavapaikka");

			//TODO:: Assert.AreEqual(true, conf.CurrentConfig.PatternIsAllowedAnyInfeedTrack_PalletPlace(11, 11));

			l = conf.CurrentConfig.PatternsForInfeed(11, 11);
			Assert.AreEqual(1, l.Count, $"PatternsForInfeed_Palletplace");

			/*conf.CurrentConfig.AddPattern();
			conf.CurrentConfig.AddPatternInfeedTrack();
			conf.CurrentConfig.AddPatternPalletPlace();
			conf.CurrentConfig.AddPatternPalletType();
			conf.CurrentConfig.AddRobot();
			conf.CurrentConfig.AddRobotInfeedTrack();
			conf.CurrentConfig.AddRobotPalletPlace();

			conf.CurrentConfig.RemoveInfeedTrack();
			conf.CurrentConfig.RemovePalletPlace();
			conf.CurrentConfig.RemovePalletType();
			conf.CurrentConfig.RemovePattern();
			conf.CurrentConfig.RemovePatternInfeedTrack(); ;
			conf.CurrentConfig.RemovePatternPalletPlace();
			conf.CurrentConfig.RemovePatternPalletType();
			conf.CurrentConfig.RemoveRobot();
			conf.CurrentConfig.RemoveRobotInfeedTrack();
			conf.CurrentConfig.RemoveRobotPalletPlace();
			*/

            Assert.AreEqual(cred, conf.CurrentConfig.UseLocalUserCredentials, "UseLocalUserCredentials");

			//Assert.Fail("Under Construction");
		}
	}
}
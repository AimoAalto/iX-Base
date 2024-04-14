using Core.Api.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.ApplicationFramework.Interfaces;
using Neo.ApplicationFramework.Interfaces.Tag;

namespace Neo.ApplicationFramework.Generated.Tests
{
	[TestClass()]
	public class TagsTests
	{
		const string conf_filename = @"d:\tests\test_config.json";

		Tags CreateTestConfig()
		{
			Tags tags = new Tags(new Globals_());
			tags.Globals._Konfiguraatio.CurrentConfigFileName = conf_filename;

			return tags;
		}

		[TestMethod()]
		public void GetTagTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			IBasicTag t = tags.GetTag("");
			Assert.AreEqual(null, t, string.Format("GetTag : [UnKnown]", tags.HMI_Aloitus_kesken.Value));

			t = tags.GetTag("HMI_Aloitus_kesken");
			Assert.AreEqual("HMI_Aloitus_kesken", t.Name, string.Format("GetTag : [{0}]", tags.HMI_Aloitus_kesken.Name));
		}

		[TestMethod()]
		public void GetTagValueIntTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			tags.GetTagValueInt("");

			tags.HMI_Aloitus_kesken.ResetTag();
			int i = tags.GetTagValueInt("HMI_Aloitus_kesken");
			Assert.AreEqual(0, i, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.SetTag();
			i = tags.GetTagValueInt("HMI_Aloitus_kesken");
			Assert.AreEqual(1, i, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = 100;
			i = tags.GetTagValueInt("HMI_Aloitus_kesken");
			Assert.AreEqual(100, i, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));
		}

		[TestMethod()]
		public void GetTagValueStringTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			tags.GetTagValueString("");

			tags.HMI_Aloitus_kesken.ResetTag();
			string s = tags.GetTagValueString("HMI_Aloitus_kesken");
			Assert.AreEqual("", s, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.SetTag();
			s = tags.GetTagValueString("HMI_Aloitus_kesken");
			Assert.AreEqual("1", s, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = 100;
			s = tags.GetTagValueString("HMI_Aloitus_kesken");
			Assert.AreEqual("100", s, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = "Testing...";
			s = tags.GetTagValueString("HMI_Aloitus_kesken");
			Assert.AreEqual("Testing...", s, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));
		}

		[TestMethod()]
		public void GetTagValueTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			tags.GetTagValue("");

			tags.HMI_Aloitus_kesken.ResetTag();
			VariantValue o = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual("", o.String, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.SetTag();
			o = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(1, (int)o, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = 100.123;
			o = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual((double)100.123, (double)o, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = "Testing...";
			o = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual("Testing...", o.String, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));
		}

		[TestMethod()]
		public void GetTagValuesTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			tags.GetTagValue("");

			tags.HMI_Aloitus_kesken.ResetTag();
			bool b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(false, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.SetTag();
			b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(true, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = 0;
			b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(false, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = 1;
			b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(true, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = "False";
			b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(false, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.Value = "True";
			b = tags.GetTagValue("HMI_Aloitus_kesken");
			Assert.AreEqual(true, b, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));
		}

		[TestMethod()]
		public void SetTagValueTest()
		{
			Tags tags = CreateTestConfig();

			// just checking if crash
			tags.SetTagValue("UnKnownTagName", new VariantValue(2));

			tags.SetTagValue("Me_Debug", new VariantValue(100));
			Assert.AreEqual(100, (int)tags.Me_Debug.Value, string.Format("Me_Debug : [{0}]", tags.Me_Debug.Value));

			tags.SetTagValue("HMI_Aloitus_kesken", true);
			Assert.AreEqual(true, (bool)tags.HMI_Aloitus_kesken.Value, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.HMI_Aloitus_kesken.ResetTag();
			Assert.AreEqual(false, (bool)tags.HMI_Aloitus_kesken.Value, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));

			tags.SetTagValue("HMI_Aloitus_kesken", 1);
			Assert.AreEqual(true, (bool)tags.HMI_Aloitus_kesken.Value, string.Format("HMI_Aloitus_kesken : [{0}]", tags.HMI_Aloitus_kesken.Value));
		}

		[TestMethod()]
		public void BtnHandlerTest()
		{
			Tags tags = CreateTestConfig();

			tags.HMI_Settings_PanelNumber.SetAnalog(1);

			Assert.AreEqual(0, (int)tags.SystemTagNewScreenId.Value, "SystemTagNewScreenId");

			tags.BtnHandler(tags.HMI_Settings_PanelNumber.Value, Tags.Screens.Alarms, "", 3);
			Assert.AreEqual(0, (int)tags.SystemTagNewScreenId.Value, "SystemTagNewScreenId : " + tags.SystemTagNewScreenId.Value);

			tags.BtnHandler(tags.HMI_Settings_PanelNumber.Value, Tags.Screens.Alarms, "Isis_1", 0);
			Assert.AreEqual(0, (int)tags.SystemTagNewScreenId.Value, "SystemTagNewScreenId : " + tags.SystemTagNewScreenId.Value);

			tags.BtnHandler(tags.HMI_Settings_PanelNumber.Value, Tags.Screens.Alarms, "Isis_1", 5);
			Assert.AreEqual(10601, (int)tags.SystemTagNewScreenId.Value, "SystemTagNewScreenId : " + tags.SystemTagNewScreenId.Value);

			Assert.AreEqual(true, (bool)tags.ScreenChangePending.Value, "SystemTagNewScreenId");
			Assert.AreEqual(1, (int)tags.Menu_SubMenu_Btn_Anim.Value, "Menu_SubMenu_Btn_Anim");
		}

		[TestMethod()]
		public void StartProductionTest()
		{
			//Assert.Fail("Under Construction");
		}

		[TestMethod()]
		public void SystemTagCurrentUser_ValueChangeTest()
		{
			Tags tags = CreateTestConfig();

			tags.SystemTagCurrentUser.Value = "";
			tags.SystemTagCurrentUser_ValueChange(null, null);
			Assert.AreEqual(0, (int)tags.CurrentUserInt.Value, "NO user");
			Assert.AreEqual(false, (bool)tags.HMI_AdminUser.Value, "NO user - Admin user");

			tags.SystemTagCurrentUser.Value = "Administrator";
			tags.SystemTagCurrentUser_ValueChange(null, null);
			Assert.AreEqual(3, (int)tags.CurrentUserInt.Value, "Administrator");
			Assert.AreEqual(true, (bool)tags.HMI_AdminUser.Value, "Adm - Admin user");

			tags.SystemTagCurrentUser.Value = "Supervisor";
			tags.SystemTagCurrentUser_ValueChange(null, null);
			Assert.AreEqual(2, (int)tags.CurrentUserInt.Value, "Supervisor");
			Assert.AreEqual(true, (bool)tags.HMI_AdminUser.Value, "Super - Admin user");

			tags.SystemTagCurrentUser.Value = "Operator";
			tags.SystemTagCurrentUser_ValueChange(null, null);
			Assert.AreEqual(1, (int)tags.CurrentUserInt.Value, "Operator");
			Assert.AreEqual(false, (bool)tags.HMI_AdminUser.Value, "Oper - Admin user");
		}

		[TestMethod()]
		public void Line1_PLC_Auto_Area_Mode_ValueChangeTest()
		{
			Tags tags = CreateTestConfig();

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 0;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 0;
			tags.HMI_Manual_Area_Enabled_1.Value = 0;
			tags.HMI_Internal_ManualEnabled_1.Value = 0;
			tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value = 0;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1, new ValueChangedEventArgs(1));

			Assert.AreEqual(0, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 1;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 20;
			tags.HMI_Manual_Area_Enabled_1.Value = 1;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(1, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 1;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 50;
			tags.HMI_Manual_Area_Enabled_1.Value = 1;
			tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value = 3000;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(0, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(0, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 1;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 0;
			tags.HMI_Manual_Area_Enabled_1.Value = 1;
			tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value = 3000;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(0, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(0, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 1;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 20;
			tags.HMI_Manual_Area_Enabled_1.Value = 1;
			tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value = 3000;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(1, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(3000, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 1;
			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 20;
			tags.HMI_Manual_Area_Enabled_1.Value = 1;
			tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value = 3000;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(1, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(3000, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1.Value = 0;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(1, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(3000, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());

			tags.S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.Value = 0;
			tags.Line1_PLC_Auto_Area_Mode_ValueChange(tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_1, new ValueChangedEventArgs(1));
			Assert.AreEqual(0, (int)tags.HMI_Internal_ManualEnabled_1.Value, "Manual enabled : " + tags.HMI_Internal_ManualEnabled_1.Value.ToString());
			Assert.AreEqual(0, (int)tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value, "Manual ctrl code : " + tags.S7HMI_DB_ToPLC_ManualCtrl_1.Value.ToString());
		}

		[TestMethod()]
		public void HMI_Language_Current_ValueChangeTest()
		{
			Tags tags = CreateTestConfig();

			tags.HMI_Language_Current.Value = 1;

			tags.HMI_Language_Current_ValueChange(null, null);
		}
	}
}
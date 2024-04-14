using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.ApplicationFramework.Generated;

namespace UnitTest_OrferBase
{
	[TestClass]
	public class OrferBase_UnitTest
	{
		const string conf_filename = @"d:\tests\test_config.json";

		[TestMethod]
		public void TestMethod1()
		{
			_Konfiguraatio conf = new _Konfiguraatio(new Globals_())
			{
				CurrentConfigFileName = conf_filename
			};
			conf.Read();

			Assert.AreEqual("25", Lavaus.Kuvio.Version, "Lavaus.dll version");
			
			//Assert.Fail("Under Construction");
		}
	}
}

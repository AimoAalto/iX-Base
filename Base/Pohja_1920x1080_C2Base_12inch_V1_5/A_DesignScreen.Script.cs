//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{

	public partial class A_DesignScreen
	{
		
		void CBRob1_nostopaikka11_Click(System.Object sender, System.EventArgs e)
		{
			if (CBRob1_nostopaikka11.Checked)
				Globals.Tags.Rob1_nostopaikka.SetAnalog(11);
			else
				Globals.Tags.Rob1_nostopaikka.SetAnalog(0);
		}
		
		void CBRob2_nostopaikka11_Click(System.Object sender, System.EventArgs e)
		{
			if (CBRob2_nostopaikka11.Checked)
				Globals.Tags.Rob2_nostopaikka.SetAnalog(11);
			else
				Globals.Tags.Rob2_nostopaikka.SetAnalog(0);
		}
		
		void CBRob3_nostopaikka11_Click(System.Object sender, System.EventArgs e)
		{
			if (CBRob3_nostopaikka11.Checked)
				Globals.Tags.Rob3_nostopaikka.SetAnalog(11);
			else
				Globals.Tags.Rob3_nostopaikka.SetAnalog(0);
		}
	}
}

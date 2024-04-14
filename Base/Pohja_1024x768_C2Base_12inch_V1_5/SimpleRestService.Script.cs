//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;
	
	
	public partial class SimpleRestService
	{
		SimpleRestServer.RestService restserver = null;
		
		public void StartRestService(int port)
		{
			if (port == 0) port = 50201;
			restserver = new SimpleRestServer.RestService(port, TagQuery, Globals.Tags.Log);
		}
		
		public void TagQuery(object sender, SimpleRestServer.IoTEventArgs e)
		{
			try
			{
				switch (e.QueryType)
				{
					case SimpleRestServer.IotQueryType.Get:
						restserver.SetCurrentValue(Globals.Tags.GetTagValue(e.TagName).ToString());
						break;

					case SimpleRestServer.IotQueryType.Set:
						Globals.Tags.SetTagValue(e.TagName, e.TagValue);
						break;

					default:
						Globals.Tags.Log(string.Format("[{0}] - [{1}] - [{2}]", e.QueryType, e.TagName, e.TagValue));
						break;
				}
			}
			catch (Exception x)
			{
				Globals.Tags.Log(string.Format("RestTagQuery: {0}", x.Message));
			}
		}
	}
}

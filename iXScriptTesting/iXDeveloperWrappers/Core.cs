using Core.Api.DataSource;
using Neo.ApplicationFramework.Tools.OpcClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Neo.ApplicationFramework.Generated
{
#pragma warning disable IDE0060 // Remove unused parameter
	/// <summary>
	/// asetukset
	///  - robotin lavapakat, tulolinjat, kuviot jne.
	///  - ajatimien kiertoajat
	///  
	/// oletus tiedosto c:\lavaus\asetukset\conf.json
	/// </summary>
	public partial class _Konfiguraatio
	{
		public Globals_ Globals { get; set; }

		public _Konfiguraatio(Globals_ globals) { Globals = globals; }
	}

	/// <summary>
	/// tuotanto - tarkistusrutiinit
	/// ajossaolevat linjat
	/// Wipak 
	///  - lukijat
	///  - lavapaikat
	/// </summary>
	public partial class Ajotiedot
	{
		public Globals_ Globals { get; set; }

		public Ajotiedot(Globals_ globals) { Globals = globals; }
	}

	/// <summary>
	/// Binding string list
	/// </summary>
	public class BindingList : List<string> { }

	/// <summary>
	/// Tag type, event sender object
	/// </summary>
	public class DesignerItemBase : IDesignerItemBase { public string Name { get; set; } }

	/// <summary>
	/// Mockup for iX Globals class
	/// </summary>
	public partial class Globals_
	{
		public Popup_Error Popup_Error { get; internal set; }
		public Popup_Success Popup_Success { get; internal set; }
		public ScreenSelection ScreenSelection { get; internal set; }
		public Popup_StartProduction Popup_StartProduction { get; }
		public Popup_ProdStartError Popup_ProdStartError { get; }
		public Tuotetiedot Tuotetiedot { get; set; }
		public Valikkeet_DB Valikkeet_DB { get; set; }
		public Robotit Robotit { get; set; }
		public Neo.ApplicationFramework.Generated.Tags Tags { get; set; }
#pragma warning disable IDE1006 // Naming Styles
		public _Konfiguraatio _Konfiguraatio { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		public Logiikat Logiikat { get; set; }
		public Ajotiedot Ajotiedot { get; set; }

		public Globals_()
		{
			string path = @"D:\Tests\Database";

			_Konfiguraatio = new _Konfiguraatio(this);
			Tags = new Tags(this);
			Robotit = new Robotit(this);
			Logiikat = new Logiikat(this);
			Ajotiedot = new Ajotiedot(this);
			Tuotetiedot = new Tuotetiedot(this, path);
			Valikkeet_DB = new Valikkeet_DB(this, path);
			Popup_Error = new Popup_Error();
			Popup_Success = new Popup_Success();
		}
	}

	/// <summary>
	/// interface, event sender object
	/// </summary>
	public interface IDesignerItemBase { string Name { get; set; } }

	/// <summary>
	/// interface
	/// </summary>
	public interface IMessageItem { string Message { get; set; } }

	/// <summary>
	/// interface
	/// </summary>
	public interface IMessageGroup { }

	/// <summary>
	/// interface
	/// </summary>
	public interface IProtectableItem { }

	/// <summary>
	/// interface
	/// </summary>
	public interface IPublic { }

	/// <summary>
	/// interface
	/// </summary>
	public interface IProjectGuid { }

	/// <summary>
	/// interface PopupScreenAdapter
	/// </summary>
	public interface IScreenAdapter
	{
		void Show();
		event EventHandler<ValueChangedEventArgs> Closed;
	}

	/// <summary>
	/// Mockup Logiikat 
	/// handles Globals reference
	/// </summary>
	public partial class Logiikat
	{
		public Globals_ Globals { get; set; }

		public Logiikat(Globals_ globals)
		{
			Globals = globals;
			Globals.Logiikat = this;
		}
	}

	/// <summary>
	/// TextLibrary messages
	/// </summary>
	public class MessageGroup : IDesignerItemBase, IMessageGroup, IComponent, IDisposable, IProtectableItem, IPublic, IProjectGuid
	{
		public MessageGroup() { }
		public BindingList<IMessageItem> Messages { get; }

		public MessageLibrary MessageLibrary { get; set; }
		public string Name { get; set; }

		public ISite Site { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public event EventHandler Disposed;

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		protected void Dispose(bool disposing) { }

		protected IMessageItem GetMessageItem(double value) { return null; }

		protected bool IsSubItemValid(IComponent component) { return false; }
	}

	/// <summary>
	/// TextLibrary message item
	/// </summary>
	public class MessageItem : IMessageItem { public string Message { get; set; } }

	/// <summary>
	/// MessageLibrary
	/// 
	/// check for using real Library from iX project
	/// </summary>
	public class MessageLibrary { }

	/// <summary>
	/// Mockup
	/// </summary>
	public class Popup_ProdStartError
	{
		public void Show() { }
	}

	/// <summary>
	/// Mockup
	/// </summary>
	public class Popup_StartProduction
	{
		public void Close() { }
		public void Show() { }
	}

	/// <summary>
	/// Mockup
	/// </summary>
	public class Popup_Error : IScreenAdapter
	{
		public event EventHandler<ValueChangedEventArgs> Closed;

		public void Close() { Closed?.Invoke(this, new ValueChangedEventArgs(null)); }
		public void Show() { }
	}

	/// <summary>
	/// Mockup
	/// </summary>
	public class Popup_Success : IScreenAdapter
	{
		public event EventHandler<ValueChangedEventArgs> Closed;

		public void Close() { Closed?.Invoke(this, new ValueChangedEventArgs(null)); }
		public void Show() { }
	}

	/// <summary>
	/// Mockup Robotit 
	/// handles Globals reference
	/// </summary>
	public partial class Robotit
	{
		public Globals_ Globals { get; set; }

		public Robotit(Globals_ globals)
		{
			Globals = globals;
			Globals.Robotit = this;
		}
	}

	/// <summary>
	/// Mockup
	/// </summary>
	public class ScreenSelection
	{
		public void Show() { }
	}

	/// <summary>
	/// Tags class
	/// Globals reference
	/// placeholdes for iXDeveloper taglists
	/// </summary>
	public partial class Tags : ISupportInitialize
	{
		public string ConnectedImportFilePath { get; set; }
		public Globals_ Globals { get; set; }
		//public SortedDictionary<string, GlobalDataItem> GlobalDataItems { get; set; }
		//public SortedDictionary<string, LightweightTag> LightweightTags { get; set; }
		//public SortedDictionary<string, GlobalDataSubItem> GlobalDataSubItems { get; }
		public List<IGlobalDataItem> GlobalDataItems { get; set; }
		public List<LightweightTag> LightweightTags { get; set; }
		public List<GlobalDataSubItem> GlobalDataSubItems { get; }
		public List<PollGroup> PollGroups { get; set; }
		public Tags(Globals_ globals)
		{
			GlobalDataItems = new List<IGlobalDataItem>();
			LightweightTags = new List<LightweightTag>();
			GlobalDataSubItems = new List<GlobalDataSubItem>();
			PollGroups = new List<PollGroup>();
			Globals = globals;
			Globals.Tags = this;
#if !IXTEST_EXE
			this.InitializeComponent();
			this.ApplyLanguageInternal();
#endif
		}

		public void BeginInit() { }

		public void EndInit() { }
	}

	/// <summary>
	/// iXDeveloper texts
	/// </summary>
	public class TextLibrary
	{
		public MessageGroup Terms { get; set; }
	}

	/// <summary>
	/// Tuotetiedot DB mockup
	/// </summary>
	public partial class Tuotetiedot
	{
#if IXTEST_EXE
		public string DbPath { get; set; }
#endif

		public Globals_ Globals { get; set; }

		public Tuotetiedot(Globals_ globals, string path)
		{
			Globals = globals;
			DbPath = path;
		}
	}

	public partial class Valikkeet_DB
	{
#if IXTEST_EXE
		public string DbPath { get; set; }
#endif

		public Globals_ Globals { get; set; }

		public Valikkeet_DB(Globals_ globals, string path)
		{
			Globals = globals;
			DbPath = path;
		}
	}
#pragma warning restore IDE0060 // Remove unused parameter
}

/*
 Neo Wrapper namespaces and classes
 */

using Core.Api.DataSource;
using Neo.ApplicationFramework.Generated;
using Neo.ApplicationFramework.Interfaces;
using Neo.ApplicationFramework.Interfaces.Tag;
using System;
using System.Collections.Generic;

/// <summary>
/// accessrights enum
/// </summary>
namespace Core.Api.DataSource
{
	[Flags]
	public enum AccessRights
	{
		None = 0,
		Read = 1,
		Write = 16,
		ReadWrite = 17
	}
}

namespace Neo
{
	/// <summary>
	/// 
	/// </summary>
	namespace ApplicationFramework
	{
		namespace Common
		{
			namespace Designer
			{
			}
		}

		namespace Generated
		{
			public enum ErrorTexts
			{
				UnexpectedError = 0, // "Unexpected error occured.","On tapahtunut odottamaton virhe",,
				ChooseProduct = 1, // "Please choose the product to be started.","Valitse aloitettava tuote",,
				ChoosePalletPlaces = 2, // "Please choose the pallet place(s) to be started on.","Valitse aloitettavat lavapaikat",,
				PalletPlaceAlreadyOnPattern = 3, // "Selected pallet place already has a pattern.Please reset the existing pattern or contact Orfer oCare Customer service.","Valittu lavapaikka on jo valittu kuvioon",,
				PatternLoadFailed = 4, // "Pattern load failed.",,,
				PatternImageLoadFailed = 5, // "Pattern picture load failed.",,,
				RecipeLoadFailed = 6, // "Loading of recipes failed.",,,
				SendStartFailed = 7, // "Sending production start command to robot failed.",,,
				CheckStartConditions = 8, // "Please check the starting conditions.",,,
				NoPatternFile = 9, // "There is no pattern file for the selected pattern number in C:\Lavaus\Kuviot\",,,
				PatternAlreadyExist = 10, // "The selected pattern number already exists.",,,
				SelectAtleastOne = 11, // "Mark at least one choice from each area.",,,
				LayerOpenFailed = 12, // "Could not open the layer.",,,
				UsePositiveNumbers = 13, // "Input fields must be positive numbers.",,,
				SheetDataFormatError = 14, // "Please input the sheet data in format:<product number><quantity>",,,
				FillAllFields = 15, // "Please fill in all the fields.",,,
				PrintingFailed = 16, // "Printing failed.",,,
				IPAddrError = 17, // "The IP-address is not valid. Please input the address in format X.X.X.X where X is a number in 0-255.",,,
				LayerSendFailed = 18, // "Could not send new layer count to robot. Layer count was not a number.",,,
				SpeedSendFailed = 19, // "Could not send new speed and delay values to robot. All values were not numbers.",,,
				DbLoadFailed = 20, // "Loading from database failed.",,,
				CheckBoxOutOfRange = 21, // "There were not enough CheckBox-elements for a section.",,,
				PalletPlaceOutOfRange = 22, // "There are not enough PlaceBoxes to select each available pallet place.",,,
				UnknownTag = 23, // "Tag was not found:",,,
				ChoosePalletType = 24, // "Choose pallet type",,,
				NoPermission = 25, // "No permission",,,"No permission"
				NotAllowedPattern = 26,
				InfeedTrackAlreadyStarted = 27,
				PalletPlaceAlreadyStarted = 28,
				MixedPalletChooseOther = 29,
				NoImageFile = 30, //
				MixedPalletSameBox = 31,
				ProductionStartError = 32,
				NothingToDelete = 33,
				RobotIdAlreadyExist = 34,
				UnknownRobotId = 35,
				NoAllowedInfeedTracks = 36,
				Info = 37,
				UnknownPatternNumber
			};
		}

		namespace Interfaces
		{
			public interface ISupportMultiLanguage
			{
				void ApplyLanguage();
			}

			public interface ITagExposure { }

			/// <summary>
			/// 
			/// </summary>
			namespace Tag
			{
				public interface IBasicTag
				{
					string Name { get; set; }
					VariantValue Value { get; set; }
					Interop.DataSource.BEDATATYPE DataType { get; set; }
					VariantValue[] Values { get; }
					void ResetTag();
					void SetTag();
					int SetAnalog(int? value);
					string SetString(string value);

					event EventHandler<ValueChangedEventArgs> ValueChange;
				}
			}
		}

		namespace Interop
		{
			/// <summary>
			/// BEDATATYPE enum
			/// </summary>
			namespace DataSource
			{
				public enum BEDATATYPE
				{
					DT_DEFAULT = 0,
					DT_INTEGER2 = 2,
					DT_INTEGER4 = 3,
					DT_REAL4 = 4,
					DT_REAL8 = 5,
					DT_DATETIME = 7,
					DT_STRING = 8,
					DT_BOOLEAN = 11,
					DT_UINTEGER2 = 18,
					DT_UINTEGER4 = 19,
					DT_BIT = 32769
				}
			}
		}

		namespace Tools
		{
			namespace Actions { }

			namespace MultiLanguage
			{
#pragma warning disable IDE0060 // Remove unused parameter
				public class TranslationValuePair : object
				{
					public TranslationValuePair(object key, string defaultText) { }
					public object Key { get; }
					public string DefaultText { get; }
				}

				public interface IMultiLanguageServiceCF { }

				public class MultiLanguageResourceManager
				{
					public MultiLanguageResourceManager(Type type) { }
					public IMultiLanguageServiceCF MultiLanguageService { get; }
					public void ApplyTexts(object owner, string propertyName, params TranslationValuePair[] keys) { }
					public void ApplyTexts(object owner, string propertyName, params string[] keys) { }
					public string GetText(string key, string defaultValue) { return null; }
					public string GetText(uint textId, string defaultValue) { return null; }
					public List<string> GetTexts(params string[] keys) { return null; }
				}
#pragma warning restore IDE0060 // Remove unused parameter
			}

			namespace OpcClient
			{
#pragma warning disable IDE0060 // Remove unused parameter
				public interface IPollGroup { string Name { get; set; } }

				public class PollGroup : IPollGroup
				{
					public string Name { get; set; }
					public int Interval { get; set; }
				}

				//Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_STRING
				public enum BEDATATYPE { }

				public interface IGlobalDataItem
				{
					string Name { get; set; }
				}

				public class ITem : IBasicTag, IGlobalDataItem, IDesignerItemBase
				{
					public EventHandler ValueOn { get; set; }
					public string Name { get; set; }
					public Interop.DataSource.BEDATATYPE DataType { get; set; }

					public int SynchId { get; set; }

					//int ival;
					//string sval;
					//bool bval;
					VariantValue oval = new VariantValue(null, DataQuality.Bad);
					public VariantValue Value { get { return new VariantValue(oval.Value); } set { if (oval.Value is VariantValue) oval = (VariantValue)value; else oval.Value = value; } }
					public VariantValue[] Values { get; set; }
					public int SetAnalog(int? value) { oval.Value = value; return (int)oval.Value; }
					public string SetString(string value) { oval.Value = value; return (string)oval.Value; }
					public void ResetTag() { oval.Value = null; }
					public void SetTag() { oval.Value = 1; }
					public void IncrementAnalog(int val) { ((VariantValue)oval.Value).IncrementAnalog(val); }

					public List<GlobalDataSubItem> GlobalDataSubItems { get; }

					public event EventHandler<ValueChangedEventArgs> ValueChange;
					public event EventHandler<ValueChangedEventArgs> ValueChangeOrError;
					public ITem() { GlobalDataSubItems = new List<GlobalDataSubItem>(); }
				}

				public class GlobalController : IDisposable
				{
					public void Dispose() { }
					protected virtual void Dispose(bool disposing) { }
				}

				public class SystemDataItem : ITem
				{
					// SystemDataItem("Current User",
					// "SystemTagCurrentUser",
					// ((Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE)(Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_STRING)),
					// ((short)(100)), 0D, 1D, ((short)(0)), false, "Value Change", Core.Api.DataSource.AccessRights.Read,
					// "PollGroup500", false, false,
					// ((Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE)(Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_DEFAULT)),
					// "The username of the currently logged in user", ((short)(1)), "", "");
					public SystemDataItem(
						string shortdesc, string name, Interop.DataSource.BEDATATYPE datatype, short sh1, double dbl1, double dbl2, short sh2, bool bo1, string str1,
						AccessRights ar, string plgname, bool bo2, bool bo3,
						Interop.DataSource.BEDATATYPE plcdatatype, string desc, short sh3, string str4, string str5)
					{
						//Description = desc; // "Current User"
						Name = name;
						DataType = datatype;
					}
				}

				public class GlobalDataItem : ITem
				{
					// "Menu_MainMenu_Btn_Anim", ((Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE)(Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_DEFAULT)),
					// ((short)(1)), 0D, 1D, ((short)(0)), false, "Value Change",
					// ((Core.Api.DataSource.AccessRights)((Core.Api.DataSource.AccessRights.Read | Core.Api.DataSource.AccessRights.Write))), "PollGroup500",
					// false, false, ((Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE)(Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_DEFAULT)),
					// "Button outline color control", ((short)(1)), "", ""
					public GlobalDataItem(string name, Interop.DataSource.BEDATATYPE datatype, short sh1, double dbl1, double dbl2, short sh2, bool bo1, string str1,
						AccessRights ar, string plgname, bool bo2, bool bo3,
						Interop.DataSource.BEDATATYPE plcdatatype, string desc, short sh3, string str4, string str5)
					{
						//Description = description; // "Current User"
						Name = name;
						DataType = datatype;
					}
				}

				public class GlobalDataSubItem : ITem
				{
					//null, 0, new string[0], new string[0], true
					public GlobalDataSubItem(object o1, int i1, string[] s2, string[] o3, bool b1) { }
				}

				public class LightweightTag : ITem
				{
					// "HMI_Overview_Shadow", "", 533, ((Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE)(Neo.ApplicationFramework.Interop.DataSource.BEDATATYPE.DT_DEFAULT)),
					// false, "Overview ikkunan taustavarjon animointitagi", null
					public LightweightTag(string name, string shortdesc, int no, Interop.DataSource.BEDATATYPE datatype, bool bo1, string description, object o1)
					{
						Name = name;
						DataType = datatype;
					}
				}

				public class BasicTag : ITem, IBasicTag { }
#pragma warning restore IDE0060 // Restore unused parameter
			}
		}
	}
}

namespace Neo.ApplicationFramework.Generated
{
	using System.Collections.Generic;
	using System.Linq;
	using System.IO;
	
    
	/// <summary>
	/// Sisältää tietoja kuvion mahdollisista käyttökohteista. Kuvion numeron tulee
	/// täsmätä D:\Lavaus\Kuviot\-kansiossa olevaan Kuvio[numero].json-tiedostoon.
	/// <list type="bullet">
	/// <listheader>
	/// <description>Kuvion tiedot:</description>
	/// </listheader>
	/// <item>
	/// <description>Tuloradat, joille kuvio voidaan aloittaa</description>
	/// </item>
	/// <item>
	/// <description>Lavapaikat, joille kuvio voidaan aloittaa</description>
	/// </item>
	/// <item>
	/// <description>Lavatyypit, joille kuvio on tarkoitettu</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 29.6.2017</remarks>
	public class Kuviotiedot
	{
		/// <summary>
		/// Kuvion numero.
		/// </summary>
		public int numero;
		/// <summary>
		/// Kuviolle sallitut lavatyypit. Numerojen merkitykset on määritetty
		/// _Konfiguraatioon.
		/// </summary>
		public List<int> sallitutLavatyypit;
		/// <summary>
		/// Kuviolle sallitut tuloradat.
		/// </summary>
		public List<int> sallitutTuloradat;
		/// <summary>
		/// Kuviolle sallitut lavapaikat.
		/// </summary>
		public List<int> sallitutLavapaikat;

		public Kuviotiedot()
		{
			sallitutLavatyypit = new List<int>();
			sallitutTuloradat = new List<int>();
			sallitutLavapaikat = new List<int>();
		}

		/// <param name="numero">Kuvion numero</param>
		public Kuviotiedot(int numero)
		{
			this.numero = numero;
			sallitutLavatyypit = new List<int>();
			sallitutTuloradat = new List<int>();
			sallitutLavapaikat = new List<int>();
		}

		/// <summary>
		/// Lataa kuviolistan JSON-tiedostosta. Tiedoston polku on määritetty 
		/// _Kontiguraatioon.
		/// </summary>
		/// <returns>Ladattu kuviolista</returns>
		public static List<Kuviotiedot> LataaKuviot()
		{
			List<Kuviotiedot> kuviot;
			
			try 
			{	        
				kuviot = JsonSerialization.ReadFromJsonFile<List<Kuviotiedot>>(_Konfiguraatio.Kuviotietolista_Path);
				if (kuviot == null)
				{
					kuviot = LuoKuvioLista();
				}
			}
			catch
			{
				kuviot = LuoKuvioLista();
			}
			
			return kuviot;
		}
		
		/// <summary>
		/// Luo malli kuviolistan JSON-tiedostoon. 
		/// - Sallii kaikki tuoloradat ja lavatyypit
		/// </summary>
		/// <returns>Ladattu kuviolista</returns>
		public static List<Kuviotiedot> LuoKuvioLista()
		{
			// Tiedostoa ei ole vielä, jatketaan tyhjällä listalla
			List<Kuviotiedot> kuviot = new List<Kuviotiedot>();
			Kuviotiedot esim = new Kuviotiedot();
			esim.numero = 101;
			esim.sallitutLavapaikat.Add(1);
			esim.sallitutLavapaikat.Add(2);
			esim.sallitutLavapaikat.Add(3);
			esim.sallitutLavapaikat.Add(4);
			esim.sallitutTuloradat.Add(1);
			esim.sallitutTuloradat.Add(2);
			esim.sallitutTuloradat.Add(3);
			esim.sallitutTuloradat.Add(4);
			foreach(KeyValuePair<int, string> dict in Globals._Konfiguraatio.CurrentConfig.Lavatyypit)
			{
				esim.sallitutLavatyypit.Add(dict.Key);
			}
			kuviot.Add(esim);
	
				
			TallennaKuviot(kuviot);
			return kuviot;
		}

		/// <summary>
		/// Tallentaa kuviolistan JSON-tiedostoon. Tiedoston polku on määritetty
		/// _Konfigraatiossa.
		/// </summary>
		/// <param name="kuviot">Tallennettava kuviolista</param>
		public static void TallennaKuviot(List<Kuviotiedot> kuviot)
		{
			// Luo kansio jos ei olemassa
			if (!Directory.Exists(_Konfiguraatio.Kuviotietolista_Path))
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_Konfiguraatio.Kuviotietolista_Path));
			// Tallennetaan JSON-tiedostoon
			JsonSerialization.WriteToJsonFile<List<Kuviotiedot>>(_Konfiguraatio.Kuviotietolista_Path, kuviot.OrderBy(p => p.numero).ToList());
		}

		/// <summary>
		/// Näyttää vain kuvion numeron. Käytetään automaattisesti objektin esittämiseen
		/// esimerkiksi ListBox:ssa ja ComboBox:ssa.
		/// </summary>
		/// <returns>Kuvion numeron listaesityksiä varten.</returns>
		public override string ToString()
		{
			return numero.ToString();
		}
	}
	

}

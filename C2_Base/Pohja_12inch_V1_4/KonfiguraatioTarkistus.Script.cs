namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Configuration;
	using System.Collections.Generic;
	
    
	/// <summary>
	/// Sisältää tarkistuksen, että määritetty konfiguraatio täyttää tietyt 
	/// ehdot muutosvirheiden estämiseksi.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 5.4.2018</remarks>
    public partial class KonfiguraatioTarkistus
    {
		/// <summary>
		/// Tarkistaa konfiguraation sovelluksen käynnistyessä.
		/// </summary>
		/// <param name="sender">KonfiguraatioTarkistus</param>
		void KonfiguraatioTarkistus_Created(System.Object sender, System.EventArgs e)
		{
			// Tarkistetaan Konfiguraatio
			Konfiguraation_Tarkistus();
		}

		/// <summary>
		/// Tarkistaa Konfiguraation rajoitukset ja heittää exceptionin jos rajoitukset ei toteudu.
		/// Estää konfiguraatiovirheitä pääsemästä tuotantoon asti. Tulorata/Lavapaikka voi kuulua 
		/// vain yhdelle robotille. Robotin tulorata/lavapaikka saa esiintyä vain kerran.
		/// </summary>
		/// <exception cref="ConfigurationFaultException">Tulorata/lavapaikka löytyy robottien määrittelystä useamman kerran tai kuviolta puuttuu tulorata/lavapaikka.</exception>
		void Konfiguraation_Tarkistus()
		{
			#region - Tulorata voi kuulua vain yhdelle robotille
			foreach (Dictionary<int, int> tuloradat in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in tuloradat.Keys)
				{
					// Tarkistetaan, että lavapaikka löytyy konfiguraatiosta vain kerran
					int loytyi = 0;
					foreach (Dictionary<int, int> muutTuloradat in _Konfiguraatio.RobotinTuloradat.Values)
					{
						if (muutTuloradat.ContainsKey(tulorata))
						{
							loytyi++;
						}
										
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Tulorata " + tulorata + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinTuloradat");
						}
					}
				}
			}
			#endregion
							
			#region - Lavapaikka voi kuulua vain yhdelle robotille
			foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
			{
				foreach (int lavapaikka in lavapaikat.Keys)
				{
					// Tarkistetaan, että lavapaikka löytyy konfiguraatiosta vain kerran
					int loytyi = 0;
					foreach (Dictionary<int, int> muutLavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
					{
						if (muutLavapaikat.ContainsKey(lavapaikka))
						{
							loytyi++;
						}
										
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Lavapaikka " + lavapaikka + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinLavapaikat");
						}
					}
				}
			}
			#endregion
	
			#region - Robotin tulorata voi esiintyä vain kerran
			foreach (Dictionary<int, int> tuloradat in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in tuloradat.Values)
				{
					// Tarkistetaan, että tulorata löytyy robotilta vain kerran
					int loytyi = 0;
					foreach (int muuTulorata in tuloradat.Values)
					{
						if (muuTulorata == tulorata)
						{
							loytyi++;
						}
												
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Robotin tulorata " + tulorata + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinTuloradat");
						}
					}
				}
			}
			#endregion
	
			#region - Robotin lavapaikka voi esiintyä vain kerran
			foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
			{
				foreach (int lavapaikka in lavapaikat.Values)
				{
					// Tarkistetaan, että lavapaikka löytyy robotilta vain kerran
					int loytyi = 0;
					foreach (int muuLavapaikka in lavapaikat.Values)
					{
						if (muuLavapaikka == lavapaikka)
						{
							loytyi++;
						}
														
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Robotin lavapaikka " + lavapaikka + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinLavapaikat");
						}
					}
				}
			}
			#endregion
		}
	}

	[Serializable]
	/// <summary>
	/// Kertoo, että sovelluksen konfiguraatiossa _Konfiguraatio-scriptissä on jotain vikaa. 
	/// </summary>
	public class ConfigurationFaultException : ArgumentException
	{
		public ConfigurationFaultException(string message, string field) : base(message, field) {}
		public ConfigurationFaultException(string message, string parameter, Exception innerException) : base(message, parameter, innerException) {}
	}
}
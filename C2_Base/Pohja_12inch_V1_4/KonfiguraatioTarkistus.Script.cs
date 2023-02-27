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
			foreach (Neo.ApplicationFramework.Generated.RobotConf robot in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
			{
				#region - Tulorata voi kuulua vain yhdelle robotille
				foreach (int tulorata in robot.Tuloradat)
				{
					// Tarkistetaan, että lavapaikka löytyy konfiguraatiosta vain kerran
					int loytyi = 0;
					foreach (Neo.ApplicationFramework.Generated.RobotConf robotcheck in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
					{
						if (robotcheck.Tuloradat.Contains(tulorata))
						{
							loytyi++;
						}
						
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Tulorata " + tulorata + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinTuloradat");
						}
					}
				}
				#endregion
				
				#region - Lavapaikka voi kuulua vain yhdelle robotille
				foreach (int lavapaikka in robot.Lavapaikat)
				{
					// Tarkistetaan, että lavapaikka löytyy konfiguraatiosta vain kerran
					int loytyi = 0;
					foreach (Neo.ApplicationFramework.Generated.RobotConf robotcheck in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
					{
						if (robotcheck.Lavapaikat.Contains(lavapaikka))
						{
							loytyi++;
						}
						
						if (loytyi > 1)
						{
							throw new ConfigurationFaultException("Lavapaikka " + lavapaikka + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinLavapaikat");
						}
					}
				}
				#endregion
			}
			#region - Robotin tulorata voi esiintyä vain kerran
			foreach (int rtulorata in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Values)
			{
				// Tarkistetaan, että lavapaikka löytyy tulorata vain kerran
				int loytyi = 0;
				foreach (int no in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Values)
				{
					if (rtulorata == no)
					{
						loytyi++;
					}
						
					if (loytyi > 1)
					{
						throw new ConfigurationFaultException("Robotin tulorata " + rtulorata + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinTuloradat");
					}
				}
			}
			#endregion
				
			#region - Robotin lavapaikka voi esiintyä vain kerran
			foreach (int rlavapaikka in Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Values)
			{
				// Tarkistetaan, että lavapaikka löytyy robotilta vain kerran
				int loytyi = 0;
				foreach (int no in Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Values)
				{
					if (rlavapaikka == no)
					{
						loytyi++;
					}
						
					if (loytyi > 1)
					{
						throw new ConfigurationFaultException("Robotin lavapaikka " + rlavapaikka + " löytyy _Konfiguraatiosta useamman kerran.", "_Konfiguraatio.robotinLavapaikat");
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

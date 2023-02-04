//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System.Windows.Forms;
    using System;
    using System.Drawing;
    using Neo.ApplicationFramework.Tools;
    using Neo.ApplicationFramework.Common.Graphics.Logic;
    using Neo.ApplicationFramework.Controls;
    using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using Neo.ApplicationFramework.Generated;
	using Neo.ApplicationFramework.Controls.Script;

	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Media;

    
    
    public class Kasiajot
    {
		private Neo.ApplicationFramework.Controls.Screen.ScreenWindow sivu;
		private string ryhma_nimi;
		private IBasicTag Manual_Ctrl_Nr;
		private string Manual_Ctrl_Nr_str;
		private bool eka = true;
		

		/// <summary>
		/// Poistaa kaikki valinnat
		/// </summary>
		public void Init(Neo.ApplicationFramework.Controls.Screen.ScreenWindow _sivu, string _ryhma_nimi, IBasicTag _Manual_Ctrl_Nr)
		{
			sivu = _sivu;
			ryhma_nimi = _ryhma_nimi;
			Manual_Ctrl_Nr = _Manual_Ctrl_Nr;
		}
		
		
		/// <summary>
		/// Poistaa kaikki valinnat
		/// </summary>
		public void ManualResetButtons()
		{
			List<FrameworkElement> anturit = HaeElementit(ryhma_nimi);
			
			//String kasiajot = "";
			foreach (FrameworkElement nappi in anturit)
			{
				try
				{
					Neo.ApplicationFramework.Controls.Group elementCanvas = (Neo.ApplicationFramework.Controls.Group)nappi;
    
					// ElementCanvasin alla on näkyvät elementit
					for(int i=0; i<elementCanvas.Items.Count; i++)
					{
						
						try
						{
							// Hide all arrow images in first round
							Neo.ApplicationFramework.Controls.Symbol.Picture lapsi = (Neo.ApplicationFramework.Controls.Symbol.Picture)elementCanvas.Items[i];
			
							lapsi.Visibility = Visibility.Hidden;
							
						}
						catch(Exception ex)
						{
							Globals.Tags.Log(ex.ToString());
						}
					}
				}
				catch(Exception ex)
				{
					Globals.Tags.Log(ex.ToString());
				}
			}
				
			Globals.Tags.Kasiajo.Value = 0;
		}
		
		/// <summary>
		/// Liittää Sensor_Click handlerin kaikkiin Rectangle-elementteihin
		/// </summary>
		/// <param name="sender">this</param>
		public void LuoClickHandlerit()
		{
			List<FrameworkElement> anturit = HaeElementit(ryhma_nimi);
			
			String kasiajot = "";
			foreach (FrameworkElement nappi in anturit)
			{
				try
				{
					Neo.ApplicationFramework.Controls.Group elementCanvas = (Neo.ApplicationFramework.Controls.Group)nappi;
    
					// ElementCanvasin alla on näkyvät elementit
					for(int i=0; i<elementCanvas.Items.Count; i++)
					{
						// Yritetään tyypittää pictures
						if(eka)
						{
							try
							{
								string nama = elementCanvas.Items[i].GetType().Name;
								Globals.Tags.Log(nama);
						
								Neo.ApplicationFramework.Controls.Label lapsi = (Neo.ApplicationFramework.Controls.Label)elementCanvas.Items[i];
								//lapsi.Text = ParsiNimi(nappi.Name);
								
							
							}
							catch(Exception ex)
							{
								Globals.Tags.Log(ex.ToString());
							}
						}
						
						try
						{
							// Hide all arrow images in first round
							Neo.ApplicationFramework.Controls.Symbol.Picture lapsi = (Neo.ApplicationFramework.Controls.Symbol.Picture)elementCanvas.Items[i];
							
							//if(eka)
							lapsi.Visibility = Visibility.Hidden;
							
							// Read selected man buttons to string tag
							if(Neo.ApplicationFramework.Generated._Konfiguraatio.ManualMode == Neo.ApplicationFramework.Generated._Konfiguraatio.man_mode.ManMultiString)
							{
								if(lapsi.Visibility == Visibility.Visible)
								{
									if(kasiajot.Length == 0)
										kasiajot = ParsiNimi(nappi.Name);
									else
										kasiajot = kasiajot+";"+ParsiNimi(nappi.Name);
									break;
								}
							}

						}
						catch(Exception ex)
						{
							Globals.Tags.Log(ex.ToString());
						}
					}
				}
				catch(Exception ex)
				{
					Globals.Tags.Log(ex.ToString());
				}
			}
				
			Globals.Tags.Kasiajo.Value = kasiajot;
			
			if(eka)
				eka = false;
		

			foreach (FrameworkElement anturi in anturit)
			{
				anturi.MouseDown += Sensor_Click;
			}
			
			Manual_Ctrl_Nr.ValueChange += Manual_Ctrl_Nr_ValueChange;
			//Globals.Tags.Line1_HMI1_Manual_Ctrl_Nr.ValueChange += Line1_HMI1_Manual_Ctrl_Nr_ValueChange;
		}

		
		public void Manual_Ctrl_Nr_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(System.Convert.ToInt32(e.Value) == 0)
				ManualResetButtons();
		}
		
		/// <summary>
		/// Irroittaa Sensor_Click-handlerin kaikista Rectangle-elementeistä
		/// </summary>
		/// <param name="sender">this</param>
		public void RemoveClickHandlers()
		{
			List<FrameworkElement> anturit = HaeElementit(ryhma_nimi);

			foreach (FrameworkElement anturi in anturit)
			{
				anturi.MouseDown -= Sensor_Click;
			}	
			
			Manual_Ctrl_Nr.ValueChange -= Manual_Ctrl_Nr_ValueChange;
		}
		
		/// <summary>
		/// Hakee sopivat näytön elementit nimen perusteella.
		/// </summary>
		/// <param name="nimenOsa">Pätkä elementin nimestä</param>
		/// <returns>Sopivat elementit</returns>
		private List<FrameworkElement> HaeElementit(string nimenOsa)
		{
			List<FrameworkElement> elementit = new List<FrameworkElement>();
			
			// Ikkunan alla on vain yksi elementti (ContentPresenter)
			FrameworkElement contentPresenter = (FrameworkElement)VisualTreeHelper.GetChild(sivu, 0);

			// Ikkunan alla on vain yksi elementti (ElementCanvas)
			FrameworkElement elementCanvas = (FrameworkElement)VisualTreeHelper.GetChild(contentPresenter, 0);
			
			// ElementCanvasin alla on näkyvät elementit
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(elementCanvas); i++)
			{
				FrameworkElement lapsi = (FrameworkElement)VisualTreeHelper.GetChild(elementCanvas, i);

				// Napataan vain lavapaikkaelementit
				if (lapsi.Name.Contains(nimenOsa))
				{
					// Lisätään listaan
					elementit.Add(lapsi);
				}
			}
			
			return elementit;
		}
		
		/// <summary>
		/// Etsii ryhmän nimestä position, tyypin ja numeron
		/// anturi-infoikkunan.
		/// </summary>
		/// <param name="sender">this.Rectangle</param>
		private string ParsiNimi(string nimi)
		{
			string ret="";
			int pos = nimi.IndexOf(ryhma_nimi)+3;
			if(pos != -1)
			{
				ret = nimi.Substring(pos,nimi.Length-pos);
			}
			return ret;
	
		}

		/// <summary>
		/// Hakee anturin tunnuksen neliötä ohjaavasta tagista ja avaa 
		/// anturi-infoikkunan.
		/// </summary>
		/// <param name="sender">this.Rectangle</param>
		private void Sensor_Click(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				if(_Konfiguraatio.ManualMode == _Konfiguraatio.man_mode.ManNumber)
				{
					// Hide all buttons before show the selected one
					List<FrameworkElement> anturit = HaeElementit(ryhma_nimi);
			
					foreach (FrameworkElement nappi in anturit)
					{
						try
						{
							Neo.ApplicationFramework.Controls.Group elementCanvas = (Neo.ApplicationFramework.Controls.Group)nappi;
    
							// ElementCanvasin alla on näkyvät elementit
							for(int i=0; i<elementCanvas.Items.Count; i++)
							{												
								try
								{
									// Hide all arrow images in first round
									Neo.ApplicationFramework.Controls.Symbol.Picture lapsi = (Neo.ApplicationFramework.Controls.Symbol.Picture)elementCanvas.Items[i];
			
									
									lapsi.Visibility = Visibility.Hidden;

								}
								catch(Exception ex)
								{
									Globals.Tags.Log(ex.ToString());
								}
							}
						}
						catch(Exception ex)
						{
							Globals.Tags.Log(ex.ToString());
						}
					}
				}
				
				
				Neo.ApplicationFramework.Controls.Group elementCanvas1 = (Neo.ApplicationFramework.Controls.Group)sender;
    
				// ElementCanvasin alla on näkyvät elementit
				for(int i=0; i<elementCanvas1.Items.Count; i++)
				{
					// Yritetään tyypittää pictures
					try
					{
						Neo.ApplicationFramework.Controls.Symbol.Picture lapsi = (Neo.ApplicationFramework.Controls.Symbol.Picture)elementCanvas1.Items[i];
		
						if(lapsi.Visibility == Visibility.Visible)
							lapsi.Visibility = Visibility.Hidden;
						else
							lapsi.Visibility = Visibility.Visible;

					}
					catch(Exception)
					{
					}
				}
				
				// Luodaan lista käsiajoista
				String kasiajot = "";
				List<FrameworkElement> man_napit = HaeElementit(ryhma_nimi);

				foreach (FrameworkElement nappi in man_napit)
				{
					try
					{
						elementCanvas1 = (Neo.ApplicationFramework.Controls.Group)nappi;
    
						// ElementCanvasin alla on näkyvät elementit
						for(int i=0; i<elementCanvas1.Items.Count; i++)
						{
							// Yritetään tyypittää pictures
							try
							{
								Neo.ApplicationFramework.Controls.Symbol.Picture lapsi = (Neo.ApplicationFramework.Controls.Symbol.Picture)elementCanvas1.Items[i];
		
								if(lapsi.Visibility == Visibility.Visible)
								{
									if(kasiajot.Length == 0)
										kasiajot = ParsiNimi(nappi.Name);
									else
										kasiajot = kasiajot+";"+ParsiNimi(nappi.Name);
									break;
								}
							}
							catch(Exception)
							{
							}
						}
					}
					catch(Exception)
					{
					}
				}
				
				if(_Konfiguraatio.ManualMode == _Konfiguraatio.man_mode.ManNumber)
				{
					//this.anbManNum.Visible = true;
					//this.txtManNum.Visible = false;
					Manual_Ctrl_Nr.Value = System.Convert.ToInt32(kasiajot);
				}
				else
				{
					//this.anbManNum.Visible = false;
					//this.txtManNum.Visible = true;
					Manual_Ctrl_Nr_str = kasiajot;
				}
			}
			catch(Exception ex)
			{
				Globals.Tags.Log("Exception "+ex.ToString());	
			}
	
		}
		
		public void VaihdaManTapa()
		{
			if(_Konfiguraatio.ManualMode == _Konfiguraatio.man_mode.ManMultiString)
			{
				_Konfiguraatio.ManualMode = _Konfiguraatio.man_mode.ManNumber;
				//this.btnManTapa.Text = "Num";
			}
			else
			{
				_Konfiguraatio.ManualMode = _Konfiguraatio.man_mode.ManMultiString;
				//this.btnManTapa.Text = "String";
			}
		}
		
	
		
    }
}

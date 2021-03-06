using System;
using System.Collections.Generic;

using Gtk;

namespace MathTextCustomWidgets.Dialogs
{

	/// <summary>
	/// Esta clase crea el asistente que se usa para guiar el proceso de creación
	/// de una nueva base de datos.
	/// <summary>
	public class PanelAssistant
	{
		
		#region Widgets de Glade
		
		[Glade.WidgetAttribute]
		private Button acceptButton = null;
		
		[Glade.WidgetAttribute]
		private Button backButton = null;		
		
		[Glade.WidgetAttribute]
		private Dialog panelAssistant = null;
		
		[Glade.WidgetAttribute]
		private Button nextButton = null;
		
		[Glade.WidgetAttribute]
		private HBox stepsHBox = null;
		
		#endregion Widgets de Glade
		
		#region Atributos
		
		private int panelIdx;	
				
		private List<PanelAssistantStep> steps;
		
		#endregion Atributos
		
		#region Constructores
		
		/// <summary>
		/// Crea una instancia de <c>PanelAssistant</c>.
		/// </summary>
		/// <param name = "parent">
		/// La ventana desde la que se lanza el asistente.
		/// </param>
		/// <param name = "title">
		/// El título de la ventana del asistente.
		/// </param>
		public PanelAssistant(Window parent, string title)
		{
			Glade.XML gxml =
				new Glade.XML(null,"gui.glade","panelAssistant",null);
				
			gxml.Autoconnect(this);
			
			panelAssistant.Title = title;
			
			if(parent != null)
				panelAssistant.Icon = parent.Icon;
			
			panelAssistant.TransientFor = parent;
			panelAssistant.Modal = true;			
			
			panelIdx = 0;			
			
			steps = new List<PanelAssistantStep>();
		}
		
		#endregion Constructores
		
		#region Propiedades
		
		/// <value>
		/// Permite recuperar la ventana en la que se muestra el 
		/// value.
		/// </value>
		public Window Window
		{
			get
			{
				return panelAssistant;
			}
		}
		
		/// <value>
		/// Permite recuperar la lista de paneles almancenados en el
		/// asistente.
		/// </value>
		public List<PanelAssistantStep> Steps
		{
			get{
				return steps;
			}
		}
		
		#endregion Propiedades
		
		#region Metodos publicos
		
		/// <summary>
		/// Adds a new step to the assistant.
		/// </summary>
		/// <param name="step">
		/// The step being added.
		/// </param>
		public void AddStep(PanelAssistantStep step)
		{
			// Añadimos un paso al asistente.
			
			steps.Add(step);
			stepsHBox.Add(step.StepWidget);
			step.Hide();
			
			SetIndex(0);					
		}
		
		public void Destroy()
		{
			this.panelAssistant.Destroy();
		}
		
		/// <summary>
		/// Este método permite mostrar el diálogo, esperando a que el asistente
		/// responda un resultado.
		/// </summary>
		public ResponseType Run()
		{
			// Así no salimos si pulsamos un botón del asistente que no sea 
			// cancelar o aceptar.
			ResponseType res = ResponseType.None;
			
			while(res == ResponseType.None)
				res= (ResponseType)panelAssistant.Run();
			
			return res;
		}
		
		#endregion Metodos publicos
		
		#region Metodos privados
		
	
		
		private void OnAcceptButtonClicked(object sender, EventArgs a)
		{
			if(steps[steps.Count-1].HasErrors)
			{
				ShowErrors(steps.Count-1);
				panelAssistant.Respond(ResponseType.None);
			}
			else
			{
				panelAssistant.Respond(ResponseType.Ok);
			}
		}
		
		private void OnBackButtonClicked(object sender, EventArgs a)
		{
			SetIndex(panelIdx - 1);
			
			// Porque sino se cierra el dialogo
			panelAssistant.Respond(ResponseType.None);
		}
		
		private void OnCancelButtonClicked(object sender, EventArgs a)
		{
			panelAssistant.Respond(ResponseType.Cancel);
		}
		
		private void OnNextButtonClicked(object sender, EventArgs a)
		{			
			SetIndex(panelIdx + 1);// nos vamos al siguiente panel.			
			panelAssistant.Respond(ResponseType.None);// sino se cierra el dialogo
		}
		
		/// <summary>
		/// Selects an assistant's panel by its index.
		/// </summary>
		/// <param name="idx">
		/// The index of the panel to be selected.
		/// </param>
		private void SetIndex(int idx)
		{
			if(idx > panelIdx && ((PanelAssistantStep)(steps[panelIdx])).HasErrors)
			{
				ShowErrors(idx);
			}
			else
			{
				// Activamos o desactivamos los botones segun corresponda.
				panelIdx = idx;
				backButton.Sensitive = idx > 0;
				nextButton.Visible = idx < steps.Count -1;
				
				acceptButton.Visible = idx == steps.Count -1;
				
				// Mostramos los paneles adecuados.
				int i = 0;
				foreach(PanelAssistantStep step in steps)
				{
					if(i == panelIdx)
						step.Show();
					else
						step.Hide();
						
					i++;
				}
			
			}
		}
		
		/// <summary>
		/// Shows the error of the given panel.
		/// </summary>
		/// <param name="idx">
		/// The index of the panel.
		/// </param>
		private void ShowErrors(int idx)
		{
			string errors = "";
				
			foreach(string error in steps[panelIdx].Errors)
			{
				errors += String.Format("{0}\n",error);
			}
			
			OkDialog.Show(
				panelAssistant,
				MessageType.Warning,
				"Para continuar, soluciona los siguientes problemas:\n\n{0}",
				errors);
		}
		
		#endregion Metodos privados
	}

} 

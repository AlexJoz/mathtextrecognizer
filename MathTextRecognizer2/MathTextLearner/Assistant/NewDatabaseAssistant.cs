
using System;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets;
using MathTextCustomWidgets.Dialogs;

using MathTextLibrary.Databases;

namespace MathTextLearner.Assistant
{
	/// <summary>
	/// Esta clase contiene el punto de entrada principal de la aplicación
	/// <c>MathTextBatchLearner</c>.
	/// </summary>
	public class NewDatabaseAsisstant : PanelAssistant
	{				
		
		private DatabaseTypeStep databaseStep;
		private FileSelectionStep fileStep;
		private BitmapProcessesStep processesStep;
		private DatabasePropertiesStep propertiesStep;
		
		/// <summary>
		/// <c>NewDatabaseAssistant</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The window this dialog will be modal of.
		/// </param>
		public NewDatabaseAsisstant(Window parent) : 
			base(parent, "Asistente de nueva base de datos de caracteres")
		{
			databaseStep = new DatabaseTypeStep(this);
			fileStep = new FileSelectionStep(this);
			processesStep = new BitmapProcessesStep(this, fileStep.ImagesStore);
			propertiesStep = new DatabasePropertiesStep(this);
			
			this.AddStep(databaseStep);		
			this.AddStep(fileStep);
			this.AddStep(processesStep);
			this.AddStep(propertiesStep);
			
			
			foreach(PanelAssistantStep panel in Steps)
			{
				panel.StepWidget.SetSizeRequest(500,300);
			}
			
			this.Window.Icon = ImageResources.LoadPixbuf("database-new16");
		}
		
		/// <summary>
		/// <c>NewDatabaseAssistant</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The window this dialog will be modal of.
		/// </param>
		/// <param name="startImage">
		/// An image to be loaded by default.
		/// </param>
		/// <param name="imageName">
		/// The added image's name.
		/// </param>
		public NewDatabaseAsisstant(Window parent, 
		                            Gdk.Pixbuf startImage, 
		                            string imageName)
			: this(parent)
		{
			
			fileStep.AddImageFile(startImage, imageName);
			
		}
		
#region Propiedades 
		
		/// <value>
		/// Contiene la base de datos creada por el asistente.
		/// </value>
		public MathTextDatabase Database
		{
			get
			{
				return CreateDatabase();
			}
		}
		
		/// <value>
		/// Contiene las imagenes para aprender seleccionadas en 
		/// el asistente.
		/// </value>
		public List<Gdk.Pixbuf> Images
		{
			get{
				return RetrieveImages();
			}
		}
#endregion
		
#region Metodos privados
		
		private MathTextDatabase CreateDatabase()
		{
			MathTextDatabase mtd = new MathTextDatabase();
			
			mtd.Database = databaseStep.Database;
			mtd.Processes = processesStep.Processes;
			mtd.Description = propertiesStep.LongDescription;
			mtd.ShortDescription = propertiesStep.ShortDescription;
			
			return mtd;
		}
		
		private List<Gdk.Pixbuf> RetrieveImages()
		{
			
			return fileStep.Images;
		}

#endregion Metodos privados
	}
}

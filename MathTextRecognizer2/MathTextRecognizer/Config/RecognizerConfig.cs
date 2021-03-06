// RecognizerConfig.cs created with MonoDevelop
// User: luis at 17:44 05/04/2008

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Utils;
using MathTextLibrary.Analisys;
using MathTextLibrary.Databases;
using MathTextRecognizer.DatabaseManager;

namespace MathTextRecognizer.Config
{
	
	/// <summary>
	/// Esta clase almacena la configuracion de la aplicacion de reconocimiento.
	/// </summary>
	public class RecognizerConfig
	{
		private List<DatabaseFileInfo> databaseFilesInfo;
		private List<LexicalRule> lexicalRules;
		private List<SyntacticalRule> syntacticalRules;
		
		private bool showOutputConversion;
		private string outputConversionCommand;
		
		private static RecognizerConfig config;
		
		/// <summary>
		/// This event is launched when the settings are changed.
		/// </summary>
		public event EventHandler Changed;
	
		
		public RecognizerConfig()
		{
			databaseFilesInfo = new List<DatabaseFileInfo>();
			lexicalRules = new List<LexicalRule>();
		}
		
		/// <value>
		/// Contiene la informacion para persistir de las bases de datos.
		/// </value>
		public List<DatabaseFileInfo> DatabaseFilesInfo
		{
			get
			{
				return databaseFilesInfo;
			}
			set
			{
				databaseFilesInfo = value;
				ChangedInvoker();
			}
		}
		
		/// <value>
		/// Contains the stored databases objects.
		/// </value>
		[XmlIgnore]
		public List<MathTextDatabase> Databases
		{
			get
			{
				List<MathTextDatabase> databases = new List<MathTextDatabase>();
				foreach(DatabaseFileInfo info in databaseFilesInfo)
				{
					databases.Add(info.Database);
				}
				
				return databases;
			}
		}
		
		/// <value>
		/// Permite recuperar la instancia de configuración.
		/// </value>
		public static RecognizerConfig Instance
		{
			get
			{
				if(config==null)
				{
					Load();
					config.ChangedInvoker();
				}
					
				return config;
			}
		}

		/// <value>
		/// Contains the rules used in the lexical analisys.
		/// </value>
		public List<LexicalRule> LexicalRules 
		{			
			get
			{
				return lexicalRules;
			}
			set
			{
				lexicalRules = value;
				ChangedInvoker();
			}
		}

		/// <value>
		/// Contains the rules used in the syntactical analisys process.
		/// </value>
		public List<SyntacticalRule> SyntacticalRules 
		{
			get 
			{
				return syntacticalRules;
			}
			set 
			{
				syntacticalRules = value;
				ChangedInvoker();
			}
		}

		/// <value>
		/// Contains the setting telling the output dialog if it must
		/// show an image with the app's text output converted into a image.
		/// </value>
		public bool ShowOutputConversion 
		{
			get 
			{
				return showOutputConversion;
			}
			set 
			{
				showOutputConversion = value;
				ChangedInvoker();
			}
		}

		/// <value>
		/// The command used for converting the text output into an image.
		/// </value>
		public string OutputConversionCommand 
		{
			get 
			{
				return outputConversionCommand;
			}
			set 
			{
				outputConversionCommand = value;
				ChangedInvoker();
			}
		}
		
		/// <summary>
		/// Carga la configuracion desde el archivo de configuracion
		/// de la aplicacion.
		/// </summary>
		private static void Load()
		{
						
			XmlSerializer serializer = 
				new XmlSerializer(typeof(RecognizerConfig),
				                  GetSerializationOverrides());	
			
			string path = PathUtils.GetConfigFilePath("MathTextRecognizer");
			
			Stream configStream;
			
			if(File.Exists(path))
			{			                                           
				configStream = new FileStream(path,FileMode.Open);
			}
			else
			{
				// Cargamos la configuración por defecto, cargandola desde
				// un archivo de recursos.				
				Assembly runningAssembly =Assembly.GetExecutingAssembly();
				configStream = 
					runningAssembly.GetManifestResourceStream("defaultConfig");			
				
			}
			
			// Deserializamos
			config = (RecognizerConfig)serializer.Deserialize(configStream);					
			configStream.Close();
			
			
			
		}
		
		/// <summary>
		/// Guarda la configuracion.
		/// </summary>
		public void Save()
		{
			XmlSerializer serializer =
				new XmlSerializer(typeof(RecognizerConfig),
				                  GetSerializationOverrides());	
			
			string path = PathUtils.GetConfigFilePath("MathTextRecognizer");
			
			using(StreamWriter w = new StreamWriter(path,false))
			{
				serializer.Serialize(w,this);
				w.Close();
			}
		}
		
		/// <summary>
		/// Crea el perfil para poder serializar la configuracion.
		/// </summary>
		/// <returns>
		/// A <see cref="XmlAttributeOverrides"/>
		/// </returns>
		private static XmlAttributeOverrides GetSerializationOverrides()
		{
			// En principio nada.
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
		
			
			return attrOverrides;
		}
		
		/// <summary>
		/// Invokes the Changed event;
		/// </summary>
		private void ChangedInvoker()
		{
			if(Changed!=null)
			{
				Changed(this, EventArgs.Empty);
			}
		}
	}
}

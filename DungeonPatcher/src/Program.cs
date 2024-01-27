// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DungeonPatcher;
using DungeonPatcher.XML_Settings;
using NLog;

class Program {
	private static string version = "0.1";
	static Logger log = LogManager.GetLogger(typeof(Program).ToString());


	private static void PrintUsage() {
		log.Info("Usage: DungeonPatcher <FolderPath>");
	}
	
	static void Main(string[] args) {
		LogFormatter.InitLogger();

		log.Info("++++ Starting Dungeon Patcher! ++++");
		log.Info("	Version: "+version+"\n");
		
		if (args.Length != 1) {
			log.Warn("Wrong Arguments..");
			PrintUsage();
			return;
		}

		if(args[0].Equals("-h")) {
			PrintUsage();
			return;
		}

		if (args[0].Equals("--version")) {
			log.Info(version);
			return;
		}
		

		var folderLoadPath = args[0];
		if (Directory.Exists(folderLoadPath)) {
			log.Info("Using Path: "+folderLoadPath);
		} else {
			log.Error($"Folder path [{folderLoadPath}] Doesn't exist.. Check Path or Create Folder first");
			return;
		}

		
		const string ArtistInfoPath =
			@"config\ArtistInfo.xml";


		if (!File.Exists(ArtistInfoPath)) {
			log.Info($"ArtistInfoFile doesnt Exists in Path {ArtistInfoPath}");
			return;
		} 
		
		


		XmlSerializer serializer = new XmlSerializer(typeof(Settings));


		Settings newSettings = new Settings {
			SharedSettings = LoadSharedSettingsFromFile(ArtistInfoPath),
			Placeables = LoadObjectsFromFolder(folderLoadPath)
		};

		string artDefPath = GetArtDefinitionsPath(folderLoadPath);
		
		TextWriter writer = new StreamWriter(artDefPath);
		serializer.Serialize(writer, newSettings);
		log.Info($"Creating Art Definition XML: [{artDefPath}]");
	}

	private static string GetArtDefinitionsPath(string folderLoadPath) {
		folderLoadPath = Regex.Replace(folderLoadPath,@"[\\/](Art)[\\/]",@"\Art Definitions\");
		folderLoadPath = Regex.Replace(folderLoadPath, @"\\$", "");
		return folderLoadPath + ".xml";
	}

	private static List<Placeable> LoadObjectsFromFolder(string folderLoadPath) {
		log.Info($"Loading Artpieces from Folder...");

		List<Placeable> files = Directory.GetFiles(folderLoadPath, "*.png")
			.Select(filePath => {
				var fileInfo = new FileInfo(filePath);
				var relativePath = Regex.Replace(filePath, @".*Custom[\\/]", "").Replace(@"\","/");
				var name = fileInfo.Name.Replace(".png", "");
				if (Regex.IsMatch(relativePath, "[äöüß]")) {
					log.Warn($"File {filePath} contains Umlaute!!");
				}
				return fileInfo.Directory != null 
					? Placeable.ConstructDefault(name, relativePath, fileInfo.Directory.Name) 
					: null;
			})
			.Where(placeable => placeable != null)
			.ToList()!;

		log.Info($"loaded {files.Count} Images");
		return files;

	}

	private static SharedSettings LoadSharedSettingsFromFile(string artistInfoPath) {
		XmlSerializer serializer = new XmlSerializer(typeof(SharedSettings));
		var reader = XmlReader.Create(artistInfoPath);
		var xml = serializer.Deserialize(reader);
		if (xml != null && xml.GetType() == typeof(SharedSettings)) {
			var outObj = (SharedSettings)xml;
			log.Info($"Loaded Artist Information.. Hello {outObj.Settings[0].Value}");
			return outObj;
		} else {
			log.Error($"Couldnt load Artist Info from Path {artistInfoPath}");
			return new SharedSettings();
		}
	}
}
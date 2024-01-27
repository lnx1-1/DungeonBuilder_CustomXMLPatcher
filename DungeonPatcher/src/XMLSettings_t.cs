using System.Xml.Serialization;

namespace DungeonPatcher.XML_Settings;

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Settings")]
public class Settings {
	[XmlElement("SharedSettings")] public SharedSettings SharedSettings { get; set; } = null!;

	[XmlElement("Placeable")] public List<Placeable> Placeables { get; set; }
}

public class SharedSettings {
	[XmlElement("Setting")] public List<Setting> Settings { get; set; }
}

public class Placeable {
	[XmlAttribute("Name")] public string Name { get; set; }

	[XmlElement("Setting")] public List<Setting> Settings { get; set; }

	public static Placeable ConstructDefault(string name, string path, string cat) {
		var outObj = new Placeable();
		outObj.Name = name;
		outObj.Settings = new List<Setting>() { Setting.ImagePath(path), Setting.Category(cat) };
		outObj.Settings.AddRange(Setting.GetDefaultOffsets());
		return outObj;
	}
}

public class Setting {
	[XmlAttribute("Key")] public string Key { get; set; }

	[XmlAttribute("Value")] public string Value { get; set; }

	public static Setting ImagePath(String path) {
		return new Setting() { Key = "ImagePath", Value = path };
	}

	public static Setting Category(String category) {
		return new Setting() { Key = "Category", Value = category };
	}

	public static IEnumerable<Setting> GetDefaultOffsets() {
		return new HashSet<Setting>() {
			new() { Key = "DrawingOffsetX", Value = "0" },
			new() { Key = "DrawingOffsetY", Value = "0" },
			new() { Key = "DrawingOffsetZ", Value = "0" },
			new() { Key = "CornerOffsetX", Value = "0" },
			new() { Key = "CornerOffsetY", Value = "0" }
		};
	}
}
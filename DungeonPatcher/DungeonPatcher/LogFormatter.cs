using NLog;

namespace DungeonPatcher; 

public class LogFormatter {
	public static void InitLogger() {
		LogManager.Setup().LoadConfigurationFromFile("config/Nlog.config");
	}
}
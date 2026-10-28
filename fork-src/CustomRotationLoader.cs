using ECommons.DalamudServices;
using ECommons.Logging;
using System.Runtime.Loader;

namespace RotationSolver.Helpers;

/// <summary>
/// Loads external rotation assemblies from the plugin config's CustomRotations folder.
/// Assemblies are loaded from memory into the plugin's own load context, so they share
/// RotationSolver, ECommons and Dalamud with the plugin and the DLL files stay unlocked
/// for rebuilding. Changes are picked up when the plugin is reloaded.
/// </summary>
internal static class CustomRotationLoader
{
	public const string FolderName = "CustomRotations";

	private static readonly Dictionary<string, Assembly> _loaded = [];

	public static string Directory => Path.Combine(Svc.PluginInterface.ConfigDirectory.FullName, FolderName);

	public static List<Assembly> LoadAll()
	{
		List<Assembly> result = [];
		var directory = Directory;
		if (!System.IO.Directory.Exists(directory))
		{
			_ = System.IO.Directory.CreateDirectory(directory);
			return result;
		}

		var context = AssemblyLoadContext.GetLoadContext(typeof(CustomRotationLoader).Assembly) ?? AssemblyLoadContext.Default;
		foreach (var file in System.IO.Directory.GetFiles(directory, "*.dll"))
		{
			try
			{
				if (!_loaded.TryGetValue(file, out var assembly))
				{
					assembly = Load(context, file);
					_loaded[file] = assembly;
					PluginLog.Information($"Loaded custom rotation assembly {assembly.GetName().Name} from {file}");
				}
				result.Add(assembly);
			}
			catch (Exception ex)
			{
				PluginLog.Error($"Failed to load custom rotation assembly {file}: {ex}");
			}
		}
		return result;
	}

	private static Assembly Load(AssemblyLoadContext context, string file)
	{
		using var dll = new MemoryStream(File.ReadAllBytes(file));
		var pdbPath = Path.ChangeExtension(file, ".pdb");
		if (!File.Exists(pdbPath))
		{
			return context.LoadFromStream(dll);
		}

		using var pdb = new MemoryStream(File.ReadAllBytes(pdbPath));
		return context.LoadFromStream(dll, pdb);
	}
}

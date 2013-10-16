using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class AutoBuild
{
	[MenuItem("AutoBuild/Build WebPlayer")]
	public static void WebPlayer()
	{
		string temp = MoveSvnToTemp("WebPlayer");

		string[] scenes = new string[] { "Assets/Scenes/ClientWorld1.unity", "Assets/Scenes/ClientWorld2.unity" };

		BuildPipeline.BuildPlayer(scenes, "WebPlayer", BuildTarget.WebPlayer, 0);

		MoveTempToSvn(temp, "WebPlayer");
	}

	[MenuItem("AutoBuild/Build Servers")]
	public static void Servers()
	{
		string[] scene1 = new string[] { "Assets/Scenes/ServerWorld1.unity" };
		string[] scene2 = new string[] { "Assets/Scenes/ServerWorld2.unity" };
		
		BuildTarget target;
		string extension = "";
		
		switch (Application.platform)
		{
			case RuntimePlatform.WindowsEditor: target = BuildTarget.StandaloneWindows; extension = ".exe"; break;
			case RuntimePlatform.OSXEditor: target = BuildTarget.StandaloneOSXIntel; extension = ".app"; break;
			default: return;
		}
		
		BuildPipeline.BuildPlayer(scene1, "Servers/SnowboxServer1" + extension, target, 0);
		BuildPipeline.BuildPlayer(scene2, "Servers/SnowboxServer2" + extension, target, 0);
	}
	
	private static string MoveSvnToTemp(string folder)
	{
		string svn = folder + Path.DirectorySeparatorChar + ".svn";
		if (!Directory.Exists(svn)) return null;

		string temp = GetTempFolder();
		Directory.Move(svn, temp);
		return temp;
	}

	private static void MoveTempToSvn(string temp, string folder)
	{
		if (String.IsNullOrEmpty(temp) || !Directory.Exists(temp)) return;

		string svn = folder + Path.DirectorySeparatorChar + ".svn";
		Directory.Move(temp, svn);
	}
	
	private static string GetTempFolder()
	{
		string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

		while (Directory.Exists(temp))
		{
			temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}

		return temp;
	}
}

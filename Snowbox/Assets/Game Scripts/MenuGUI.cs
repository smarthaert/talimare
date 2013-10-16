using System;
using System.Net;
using UnityEngine;
using Random=UnityEngine.Random;

public class MenuGUI : uLink.MonoBehaviour
{
	public string quickHost = "unitypark.dyndns.org";
	public int quickPort = 7100;

	public MonoBehaviour enableWhenBusy = null;

	public string findPlayerByTag = "";

	public GameClient client;

	public Texture2D iconFavorite;
	public Texture2D iconUnfavorite;

	private string playerName;

	private bool isQuickMode = true;
	
	public GUITexture pauseScreen;

	private const float QUICK_WIDTH = 220;
	private const float ADVANCED_WIDTH = 580;
	private const float BUSY_WIDTH = 220;

	private Vector2 scrollPosition = Vector2.zero;
	private int selectedGrid = 0;
	private uLink.NetworkConnectionError lastError = uLink.NetworkConnectionError.NoError;

	void Awake()
	{
		playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 100));
	}

	void OnDisable()
	{
		PlayerPrefs.SetString("playerName", playerName);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) Screen.lockCursor = false;
	}

	void OnGUI()
	{
		enableWhenBusy.enabled = true;
		pauseScreen.enabled = false;
		
		if (lastError == uLink.NetworkConnectionError.NoError && uLink.Network.status == uLink.NetworkStatus.Connected && GameObject.FindGameObjectWithTag(findPlayerByTag) != null)
		{
			if (Screen.lockCursor)
			{
				enableWhenBusy.enabled = false;
			}
			else
			{
				pauseScreen.enabled = true;
			}
			
			return;
		}

		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

		if (lastError != uLink.NetworkConnectionError.NoError || uLink.Network.status != uLink.NetworkStatus.Disconnected)
		{
			GUILayout.BeginVertical("Box", GUILayout.Width(BUSY_WIDTH));
			BusyGUI();
			GUILayout.EndVertical();
		}
		else if (isQuickMode)
		{
			enableWhenBusy.enabled = false;
			
			GUILayout.BeginVertical("Box", GUILayout.Width(QUICK_WIDTH));
			QuickGUI();
			GUILayout.EndVertical();
		}
		else
		{
			enableWhenBusy.enabled = false;
			
			GUILayout.BeginVertical("Box", GUILayout.Width(ADVANCED_WIDTH));
			AdvancedGUI();
			GUILayout.EndVertical();
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	void BusyGUI()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if (lastError != uLink.NetworkConnectionError.NoError)
		{
			GUILayout.Label("Error: " + lastError);
		}
		else if (uLink.Network.status == uLink.NetworkStatus.Connected)
		{
			GUILayout.Label("Instantiating...");
		}
		else if (uLink.Network.status == uLink.NetworkStatus.Connecting)
		{
			GUILayout.Label((client.isRedirected ? "Redirecting to " : "Connecting to ") + uLink.NetworkPlayer.server.endpoint);
		}
		else if (uLink.Network.status == uLink.NetworkStatus.Disconnecting)
		{
			GUILayout.Label("Disconnecting");
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (uLink.Network.status == uLink.NetworkStatus.Connecting && !client.isRedirected)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			GUILayout.EndVertical();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Cancel", GUILayout.Width(80), GUILayout.Height(25)))
			{
				uLink.Network.DisconnectImmediate();
				lastError = uLink.NetworkConnectionError.NoError;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();
	}

	void QuickGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Please enter your name:");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();
					
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		playerName = GUILayout.TextField(playerName, GUILayout.MinWidth(80));
		GUILayout.Space(10);
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		string quickCaption = (quickHost == "unitypark.dyndns.org") ? "Play at UnityPark" : "Play on " + quickHost;

		if (GUILayout.Button(quickCaption, GUILayout.Width(120), GUILayout.Height(25)))
		{
			client.Connect(quickHost, quickPort, playerName);
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Advanced", GUILayout.Width(80), GUILayout.Height(25)))
		{
			isQuickMode = false;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(2);
		GUILayout.EndVertical();
	}

	void AdvancedGUI()
	{
		GUILayout.BeginHorizontal();
		selectedGrid = GUILayout.SelectionGrid(selectedGrid, new string[] { "Internet", "LAN", "Favorites" }, 3, GUILayout.Height(22));
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal("Box");
		GUILayout.Space(5);

		GUILayout.Label("Name", GUILayout.Width(80));
		GUILayout.Label("Level", GUILayout.Width(80));
		GUILayout.Label("Players", GUILayout.Width(80));
		GUILayout.Label("Host", GUILayout.Width(140));
		GUILayout.Label("Ping", GUILayout.Width(80));

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(GUILayout.Height(260));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");

		uLink.HostData[] hosts = null;
		switch (selectedGrid)
		{
			case 0: hosts = uLink.MasterServer.PollAndRequestHostList("Snowbox", 2); break;
			case 1: hosts = uLink.MasterServer.PollAndDiscoverLocalHosts("Snowbox", 7101, 7102, 2); break;
			case 2: hosts = uLink.MasterServer.PollAndRequestKnownHosts(2); break;
		}

		if (hosts != null && hosts.Length > 0)
		{
			Array.Sort(hosts, delegate(uLink.HostData x, uLink.HostData y) { return x.gameName.CompareTo(y.gameName); });

			foreach (uLink.HostData data in hosts)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(5);

				GUILayout.Label(data.gameName, GUILayout.Width(80));
				GUILayout.Label(data.gameLevel, GUILayout.Width(80));
				GUILayout.Label(data.connectedPlayers + "/" + data.playerLimit, GUILayout.Width(80));
				GUILayout.Label(data.ipAddress + ":" + data.port, GUILayout.Width(140));
				GUILayout.Label(data.ping.ToString(), GUILayout.Width(80));

				GUILayout.FlexibleSpace();

				if (uLink.MasterServer.PollKnownHostData(data.externalEndpoint) != null)
				{
					if (GUILayout.Button(iconFavorite, "Label"))
					{
						uLink.MasterServer.RemoveKnownHostData(data.externalEndpoint);
					}
				}
				else
				{
					if (GUILayout.Button(iconUnfavorite, "Label"))
					{
						uLink.MasterServer.AddKnownHostData(data);
					}
				}

				GUILayout.Space(5);

				if (uLink.Network.status == uLink.NetworkStatus.Disconnected)
				{
					if (GUILayout.Button("Join"))
					{
						client.Connect(data, playerName);
					}
				}

				GUILayout.Space(10);
				GUILayout.EndHorizontal();
			}
		}
		else if (selectedGrid == 2)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("No hosts have been marked as favorite");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else if (selectedGrid == 1 && (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer))
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("No LAN hosts have been discovered yet");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("this may be due to security restrictions on the webplayer");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Please wait for the list to begin populating...");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(5);
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Space(2);

		if (GUILayout.Button("Back", GUILayout.Width(80), GUILayout.Height(25)))
		{
			isQuickMode = true;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Space(2);
		GUILayout.EndVertical();
	}

	void uLink_OnFailedToConnect(uLink.NetworkConnectionError error)
	{
		lastError = error;
	}

	void uLink_OnRedirectingToServer(IPEndPoint newServer)
	{
		enableWhenBusy.enabled = true;
	}
}

using System;
using UnityEngine;
using uLink;

public class GameServer : uLink.MonoBehaviour
{
	public string gameName = "Snowbox";

    public int port = 7100;
	public int maxConnections = 64;
	
	public string SpawnTag = "";

	public GameObject proxyPrefab = null;
	public GameObject ownerPrefab = null;
	public GameObject creatorPrefab = null;

	public bool isCellServer = false; //true only when part of a "uLink Unlimited" cluster environemnt
	public string pikkoServerIP = "127.0.0.1"; //Only used when part of a "uLink Unlimited" cluster environemnt
	public int pikkoServerPort = 4001; //Only used when part of a "uLink Unlimited" cluster environemnt

	public ChatServer chat;
	
	private string level;

	void Awake()
	{
		level = Application.loadedLevelName.Replace("Server", "");
		
		uLink.Network.isAuthoritativeServer = true;
		if (isCellServer) 
		{
			uLink.Network.InitializeCellServer(maxConnections, pikkoServerIP, pikkoServerPort);
		}
		else
		{			
			uLink.Network.InitializeServer(maxConnections, port);
		}
	}

    void uLink_OnServerInitialized()
	{
		Debug.Log("Server successfully started on port " + uLink.Network.listenPort);
		
		uLink.MasterServer.dedicatedServer = true;
		uLink.MasterServer.RegisterHost("Snowbox", gameName, "", "", level);
	}

    void uLink_OnPlayerApproval(uLink.NetworkPlayerApproval approval)
	{
        approval.Approve(level);
	}

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		string playerName = "Nameless";

		if (player.loginData != null) player.loginData.TryRead(out playerName);

		if (uLink.NetworkView.FindByOwner(player).Length > 0)
		{
			return;
		}

		Color playerColor = HSVToRGB(UnityEngine.Random.Range(0.0f, 360.0f), 0.3f, 1);

		GameObject[] spawns = GameObject.FindGameObjectsWithTag(SpawnTag);
		int spawnindex = UnityEngine.Random.Range(0, spawns.Length - 1);
		Transform spawn = spawns[spawnindex].transform;

		uLink.Network.Instantiate(player, proxyPrefab, ownerPrefab, creatorPrefab, spawn.position, spawn.rotation, 0, playerColor, playerName);

		chat.Chat(playerName + " has joined", playerColor);
	}

	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player)
	{
		PlayerCreator playerCreator = player.localData as PlayerCreator;
		if (playerCreator != null) chat.Chat(playerCreator.playerName + " has quit", playerCreator.playerColor);
		
		uLink.Network.DestroyPlayerObjects(player);
		uLink.Network.RemoveRPCs(player);
	}

	void uLink_OnHandoverTimeout(uLink.NetworkPlayer player)
	{
        //This code is needed to destroy the player object (Player@Creator) in the new game server after the handover.
        //This code is executed if the client fails to connect to the new game server (for any reason).
		uLink.Network.DestroyPlayerObjects(player);
		uLink.Network.RemoveRPCs(player);
	}
	
	private static Color HSVToRGB(float h, float s, float v)
	{
		if (s == 0)
		{
			return new Color(v, v, v);
		}

		h /= 60;
		int i = Convert.ToInt32(Mathf.Floor(h));
		float f = h - i;

		float p = v * (1 - s);
		float q = v * (1 - s * f);
		float t = v * (1 - s * (1 - f));

		switch (i)
		{
			case 0: return new Color(v, t, p);
			case 1: return new Color(q, v, p);
			case 2: return new Color(p, v, t);
			case 3: return new Color(p, q, v);
			case 4: return new Color(t, p, v);
			default: return new Color(v, p, q);
		}
	}
}

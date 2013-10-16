using System.Net;
using UnityEngine;
using uLink;

public class GameClient : uLink.MonoBehaviour
{
	[System.NonSerialized]
	public bool isRedirected = false;

	void OnLevelWasLoaded()
	{
		uLink.Network.isMessageQueueRunning = true;
	}

	void Awake()
	{
		uLink.Network.isAuthoritativeServer = true;
	}

	public void Connect(string host, int port, string playerName)
	{
		isRedirected = false;

		uLink.NetworkConnectionError error = uLink.Network.Connect(host, port, "", playerName);

		if (error != uLink.NetworkConnectionError.NoError)
		{
			SendMessage("uLink_OnFailedToConnect", error);
		}
	}

	public void Connect(uLink.HostData hostData, string playerName)
	{
		isRedirected = false;

		uLink.NetworkConnectionError error = uLink.Network.Connect(hostData, "", playerName);

		if (error != uLink.NetworkConnectionError.NoError)
		{
			SendMessage("uLink_OnFailedToConnect", error);
		}
	}

	void uLink_OnConnectedToServer(IPEndPoint server)
	{
		string level;

        if (!uLink.Network.approvalData.TryRead(out level))
        {
            throw new System.Exception("Can't read string level in uLink.Network.approvalData");
        }
		level = "Client" + level;

        //Debug.Log("uLink_OnConnectedToServer(). Level = " + level);

		if (level != Application.loadedLevelName)
		{
			Application.LoadLevel(level);
			uLink.Network.isMessageQueueRunning = false;
		}
	}

	void uLink_OnRedirectingToServer(IPEndPoint newServer)
	{
		//Code executed when the player goes into the portal.
		isRedirected = true;
	}

	void uLink_OnDisconnectedFromServer(uLink.NetworkDisconnection mode)
	{
		if (mode == uLink.NetworkDisconnection.Redirecting)
		{
			//Code executed when the player goes into the portal.
			uLink.Network.isMessageQueueRunning = false;
			string level = (Application.loadedLevelName == "ClientWorld1") ? "ClientWorld2" : "ClientWorld1";
			
			Application.LoadLevel(level);
		}
		else
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}

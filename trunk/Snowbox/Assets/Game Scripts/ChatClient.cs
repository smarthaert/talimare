using UnityEngine;

public class ChatClient : uLink.MonoBehaviour
{
	public static bool isEnabled = false;
	
	public string prefix = "> ";
	public string cursor = "_";
	public MessageList messageList;
	
	private string input = "";
	private bool showCursor = true;
	
	void Awake()
	{
		InvokeRepeating("BlinkCursor", 0.5f, 0.5f);
	}

	void BlinkCursor()
	{
		showCursor = !showCursor;
	}
	
	void Update()
	{
		foreach (char c in Input.inputString)
		{
			if (c == '\b')
			{
				if (isEnabled && input.Length != 0)
				{
					input = input.Substring(0, input.Length - 1);
				}
			}
			else if (c == '\n' || c == '\r')
			{
				if (isEnabled)
				{
					PlayerOwner playerOwner = uLink.Network.player.localData as PlayerOwner;
					if (playerOwner != null) networkView.RPC("Chat", uLink.RPCMode.Server, playerOwner.playerName + ": " + input, playerOwner.playerColor);
					input = "";
				}
				
				isEnabled = !isEnabled;
			}
			else if (isEnabled)
			{
				input += c;
			}
		}
		
		if (!uLink.Network.isClient || uLink.Network.player.localData == null) isEnabled = false;
		
		guiText.enabled = isEnabled;
		guiText.text = prefix + input + (showCursor ? cursor : "");
	}
	
	[RPC]
	void Chat(string message, Color color)
	{
		messageList.AddMessage(message, color);
	}
}

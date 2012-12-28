using UnityEngine;
using Lidgren.Network;
using System.Reflection;

// This class handles all network communications required by the game
public abstract class NetworkHub {
	
	// The sequence channel on which all messages are sent; don't worry about this
	protected static int sequenceChannel = 0;
	
	protected static NetPeer peer;
	
	public static void Start() {
		NetPeerConfiguration config = new NetPeerConfiguration("Apocalyptia");
		config.Port = 12345;
		peer = new NetPeer(config);
		peer.Start();
		
		//peer.DiscoverKnownPeer("192.168.111.23", 12345);
	}
	
	public static void Update() {
		NetIncomingMessage msg;
		while ((msg = peer.ReadMessage()) != null) {
		    switch (msg.MessageType) {
		        case NetIncomingMessageType.VerboseDebugMessage:
		        case NetIncomingMessageType.DebugMessage:
		        case NetIncomingMessageType.WarningMessage:
		        case NetIncomingMessageType.ErrorMessage:
		            Debug.Log(msg.ReadString());
		            break;
				case NetIncomingMessageType.DiscoveryRequest:
					NetOutgoingMessage response = peer.CreateMessage();
					response.Write("Local Apocalyptia Peer");
					peer.SendDiscoveryResponse(response, msg.SenderEndPoint);
					break;
				case NetIncomingMessageType.DiscoveryResponse:
					Debug.Log("Found peer at "+msg.SenderEndPoint+" with name: "+msg.ReadString()+". Attempting connection...");
					peer.Connect(msg.SenderEndPoint);
					break;
				case NetIncomingMessageType.ConnectionApproval:
					Debug.Log("Connected to "+msg.SenderEndPoint);
					break;
				case NetIncomingMessageType.StatusChanged:
					if((NetConnectionStatus)msg.ReadByte() == NetConnectionStatus.Connected) {
						ConnectedToPeer(msg.SenderConnection);
					}
					break;
				case NetIncomingMessageType.Data:
					MessageHandler.HandleMessage(msg);
					break;
		        default:
		            Debug.Log("Unhandled type: " + msg.MessageType);
		            break;
		    }
		    peer.Recycle(msg);
		}
	}
	
	public static void OnDestroy() {
		peer.Shutdown("Peer shutting down.");
	}
	
	protected static void ConnectedToPeer(NetConnection connection) {
		Debug.Log("Connected to peer: "+connection.RemoteEndPoint);
		// This is our first peer, so we can start the game
		if(GetNumPeers() == 1)
			Game.Paused = false;
	}
	
	// Returns the number of other peers we're communicating with
	public static int GetNumPeers() {
		return peer.ConnectionsCount;
	}
	
	public static void SendMessage(Message message) {
		NetOutgoingMessage msg = peer.CreateMessage();
		message.SerializeTo(msg);
		peer.SendMessage(msg, peer.Connections, NetDeliveryMethod.ReliableUnordered, sequenceChannel);
	}
}
using UnityEngine;
using System.Collections.Generic;

public class MessageList : MonoBehaviour
{
	public GUIText messagePrefab;            // The prefab for our text object
	public float lineSize = 20.0f;                    // The pixel spacing between text objects
	public int layerTag = 0;
	
	private List<GUIText> messages = new List<GUIText>();     // The array that holds all our text objects

	void Awake () {
		// First make sure that we have a prefab set.  If not, then disable the script
		if (!messagePrefab) { enabled = false; Debug.Log("Must set the GUIText prefab for MessageList"); }
	}

	// AddMessage() accepts a text value and adds it as a status message.
	// All other status messages will be moved along the y axis by a normalized distance of lineSize.
	// AddMessage() also handles automatic removing of any GUIText objects that automatically destroy
	//   themselves.
	public void AddMessage (string text, Color color) {
		// Itterate through the messages, removing any that don't exist anymore, and moving the rest
		for (int i = 0; i < messages.Count; i++) {
			// If this message is null, remove it, drop back the i count, and jump back to the begining
			//   of the loop.
			if (!messages[i]) { messages.RemoveAt(i); i--; continue;}
			
			// If this message object does exist, then move it along the y axis by lineSize.
			// The y axis uses normalized coordinates, so we divide by the screen height to convert 
			//  pixel coordinates into normalized screen coordinates.
			messages[i].pixelOffset = messages[i].pixelOffset + new Vector2(0, lineSize);
		}
		
		// All the existing messages have been moved, making room for the new one.
		// Instantiate a new message from the prefab, set it's text value, and add it to the
		//   array of messages.
		GUIText newMessage = Instantiate(messagePrefab, messagePrefab.transform.position, messagePrefab.transform.rotation) as GUIText;
		newMessage.text = text;
		newMessage.material.color = color;
		newMessage.gameObject.layer = layerTag;
		messages.Add(newMessage);
	}

}

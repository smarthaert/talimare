using UnityEngine;
using System.Collections;

public class GUITextColor : MonoBehaviour
{
	public Color textColor = Color.white;

	void Awake ()
	{
		guiText.material.color = textColor;
	}
}

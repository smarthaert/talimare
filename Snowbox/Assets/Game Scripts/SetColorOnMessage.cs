using UnityEngine;

public class SetColorOnMessage : MonoBehaviour
{
	void SetColor(Color color)
	{
		renderer.material.color = color;
	}
}

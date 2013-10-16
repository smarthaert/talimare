using UnityEngine;

public class SetLabelOnMessage : MonoBehaviour
{
	public GameObject prefabLabel;

	public float maxDistance = 500;

	public Vector3 offset = Vector3.up;    // Units in world space to offset; 1 unit above object by default

	public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
	public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped

	private Transform target;
	private Camera cam;
	private GUIText label;
	private Transform labelTransform;
	private Transform camTransform;

	void SetLabel(string text)
	{
		label = ((GameObject)Instantiate(prefabLabel, Vector3.zero, Quaternion.identity)).guiText;
		label.text = text;

		target = transform;
		cam = Camera.main;

		labelTransform = label.transform;
		camTransform = cam.transform;
	}

	void SetColor(Color color)
	{
		label.material.color = color;
	}

	void OnDisable()
	{
		if (label != null)
		{
			DestroyImmediate(label);
			label = null;
		}
	}

	void LateUpdate()
	{
		if (target == null || label == null || cam == null) return;

		Vector3 pos;

		if (clampToScreen)
		{
			Vector3 rel = camTransform.InverseTransformPoint(target.position);
			rel.z = Mathf.Max(rel.z, 1.0f);

			pos = cam.WorldToViewportPoint(camTransform.TransformPoint(rel + offset));
			pos = new Vector3(
				Mathf.Clamp(pos.x, clampBorderSize, 1.0f - clampBorderSize),
				Mathf.Clamp(pos.y, clampBorderSize, 1.0f - clampBorderSize),
				pos.z);
		}
		else
		{
			pos = cam.WorldToViewportPoint(target.position + offset);
		}

		labelTransform.position = pos;
		label.enabled = (pos.z > 0 && pos.z <= maxDistance);
	}
}

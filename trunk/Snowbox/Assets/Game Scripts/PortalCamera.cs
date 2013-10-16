using UnityEngine;

public class PortalCamera : MonoBehaviour
{
	public Transform portalCam;

	public Transform portal1;
	public Transform portal2;

	public float nearClipPlaneMargin = 60;

	public void AdjustToCamera(Transform myCam)
	{
		Vector3 diff = portal1.InverseTransformPoint(myCam.position);
		Quaternion rot = Quaternion.Inverse(portal1.rotation) * myCam.rotation;

		portalCam.position = portal2.TransformPoint(diff);
		portalCam.rotation = portal2.rotation * rot;

		portalCam.camera.nearClipPlane = Mathf.Max(diff.magnitude - nearClipPlaneMargin, 1);
	}
}

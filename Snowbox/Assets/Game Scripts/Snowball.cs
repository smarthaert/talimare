using UnityEngine;
using System.Collections;

public class Snowball : MonoBehaviour
{
	public GameObject hitSplatter = null;
	public GameObject hitPlayerSplatter = null;
	public float timeToLive = 3.0f;

	private const float SPEED = 200;
	private const float SPIN = -10;

	void Awake()
	{
		rigidbody.velocity = transform.forward * SPEED;
		rigidbody.angularVelocity = new Vector3(SPIN, SPIN, SPIN);

		Destroy(gameObject, timeToLive);
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		if (hitSplatter != null)
			Instantiate(hitSplatter, transform.position, Quaternion.identity);

		if (hitPlayerSplatter != null && collisionInfo.transform.root.tag == "Player")
			Instantiate(hitPlayerSplatter, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class ExplodeOnClick : MonoBehaviour
{
	public GameObject player;
	public float distance = 1f;

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var distance = Vector2.Distance(player.transform.position, pos);
			if (distance < this.distance)
			{
				ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
				ef.DoExplosion(pos);
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ExplosionForce : MonoBehaviour
{
    public float force = 0;
    public float radius = 0.1f;
    public float upliftModifer = 0;

    /// <summary>
    /// create an explosion force
    /// </summary>
    /// <param name="position">location of the explosion</param>
    public void DoExplosion(Vector3 position)
    {
        transform.localPosition = position;
        StartCoroutine(WaitAndExplode());
    }

    /// <summary>
    /// exerts an explosion force on all rigidbodies within the given radius
    /// </summary>
    /// <returns></returns>
	private IEnumerator WaitAndExplode()
    {
        yield return new WaitForFixedUpdate();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Wall"))
            {
                coll.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// adds explosion force to given rigidbody
    /// </summary>
    /// <param name="body">rigidbody to add force to</param>
    /// <param name="explosionForce">base force of explosion</param>
    /// <param name="explosionPosition">location of the explosion source</param>
    /// <param name="explosionRadius">radius of explosion effect</param>
    /// <param name="upliftModifier">factor of additional upward force</param>
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        baseForce.z = 0;
        body.AddForce(baseForce);

        if (upliftModifer != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnStart : MonoBehaviour
{
    private Explodable _explodable;

    // Start is called before the first frame update
    void Start()
    {
        _explodable = GetComponent<Explodable>();
		_explodable.Explode();
    }
}

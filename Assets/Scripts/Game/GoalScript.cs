using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public BoxCollider2D goal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You win");
    }
}

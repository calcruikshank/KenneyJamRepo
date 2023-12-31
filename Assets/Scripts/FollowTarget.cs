using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] public Transform target;

    private void Update()
    {
        if (target != null)
        {
            this.transform.position = new Vector3( target.transform.position.x, 100, target.transform.position.z);
        }
    }
}

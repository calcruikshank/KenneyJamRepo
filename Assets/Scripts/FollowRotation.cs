using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        if (target != null)
        {
            this.transform.rotation = target.rotation;
        }
    }
}

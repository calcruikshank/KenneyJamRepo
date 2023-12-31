using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chartable : MonoBehaviour
{
    private void Start()
    {
        this.transform.rotation = Quaternion.Euler(Vector3.zero);

        GameManager.singleton.allChartables.Add(this);
        transform.parent = null;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //prompt player other.GetComponent<PlayerController>().Chj();

            ChartOnMap();
        }
    }

    private void ChartOnMap()
    {
        if (!GameManager.singleton.activeChartables.Contains(this))
        {
            this.GetComponentInChildren<SpriteRenderer>().enabled = true;
            transform.localScale = Vector3.one * 3;

            GameManager.singleton.AddActiveChartable(this);
        }
    }
}

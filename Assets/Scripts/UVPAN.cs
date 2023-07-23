using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVPAN : MonoBehaviour
{

    public float Scrollx = 0.075f;
    public float Scrolly = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float OffsetX = Time.time * Scrollx;
        float OffsetY = Time.time * Scrolly;

        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX, OffsetY);


    }
}

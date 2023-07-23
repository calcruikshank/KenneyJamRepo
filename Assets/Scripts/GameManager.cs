using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager singleton;

    public List<Chartable> activeChartables = new List<Chartable>();

    [SerializeField] public Transform fullMapTransform;

    [SerializeField] Transform playerSprite;


    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateFullMap()
    {
        foreach (Chartable c in activeChartables)
        {
            c.transform.localScale = Vector3.one * 9 ;
        }
        fullMapTransform.gameObject.SetActive(true);
        
        playerSprite.localScale = Vector3.one * 9 * 2;
    }
    public void DeactivateFullMap()
    {
        fullMapTransform.gameObject.SetActive(false);

        playerSprite.localScale = Vector3.one * 3 * 2;
        foreach (Chartable c in activeChartables)
        {
            c.transform.localScale = Vector3.one * 3;
        }
    }
}

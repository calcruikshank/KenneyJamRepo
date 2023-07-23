using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager singleton;

    public List<Chartable> activeChartables = new List<Chartable>();
    public List<Chartable> allChartables = new List<Chartable>();

    [SerializeField] public Transform fullMapTransform;

    [SerializeField] Transform playerSprite;

    [SerializeField] TextMeshProUGUI completionText;
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

    public float percantageDone = 0;

    internal void AddActiveChartable(Chartable chartable)
    {
        activeChartables.Add(chartable);

        percantageDone = (float)activeChartables.Count / (float)allChartables.Count;
        percantageDone *= 100;
        completionText.text = "Map progress: " + (int)percantageDone + "%";
        Debug.Log(percantageDone + " all charts " + allChartables.Count + " active " + activeChartables.Count);
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

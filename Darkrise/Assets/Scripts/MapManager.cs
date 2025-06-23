using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject largeMap;

    public bool IsLargeMapOpen { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        CloseLargeMap();
    }

    private void Update()
    {
        if (true) // Condition for opening the map - replace this when ready
        {
            OpenLargeMap();
        }
        else
        {
            CloseLargeMap();
        }
    }

    private void OpenLargeMap()
    {
        miniMap.SetActive(false);
        largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }

    private void CloseLargeMap()
    {
        miniMap.SetActive(true);
        largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }
}

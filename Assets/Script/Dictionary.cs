using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dictionary : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference gripReference;
    [SerializeField]
    private InputActionReference thumbstickReference;
    [SerializeField]
    public GameObject dictionary;
    private int pageOpen = 0;
    private int pageClose = 0;

    [Serializable]
    class Page
    {
        [SerializeField] public GameObject page;
    }

    [SerializeField]
    List<Page> pages = new List<Page>();

    void Update()
    {
        float gripValue = gripReference.action.ReadValue<float>();
        if (gripValue != 0f)
        {
            dictionary.SetActive(true);
            Vector2 thumbstickValue = thumbstickReference.action.ReadValue<Vector2>();
            NextPage(thumbstickValue);
        }
        else
        {
            dictionary.SetActive(false);
        }
    }

    public void NextPage(Vector2 thumbstickValue)
    {
        if (thumbstickValue.x == 1)
        {
            pages[pageOpen].page.SetActive(false);
            if (pageOpen == 4)
                pageOpen = 0;
            else
                pageOpen++;
            pages[pageOpen].page.SetActive(true);
        }
        if (thumbstickValue.x == -1)
        {
            pages[pageOpen].page.SetActive(false);
            if (pageOpen == 0)
                pageOpen = 4;
            else
                pageOpen--;
            pages[pageOpen].page.SetActive(true);
        }
    }
}

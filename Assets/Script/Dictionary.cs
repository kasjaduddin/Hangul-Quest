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
    public GameObject dictionary;

    [Serializable]
    public class Page
    {
        [SerializeField] GameObject page;
    }

    [SerializeField]
    public List<Page> pages = new List<Page>();

    void Update()
    {
        float gripValue = gripReference.action.ReadValue<float>();
        if (gripValue != 0f)
        {
            dictionary.SetActive(true);
        }
        else
        {
            dictionary.SetActive(false);
        }
    }
}

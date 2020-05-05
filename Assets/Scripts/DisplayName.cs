using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayName : MonoBehaviour
{
    public String objectName;
    public Text myText;
    public float fadeTime;
    public bool displayInfo;

    private void Start()
    {
        myText = GameObject.Find("Text").GetComponent<Text>();
        myText.color = Color.clear;
    }

    private void Update()
    {
        FadeText();
    }

    private void OnMouseOver()
    {
        displayInfo = true;
    }

    private void OnMouseExit()
    {
        displayInfo = false;
    }
    void FadeText()
    {
        if (displayInfo)
        {
            myText.text = objectName;
            myText.color = Color.Lerp(myText.color, Color.white, fadeTime * Time.deltaTime);
        }
        else
        {
            myText.color = Color.Lerp(myText.color, Color.clear, fadeTime * Time.deltaTime);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingMenu : MonoBehaviour
{
    [NonSerialized]
    RawImage img;

    void Awake()
    {
        img = GetComponent<RawImage>();
    }

    void Update()
    {
        img.uvRect = new Rect(img.uvRect.x - Time.deltaTime * 0.01f, img.uvRect.y - Time.deltaTime * 0.01f, img.uvRect.width, img.uvRect.height);
    }
}

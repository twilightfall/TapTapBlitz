using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIAdjuster : MonoBehaviour
{
    [SerializeField] RectTransform gamePanel;
    [SerializeField] RectTransform safePanel;
    [SerializeField] RectTransform closeButton;

    public Rect safeArea;

    // Start is called before the first frame update
    void Start()
    {
        safeArea = Screen.safeArea;

        AdjustPanelHeight();
        SetSafeArea();
    }

    private void SetSafeArea()
    {
        if (safePanel != null)
        {
            safePanel.anchorMin = safeArea.position;
            safePanel.anchorMax = safeArea.position;
            safePanel.pivot = safeArea.position;
            safePanel.anchoredPosition = safeArea.position;
            safePanel.sizeDelta = new(Screen.safeArea.width, Screen.safeArea.height);
        }
    }

    void AdjustPanelHeight()
    {
        if (gamePanel != null)
        {
            print(Screen.width + " " + Screen.height);

            gamePanel.sizeDelta = new Vector2(gamePanel.sizeDelta.x, Screen.height / 3 - 50);
        }
    }
}

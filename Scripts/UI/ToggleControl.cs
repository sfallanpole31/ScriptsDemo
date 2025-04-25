using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Drawing;

public class ToggleControl : MonoBehaviour
{
    public RectTransform rect;  // UI 元件的 RectTransform
    private Vector2 originalPosition;
    private void Start()
    {
        originalPosition = rect.anchoredPosition;
    }

    public void OnValueChanged()
    {
        bool isOn = this.gameObject.GetComponent<Toggle>().isOn;

        float duration = 0.5f;

        Vector2 targetAnchor = isOn ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f);
        Vector2 targetPivot = isOn ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f);

        // Animate anchorMin
        DOTween.To(() => rect.anchorMin, x => rect.anchorMin = x, targetAnchor, duration).SetEase(Ease.OutQuad);

        // Animate anchorMax
        DOTween.To(() => rect.anchorMax, x => rect.anchorMax = x, targetAnchor, duration).SetEase(Ease.OutQuad);

        // Animate pivot
        DOTween.To(() => rect.pivot, x => rect.pivot = x, targetPivot, duration).SetEase(Ease.OutQuad);


    }

}

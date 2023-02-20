using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagOnDrag : MonoBehaviour, IDragHandler
{
    private RectTransform startRect;

    private void Awake()
    {
        startRect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        startRect.anchoredPosition += eventData.delta;//移动中心点
    }
}

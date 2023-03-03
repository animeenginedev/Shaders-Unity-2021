using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInfoText : MonoBehaviour
{
    public List<GameObject> displayElements;
    public TMPro.TextMeshProUGUI largeText;
    public TMPro.TextMeshProUGUI smallText;

    Canvas parent;
    UITooltip last_tooltip;

    public float showTimer = 0.25f;
    float showAccumulator = 0.0f;
    bool showing = false;
    bool display = false;

    public void Start()
    {
        Hide();

        GameObject p = transform.parent.gameObject; 
        while(p != null && p.GetComponent<Canvas>() == null)
        {
            p = transform.parent.gameObject;
        }
        if (p != null)
            parent = p.GetComponent<Canvas>();

    }

    public void Update()
    {
        if(showing)
        {
            //Follow Mouse
            var currentRect = GetComponent<RectTransform>();
            currentRect.anchoredPosition = Input.mousePosition + new Vector3(12, -12, 0);
            var canvasRect = parent.GetComponent<RectTransform>();

            float offsetX = canvasRect.sizeDelta.x - (currentRect.anchoredPosition.x + (currentRect.sizeDelta.x + 9));
            float offsetY = currentRect.anchoredPosition.y - (currentRect.sizeDelta.y + 9);
            if(offsetX < 0 || offsetY < 0)
            {
                currentRect.anchoredPosition += new Vector2( Mathf.Min(offsetX, 0), - Mathf.Min(offsetY, 0));
            }
            
            
        }
        else if(display)
        {
            showAccumulator += Time.deltaTime;
            if(showAccumulator >= showTimer)
            {
                Show();
            }
        }

        if(EventSystem.current.IsPointerOverGameObject())
        {
            var eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

            //Create a list of Raycast Results
            List<RaycastResult> _hits = new List<RaycastResult>();

            // Get all existing graphicsRaycaster
            // NOTE: Of course you should rather cache this result e.g. in Awake
            // and only update it if a Canvas is added to or removed from the scene
            var graphicsRaycasters = FindObjectsOfType<GraphicRaycaster>();

            foreach (var graphicsRaycaster in graphicsRaycasters)
            {
                graphicsRaycaster.Raycast(eventData, _hits);
            }

            // Do something with the hits
            foreach (var result in _hits)
            {
                var tooltipObj = result.gameObject.GetComponent<UITooltip>();

                if (tooltipObj == null)
                    continue;
                if (tooltipObj == last_tooltip)
                    return;

                last_tooltip = tooltipObj;
                if (!display)
                {
                    PrepareShow();
                }

                Setup(tooltipObj);
                return;
            }

            Hide();
        } 
        else if(display)
        {
            Hide();
        }
    }

    public void Setup(UITooltip target)
    {
        largeText.SetText(target.GetTitleText());
        smallText.SetText(target.GetDescriptionText());
    }


    void Hide()
    {
        showing = false;
        display = false;
        last_tooltip = null;
        for (int i = 0; i < displayElements.Count; i++)
        {
            displayElements[i].SetActive(false);
        }
    }

    void PrepareShow()
    {
        showing = false;
        display = true;
        showAccumulator = 0.0f;
    }

    void Show()
    {
        showing = true;
        showAccumulator = 0.0f;
        for (int i = 0; i < displayElements.Count; i++)
        {
            displayElements[i].SetActive(true);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }


}

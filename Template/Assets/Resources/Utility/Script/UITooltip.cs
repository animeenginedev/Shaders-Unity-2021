
using UnityEngine;

public class UITooltip : MonoBehaviour
{
    public string text;
    public string desc;

    public string GetTitleText()
    {
        return text;
    }
    public string GetDescriptionText()
    {
        return desc;
    }
}
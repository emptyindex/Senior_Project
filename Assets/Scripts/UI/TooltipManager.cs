using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    public static Tooltip tooltip;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        tooltip = instance.transform.Find("TooltipBox").GetComponent<Tooltip>();

        tooltip.gameObject.SetActive(false);
    }

    public static void Show(string header = "")
    {
        tooltip.SetText(header);
        tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        tooltip.gameObject.SetActive(false);
    }
}

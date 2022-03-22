using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI header;

    public void SetText(string newHeaderText)
    {
        header.text = newHeaderText;
    }

    private void Update()
    {
        Vector2 position = Input.mousePosition;
        transform.position = position;
    }
}

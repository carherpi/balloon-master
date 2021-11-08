using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberField : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // empty numbers for startup
        this.gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddNumber(string number)
    {
        TextMeshProUGUI textMesh = this.gameObject.GetComponent<TextMeshProUGUI>();
        int lenText = textMesh.text.Length;
        if (lenText < 4)
        {
            textMesh.text += number;
        }
    }
    public void DeleteLastNumber()
    {
        TextMeshProUGUI textMesh = this.gameObject.GetComponent<TextMeshProUGUI>();
        int lenText = textMesh.text.Length;
        if (lenText > 0)
        {
            textMesh.text = textMesh.text.Remove(lenText - 1);
        }
    }
}

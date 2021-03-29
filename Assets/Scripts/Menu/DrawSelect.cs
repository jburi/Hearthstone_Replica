using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSelect : MonoBehaviour
{
    public Color defaultColor;
    public Color selectedColor;
    public Image[] ccTemplates;
    public Text[] texts;
    public bool selected;

    // Start is called before the first frame update
    private void Start()
    {
        defaultColor = Color.white;
        selectedColor = Color.grey;
        ccTemplates = gameObject.GetComponentsInChildren<Image>();
        texts = gameObject.GetComponentsInChildren<Text>();
        selected = true;
    }

    public void Select()
    {
        if (selected == true)
        {
            for (int i = 0; i < ccTemplates.Length; i++)
            {
                ccTemplates[i].color = selectedColor;
            }
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = selectedColor;
            }
            selected = false;
        }
        else
        {
            for (int i = 0; i < ccTemplates.Length; i++)
            {
                ccTemplates[i].color = defaultColor;
            }
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = defaultColor;
            }
            selected = true;
        }
    }

    
}

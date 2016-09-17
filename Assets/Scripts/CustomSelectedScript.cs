using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomSelectedScript : MonoBehaviour
{
    public Color NormalColor;
    public Color SelectedColor;

    private int SelectedIndex = -1;
    private string SelectedName = null;

    public GameObject Container;
    private GameObject _selected = null;

    public void OnSelect()
    {
        if (_selected != null)
            SelectedDisable(_selected);
        SelectedEnable(gameObject);
    }


    private void SelectedEnable(GameObject element)
    {
        var panel = element.transform.GetChild(0).gameObject;
        panel.GetComponent<Image>().color = NormalColor; 
    }

    private void SelectedDisable(GameObject element)
    {
        var childs = transform.childCount; 
        var panel = element.transform.GetChild(0).gameObject;
        panel.GetComponent<Image>().color  = SelectedColor;
    }

    
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectedManager : MonoBehaviour
{

    public Color SelectedColor;
    public Color NormalColor;
    private GameObject SelectedObject = null; 
    public GameObject Selected
    {
        get { return SelectedObject; }
        set
        {
            if (SelectedObject != null)
                SelectDisable(SelectedObject);
            SelectEnable(value);
            SelectedObject = value;
        }
    }

    private void SelectEnable(GameObject element)
    {
        element.GetComponent<Image>().color = SelectedColor; 
    }

    private void SelectDisable(GameObject element)
    {
        element.GetComponent<Image>().color = NormalColor;
    }
}

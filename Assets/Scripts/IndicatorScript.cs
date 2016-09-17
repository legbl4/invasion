using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IndicatorScript : MonoBehaviour
{
    public GameObject TextComponent;
    public float DefaultValue;
    public string Mask; 
    

    private Text _currentTextComponent; 
	void Start () {
	    if(TextComponent == null)
            throw new UnityException("Text component not exist. Please set text component");
	    _currentTextComponent = TextComponent.GetComponent<Text>(); 
        if(_currentTextComponent == null)
            throw new UnityException("Invalid text component. Not contain text script");
	}


    public void UpdateText(string text, Color textColor)
    {
        if (_currentTextComponent == null)
            _currentTextComponent = TextComponent.GetComponent<Text>(); 
        _currentTextComponent.color = textColor; 
        _currentTextComponent.text = Mask + (Mask == "" ? "" :  "\n") + text; 
    }
}

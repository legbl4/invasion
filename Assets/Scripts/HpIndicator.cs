using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HpIndicator : MonoBehaviour
{
    public GameObject HpLineSlider;
    public GameObject HpCountLabel;

    public int SetHp
    {
        set
        {
            var slider = HpLineSlider.GetComponent<Slider>();
            slider.value = value / 100.0f;

            var countLabel = HpCountLabel.GetComponent<Text>();
            countLabel.text = value.ToString();

            if (value > 70)
                countLabel.color = Color.green;
            if (value > 20 && value <= 70)
                countLabel.color = Color.yellow;
            if (value <= 20)
                countLabel.color = Color.red;
        }
    }
}

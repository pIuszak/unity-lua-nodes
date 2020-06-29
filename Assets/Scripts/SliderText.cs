using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderText : MonoBehaviour
{
    public Text Text;
    public Slider Slider;
    void Update()
    {
        Text.text = Slider.value.ToString();
    }
}

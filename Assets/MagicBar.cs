using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxMagic(int magicPoints)
    {
        slider.maxValue = magicPoints;
        slider.value = magicPoints;
    }

    public void SetMagic(int magicPoints)
    {
        slider.value = magicPoints;
    }
}

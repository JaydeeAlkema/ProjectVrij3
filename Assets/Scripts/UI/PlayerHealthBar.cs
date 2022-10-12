using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHP(float maxHP)
	{
        slider.maxValue = maxHP;
        slider.value = maxHP;
	}

    public void SetHP(float HP)
	{
        slider.value = HP;
	}

}

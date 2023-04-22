using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveBar : MonoBehaviour
{
    public Slider slider;
    //public Gradient gradient;
    //public Image fill;
    public Text text;
    public EnemySpawnManager enemy;

    public void SetMaxWave(float wave)
    {
        slider.maxValue = wave;
        slider.value = wave;
        //fill.color = gradient.Evaluate(1f);
        text.text = 1.ToString();
    }

    public void SetWave(float wave)
    {
        slider.value = wave;
        //fill.color = gradient.Evaluate(slider.normalizedValue);
        text.text = wave.ToString();
    }
}

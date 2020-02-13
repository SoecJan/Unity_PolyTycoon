using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarChartValueView : MonoBehaviour
{
    [SerializeField] private TMP_Text _xValueLabelText;
    [SerializeField] private Image[] _yValueTransforms;
    private BarChartValue _barChartValue;
    private int _maximumHeight;
    private int _maximumValue;

    public BarChartValue BarChartValue
    {
        get => _barChartValue;
        set => _barChartValue = value;
    }

    public int MaximumHeight
    {
        get => _maximumHeight;
        set => _maximumHeight = value;
    }

    public int MaximumValue
    {
        get => _maximumValue;
        set => _maximumValue = value;
    }

    public void SetXValue(string value)
    {
        _barChartValue.Label = value;
    }

    public void SetYValue(int[] values)
    {
        _barChartValue.Values = values;
    }

    public void SetColors(Color[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            _yValueTransforms[i].color = colors[i];
        }
    }

    /// <summary>
    /// The maximum height (height of the canvas) a chart can have
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxHeight(int maxValue)
    {
        _maximumHeight = maxValue;
    }

    /// <summary>
    /// The highest value of the displayed bar chart
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxValue(int value)
    {
        this._maximumValue = value;
    }

    public void UpdateChart()
    {
        for (int i = 0; i < _barChartValue.Values.Length; i++)
        {
            Vector2 sizeDelta = _yValueTransforms[i].rectTransform.sizeDelta;
            sizeDelta.y = Math.Abs((_barChartValue.Values[i] / (float)_maximumValue) * _maximumHeight);
            _yValueTransforms[i].rectTransform.sizeDelta = sizeDelta;
        }
    }
}
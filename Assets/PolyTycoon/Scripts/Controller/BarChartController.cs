using System;
using System.Collections.Generic;
using UnityEngine;

public class BarChartController
{
    private List<BarChartValue> _barChartValues;
    public System.Action<BarChartValue> OnValueChange;

    public BarChartController(MoneyController moneyController)
    {
        BarChartValues = new List<BarChartValue> {new BarChartValue("0", 0, 0)};
        moneyController.OnValueChange += delegate(long oldValue, long newValue)
        {
            long difference = newValue - oldValue;
            BarChartValue barChartValue = BarChartValues[BarChartValues.Count - 1];
            int currentValue = barChartValue.Values[difference < 0 ? 1 : 0];
            barChartValue.Values[difference < 0 ? 1 : 0] = (int) difference + currentValue;
        };

        TimeScaleView._onDayOver += delegate(int day)
        {
            OnValueChange?.Invoke(BarChartValues[BarChartValues.Count - 1]);
            BarChartValues.Add(new BarChartValue((day + 1).ToString(), 0, 0));
        };
    }

    public List<BarChartValue> BarChartValues
    {
        get => _barChartValues;
        set => _barChartValues = value;
    }
}

public struct BarChartValue
{
    private string label;
    private int[] values;

    public BarChartValue(string label, params int[] valueLength)
    {
        this.label = label;
        this.values = valueLength;
    }

    public BarChartValue(string label, int valueLength)
    {
        this.label = label;
        this.values = new int[valueLength];
    }

    public string Label
    {
        get => label;
        set => label = value;
    }

    public int[] Values
    {
        get => values;
        set => values = value;
    }
}
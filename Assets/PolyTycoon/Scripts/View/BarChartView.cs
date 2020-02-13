using System;
using System.Collections.Generic;
using UnityEngine;

public class BarChartView : MonoBehaviour
{
    private BarChartValueView _barChartValueViewPrefab;
    private List<BarChartValueView> _barChartValueElements;
    private BarChartController _barChartController;
    [SerializeField] private RectTransform _graphTransform;
    [SerializeField] private Color[] _colors;
    private int _maxDisplayedValue;

    void Start()
    {
        MoneyController moneyController = FindObjectOfType<MoneyUiView>().MoneyController;
        _barChartController = new BarChartController(moneyController);

        _barChartValueViewPrefab = Resources.Load<BarChartValueView>(PathUtil.Get("BarChartValueView"));
        _barChartValueElements = new List<BarChartValueView>();
        _barChartController.OnValueChange += OnValueEmitted;
    }

    void OnValueEmitted(BarChartValue barChartValue)
    {
        foreach (int value in barChartValue.Values)
        {
            if (Math.Abs(value) > _maxDisplayedValue) _maxDisplayedValue = Math.Abs(value);
        }
        BarChartValueView barChartValueView = Instantiate(_barChartValueViewPrefab, _graphTransform);
        barChartValueView.SetXValue(barChartValue.Label);
        barChartValueView.SetYValue(barChartValue.Values);
        barChartValueView.SetColors(_colors);
        _barChartValueElements.Add(barChartValueView);

        foreach (BarChartValueView barChartValueElement in _barChartValueElements)
        {
            barChartValueElement.SetMaxHeight((int) _graphTransform.rect.height);
            barChartValueElement.SetMaxValue(_maxDisplayedValue);
            barChartValueElement.UpdateChart();
        }
    }
}
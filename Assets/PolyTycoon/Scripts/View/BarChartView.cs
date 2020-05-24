using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarChartView : MonoBehaviour
{
    [SerializeField] private Canvas _visibleObject;
    [SerializeField] private Button _visibilityToggle;
    [SerializeField] private Button _closeButton;
    private BarChartValueView _barChartValueViewPrefab;
    private List<BarChartValueView> _barChartValueElements;
    private BarChartController _barChartController;
    [SerializeField] private RectTransform _graphTransform;
    [SerializeField] private Color[] _colors;
    private int _maxDisplayedValue;

    [SerializeField] private RectTransform _maxValueLineTransform;
    [SerializeField] private TMP_Text _maxValueLineText;

    void Start()
    {
        MoneyController moneyController = FindObjectOfType<MoneyUiView>().MoneyController;
        _barChartController = new BarChartController(moneyController);

        _barChartValueViewPrefab = Resources.Load<BarChartValueView>(PathUtil.Get("BarChartValueView"));
        _barChartValueElements = new List<BarChartValueView>();
        _barChartController.OnValueChange += OnValueEmitted;
        
        _visibilityToggle.onClick.AddListener(delegate { _visibleObject.enabled = !_visibleObject.enabled; });
        _closeButton.onClick.AddListener(delegate { _visibleObject.enabled = false; });
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
        
        _maxValueLineTransform.anchoredPosition = new Vector2(0, (int) _graphTransform.rect.height);
        _maxValueLineText.text = _maxDisplayedValue + "€";
    }
}
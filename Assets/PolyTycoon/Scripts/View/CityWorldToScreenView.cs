using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityWorldToScreenView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _visibleGameObject;

    [SerializeField] private ProductView _productViewPrefab;
    [SerializeField] private Transform _productViewParent;

    private CityPlaceable _cityPlaceable;
    private Animator _animator;

    private void Start()
    {
        this._animator = GetComponent<Animator>();
        this._visibleGameObject.SetActive(false);
    }

    public string Text
    {
        get { return _text.text; }
        set { _text.text = value; }
    }

    public CityPlaceable City
    {
        set
        {
            _cityPlaceable = value;
            Text = value.name;
            UpdateProductUi();
            this._cityPlaceable.onVisibilityChange += isVisible =>
            {
                if (!this._visibleGameObject) return;
                this._visibleGameObject.SetActive(isVisible);
            };
        }
    }

    private void UpdateProductUi()
    {
        foreach (ProductData productData in _cityPlaceable.ReceivedProductList())
        {
            ProductView productView = Instantiate(_productViewPrefab, _productViewParent);
            Debug.Log(productView.ProductButton);
            productView.ProductButton.onClick.AddListener(OnClick);
            productView.ProductData = productData;
            TooltipText tooltipText = productView.gameObject.AddComponent<TooltipText>();
            tooltipText.Text = productData.ProductName;
            tooltipText.TimeUntilDisplay = 0.2f;
        }
    }

    private void OnClick()
    {
        FindObjectOfType<GameHandler>().BuildingManager.OnPlaceableClick(_cityPlaceable.MainBuilding);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetBool("MouseOver", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool("MouseOver", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
}
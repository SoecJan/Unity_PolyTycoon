using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityWorldToScreenUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
            Text = value.BuildingName;
            StartCoroutine(UpdateProductUi(value));
        }
    }

    private IEnumerator UpdateProductUi(CityPlaceable city)
    {
        yield return new WaitForSeconds(1);
        foreach (ProductData productData in city.ReceivedProductList())
        {
            ProductView productView = Instantiate(_productViewPrefab, _productViewParent);
            productView.ProductButton.onClick.AddListener(OnClick);
            productView.ProductData = productData;
            TooltipText tooltipText = productView.gameObject.AddComponent<TooltipText>();
            tooltipText.Text = productData.ProductName;
            tooltipText.TimeUntilDisplay = 0.2f;
        }
    }

    private void OnClick()
    {
        FindObjectOfType<PlacementManager>().BuildingManager.OnPlaceableClick(_cityPlaceable.MainBuilding);
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
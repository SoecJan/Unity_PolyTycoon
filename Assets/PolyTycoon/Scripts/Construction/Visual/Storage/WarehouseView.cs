using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseView : AbstractUi
{
    private Warehouse _warehouse;
    private Coroutine _updateUiCoroutine;
    [SerializeField] private Button _exitButton;
    [SerializeField] private NeededProductView _storedProductView;
    [SerializeField] private RectTransform _scrollView;
    
    public Warehouse Warehouse
    {
        get { return _warehouse; }
        set
        {
            _warehouse = value;
            if (!_warehouse)
            {
                SetVisible(false);
                return;
            }

            foreach (ProductStorage productStorage in _warehouse.StoredProducts().Values)
            {
                NeededProductView neededProductView = Instantiate(_storedProductView, _scrollView);
                neededProductView.ProductData = productStorage.StoredProductData;
                neededProductView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
            }
            
            SetVisible(true);
            if (_updateUiCoroutine == null)
                _updateUiCoroutine = StartCoroutine(UpdateUi());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _exitButton.onClick.AddListener(delegate
        {
            SetVisible(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateUi()
    {
        while (VisibleObject.activeSelf)
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                NeededProductView neededProductView = _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                neededProductView.NeededAmountText.text = _warehouse.StoredProducts()[neededProductView.ProductData].Amount + "/" +
                                                          _warehouse.StoredProducts()[neededProductView.ProductData].MaxAmount;
            }
            yield return new WaitForSeconds(0.1f);
        }
        _updateUiCoroutine = null;
    }
    
    

    public override void Reset()
    {
        for (int i = 0; i < _scrollView.childCount; i++)
        {
            Destroy(_scrollView.GetChild(i).gameObject);
        }
    }
}

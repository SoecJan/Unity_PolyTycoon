using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageContainerView : AbstractUi
{
    private AbstractStorageContainer _storageContainer;
    private Coroutine _updateUiCoroutine;
    [SerializeField] private Button _exitButton;
    [SerializeField] private NeededProductView _storedProductView;
    [SerializeField] private RectTransform _scrollView;
    
    public AbstractStorageContainer StorageContainer
    {
        set
        {
            _storageContainer = value;
            if (!_storageContainer)
            {
                SetVisible(false);
                return;
            }

            foreach (ProductStorage productStorage in _storageContainer.StoredProducts().Values)
            {
                NeededProductView neededProductView = Instantiate(_storedProductView, _scrollView);
                neededProductView.ProductData = productStorage.StoredProductData;
                neededProductView.Text(productStorage);
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

    IEnumerator UpdateUi()
    {
        while (VisibleObject.activeSelf)
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                NeededProductView neededProductView = _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                neededProductView.Text(_storageContainer.StoredProducts()[neededProductView.ProductData]);
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

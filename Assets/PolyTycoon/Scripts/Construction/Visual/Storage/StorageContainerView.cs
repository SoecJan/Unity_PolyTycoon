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
        // Update while visible
        while (VisibleObject.activeSelf)
        {
            // Add view on change
            if (_scrollView.childCount < _storageContainer.EmittedProductList().Count)
            {
                List<ProductData> copy = _storageContainer.EmittedProductList();
                // Remove entries from the copy
                for (int i = 0; i < _scrollView.childCount; i++)
                {
                    NeededProductView neededProductView =
                        _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                    copy.Remove(neededProductView.ProductData);
                }
                // Instantiate all entries of the copy
                foreach (ProductData productData in copy)
                {
                    NeededProductView neededProductView = Instantiate(_storedProductView, _scrollView);
                    neededProductView.ProductData = productData;
                    neededProductView.Text(_storageContainer.EmitterStorage(productData));
                }
            }
            
            // Update existing views
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                NeededProductView neededProductView = _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                neededProductView.Text(_storageContainer.EmitterStorage(neededProductView.ProductData));
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

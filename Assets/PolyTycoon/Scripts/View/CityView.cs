﻿using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;


public class CityView : AbstractUi
{
	#region Attributes
	private ICityBuilding _cityBuilding;
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;

	[Header("UI")] 
	[SerializeField] private TextMeshProUGUI _titleText;

	private string _defaultTitleText;
	[SerializeField] private RectTransform _neededProductScrollView;
	[SerializeField] private RectTransform _producedProductScrollView;
	[SerializeField] private AmountProductView _productUiPrefab;
	[SerializeField] private Slider _slider;
	#endregion

	#region Getter & Setter
	public ICityBuilding CityBuilding {
		private get => _cityBuilding;

		set {
			if (_cityBuilding == value) return;
			// if (_cityBuilding != null) CityBuilding.CityPlaceable.Outline.enabled = false;
			_cityBuilding = value;
			Reset();
			if (_cityBuilding == null)
			{
				SetVisible(false);
				return;
			}

			Debug.Log(CityBuilding);
			Debug.Log(CityBuilding.CityPlaceable);
			// Debug.Log(CityBuilding.CityPlaceable.Outline);
			// Destroy(CityBuilding.CityPlaceable.Outline);
			// CityBuilding.CityPlaceable.Outline = new Outline();
			// CityBuilding.CityPlaceable.Outline.enabled = true;
			_titleText.text = CityBuilding.CityPlaceable.name + "\nLevel: " + CityBuilding.CityPlaceable.Level;
			
			IProductReceiver cityPlaceable = ((IProductReceiver) CityBuilding.CityPlaceable);
			foreach (ProductData neededProduct in cityPlaceable.ReceivedProductList())
			{
				AmountProductView amountProductView = GameObject.Instantiate(_productUiPrefab, _neededProductScrollView);
				amountProductView.ProductData = neededProduct;
				amountProductView.Text(cityPlaceable.ReceiverStorage(neededProduct));
			}

			IProductEmitter productEmitter = (IProductEmitter) CityBuilding.CityPlaceable;
			foreach (ProductData productData in productEmitter.EmittedProductList())
			{
				AmountProductView producedProductView = GameObject.Instantiate(_productUiPrefab, _producedProductScrollView);
				producedProductView.ProductData = productData;
				producedProductView.Text(productEmitter.EmitterStorage(productData));
			}

			StartCoroutine(UpdateUi());
			SetVisible(true);
		}
	}
	#endregion

	#region Methods
	private void Start()
	{
		_exitButton.onClick.AddListener(delegate { CityBuilding = null; });
		_defaultTitleText = _titleText.text;
	}

	/// <summary>
	/// Coroutine that updates the CityPlaceable UI
	/// </summary>
	/// <returns></returns>
	private IEnumerator UpdateUi()
	{
		while (_cityBuilding != null)
		{
			CityPlaceable cityPlaceable = _cityBuilding.CityPlaceable;
			for (int i = 0; i < _neededProductScrollView.childCount; i++)
			{
				AmountProductView productView = _neededProductScrollView.transform.GetChild(i).GetComponent<AmountProductView>();
				ProductStorage productStorage = ((IProductReceiver)cityPlaceable).ReceiverStorage(productView.ProductData);
				productView.Text(productStorage);
			}
			for (int i = 0; i < _producedProductScrollView.childCount; i++)
			{
				AmountProductView productView = _producedProductScrollView.transform.GetChild(i).GetComponent<AmountProductView>();
				ProductStorage productStorage = ((IProductEmitter)cityPlaceable).EmitterStorage();
				productView.Text(productStorage);
			}

			_slider.value = CityPlaceable.GetLevelFromExp(cityPlaceable.ExperiencePoints, cityPlaceable.Level) / (cityPlaceable.Level + 1f);
			yield return new WaitForSeconds(1);
		}
	}

	public override void Reset()
	{
		for (int i = 0; i < _neededProductScrollView.childCount; i++)
		{
			Destroy(_neededProductScrollView.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < _producedProductScrollView.childCount; i++)
		{
			Destroy(_producedProductScrollView.transform.GetChild(i).gameObject);
		}

		_titleText.text = _defaultTitleText;
	}
	#endregion
}

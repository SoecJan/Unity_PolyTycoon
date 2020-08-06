
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouteElementView : MonoBehaviour
{
	private TransportRouteElement _transportRouteElement;
	public static System.Action<RouteElementView> _onDelete; 
	
	[SerializeField] private TMP_Text _stationName;
	[SerializeField] private Button _stationRemoveButton;
	[SerializeField] private RouteElementActionView _actionPrefab;
	[SerializeField] private RectTransform _actionParent;
	[SerializeField] private Button _actionAddButton;

	public TransportRouteElement TransportRouteElement
	{
		get => _transportRouteElement;
		set
		{
			_transportRouteElement = value;
			_stationName.text = _transportRouteElement.FromNode.name;
		}
	}

	private void Start()
	{
		_stationRemoveButton.onClick.AddListener(delegate
		{
			_onDelete?.Invoke(this);
		});
		_actionAddButton.onClick.AddListener(delegate
		{
			RouteElementActionView actionObj = Instantiate(_actionPrefab, _actionParent);
			actionObj._onDelete += delegate(RouteElementActionView actionElement)
				{
					_transportRouteElement.RouteSettings.Remove(actionElement.RouteSetting);
					Destroy(actionElement.gameObject);
				};
			List<ProductData> emittedProducts = _transportRouteElement.FromNode.GetComponent<IProductEmitter>().EmittedProductList();
			List<ProductData> receivedProducts = _transportRouteElement.FromNode.GetComponent<IProductReceiver>().ReceivedProductList();
			TransportRouteSetting transportRouteSetting = new TransportRouteSetting
			{
				Amount = 1,
				WaitStatus = TransportRouteSetting.RouteSettingWaitStatus.DONTWAIT,
				IsLoad = emittedProducts.Count > 0,
				ProductData = emittedProducts.Count > 0 ? emittedProducts[0] :
					receivedProducts.Count > 0 ? receivedProducts[0] : null
			};
			actionObj.RouteSetting = transportRouteSetting;
			actionObj.RouteElement = _transportRouteElement;
			_transportRouteElement.RouteSettings.Add(transportRouteSetting);
			actionObj.transform.SetSiblingIndex(actionObj.transform.GetSiblingIndex()-1);
		});
	}
}

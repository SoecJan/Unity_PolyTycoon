using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TransportVehicle : Vehicle
{
	#region Attributes
	private TransportRoute _transportRoute;
	private int _totalCapacity = 4;
	private static System.Action<TransportVehicle> _onClickAction;
	[SerializeField] private Dictionary<ProductData, ProductStorage> _loadedProducts;
	[SerializeField] private int _unloadSpeed;
	#endregion

	#region Getter & Setter
	public int TotalCapacity {
		get { return _totalCapacity; }
	}

	public int UnloadSpeed {
		get {
			return _unloadSpeed;
		}

		set {
			_unloadSpeed = value;
		}
	}

	public TransportRoute TransportRoute {
		get {
			return _transportRoute;
		}

		set {
			_transportRoute = value;
			List<Path> vehiclePathList = new List<Path>();
			_loadedProducts = new Dictionary<ProductData, ProductStorage>();
			foreach (TransportRouteElement element in _transportRoute.TransportRouteElements)
			{
				vehiclePathList.Add(element.Path);
			}
			base.PathList = vehiclePathList;
		}
	}

	public Action<TransportVehicle> OnClickAction {
		get {
			return _onClickAction;
		}

		set {
			_onClickAction = value;
		}
	}

	public Dictionary<ProductData, ProductStorage> LoadedProducts {
		get {
			return _loadedProducts;
		}
	}
	#endregion

	#region Methods

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			OnClickAction(this);
		}
	}

	protected override void OnArrive()
	{

		StartCoroutine(HandleLoad());
	}

	private IEnumerator HandleLoad() // TODO: Missing: Maximum Amounts of Storage
	{
		TransportRouteElement element = _transportRoute.TransportRouteElements[(RouteIndex + 1) % _transportRoute.TransportRouteElements.Count];

		IConsumer consumer = (IConsumer)element.FromNode;
		foreach (TransportRouteSetting setting in element.RouteSettings)
		{
			if (setting.IsLoad || !consumer.NeededProducts().ContainsKey(setting.ProductData)) continue;
			Debug.Log(setting.ProductData);
			while (LoadedProducts[setting.ProductData].Amount > 0)
			{
				LoadedProducts[setting.ProductData].Amount -= 1;
				consumer.NeededProducts()[setting.ProductData].Amount += 1;
				yield return new WaitForSeconds(_unloadSpeed);
				if (LoadedProducts[setting.ProductData].Amount == 0 || consumer.NeededProducts()[setting.ProductData].Amount == consumer.NeededProducts()[setting.ProductData].MaxAmount) break;
			}
		}

		IProducer producer = element.FromNode as IProducer;
		if (producer != null)
		{
			foreach (TransportRouteSetting setting in element.RouteSettings)
			{
				if (!setting.IsLoad || !setting.ProductData.Equals(producer.ProductStorage().StoredProductData)) continue;
				int loadAmount = setting.Amount;
				while (loadAmount > 0)
				{
					while (producer.ProductStorage().Amount == 0) { yield return new WaitForSeconds(0.1f); }
					producer.ProductStorage().Amount -= 1;
					if (!LoadedProducts.ContainsKey(setting.ProductData)) LoadedProducts.Add(setting.ProductData, new ProductStorage(setting.ProductData) { MaxAmount = TransportRoute.Vehicle.TotalCapacity, Amount = 0 });
					LoadedProducts[setting.ProductData].Amount += 1;
					loadAmount--;
					yield return new WaitForSeconds(_unloadSpeed);
					if (LoadedProducts[setting.ProductData].Amount == LoadedProducts[setting.ProductData].MaxAmount) break;
				}
			}
		}
		base.OnArrive();
	}
	#endregion
}
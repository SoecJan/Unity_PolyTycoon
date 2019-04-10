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
		// Unload Products
		TransportRouteElement element = _transportRoute.TransportRouteElements[(RouteIndex + 1) % _transportRoute.TransportRouteElements.Count];
		IConsumer consumer = element.FromNode as IConsumer;
		if (consumer != null)
		{
			foreach (TransportRouteSetting setting in element.RouteSettings)
			{
				if (setting.IsLoad || !consumer.NeededProducts().ContainsKey(setting.ProductData) || !LoadedProducts.ContainsKey(setting.ProductData)) continue;
				
				int unloadAmount = setting.Amount;
				while (unloadAmount > 0 
				       && LoadedProducts[setting.ProductData].Amount > 0
				       && consumer.NeededProducts()[setting.ProductData].Amount < consumer.NeededProducts()[setting.ProductData].MaxAmount)
				{
					unloadAmount--;
					LoadedProducts[setting.ProductData].Amount -= 1;
					consumer.NeededProducts()[setting.ProductData].Amount += 1;
					yield return new WaitForSeconds(_unloadSpeed);
				}
			}
		}
		
		// Load Products
		IProducer producer = element.FromNode as IProducer;
		if (producer != null)
		{
			foreach (TransportRouteSetting setting in element.RouteSettings)
			{
				if (!setting.IsLoad) continue;
				if (!setting.ProductData.Equals(producer.ProducedProductStorage().StoredProductData)) continue;
				
				if (!LoadedProducts.ContainsKey(setting.ProductData))
				{
					LoadedProducts.Add(setting.ProductData, new ProductStorage(setting.ProductData) 
						{ MaxAmount = TransportRoute.Vehicle.TotalCapacity, Amount = 0 });
				}
				
				int loadAmount = setting.Amount;
				while (loadAmount > 0 
				       && LoadedProducts[setting.ProductData].Amount != LoadedProducts[setting.ProductData].MaxAmount)
				{
					loadAmount--;
					while (producer.ProducedProductStorage().Amount == 0) 
						{ yield return new WaitForSeconds(0.1f); }
					producer.ProducedProductStorage().Amount -= 1;
					LoadedProducts[setting.ProductData].Amount += 1;
					yield return new WaitForSeconds(_unloadSpeed);
				}
			}
		}
		
		// Storage loading/unloading
		IStore storage = element.FromNode as IStore;
		if (storage != null)
		{
			// Unload to storage
			foreach (TransportRouteSetting setting in element.RouteSettings)
			{
				if (setting.IsLoad) continue;
				if (!LoadedProducts.ContainsKey(setting.ProductData)) continue;
				if (!(LoadedProducts[setting.ProductData].Amount > 0)) continue;
				
				if (!storage.StoredProducts().ContainsKey(setting.ProductData))
				{
					storage.StoredProducts().Add(setting.ProductData, new ProductStorage(setting.ProductData) 
						{ MaxAmount = TransportRoute.Vehicle.TotalCapacity, Amount = 0 });
				}
				
				int unloadAmount = setting.Amount;
				while (unloadAmount > 0 && LoadedProducts[setting.ProductData].Amount > 0)
				{
					unloadAmount--;
					LoadedProducts[setting.ProductData].Amount -= 1;
					storage.StoredProducts()[setting.ProductData].Amount += 1;
					yield return new WaitForSeconds(_unloadSpeed);
				}
			}
			
			// Load from storage
			foreach (TransportRouteSetting setting in element.RouteSettings)
			{
				if (!setting.IsLoad) continue;
				if (!storage.StoredProducts().ContainsKey(setting.ProductData)) continue;
				if (!(storage.StoredProducts()[setting.ProductData].Amount > 0)) continue;
				if (!LoadedProducts.ContainsKey(setting.ProductData))
				{
					LoadedProducts.Add(setting.ProductData, new ProductStorage(setting.ProductData) 
						{ MaxAmount = TransportRoute.Vehicle.TotalCapacity, Amount = 0 });
				}
				
				int loadAmount = setting.Amount;
				while (loadAmount > 0 
				       && storage.StoredProducts()[setting.ProductData].Amount > 0 
				       && LoadedProducts[setting.ProductData].Amount != LoadedProducts[setting.ProductData].MaxAmount)
				{
					loadAmount--;
					storage.StoredProducts()[setting.ProductData].Amount -= 1;
					LoadedProducts[setting.ProductData].Amount += 1;
					yield return new WaitForSeconds(_unloadSpeed);
				}
			}
		}
		base.OnArrive();
	}
	#endregion
}
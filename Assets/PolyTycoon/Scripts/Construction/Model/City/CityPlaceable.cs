using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface ICityPlaceable
{
	CityMainBuilding MainBuilding { get; set; }

	/// <summary>
	/// Count of people living in this cityPlaceable.
	/// </summary>
	/// <returns></returns>
	int CurrentInhabitantCount();

	Path PathTo(PathFindingNode targetNode);
	void AddPath(PathFindingNode targetNode, Path path);
	void RemovePath(PathFindingNode targetNode);
	void Rotate(Vector3 axis, float rotationAmount);
}

/// <summary>
///  Holds a reference to all buildings contained in this cityPlaceable. 
/// </summary>
public class CityPlaceable : ComplexMapPlaceable, IProductReceiver, IProductEmitter, IPathNode, ICityPlaceable
{
	#region Attributes
	private Dictionary<ProductData, ProductStorage> _receivedProducts;
	private List<ProductStorage> _emittedProducts;
	
	[SerializeField] private CityMainBuilding _mainBuilding;
	[SerializeField] private List<ProductData> _producedProducts;
	
	private Dictionary<PathFindingNode, Path> _paths;
	#endregion

	#region Getter & Setter
	public ProductStorage ReceiverStorage(ProductData productData = null)
	{
		if (_receivedProducts.Count == 0) return null;
		if (productData == null && _receivedProducts.Count == 1) return _receivedProducts.Values.GetEnumerator().Current;
		return productData != null ? _receivedProducts[productData] : null;
	}

	public List<ProductData> ReceivedProductList()
	{
		return new List<ProductData> (_receivedProducts.Keys);
	}

	#region ProductEmitter
	public ProductStorage EmitterStorage(ProductData productData = null)
	{
		if (productData != null) return _emittedProducts.Find(x => x.StoredProductData.Equals(productData));
		switch (_emittedProducts.Count)
		{
			case 1:
				return _emittedProducts.ToArray()[0];
			default:
				return null;
		}
	}

	public List<ProductData> EmittedProductList()
	{
		return _producedProducts;
	}
	#endregion
	
	
	/// <summary>
	/// Count of people living in this cityPlaceable.
	/// </summary>
	/// <returns></returns>
	public int CurrentInhabitantCount()
	{
		return ChildMapPlaceables.Count * 3;
	}

	public CityMainBuilding MainBuilding {
		get => _mainBuilding;

		set => _mainBuilding = value;
	}
	#endregion

	#region Methods

	void Start()
	{
		if (ChildMapPlaceables.Count == 0)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				SimpleMapPlaceable simpleMapPlaceable = transform.GetChild(i).gameObject.GetComponent<SimpleMapPlaceable>();
				if (simpleMapPlaceable) ChildMapPlaceables.Add(simpleMapPlaceable);
				if (simpleMapPlaceable is CityMainBuilding building && !_mainBuilding) _mainBuilding = building;
			}
		}
		_paths = new Dictionary<PathFindingNode, Path>();
		_receivedProducts = new Dictionary<ProductData, ProductStorage>();
		_emittedProducts = new List<ProductStorage>(); 
		FillEmittedProducts();
		FillReceivedProducts();
	}

	private void FillEmittedProducts()
	{
		foreach (ProductData product in _producedProducts)
		{
			_emittedProducts.Add(new ProductStorage(product, Random.Range(3, 7)));
		}
	}
	
	private void FillReceivedProducts()
	{
		foreach (SimpleMapPlaceable simpleMapPlaceable in ChildMapPlaceables)
		{
			if (!(simpleMapPlaceable is CityBuilding)) continue;
			CityBuilding cityBuilding = ((CityBuilding) simpleMapPlaceable);
			foreach (NeededProduct neededProduct in cityBuilding.ConsumedProducts)
			{
				if (_receivedProducts.ContainsKey(neededProduct.Product))
				{
					_receivedProducts[neededProduct.Product].MaxAmount += neededProduct.Amount;
				}
				else
				{
					_receivedProducts.Add(neededProduct.Product, new ProductStorage(neededProduct.Product, neededProduct.Amount));
				}
			}
		}
	}

	public Path PathTo(PathFindingNode targetNode)
	{
		return _paths.ContainsKey(targetNode) ? _paths[targetNode] : null;
	}

	public void AddPath(PathFindingNode targetNode, Path path)
	{
		if (_paths.ContainsKey(targetNode))
		{
			_paths[targetNode] = path;
		}
		else
		{
			_paths.Add(targetNode, path);
		}
	}

	public void RemovePath(PathFindingNode targetNode)
	{
		_paths.Remove(targetNode);
	}
	
	#endregion
}

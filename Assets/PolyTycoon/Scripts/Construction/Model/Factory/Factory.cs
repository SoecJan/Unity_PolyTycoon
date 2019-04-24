using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : PathFindingNode, IConsumer, IProducer, IPathNode
{
	#region Attributes

	[SerializeField] private Dictionary<ProductData, ProductStorage> _neededProducts; // Dict of needed Products
	[SerializeField] private ProductStorage _producedProduct; // The currently produced product
	private bool _isProductSelectable = true;
	private bool _isProducing = false;
	private float _elapsedTime; // Time elapsed since the begin of the production process
	private int _tempMaxAmount = 20;
	private Coroutine _produceCoroutine;
//	private Dictionary<BiomeGenerator.Biome, float> _biomeValueDictionary;
	private Dictionary<PathFindingNode, Path> _paths;
	#endregion

	#region Getter & Setter
	public ProductData ProductData {
		get {
			return ProducedProductStorage().StoredProductData;
		}

		set {
			// Don't alter if this is the same Product
			if (ProducedProductStorage().StoredProductData != null && ProducedProductStorage().StoredProductData.Equals(value)) return;

			// Change Product to selected
			ProducedProductStorage().StoredProductData = value;
			if (ProductData) InitializeProduction();
		}
	}

	public float ProductionProgress {
		get {
			return _elapsedTime / ProductData.ProductionTime;
		}
	}

	public bool IsProductSelectable
	{
		get { return _isProductSelectable; }
	}

	//public Dictionary<BiomeGenerator.Biome, float> BiomeValueDictionary {
	//	get {
	//		return _biomeValueDictionary;
	//	}

	//	set {
	//		_biomeValueDictionary = value;
	//		Debug.Log("Biome Values Set");
	//		foreach (float biomeValue in _biomeValueDictionary.Values)
	//		{
	//			Debug.Log(biomeValue);
	//		}
	//	}
	//}

	public ProductStorage ProducedProductStorage()
	{
		return _producedProduct;
	}

	public Dictionary<ProductData, ProductStorage> NeededProducts()
	{
		return _neededProducts;
	}

	public override bool IsNode()
	{
		return true;
	}

	public override bool IsTraversable()
	{
		return false;
	}
	#endregion

	#region Default Methods

	protected override void Initialize()
	{
		base.Initialize();
		IsClickable = true;
		_neededProducts = new Dictionary<ProductData, ProductStorage>();
		_paths = new Dictionary<PathFindingNode, Path>();
		// Initialize production if a product has been set in advance.
		if (_producedProduct.StoredProductData == null) return;
		
		InitializeProduction();
		_isProductSelectable = false;
	}

	private void InitializeProduction()
	{
		ProducedProductStorage().MaxAmount = _tempMaxAmount;
		ProducedProductStorage().Amount = 0;
		_isProducing = false;
		_elapsedTime = 0f;

		// Setup needed Products
		_neededProducts.Clear();
		if (ProductData.NeededProduct.Length > 0)
		{
			foreach (NeededProduct neededProduct in ProductData.NeededProduct)
			{
				_neededProducts.Add(neededProduct.Product, new ProductStorage(neededProduct.Product, _tempMaxAmount));
			}
		}

		// Start production of the new product
		if (_produceCoroutine != null)
		{
			StopCoroutine(_produceCoroutine);
			_produceCoroutine = null;
		}
		_produceCoroutine = StartCoroutine(Produce());
	}

	private void Update()
	{
		// Added to show production progress to the player
		if (_isProducing)
		{
			_elapsedTime += Time.deltaTime;
		}
	}
	#endregion

	#region Production
	/// <summary>
	/// Handles the Production process. Handled by <see cref="set_ProductData"/>.
	/// </summary>
	IEnumerator Produce()
	{
		while (ProductData != null)
		{
			// Wait until there are enough needed products
			while (!IsProductionReady())
			{
				yield return new WaitForSeconds(0.5f);
			}

			// Remove needed products from storage
			if (NeededProducts().Count > 0)
			{
				foreach (NeededProduct neededProduct in ProductData.NeededProduct)
				{
					NeededProducts()[neededProduct.Product].Amount -= neededProduct.Amount;
				}
			}

			// Produce
			_isProducing = true;
			yield return new WaitForSeconds(ProductData.ProductionTime);
			ProducedProductStorage().Amount += 1;
			_isProducing = false;
			_elapsedTime = 0f;

			// Debug
			Debug.Log("Product Finished: " + ProducedProductStorage().StoredProductData.ProductName + ": " + ProducedProductStorage().Amount);
		}
	}

	private bool IsProductionReady()
	{
		bool productionReady = ProductData != null && ProducedProductStorage().Amount < ProducedProductStorage().MaxAmount;
		if (!productionReady) return productionReady;
		foreach (NeededProduct neededProduct in ProductData.NeededProduct)
		{
			if (neededProduct.Amount > NeededProducts()[neededProduct.Product].Amount)
			{
				productionReady = false;
			}
		}
		return productionReady;
	}
	#endregion

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
}

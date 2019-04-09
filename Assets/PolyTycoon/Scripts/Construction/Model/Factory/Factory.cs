using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : PathFindingNode, IConsumer, IProducer
{
	#region Attributes

	[SerializeField] private Dictionary<ProductData, ProductStorage> _neededProducts; // Dict of needed Products
	[SerializeField] private ProductStorage _producedProduct; // The currently produced product
	private bool _isProducing = false;
	private float _elapsedTime; // Time elapsed since the begin of the production process
	private int _tempMaxAmount = 20;
	private Coroutine _produceCoroutine;
	private Dictionary<BiomeGenerator.Biome, float> _biomeValueDictionary;
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
			ProducedProductStorage().MaxAmount = _tempMaxAmount;
			ProducedProductStorage().Amount = 0;
			_elapsedTime = 0f;

			// Setup needed Products
			_neededProducts.Clear();
			if (ProductData.NeededProduct.Product != null)
			{
				_neededProducts.Add(ProductData.NeededProduct.Product, new ProductStorage(ProductData.NeededProduct.Product, _tempMaxAmount));
			}

			// Start production of the new product
			if (_produceCoroutine != null)
			{
				StopCoroutine(_produceCoroutine);
				_produceCoroutine = null;
			}
			_produceCoroutine = StartCoroutine(Produce());
		}
	}

	public float ProductionProgress {
		get {
			return _elapsedTime / ProductData.ProductionTime;
		}
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
		return true; // TODO: Add check for the access street
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
		_producedProduct = new ProductStorage();
		_neededProducts = new Dictionary<ProductData, ProductStorage>();
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
			bool needed = ProductData != null && ProductData.NeededProduct.Product != null;
			bool enoughNeeded = needed && NeededProducts()[ProductData.NeededProduct.Product].Amount >= ProductData.NeededProduct.Amount;

			// Wait until there are enough needed products
			while ((needed && !enoughNeeded) || ProducedProductStorage().Amount == ProducedProductStorage().MaxAmount)
			{
				yield return new WaitForSeconds(0.5f);
				needed = ProductData != null && ProductData.NeededProduct.Product != null;
				enoughNeeded = needed && NeededProducts()[ProductData.NeededProduct.Product].Amount >= ProductData.NeededProduct.Amount;
			}

			// Remove needed products from storage
			if (needed)
			{
				NeededProducts()[ProductData.NeededProduct.Product].Amount -= ProductData.NeededProduct.Amount;
			}

			// Produce
			_isProducing = true;
			yield return new WaitForSeconds(ProductData.ProductionTime);
			ProducedProductStorage().Amount += 1;
			_isProducing = false;
			_elapsedTime = 0f;

			// Debug
			if (needed) Debug.Log("Product Finished: " + ProducedProductStorage().StoredProductData.ProductName + ": " + ProducedProductStorage().Amount);
		}
	}
	#endregion
}

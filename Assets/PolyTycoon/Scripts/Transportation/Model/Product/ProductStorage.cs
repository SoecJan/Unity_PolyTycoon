using Assets.PolyTycoon.Resources.Data.ProductData;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Model.Product
{
	/// <summary>
	/// This is the base class of Products that need to be stored.
	/// </summary>
	[System.Serializable]
	public class ProductStorage
	{
		#region Attributes
		[SerializeField] private ProductData _storedProductData;
		[SerializeField] private int _maxAmount;
		[SerializeField] private int _storedAmount;
		[SerializeField] private BiomeGenerator.Biome _growthBiome;
		[SerializeField] private float _baseGrowthValue = 0.5f;
		#endregion

		#region Constructors & Getter & Setter
		public ProductStorage()
		{}

		public ProductStorage(ProductData productData)
		{
			_storedProductData = productData;
		}

		public ProductStorage(ProductData productData, int maxAmount) : this(productData)
		{
			_maxAmount = maxAmount;
		}

		public ProductStorage(ProductData productData, int maxAmount, int storedAmount) : this(productData, maxAmount)
		{
			_storedAmount = storedAmount;
		}

		public ProductData StoredProductData {
			get {
				return _storedProductData;
			}

			set {
				_storedProductData = value;
			}
		}

		public int MaxAmount {
			get {
				return _maxAmount;
			}

			set {
				_maxAmount = value;
			}
		}

		public int Amount {
			get {
				return _storedAmount;
			}

			set {
				_storedAmount = value;
			}
		}

		public BiomeGenerator.Biome GrowthBiome {
			get {
				return _growthBiome;
			}

			set {
				_growthBiome = value;
			}
		}

		public float BaseGrowthValue {
			get {
				return _baseGrowthValue;
			}

			set {
				_baseGrowthValue = value;
			}
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return (StoredProductData == null ? "" : StoredProductData.ToString()) + "; Amount: " + Amount;
		}

		/// <summary>
		/// Returns a new instance of the used object.
		/// </summary>
		/// <returns>New Instance</returns>
		public ProductStorage Clone()
		{
			ProductStorage storage = new ProductStorage
			{
				StoredProductData = this._storedProductData,
				MaxAmount = this._maxAmount,
				Amount = this._storedAmount
			};
			return storage;
		}

		public override bool Equals(object obj)
		{
			var storage = obj as ProductStorage;
			return storage != null &&
			       storage.StoredProductData.ProductName.Equals(this.StoredProductData.ProductName);
		}

		public override int GetHashCode()
		{
			var hashCode = -929180017;
			hashCode = hashCode * -1521134295 + _maxAmount.GetHashCode();
			hashCode = hashCode * -1521134295 + _storedAmount.GetHashCode();
			hashCode = hashCode * -1521134295 + MaxAmount.GetHashCode();
			hashCode = hashCode * -1521134295 + Amount.GetHashCode();
			return hashCode;
		}
		#endregion
	}
}

using Assets.PolyTycoon.Resources.Data.ProductData;

namespace Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute
{
	public class TransportRouteSetting {
		#region Attributes
		private ProductData _productData;
		private int _amount;
		private bool _isLoad;
		#endregion

		#region Getter & Setter
		public ProductData ProductData {
			get {
				return _productData;
			}

			set {
				_productData = value;
			}
		}

		public int Amount {
			get {
				return _amount;
			}

			set {
				_amount = value;
			}
		}

		public bool IsLoad {
			get {
				return _isLoad;
			}

			set {
				_isLoad = value;
			}
		}
		#endregion

		#region Methods
		public TransportRouteSetting Clone()
		{
			TransportRouteSetting settings = new TransportRouteSetting
			{
				ProductData = _productData, Amount = _amount, IsLoad = _isLoad
			};
			return settings;
		}

		public override string ToString()
		{
			return "Setting: IsLoad? " + IsLoad.ToString() + "; Product: " + _productData.ProductName + "; Amount: " + _amount.ToString();
		}
		#endregion
	}
}

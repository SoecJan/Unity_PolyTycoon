using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.RouteElement
{
	public class RouteElementView : PoolableObject {
		private static RouteElementController _routeViewController;
		private static RouteSettingController _routeSettingController;
		private TransportRouteElement _transportRouteElement;

		[Header("Navigation")] 
		[SerializeField] private Button _editButton;
		[SerializeField] private Button _removeButton;

		[Header("UI")]
		[SerializeField] private Image _fromNodeImage;
		[SerializeField] private Text _fromNodeText;
		[SerializeField] private Image _toNodeImage;
		[SerializeField] private Text _toNodeText;

		public PathFindingNode FromNode
		{
			get {
				return TransportRouteElement.FromNode;
			}

			set {
				TransportRouteElement.FromNode = value;
				_fromNodeText.text = FromNode ? value.BuildingName : "None";
			}
		}

		public PathFindingNode ToNode {
			get {
				return TransportRouteElement.ToNode;
			}

			set {
				TransportRouteElement.ToNode = value;
				ToNodeText.text = ToNode ? value.BuildingName : "None";
			}
		}

		public Image FromNodeImage {
			get {
				return _fromNodeImage;
			}

			set {
				_fromNodeImage = value;
			}
		}

		public Text FromNodeText {
			get {
				return _fromNodeText;
			}

			set {
				_fromNodeText = value;
			}
		}

		public Image ToNodeImage {
			get {
				return _toNodeImage;
			}

			set {
				_toNodeImage = value;
			}
		}

		public Text ToNodeText {
			get {
				return _toNodeText;
			}

			set {
				_toNodeText = value;
			}
		}

		public TransportRouteElement TransportRouteElement {
			get {
				return _transportRouteElement;
			}

			set {
				_transportRouteElement = value;

				if (_transportRouteElement == null || !_transportRouteElement.FromNode || !_transportRouteElement.ToNode || _transportRouteElement.Path == null) return;
				FromNodeText.text = _transportRouteElement.FromNode.BuildingName;
				ToNodeText.text = _transportRouteElement.ToNode.BuildingName;
			}
		}

		void Awake () {
			// Find static instances
			if (!_routeViewController) _routeViewController = FindObjectOfType<RouteElementController>();
			if (!_routeSettingController) _routeSettingController = FindObjectOfType<RouteSettingController>();
			// Init 
			TransportRouteElement = new TransportRouteElement();
			// Setup callbacks
			_editButton.onClick.AddListener(OnEditClick);
			_removeButton.onClick.AddListener(OnRemoveClick);
		}

		public void OnRemoveClick()
		{
			_routeViewController.RemoveTransportRouteElement(this);
		}

		public void OnEditClick()
		{
			_routeViewController.SelectedRouteElement = this;
			_routeSettingController.LoadRouteElementSettings(_transportRouteElement);
		}

		public override void Show()
		{
			base.Show();
		}

		public override void Hide()
		{
			base.Hide();
			TransportRouteElement = new TransportRouteElement();
			FromNodeImage.sprite = null;
			ToNodeImage.sprite = null;
			FromNodeText.text = "";
			ToNodeText.text = "";
		}
	}
}

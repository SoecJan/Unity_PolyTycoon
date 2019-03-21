using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.RouteElement;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Setting;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller
{
	/// <summary>
	/// This class controls the RouteSettings of TransportRouteCreation.
	/// </summary>
	public class RouteSettingController : MonoBehaviour
	{
		private TransportRouteCreateController _routeCreateController;
		private RouteSettingProductSelector _routeSettingProductSelector;

		[Header("General")]
		[SerializeField] private GameObject _visibleGameObject;
		[SerializeField] private Button _addButton;
		[SerializeField] private Button _exitButton;

		[Header("Route Setting")]
		[SerializeField] private ScrollViewHandle _routeSettingScrollView;
		[SerializeField] private GameObject _elementPrefab;
		[SerializeField] private Text _loadSumText;
		[SerializeField] private Text _unloadSumText;

		private ScrollViewHandle RouteSettingScrollView {
			get {
				return _routeSettingScrollView;
			}

			set {
				_routeSettingScrollView = value;
			}
		}

		private void Start()
		{
			_routeCreateController = FindObjectOfType<TransportRouteCreateController>();
			_routeSettingProductSelector = FindObjectOfType<RouteSettingProductSelector>();
			_exitButton.onClick.AddListener(delegate { _visibleGameObject.SetActive(false); Reset(); });
			_addButton.onClick.AddListener(AddSetting);
		}

		private void AddSetting()
		{
			TransportRouteSetting routeSetting = new TransportRouteSetting();
			GameObject elementGameObject = _routeSettingScrollView.AddObject((RectTransform) _elementPrefab.transform);
			RouteSettingView routeSettingView = elementGameObject.GetComponent<RouteSettingView>();
			routeSettingView.RouteSetting = routeSetting;
			routeSettingView.OnValueChangeAction += OnValueChange;
			_routeCreateController.RouteElementController.SelectedRouteElement.TransportRouteElement.RouteSettings.Add(routeSetting);
		}

		public void RemoveRouteSetting(RouteSettingView routeSettingView)
		{
			_routeCreateController.RouteElementController.SelectedRouteElement.TransportRouteElement.RouteSettings
				.Remove(routeSettingView.RouteSetting);
			RouteSettingScrollView.RemoveObject((RectTransform)routeSettingView.transform);
		}

		public void Reset()
		{
			RouteSettingScrollView.ClearObjects();
			_routeSettingProductSelector.VisibleGameObject.SetActive(false);
			_visibleGameObject.SetActive(false);
		}

		public void LoadRouteElementSettings(TransportRouteElement transportRouteElement)
		{
			_routeSettingScrollView.ClearObjects();
			foreach (TransportRouteSetting routeSetting in transportRouteElement.RouteSettings)
			{
				GameObject instantiatedGameObject = RouteSettingScrollView.AddObject((RectTransform)_elementPrefab.gameObject.transform);
				RouteSettingView routeSettingView = instantiatedGameObject.GetComponent<RouteSettingView>();
				routeSettingView.RouteSetting = routeSetting;
			}
			_visibleGameObject.SetActive(true);
		}

		private void OnValueChange(RouteSettingView routeSettingView)
		{
			UpdateLoadOverviewUi();
		}

		private void UpdateLoadOverviewUi()
		{
			int loadAmount = 0;
			int unloadAmount = 0;
			foreach (RectTransform rectTransform in RouteSettingScrollView.ContentObjects)
			{
				RouteSettingView routeSettingView = rectTransform.gameObject.GetComponent<RouteSettingView>();
				if (routeSettingView.RouteSetting.IsLoad)
				{
					loadAmount += routeSettingView.RouteSetting.Amount;
				}
				else
				{
					unloadAmount += routeSettingView.RouteSetting.Amount;
				}
			}

			_loadSumText.text = loadAmount.ToString();
			_unloadSumText.text = unloadAmount.ToString();
		}
	}
}

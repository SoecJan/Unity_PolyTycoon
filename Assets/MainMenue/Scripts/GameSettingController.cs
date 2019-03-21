using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingController : AbstractUi
{
	[Header("World Size")] 
	[SerializeField] private Slider _worldSizeSlider;
	[SerializeField] private Text _worldSizeText;
	[SerializeField] private InputField _worldSeed;
	[SerializeField] private Button _previewButton;

	[SerializeField] private InputField _startMoney;
	[SerializeField] private int _minMoneyValue = 0;
	[SerializeField] private InputField _companyName;
	[SerializeField] private Button _companyLogoChangeButton;
	[SerializeField] private Image _companyLogoImage;
	[SerializeField] private Slider _cityCountSlider;
	[SerializeField] private Text _cityCountText;

	[SerializeField] private Button _playButton;
	[SerializeField] private Button _backButton;

	private GameSettingColorPicker _gameSettingColorPicker;
	private PlayMenueController _playMenueController;
	private MapPreview _mapPreview;
	private bool _preview;

	void Start()
	{

		GameSettingColorPickerElement.OnClickAction += OnColorSelectClick;
		_gameSettingColorPicker = FindObjectOfType<GameSettingColorPicker>();
		_worldSeed.contentType = InputField.ContentType.IntegerNumber;
		_mapPreview = FindObjectOfType<MapPreview>();
		_worldSeed.text = _mapPreview.heightMapSettings.noiseSettings.seed.ToString();
		_playMenueController = FindObjectOfType<PlayMenueController>();
		_previewButton.onClick.AddListener(OnPreviewClick);
		_worldSizeSlider.onValueChanged.AddListener(OnMapSizeChange);
		_cityCountSlider.onValueChanged.AddListener(OnCityCountChange);
		_startMoney.onEndEdit.AddListener(OnStartMoneyEndEdit);
		_companyLogoChangeButton.onClick.AddListener(OnLogoChangeClick);
		_playButton.onClick.AddListener(OnPlayClick);
		_backButton.onClick.AddListener(OnBackClick);
	}

	void Update()
	{
		if (_preview && Input.anyKey)
		{
			VisibleObject.SetActive(true);
			_preview = false;
		}
	}

	private void OnPreviewClick()
	{
		_mapPreview.heightMapSettings.noiseSettings.seed = int.Parse(_worldSeed.text);
		_mapPreview.DrawMapInEditor();
		VisibleObject.SetActive(false);
		_preview = true;
	}

	private void OnLogoChangeClick()
	{
		_gameSettingColorPicker.VisibleGameObject.transform.position = _companyLogoImage.transform.position;
		_gameSettingColorPicker.VisibleGameObject.SetActive(true);
	}

	private void OnColorSelectClick(GameSettingColorPickerElement colorPickerElement)
	{
		_companyLogoImage.color = colorPickerElement.Image.color;
		_gameSettingColorPicker.VisibleGameObject.SetActive(false);
	}

	private void OnPlayClick()
	{
		GameSettingManager gameSettingManager = FindObjectOfType<GameSettingManager>();
		gameSettingManager.Setting = new GameSetting((int)_worldSizeSlider.value, int.Parse(_worldSeed.text), (int)_cityCountSlider.value, 0, int.Parse(_startMoney.text), _companyLogoImage.color, _companyName.text);
		SceneController sceneController = FindObjectOfType<SceneController>();
		sceneController.LoadScene(SceneController.Scenes.PolyTycoon);
	}

	private void OnStartMoneyEndEdit(string value)
	{
		int startMoney = 0;
		if (!int.TryParse(_startMoney.text, out startMoney))
		{
			_startMoney.text = _minMoneyValue.ToString();
		}
		if (startMoney < _minMoneyValue)
		{
			_startMoney.text = _minMoneyValue.ToString();
		}
	}

	private void OnCityCountChange(float value)
	{
		_cityCountText.text = value.ToString();
	}

	private void OnMapSizeChange(float value)
	{
		int worldSize = (int) value;
		switch (worldSize)
		{
			case 0:
				_worldSizeText.text = "Small";
				break;
			case 1:
				_worldSizeText.text = "Big";
				break;
			case 2:
				_worldSizeText.text = "Endless";
				break;
		}
		Debug.Log("Add Game Support");
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_playMenueController.SetVisible(true);
	}
}

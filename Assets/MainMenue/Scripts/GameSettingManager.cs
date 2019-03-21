using UnityEngine;

public class GameSettingManager : MonoBehaviour
{
	private GameSetting _setting;

	public GameSetting Setting {
		get {
			return _setting;
		}

		set {
			_setting = value;
		}
	}

	void Start()
	{
		Setting = new GameSetting(companyColor:Color.white);
	}
}

public struct GameSetting
{
	private int _worldSize;
	private int _worldSeed;

	private int _cityCount;
	private int _winCondition;

	private int _startMoney;
	private Color _companyColor;
	private string _companyName;

	public GameSetting(int worldSize = 1, int worldSeed = 125, int cityCount = 2, int winCondition = 0, int startMoney = 50000, Color companyColor = default(Color), string companyName = "Default Inc.")
	{
		_worldSize = worldSize;
		_worldSeed = worldSeed;
		_cityCount = cityCount;
		_winCondition = winCondition;
		_startMoney = startMoney;
		_companyColor = companyColor;
		_companyName = companyName;
	}

	public int WorldSize {
		get {
			return _worldSize;
		}

		set {
			_worldSize = value;
		}
	}

	public int WorldSeed {
		get {
			return _worldSeed;
		}

		set {
			_worldSeed = value;
		}
	}

	public int CityCount {
		get {
			return _cityCount;
		}

		set {
			_cityCount = value;
		}
	}

	public int WinCondition {
		get {
			return _winCondition;
		}

		set {
			_winCondition = value;
		}
	}

	public int StartMoney {
		get {
			return _startMoney;
		}

		set {
			_startMoney = value;
		}
	}

	public Color CompanyColor {
		get {
			return _companyColor;
		}

		set {
			_companyColor = value;
		}
	}

	public string CompanyName {
		get {
			return _companyName;
		}

		set {
			_companyName = value;
		}
	}
}

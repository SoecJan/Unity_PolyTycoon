using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
	[SerializeField] private int _playerMoney = 50000;
	[SerializeField] private Text _moneyText;

	public int PlayerMoney {
		get {
			return _playerMoney;
		}

		set {
			_playerMoney = value;
			_moneyText.text = _playerMoney.ToString();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

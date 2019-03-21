using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle", menuName = "PolyTycoon/Vehicle", order = 2)]
public class VehicleData : ScriptableObject
{
	
	private float _strength;
	private float _topSpeed;
	private int _capacity;

	private Sprite _menuSprite;
}

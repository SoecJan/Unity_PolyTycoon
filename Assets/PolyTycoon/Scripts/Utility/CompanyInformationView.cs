using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyInformationView : MonoBehaviour
{

	[SerializeField] private Text _companyNameText;
	[SerializeField] private Image _companyColorImage;

	public string CompanyName {
		get {
			return _companyNameText.text;
		}

		set { _companyNameText.text = value; }
	}

	public Color CompanyColor {
		set {
			_companyColorImage.color = value;
		}

		get { return _companyColorImage.color; }
	}
}

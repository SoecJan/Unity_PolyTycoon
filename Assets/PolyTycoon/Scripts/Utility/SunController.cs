using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SunController : MonoBehaviour
{
	public Light sun;
	public float secondsInFullDay = 120f;
	[Range(0, 1)]
	public float currentTimeOfDay = 0;

	[SerializeField] private AnimationCurve _timeMultiplierCurve;
	[SerializeField] private Slider _dayTimeSlider;

	float sunInitialIntensity;

	void Start()
	{
		RenderSettings.sun = sun;
		sunInitialIntensity = sun.intensity;
	}

	void Update()
	{
		UpdateSun();

		currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * _timeMultiplierCurve.Evaluate(currentTimeOfDay);
		_dayTimeSlider.value = currentTimeOfDay;
		if (currentTimeOfDay >= 1)
		{
			currentTimeOfDay = 0;
		}
	}

	void UpdateSun()
	{
		sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

		float intensityMultiplier = 1;
		if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
		{
			intensityMultiplier = 0;
		}
		else if (currentTimeOfDay <= 0.25f)
		{
			intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
		}
		else if (currentTimeOfDay >= 0.73f)
		{
			intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
		}

		sun.intensity = sunInitialIntensity * intensityMultiplier;
	}
}

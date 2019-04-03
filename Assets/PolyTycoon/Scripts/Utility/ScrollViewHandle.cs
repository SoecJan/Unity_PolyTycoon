using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class is a wrapper for a <see cref="_scrollView"/>. It Handles all Transformations needed to align Items in a ScrollView.
/// </summary>
public class ScrollViewHandle : MonoBehaviour
{
	#region Attributes
	[SerializeField] [Range(1, 20)] private int _rowsColumnCount = 1;
	[SerializeField] private bool _horizontal = false;
	[SerializeField] private ScrollRect _scrollView;

	private RectTransform _contentTransform;

	private ObjectPool PooledObjects { get; set; }
	public List<RectTransform> ContentObjects { get; set; }

	#endregion

	#region Default Methods
	// Use this for initialization
	void Awake()
	{
		ContentObjects = new List<RectTransform>();
		PooledObjects = new ObjectPool();
		_contentTransform = _scrollView.content;
		_scrollView.scrollSensitivity = 10f;
		Resize();
	}
	#endregion

	#region ScrollView Handling
	private void Resize()
	{
		float contentHorizontalSize = 0f;
		float contentNotHorizontalSize = 0f;
		int rowColumnIndex = 0;
		for (int i = 0; i < ContentObjects.Count; i++)
		{
			RectTransform rectTransform = ContentObjects[i];
			contentHorizontalSize = _horizontal ? rectTransform.sizeDelta.x : rectTransform.sizeDelta.y;
			contentNotHorizontalSize = !_horizontal ? rectTransform.sizeDelta.x : rectTransform.sizeDelta.y;

			int rowColumnRatio = i / _rowsColumnCount;
			if (rowColumnRatio > rowColumnIndex)
			{
				rowColumnIndex = rowColumnRatio;
			}

			float xPos = _horizontal ? contentHorizontalSize * rowColumnIndex : (i % _rowsColumnCount) * contentHorizontalSize;
			float yPos = !_horizontal ? -1 * contentHorizontalSize * rowColumnIndex : -1 * (i % _rowsColumnCount) * contentHorizontalSize;

			rectTransform.anchoredPosition = new Vector2(xPos, yPos); // Align the ChatElementTransform to the ScrollView
		}

		int countRowColumnRatio = ContentObjects.Count / _rowsColumnCount;
		int countColumnRowRatio = countRowColumnRatio != 0 ? ContentObjects.Count / countRowColumnRatio : 1;
		float xSize = _horizontal ? contentHorizontalSize * countRowColumnRatio : contentNotHorizontalSize * (countColumnRowRatio - 1);
		float ySize = !_horizontal ? contentHorizontalSize * countRowColumnRatio : contentNotHorizontalSize * (countColumnRowRatio - 1);
		_contentTransform.sizeDelta = new Vector2(xSize, ySize); // Increase the size of the ScrollView Transform
	}

	private bool PoolObject(PoolableObject poolable)
	{
		RectTransform rectTransform = (RectTransform)poolable.gameObject.transform;
		bool isRemoved = ContentObjects.Remove(rectTransform);
		if (!isRemoved) return false;
		PooledObjects.Add(poolable);
		return true;
	}

	public GameObject AddObject(RectTransform rectTransform)
	{
		GameObject instance = PooledObjects.Count() > 0 ? PooledObjects.GetPoolable().gameObject : Instantiate(rectTransform.gameObject, _contentTransform);
		ContentObjects.Add((RectTransform)instance.transform);
		Resize();
		return instance;
	}

	public void RemoveObject(RectTransform rectTransform)
	{
		PoolableObject poolableObject = rectTransform.gameObject.GetComponent<PoolableObject>();
		if (!poolableObject || !PoolObject(poolableObject))
		{
			Destroy(rectTransform.gameObject);
			ContentObjects.Remove(rectTransform);
		}
		Resize();
	}

	public void ClearObjects()
	{
		for (int i = ContentObjects.Count - 1; i >= 0; i--)
		{
			RemoveObject(ContentObjects[i]);
		}
	}
	#endregion
}

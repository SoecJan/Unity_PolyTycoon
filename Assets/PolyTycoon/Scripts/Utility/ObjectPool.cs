using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	#region Attributes
	private readonly List<PoolableObject> _internalList;
	#endregion

	#region Constructor
	public ObjectPool()
	{
		_internalList = new List<PoolableObject>();
	}
	#endregion

	#region Methods

	public void Add(PoolableObject item)
	{
		item.Hide();
		_internalList.Add(item);
	}

	public PoolableObject GetPoolable()
	{
		if (_internalList.Count == 0) return null;

		PoolableObject poolableObject = _internalList[0];
		_internalList.RemoveAt(0);
		poolableObject.Show();
		return poolableObject;
	}

	public void Clear()
	{
		foreach (PoolableObject poolable in _internalList)
		{
			Object.Destroy(poolable.gameObject);
		}
		_internalList.Clear();
	}

	public int Count()
	{
		return _internalList.Count;
	}
	#endregion
}

public abstract class PoolableObject : MonoBehaviour
{
	#region Methods
	public virtual void Show()
	{
		gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}
	#endregion
}
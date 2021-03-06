﻿using UnityEngine;

/// <summary>
/// This class is used to remove a placed <see cref="MapPlaceable"/> from the game.
/// </summary>
public class DestructionController : MonoBehaviour
{
    private Camera _mainCamera;
    private IBuildingManager _buildingManager;
    [SerializeField] private LayerMask _buildingMask;

    public static bool DestructionActive { get; set; }

    // Use this for initialization
    void Start()
    {
        _buildingManager = FindObjectOfType<GameHandler>().BuildingManager;
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!DestructionActive || !Input.GetMouseButtonDown(0)) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _buildingMask)) return;

        SimpleMapPlaceable mapPlaceable = hitInfo.collider.gameObject.GetComponent<SimpleMapPlaceable>();
        if (!mapPlaceable) return;
        _buildingManager.RemoveMapPlaceable(mapPlaceable.transform.position);
    }
}
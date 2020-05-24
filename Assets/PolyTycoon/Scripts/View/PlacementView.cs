using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementView : MonoBehaviour
{
    private GameHandler _gameHandler;
    private IPlacementController _placementController;
    private Renderer _gridDisplayRenderer;
    [SerializeField] private Material _terrainMaterial;
    [SerializeField] private LayerMask _buildingsMask; // Needed to determine the objects to place _buildings on
    [SerializeField] private Vector3 _offsetVec3 = new Vector3(0.5f, 0, 0.5f); // Needed to align _buildings correctly
    [SerializeField] private KeyCode _rotateHotKey = KeyCode.R; // Needed to rotate a selected Building
    [SerializeField] private float _rotateAmount = 90f; // The Amount to rotate a building on _rotateHotKey press
    private Camera _mainCamera;
    private UserNotificationView _userNotificationView;
    private MapPlaceable _currentPlaceableObject; // Object that is being placed
    private List<MapPlaceable> _draggedGameObjects;
    private bool _isDragging;

    void Start()
    { 
        _gameHandler = FindObjectOfType<GameHandler>();
        TerrainChunk terrainChunk =  _gameHandler.TerrainGenerator.GetChunk(0, 0);
        _gridDisplayRenderer = terrainChunk.meshObject.GetComponent<Renderer>();
        OnPlacement();
        _placementController = _gameHandler.PlacementController;
        _draggedGameObjects = new List<MapPlaceable>();
        _userNotificationView = GameObject.FindObjectOfType<UserNotificationView>();
        _mainCamera = Camera.main;
    }
    
    public BuildingData PlaceableObjectPrefab
    {
        set
        {
            _placementController.PlaceableObjectPrefab = value;
            if (_currentPlaceableObject && value) GameObject.Destroy(_currentPlaceableObject.gameObject);
            _currentPlaceableObject = GameObject.Instantiate(value.Prefab.GetComponent<MapPlaceable>());
            _gridDisplayRenderer.sharedMaterial.SetFloat("_Boolean_ShowGrid", 1f);
        }
    }

    void Update()
    {
        if (!_currentPlaceableObject) return;
        IsPlaceableFeedback();
        MoveObject(_currentPlaceableObject.transform, _mainCamera);
        RotateObject();
        HandleInput();
    }

    private void MoveObject(Transform movedTransform, Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _buildingsMask)) return;
        _gridDisplayRenderer.sharedMaterial.SetVector("_Vector3_GridCullingPosition", hitInfo.point);
        hitInfo.point -= new Vector3(_offsetVec3.x, _offsetVec3.y, _offsetVec3.z);

        Vector3 currentPosition = _currentPlaceableObject.transform.position;
        Vector3 position = _placementController.MoveObject(hitInfo.point, currentPosition, _offsetVec3);
        movedTransform.position = position;

        // Check position change
        if (!(Math.Abs(position.x - currentPosition.x) < 0.5f) &&
            !(Math.Abs(position.z - currentPosition.z) < 0.5f)) return;
        
        SimpleMapPlaceable mapPlaceable = _currentPlaceableObject.GetComponent<SimpleMapPlaceable>();
        Transform objectTransform = _currentPlaceableObject.transform;
        
        if (!mapPlaceable || !mapPlaceable.IsDraggable || !_isDragging) return;
        
        if (_draggedGameObjects.Count == 0)
        {
            _draggedGameObjects.Add(GameObject.Instantiate(_currentPlaceableObject, objectTransform.position,
                objectTransform.rotation));
        }
        else
        {
            Vector3 start = _draggedGameObjects[0].transform.position;
            Vector3 end = position;
            Vector3 difference = end - start;
            int x = Mathf.RoundToInt(difference.x);
            int z = Mathf.RoundToInt(difference.z);

            int neededCount = Mathf.Abs(x) + Mathf.Abs(z);

            // Remove Objects until neededCount is met
            while (_draggedGameObjects.Count >= 1 && _draggedGameObjects.Count > neededCount)
            {
                GameObject.Destroy(_draggedGameObjects[_draggedGameObjects.Count - 1].gameObject);
                _draggedGameObjects.RemoveAt(_draggedGameObjects.Count - 1);
            }

            // Add Objects until neededCount is met
            while (_draggedGameObjects.Count < neededCount)
            {
                _draggedGameObjects.Add(GameObject.Instantiate(_currentPlaceableObject,
                    objectTransform.position,
                    objectTransform.rotation));
            }

            for (int i = 1; i < _draggedGameObjects.Count; i++)
            {
                MapPlaceable draggedObject = _draggedGameObjects[i];
                if (x != 0)
                {
                    Vector3 direction = x > 0 ? Vector3.right : Vector3.left;
                    start += direction;
                    draggedObject.transform.position = start;
                    x = x > 0 ? x - 1 : x + 1;
                }
                else if (z != 0)
                {
                    Vector3 direction = z > 0 ? Vector3.forward : Vector3.back;
                    start += direction;
                    draggedObject.transform.position = start;
                    z = z > 0 ? z - 1 : z + 1;
                }
            }
        }
    }
    
    void HandleInput()
    {
        // Place selected Object on left click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            _isDragging = true;
        }
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            ProductProcessorBehaviour productProcessorBehaviour = _currentPlaceableObject.gameObject.GetComponent<ProductProcessorBehaviour>();
            if (productProcessorBehaviour) productProcessorBehaviour.BuildingData = _placementController.PlaceableObjectPrefab;
            
            DestroyOrPlaceObject();
            OnPlacement();
        }

        // Remove selected Object on right click
        if (!Input.GetMouseButtonDown(1)) return;

        if (_currentPlaceableObject != null && (_currentPlaceableObject.IsDraggable))
        {
            for (int i = _draggedGameObjects.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(_draggedGameObjects[i].gameObject);
            }

            _draggedGameObjects.Clear();
        }

        GameObject.Destroy(_currentPlaceableObject.gameObject);
        OnPlacement();
    }

    private void OnPlacement()
    {
        _gridDisplayRenderer.sharedMaterial.SetFloat("_Boolean_ShowGrid", 0f);
        _currentPlaceableObject = null;
        _isDragging = false;
    }

    void DestroyOrPlaceObject()
    {
        if (_currentPlaceableObject.IsDraggable)
        {
            for (int i = _draggedGameObjects.Count - 1; i >= 0; i--)
            {
                SimpleMapPlaceable previewObject = _draggedGameObjects[i].GetComponent<SimpleMapPlaceable>();
                if (!_placementController.PlaceObject(previewObject))
                {
                    GameObject.Destroy(previewObject.gameObject);
                }
            }
            _draggedGameObjects.Clear();
        }

        if (_currentPlaceableObject is ComplexMapPlaceable &&
            !_placementController.PlaceObject((ComplexMapPlaceable) _currentPlaceableObject))
        {
            GameObject.Destroy(_currentPlaceableObject.gameObject);
        }
        else if (_currentPlaceableObject is SimpleMapPlaceable &&
                 !_placementController.PlaceObject((SimpleMapPlaceable) _currentPlaceableObject))
        {
            GameObject.Destroy(_currentPlaceableObject.gameObject);
        }
    }

    void RotateObject()
    {
        // Rotate selected Object
        if (Input.GetKeyDown(_rotateHotKey))
        {
            _currentPlaceableObject.Rotate(Vector3.up, _rotateAmount);
        }
    }

    private void IsPlaceableFeedback()
    {
        if (_currentPlaceableObject is SimpleMapPlaceable placeable)
        {
            placeable.IsPlaceable =
                _placementController.IsPlaceable(placeable.transform.position, placeable.UsedCoordinates);
        }
        else if (_currentPlaceableObject is ComplexMapPlaceable complexMapPlaceable)
        {
            foreach (SimpleMapPlaceable simpleMapPlaceable in complexMapPlaceable.ChildMapPlaceables)
            {
                simpleMapPlaceable.IsPlaceable = _placementController.IsPlaceable(simpleMapPlaceable.transform.position,
                    simpleMapPlaceable.UsedCoordinates);
            }
        }

        foreach (MapPlaceable mapPlaceable in _draggedGameObjects)
        {
            SimpleMapPlaceable simpleMapPlaceable = mapPlaceable as SimpleMapPlaceable;
            if (simpleMapPlaceable)
            {
                mapPlaceable.IsPlaceable =
                    _placementController.IsPlaceable(mapPlaceable.transform.position,
                        simpleMapPlaceable.UsedCoordinates);
            }
        }
    }
}
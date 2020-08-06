using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ProductProducerManager
{
    [SerializeField] private List<List<BuildingProducerData>> _emitterLists;
    private IPlacementController _placementController;

    public ProductProducerManager(IPlacementController placementController)
    {
        _placementController = placementController;
        _emitterLists = new List<List<BuildingProducerData>>();
        FillEmitterList();
    }

    public List<List<BuildingProducerData>> EmitterLists
    {
        get => _emitterLists;
        set => _emitterLists = value;
    }

    public void OnPlacementPositionFound(object obj)
    {
        Debug.Log(obj);
        ThreadsafePlaceable threadsafePlaceable = (ThreadsafePlaceable) obj;
        MapPlaceable mapPlaceable = GameObject.Instantiate(threadsafePlaceable.MapPlaceable, threadsafePlaceable.Position,
            Quaternion.identity);
        ProductProcessorBehaviour processorBehaviour = mapPlaceable.GetComponent<ProductProcessorBehaviour>();
        processorBehaviour.BuildingData = (BuildingProducerData) threadsafePlaceable.Payload;
        Debug.Log("Producer placed at: " + threadsafePlaceable.Position);
        if (!_placementController.PlaceObject((SimpleMapPlaceable) mapPlaceable))
        {
            GameObject.Destroy(mapPlaceable.gameObject);
            return;
        }
    }

    private void FillEmitterList()
    {
        List<BuildingProducerData> productEmitters = new List<BuildingProducerData>
        {
            Resources.Load<BuildingProducerData>(Util.PathTo("Farm")),
            Resources.Load<BuildingProducerData>(Util.PathTo("Mill")),
            Resources.Load<BuildingProducerData>(Util.PathTo("Bakery"))
        };
        _emitterLists.Add(productEmitters);
    }
}

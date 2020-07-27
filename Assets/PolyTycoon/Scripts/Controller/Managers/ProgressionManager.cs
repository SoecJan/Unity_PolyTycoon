using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager
{
    private int _highestCityLevel = -1;
    private Dictionary<int, ProductData[]> _productProgressionElements;
    private Dictionary<int, BuildingData[]> _buildingProgressionElements;

    public System.Action<ProductData> onProductUnlock;
    public System.Action<BuildingData[]> onBuildingUnlock;

    public ProgressionManager()
    {
        _productProgressionElements = new Dictionary<int, ProductData[]>();
        _buildingProgressionElements = new Dictionary<int, BuildingData[]>();
        FillProductProgression();
        FillBuildingProgression();
        CityPlaceable._OnCityLevelChange += delegate(int level, CityPlaceable placeable)
        {
            if (level <= _highestCityLevel)
            {
                return;
            }
            _highestCityLevel = level;
            if (!_productProgressionElements.ContainsKey(level))
            {
                // This Level has been reached by another city
                // Determine current Max level
                // Choose random products from these levels or increase current demand
            }
            else if (_productProgressionElements[level].Length == 0)
            {
                _productProgressionElements.Remove(level);
                // There are no unlocks for this level
                // increase current Demand
            }
            else
            {
                ProductData[] progressionElement = _productProgressionElements[level];
                // Add new products to the needed products
                // Maybe drop some old ones?
                // Show Player the unlocked products
                foreach (ProductData productData in progressionElement)
                {
                    Debug.Log("You unlocked: " + productData.ProductName);
                    onProductUnlock?.Invoke(productData);
                }
            }
            
            if (!_buildingProgressionElements.ContainsKey(level))
            {
                // This Level has been reached by another city
                // Determine current Max level
                // Choose random products from these levels or increase current demand
            }
            else if (_buildingProgressionElements[level].Length == 0)
            {
                _buildingProgressionElements.Remove(level);
                // There are no unlocks for this level
                // increase current Demand
            }
            else
            {
                BuildingData[] progressionElement = _buildingProgressionElements[level];
                // Add new products to the needed products
                // Maybe drop some old ones?
                // Show Player the unlocked products
                onBuildingUnlock?.Invoke(progressionElement);
                foreach (BuildingData buildingData in progressionElement)
                {
                    Debug.Log("You unlocked: " + buildingData.BuildingName);
                }
            }
        };
    }

    private void FillBuildingProgression()
    {
        BuildingData[] levelOneData = new[]
        {
            Resources.Load<BuildingData>(Util.PathTo("Street")),
            Resources.Load<BuildingData>(Util.PathTo("Farm")),
            Resources.Load<BuildingData>(Util.PathTo("Mill")),
            // Test purpose
            Resources.Load<BuildingData>(Util.PathTo("Bakery")),
            Resources.Load<BuildingData>(Util.PathTo("Pump")),
            Resources.Load<BuildingData>(Util.PathTo("Rail")),
            Resources.Load<BuildingData>(Util.PathTo("Trainstation")),
            Resources.Load<BuildingData>(Util.PathTo("Harbor")),
            Resources.Load<BuildingData>(Util.PathTo("Storage")),
            Resources.Load<BuildingData>(Util.PathTo("Airport")),
        };
        _buildingProgressionElements.Add(0, levelOneData);

        BuildingData[] levelTwoData = new[]
        {
            Resources.Load<BuildingData>(Util.PathTo("Bakery")),
            Resources.Load<BuildingData>(Util.PathTo("Pump")),
//            Resources.Load<BuildingData>(PathUtil.Get("Trashdump"))
        };
        _buildingProgressionElements.Add(1, levelTwoData);

        BuildingData[] levelThreeData = new[]
        {
            Resources.Load<BuildingData>(Util.PathTo("Rail")),
            Resources.Load<BuildingData>(Util.PathTo("Trainstation")),
        };
        _buildingProgressionElements.Add(2, levelThreeData);

        BuildingData[] levelFourData = new[]
        {
            Resources.Load<BuildingData>(Util.PathTo("Harbor")),
            Resources.Load<BuildingData>(Util.PathTo("Storage"))
        };
        _buildingProgressionElements.Add(3, levelFourData);

        BuildingData[] levelFiveData = new[]
        {
            Resources.Load<BuildingData>(Util.PathTo("Airport")),
        };
        _buildingProgressionElements.Add(4, levelFiveData);
    }

    private void FillProductProgression()
    {
        // Flour Production
        ProductData[] levelOneData = new[]
        {
            Resources.Load<ProductData>(Util.PathTo("Flour")),
            Resources.Load<ProductData>(Util.PathTo("Wheat"))
        };
        _productProgressionElements.Add(0, levelOneData);

        // Bread Production
        ProductData[] levelTwoData = new[]
        {
            Resources.Load<ProductData>(Util.PathTo("Bread")),
            Resources.Load<ProductData>(Util.PathTo("Water")),
            Resources.Load<ProductData>(Util.PathTo("Trash"))
        };
        _productProgressionElements.Add(1, levelTwoData);

        // Mail
        ProductData[] levelThreeData = new[]
        {
            Resources.Load<ProductData>(Util.PathTo("Mail"))
        };
        _productProgressionElements.Add(2, levelThreeData);

        // Passengers
        ProductData[] levelFourData = new[]
        {
            Resources.Load<ProductData>(Util.PathTo("Passenger"))
        };
        _productProgressionElements.Add(3, levelFourData);
    }
}
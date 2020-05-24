using System;
using System.Runtime.CompilerServices;

public static class PathUtil
{
    // Data Folder
    private const string dataFolder = "Data/";

    private const string buildingDataFolder = dataFolder + "BuildingData/";
    private const string buildingDataInfrastructureFolder = buildingDataFolder + "Infrastructure/";
    private const string buildingDataProductionFolder = buildingDataFolder + "Production/";

    private const string productDataFolder = dataFolder + "ProductData/";

    private const string settingsFolder = dataFolder + "Settings/";

    private const string transportVehicleDataFolder = dataFolder + "TransportVehicleData/";

    // Blueprint Folder
    private const string blueprintFolder = "Blueprints/";

    // Materials Folder
    private const string materialFolder = "Materials/";

    // Prefabs Folder
    private const string prefabFolder = "Prefabs/";

    // Prefabs/Construction
    private const string constructionFolder = prefabFolder + "Construction/";
    private const string constructionBuildchoiceFolder = constructionFolder + "BuildChoice/";
    private const string constructionCityFolder = constructionFolder + "City/";
    private const string constructionCityCityFolder = constructionCityFolder + "City/";
    private const string constructionCityBuildingFolder = constructionCityFolder + "Building/";

    // Prefabs/Environment
    private const string environmentFolder = prefabFolder + "Environment/";

    // Prefabs/Transportation
    private const string transportationFolder = prefabFolder + "Transportation/";
    private const string transportationCreateFolder = transportationFolder + "Create/";
    private const string transportationOverviewFolder = transportationFolder + "Overview/";
    private const string transportationVehicleFolder = transportationFolder + "Vehicle/";
    private const string transportationVehicleTrailerFolder = transportationVehicleFolder + "Trailer/";

    // Prefabs/UI
    private const string uiFolder = prefabFolder + "UI/";

    public static string Get(string name)
    {
        switch (name)
        {
            // Blueprints
            case "ForrestBlueprint":
                return blueprintFolder + name;

            // Data/BuildingData/
            case "Infrastructure": return buildingDataInfrastructureFolder;
            case "Production": return buildingDataProductionFolder;
            case "Street":
            case "Rail":
            case "Airport":
            case "Harbor":
            case "Storage":
            case "Trainstation":
                return buildingDataInfrastructureFolder + name;
            case "Farm":
            case "Mill":
            case "Bakery":
            case "Pump":
                return buildingDataProductionFolder + name;

            // Data/ProductData/
            case "Bread":
            case "Flour":
            case "Mail":
            case "Passenger":
            case "Trash":
            case "Water":
            case "Wheat":
                return productDataFolder + name;

            // Data/TransportVehicleData
            case "TransportVehicleData":
                return transportVehicleDataFolder;

            // Materials
            case "TerrainMeshMaterial":
                return materialFolder + name;

            // Data/Settings/
            case "GameSettings":
            case "HeightMapSettings":
            case "MeshSettings":
                return settingsFolder + name;

            // Prefabs/Construction/BuildChoice
            case "ConstructionElementView":
                return constructionBuildchoiceFolder + name;

            // Prefabs/Construction/City
            case "CityWorldToScreenUi":
                return constructionCityFolder + name;

            // Prefabs/Construction/City/City
            case "City1-1_0":
            case "City1-1_1":
            case "City2-1":
                return constructionCityCityFolder + name;

            // Prefabs/Construction/City/Building
            case "Business3x3":
            case "Citizen3x3":
            case "Headquarter2x2":
            case "Main3x3":
                return constructionCityBuildingFolder + name;

            // Prefabs/Environment
            case "Cloud":
            case "Tree":
            case "Lake":
            case "Grass":
                return environmentFolder + name;

            // Prefabs/Transportation/Create
            case "AmountProductView":
            case "NeededProductView":
            case "ProductTreeElementConnector":
            case "ProductView":
            case "TransportRouteSettingProductView":
            case "VehicleOptionView":
                return transportationCreateFolder + name;

            // Prefabs/Transportation/Overview
            case "TransportRouteOverviewElementView":
                return transportationOverviewFolder + name;

            // Prefabs/Transportation/Vehicle
            case "Plane":
            case "Ship":
            case "Train":
            case "Truck":
                return transportationVehicleFolder + name;

            // Prefabs/Transportation/Vehicle/Trailer
            case "ContainerTrailer":
                return transportationVehicleTrailerFolder + name;

            // Prefabs/UI/
            case "CashflowAnimation":
            case "TextToolTip":
            case "BarChartValueView":
                return uiFolder + name;
        }

        throw new NotSupportedException("Path for " + name + " was not found.");
    }
}
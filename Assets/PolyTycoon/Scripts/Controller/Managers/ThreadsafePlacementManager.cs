using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

public static class ThreadsafePlacementManager
{
    public static ThreadsafePlaceable FindForrestPosition(IPlacementController placementController,
        ITerrainGenerator terrainGenerator, ThreadsafePlaceable placeable, float[,] values)
    {
        while (!terrainGenerator.IsReady(placeable.Position))
        {
            Thread.Sleep(50);
        }

        List<NeededSpace> coastNeededSpace = new List<NeededSpace>
        {
            new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Coast)
        };
        List<NeededSpace> flatLandNeededSpace = new List<NeededSpace>
        {
            new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Flatland)
        };
        List<NeededSpace> hillNeededSpace = new List<NeededSpace>
        {
            new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Hill)
        };
        List<NeededSpace> mountainNeededSpace = new List<NeededSpace>
        {
            new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Mountain)
        };
        float limit = 0.1f;

        for (int x = 0; x < values.GetLength(0) - 3; x++)
        {
            for (int y = 0; y < values.GetLength(1) - 3; y++)
            {
                Vector3Int relativePosition =
                    new Vector3Int(x - values.GetLength(0) / 2 + 1, 0, y - values.GetLength(1) / 2 + 1);
                Vector3 absolutePosition = relativePosition + placeable.Position;
                bool isThreshold = (values[x, y] > limit - 0.05f && values[x, y] < limit + 0.05f);
                if (!isThreshold) continue;

                TerrainGenerator.TerrainType terrainType = terrainGenerator.GetTerrainType(absolutePosition.x, absolutePosition.z);
                bool isPlaceable = false;
                switch (terrainType)
                {
                    case TerrainGenerator.TerrainType.Flatland:
                        isPlaceable = placementController.IsPlaceable(absolutePosition, flatLandNeededSpace);
                        break;
                    case TerrainGenerator.TerrainType.Coast:
                        isPlaceable = placementController.IsPlaceable(absolutePosition, coastNeededSpace);
                        break;
                    case TerrainGenerator.TerrainType.Hill:
                        isPlaceable = placementController.IsPlaceable(absolutePosition, hillNeededSpace);
                        break;
                    case TerrainGenerator.TerrainType.Mountain:
                        isPlaceable = placementController.IsPlaceable(absolutePosition, mountainNeededSpace);
                        break;
                }
                if (!isPlaceable) continue;
                placeable.NeededSpaces.Add(new ProceduralNeededSpace(relativePosition, terrainType, values[x, y]));
            }
        }

        return placeable;
    }


    /// <summary>
    /// This implementation turns the given placeable in a square around it's starting position until a valid position is found.
    /// </summary>
    /// <param name="placementController"></param>
    /// <param name="terrainGenerator"></param>
    /// <param name="placeable"></param>
    /// <returns></returns>
    public static ThreadsafePlaceable MoveToPlaceablePosition(IPlacementController placementController,
        ITerrainGenerator terrainGenerator, ThreadsafePlaceable placeable)
    {
        while (!terrainGenerator.IsReady(placeable.Position))
        {
            Thread.Sleep(50);
        }

        int stepSize = 2;
        int targetXMove = 0;
        int targetYMove = 0;
        int currentXMove = 0;
        int currentYMove = 0;
        int direction = -1;

        // Moves the cityPlaceable in a growing square around the starting position until a suitable location is found
        while (!placementController.IsPlaceable(placeable.Position, placeable.NeededSpaces))
        {
            // Move one step at a time
            if (currentXMove > 0)
            {
                placeable.Translate(direction * stepSize, 0, 0);
                currentXMove--;
                continue;
            }

            if (currentYMove > 0)
            {
                placeable.Translate(0, 0, direction * stepSize);
                currentYMove--;
                continue;
            }

            // Control step Amount
            if (targetXMove == targetYMove)
            {
                direction = -direction;
                targetXMove++;
                currentXMove = targetXMove;
            }
            else
            {
                targetYMove = targetXMove;
                currentYMove = targetYMove;
            }
        }

        return placeable;
    }
}
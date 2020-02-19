using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class ThreadsafePlacementManager
{
    public static ThreadsafePlaceable FindForrestPosition(IPlacementController placementController,
        ITerrainGenerator terrainGenerator, ThreadsafePlaceable placeable, Color32[] placementTexture)
    {
        while (!terrainGenerator.IsReady(placeable.Position))
        {
            Thread.Sleep(50);
        }

        List<NeededSpace> neededSpaces = new List<NeededSpace>
        {
            new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Flatland)
        };

        double sqrt = Math.Sqrt(placementTexture.Length);
        for (int i = 0; i < placementTexture.Length; i++)
        {
            int x = (int) (i % sqrt);
            int y = (int) (i / sqrt);
            Color color = placementTexture[i];
            float density = color.grayscale;
            Vector3Int relativePosition = new Vector3Int(x, 0, y);
            if (density <= 0.1f &&
                placementController.IsPlaceable(relativePosition + placeable.Position, neededSpaces))
            {
                placeable.NeededSpaces.Add(new NeededSpace(relativePosition,
                    TerrainGenerator.TerrainType.Flatland));
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
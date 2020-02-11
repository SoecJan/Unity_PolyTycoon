using System.Threading;
using UnityEngine;

public static class ThreadsafePlacementManager
{
    public static ThreadsafePlaceable MoveToPlaceablePosition(PlacementController placementController, TerrainGenerator terrainGenerator, ThreadsafePlaceable placeable)
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
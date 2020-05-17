using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestTraversalCalculation
    {
        private static readonly Vector3 TraversalOffset = Vector3.zero;
        
        // A Test behaves as an ordinary method
        [Test]
        public void TestTraversalCalculationSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        [UnityTest]
        public IEnumerator TraversalVector_MoveStraight()
        {
            WayPoint result = GetTraversalVectors(PathFindingNode.Up, PathFindingNode.Down);

            Assert.AreEqual(2, result.TraversalVectors.Length);
            Assert.AreEqual(TraversalPoints.TopLeft, result.TraversalVectors[0]);
            Assert.AreEqual(TraversalPoints.BottomLeft, result.TraversalVectors[1]);
            Assert.AreEqual(0, result.Radius);
            yield return null;
        }
        
        ///<summary>Returns the TraversalVectors as a WayPoint Object.</summary>
        private static WayPoint GetTraversalVectors(int fromDirection, int toDirection)
        {
            // Start Points
            if (fromDirection == -1)
            {
                switch (toDirection)
                {
                    case PathFindingNode.Up:
                        return new WayPoint(TraversalPoints.CenterBottomRight + TraversalOffset,
                            TraversalPoints.TopRight + TraversalOffset);
                    case PathFindingNode.Right:
                        return new WayPoint(TraversalPoints.CenterBottomLeft + TraversalOffset,
                            TraversalPoints.RightBottom + TraversalOffset);
                    case PathFindingNode.Down:
                        return new WayPoint(TraversalPoints.CenterTopLeft + TraversalOffset,
                            TraversalPoints.BottomLeft + TraversalOffset);
                    case PathFindingNode.Left:
                        return new WayPoint(TraversalPoints.CenterTopRight + TraversalOffset,
                            TraversalPoints.LeftTop + TraversalOffset);
                }
            }

            // End Points

            if (toDirection == -1)
            {
                switch (fromDirection)
                {
                    case PathFindingNode.Up:
                        return new WayPoint(TraversalPoints.TopLeft + TraversalOffset,
                            TraversalPoints.CenterBottomLeft + TraversalOffset);
                    case PathFindingNode.Right:
                        return new WayPoint(TraversalPoints.RightTop + TraversalOffset,
                            TraversalPoints.CenterTopLeft + TraversalOffset);
                    case PathFindingNode.Down:
                        return new WayPoint(TraversalPoints.BottomRight + TraversalOffset,
                            TraversalPoints.CenterTopRight + TraversalOffset);
                    case PathFindingNode.Left:
                        return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset,
                            TraversalPoints.CenterBottomRight + TraversalOffset);
                }
            }

            // Straights

            if (fromDirection == PathFindingNode.Up && toDirection == PathFindingNode.Down)
            {
                return new WayPoint(TraversalPoints.TopLeft + TraversalOffset,
                    TraversalPoints.BottomLeft + TraversalOffset);
            }

            if (fromDirection == PathFindingNode.Down && toDirection == PathFindingNode.Up)
            {
                return new WayPoint(TraversalPoints.BottomRight + TraversalOffset,
                    TraversalPoints.TopRight + TraversalOffset);
            }

            if (fromDirection == PathFindingNode.Left && toDirection == PathFindingNode.Right)
            {
                return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset,
                    TraversalPoints.RightBottom + TraversalOffset);
            }

            if (fromDirection == PathFindingNode.Right && toDirection == PathFindingNode.Left)
            {
                return new WayPoint(TraversalPoints.RightTop + TraversalOffset,
                    TraversalPoints.LeftTop + TraversalOffset);
            }

            // Inner Corners

            float innerCornerRadius = 0.5f;

            if (fromDirection == PathFindingNode.Up && toDirection == PathFindingNode.Left)
            {
                return new WayPoint(TraversalPoints.TopLeft + TraversalOffset,
                    TraversalPoints.CenterTopLeft + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset,
                    innerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Down && toDirection == PathFindingNode.Right)
            {
                return new WayPoint(TraversalPoints.BottomRight + TraversalOffset,
                    TraversalPoints.CenterBottomRight + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset,
                    innerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Left && toDirection == PathFindingNode.Down)
            {
                return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset,
                    TraversalPoints.CenterBottomLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset,
                    innerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Right && toDirection == PathFindingNode.Up)
            {
                return new WayPoint(TraversalPoints.RightTop + TraversalOffset,
                    TraversalPoints.CenterTopRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset,
                    innerCornerRadius);
            }

            // Outer Corners

            float outerCornerRadius = 0.75f;

            if (fromDirection == PathFindingNode.Up && toDirection == PathFindingNode.Right)
            {
                return new WayPoint(TraversalPoints.TopLeft + TraversalOffset,
                    TraversalPoints.CenterBottomLeft + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset,
                    outerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Down && toDirection == PathFindingNode.Left)
            {
                return new WayPoint(TraversalPoints.BottomRight + TraversalOffset,
                    TraversalPoints.CenterTopRight + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset,
                    outerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Left && toDirection == PathFindingNode.Up)
            {
                return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset,
                    TraversalPoints.CenterBottomRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset,
                    outerCornerRadius);
            }

            if (fromDirection == PathFindingNode.Right && toDirection == PathFindingNode.Down)
            {
                return new WayPoint(TraversalPoints.RightTop + TraversalOffset,
                    TraversalPoints.CenterTopLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset,
                    outerCornerRadius);
            }

            Debug.LogError("Should not reach here! Input: " + fromDirection + "; " + toDirection);
            return new WayPoint(Vector3.zero, Vector3.zero);
        }
    }
}

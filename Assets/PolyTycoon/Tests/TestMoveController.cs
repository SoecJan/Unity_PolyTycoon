using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

namespace PolyTycoon.Tests
{
    public class TestMoveController
    {
        [UnityTest]
        public IEnumerator MoveControllerTest_MoveStraightToTargetPosition()
        {
            GameObject moverObject = new GameObject("Mover");
            WaypointMoverController moverController = new WaypointMoverController(moverObject.transform);
            List<WayPoint> wayPoints = new List<WayPoint> {new WayPoint(Vector3.zero, Vector3.forward), new WayPoint(Vector3.forward*5, Vector3.forward*6)};
            moverController.WaypointList = wayPoints;

            bool hasArrived = false;
            moverController.OnArrive += () => hasArrived = true;

            while (!hasArrived) yield return moverController.Move();
            
            Assert.True(hasArrived);
            Assert.AreEqual(Vector3.forward*6, moverObject.transform.position);
        }
        
        [UnityTest]
        public IEnumerator MoveControllerTest_MoveCurveToTargetPosition()
        {
            GameObject moverObject = new GameObject("Mover");
            WaypointMoverController moverController = new WaypointMoverController(moverObject.transform);
            List<WayPoint> wayPoints = new List<WayPoint>
            {
                new WayPoint(Vector3.zero, Vector3.forward), 
                new WayPoint(Vector3.forward, Vector3.forward*2, Vector3.forward + Vector3.right, 1f),
                new WayPoint(Vector3.forward + Vector3.right, Vector3.forward + (Vector3.right*2))
            };
            moverController.WaypointList = wayPoints;

            bool hasArrived = false;
            moverController.OnArrive += () => hasArrived = true;

            while (!hasArrived) yield return moverController.Move();
            
            Assert.True(hasArrived);
            Assert.AreEqual(Vector3.forward + (Vector3.right*2), moverObject.transform.position);
        }
    }
}

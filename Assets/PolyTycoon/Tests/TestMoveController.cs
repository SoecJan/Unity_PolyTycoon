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
        /**
         * 
         */
        public IEnumerator MoveControllerTest_MoveStraightToTargetPosition()
        {
            GameObject moverObject = new GameObject("Mover");
            WaypointMoverController moverController = new WaypointMoverController(moverObject.transform);
            List<WayPoint> wayPoints = new List<WayPoint> {new WayPoint(Vector3.zero, Vector3.forward)};
            moverController.WaypointList = wayPoints;

            bool hasArrived = false;
            moverController.OnArrive += () => hasArrived = true;

            yield return moverController.Move();
            
            Assert.True(hasArrived);
            Assert.AreEqual(Vector3.forward, moverObject.transform.position);
        }
        
        [UnityTest]
        /**
         * 
         */
        public IEnumerator MoveControllerTest_CorrectCallbackAmounts()
        {
            GameObject moverObject = new GameObject("Mover");
            WaypointMoverController moverController = new WaypointMoverController(moverObject.transform);
            List<WayPoint> wayPoints = new List<WayPoint> {new WayPoint(Vector3.zero, Vector3.forward, Vector3.right, 1f)};
            moverController.WaypointList = wayPoints;

            bool hasArrived = false;
            moverController.OnArrive += () => hasArrived = true;

            yield return moverController.Move();
            
            Assert.True(hasArrived);
            Assert.AreEqual(Vector3.right, moverObject.transform.position);
        }
    }
}

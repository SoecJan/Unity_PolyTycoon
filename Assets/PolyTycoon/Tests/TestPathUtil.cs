using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PolyTycoon.Tests
{
    
    public class TestPathUtil : MonoBehaviour
    {
        [UnityTest]
        public IEnumerator Test_LoadPrefabByName()
        {
            
            Assert.AreEqual("Prefabs/UI/TextToolTip", Util.PathTo("TextToolTip"));
            yield return null;
        }
    }
}

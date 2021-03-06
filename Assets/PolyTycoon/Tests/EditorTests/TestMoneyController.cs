using System;
using NUnit.Framework;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestMoneyController
    {
        private MoneyController _moneyController;
        
        [Test]
        public void TestMoneyCallback()
        {
            _moneyController = new MoneyController(100);
            long value = 0;
            _moneyController.OnValueChange += delegate(long oldValue, long currentValue) { value = currentValue; };

            _moneyController.MoneyAmount += 100;
            Assert.AreEqual(200, value);
            Assert.AreEqual(200, _moneyController.MoneyAmount);
        }

//        [UnityTest]
//        public IEnumerator Tes()
//        {
//            
//            yield return null;
//        }
    }
}
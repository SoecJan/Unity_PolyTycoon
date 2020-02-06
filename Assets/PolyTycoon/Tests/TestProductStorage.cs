using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

namespace PolyTycoon.Tests
{
    public class TestProductStorage
    {
        [UnityTest]
        /**
         * Tests to see if the Storage does not overload and throw an Exception
         */
        public IEnumerator ProductStorageTest_NotOverload()
        {
            ProductData productData = Resources.Load<ProductData>("Data/ProductData/Wheat");
            ProductStorage productStorage = new ProductStorage(productData, 10);
            
            // For test procedure
            int amountOfAddedProducts = 10;
            int transferAmount = Random.Range(1, 5);
            
            for (int i = 0; i < amountOfAddedProducts; i++)
            {
                if (productStorage.Amount + transferAmount > productStorage.MaxAmount)
                {
                    Assert.Throws<OverflowException>(() => productStorage.Add(transferAmount));
                }
                else
                {
                    productStorage.Add(transferAmount);
                }
            }
            Assert.AreEqual((amountOfAddedProducts/transferAmount) * transferAmount, productStorage.Amount);
            yield return null;
        }
        
        [UnityTest]
        /**
         * Tests to see if the productStorage.OnAmountChange callbacks function correctly.
         */
        public IEnumerator ProductStorageTest_CorrectCallbackAmounts()
        {
            ProductData productData = Resources.Load<ProductData>("Data/ProductData/Wheat");
            ProductStorage productStorage = new ProductStorage(productData, 10);

            // For verification
            int amountOfCallbacks = 0;
            int callbackProductAmount = productStorage.Amount;
            // For test procedure
            int amountOfAddedProducts = 10;
            int transferAmount = Random.Range(1, 5);
            
            productStorage.OnAmountChange += (storage, amount) => amountOfCallbacks++;
            productStorage.OnAmountChange += (storage, amount) => callbackProductAmount += amount;

            for (int i = 0; i < amountOfAddedProducts; i++)
            {
                if (productStorage.Amount + transferAmount > productStorage.MaxAmount)
                {
                    Assert.Throws<OverflowException>(() => productStorage.Add(transferAmount));
                }
                else
                {
                    productStorage.Add(transferAmount);
                }
            }
            
            Assert.AreEqual(amountOfAddedProducts/transferAmount, amountOfCallbacks);
            Assert.AreEqual(productStorage.Amount, callbackProductAmount);
            yield return null;
        }
    }
}

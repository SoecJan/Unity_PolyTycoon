using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ProducerData", menuName = "PolyTycoon/ProducerData", order = 1)]
public class BuildingProducerData : BuildingData
{
    [SerializeField] private ProductData _producedProduct;

    public ProductData ProducedProduct => _producedProduct;
}

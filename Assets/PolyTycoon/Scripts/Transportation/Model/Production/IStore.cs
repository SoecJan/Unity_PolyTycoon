
    using System.Collections.Generic;

    public interface IStore
    {
        Dictionary<ProductData, ProductStorage> StoredProducts();
    }
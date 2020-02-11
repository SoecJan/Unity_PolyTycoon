public class TransportRouteSetting
{

    #region Getter & Setter

    public ProductData ProductData { get; set; }

    public int Amount { get; set; }

    public bool IsLoad { get; set; }

    #endregion

    #region Methods

    public TransportRouteSetting Clone()
    {
        TransportRouteSetting settings = new TransportRouteSetting
        {
            ProductData = ProductData,
            Amount = Amount,
            IsLoad = IsLoad
        };
        return settings;
    }

    public override string ToString()
    {
        return "Setting: IsLoad? " + IsLoad.ToString() + "; Product: " + ProductData.ProductName + "; Amount: " +
               Amount.ToString();
    }

    #endregion
}
public class TransportRouteSetting
{

    #region Getter & Setter

    public ProductData ProductData { get; set; }

    public int Amount { get; set; }

    public bool IsLoad { get; set; }

    public RouteSettingWaitStatus WaitStatus { get; set; }

    #endregion

    public enum RouteSettingWaitStatus { DONTWAIT, WAITFOR, WAITUNTIL };

    #region Methods

    public TransportRouteSetting Clone()
    {
        TransportRouteSetting settings = new TransportRouteSetting
        {
            ProductData = ProductData,
            Amount = Amount,
            IsLoad = IsLoad,
            WaitStatus = WaitStatus
        };
        return settings;
    }

    public override string ToString()
    {
        return "Setting: IsLoad? " + IsLoad.ToString() + "; Product: " + ProductData.ProductName + "; Amount: " +
               Amount.ToString() + "; WaitStatus: " + WaitStatus.ToString();
    }

    #endregion
}
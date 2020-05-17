public struct HeightMap {
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue) {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
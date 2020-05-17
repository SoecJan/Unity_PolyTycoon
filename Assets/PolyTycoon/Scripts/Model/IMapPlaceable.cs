/// <summary>
/// This interface describes the functionality of a MapPlaceable.
/// </summary>
public interface IMapPlaceable
{
    /// <summary>
    /// If the associated MapPlaceable can be dragged to created multiple instances in one line.
    /// </summary>
    bool IsDraggable { get; set; }
}
using UnityEngine;

public interface ITreeManager
{
    /// <summary>
    /// This Function returns a TreeBehaviour instance that can be used to get a random forrest
    /// </summary>
    /// <returns></returns>
    TreeBehaviour GetRandomTree();

    /// <summary>
    /// This method is supposed to be called by the <see cref="ThreadsafePlacementManager"/>.
    /// 
    /// </summary>
    /// <param name="placementObject"></param>
    void OnTreePositionFound(object placementObject);

    Texture2D GetRandomForrestBlueprint();
}
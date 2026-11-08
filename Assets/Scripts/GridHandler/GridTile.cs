using UnityEngine;

public class GridTile : MonoBehaviour
{

    private CollectibleHandler _collectibleHandler; 

    public virtual void OnWalk()
    {

        if (_collectibleHandler != null)
        {
            _collectibleHandler.GetCollectible();
            _collectibleHandler = null;

        }
    }

    public virtual void OnCollectibleGet(CollectibleHandler collectible)
    {
        _collectibleHandler = collectible;
    }
}

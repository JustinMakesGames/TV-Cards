using UnityEngine;

public class GridTile : MonoBehaviour
{

    private CollectibleHandler _collectibleHandler;
    private Transform _player;

    public virtual void OnWalk(Transform player)
    {

        if (_collectibleHandler != null)
        {
            _collectibleHandler.GetCollectible(player);
            _collectibleHandler = null;

        }
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }

    public void DeletePlayuer()
    {
        _player = null;
    }

    public Transform ReturnPlayer()
    {
        return _player;
    }

    
}

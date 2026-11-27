using UnityEngine;

public class GridTile : MonoBehaviour
{
    
    private GameObject _collectible;
    private GameObject _collectiblePrefab;
    private Transform _player;

    public virtual void OnWalk(Transform player)
    {

        if (_collectible != null && _collectible.TryGetComponent(out CollectibleHandler collectibleHandler))
        {
            collectibleHandler.GetCollectible(player);
            Destroy(_collectible);

        }
    }

    public void GetCollectible(GameObject collectible)
    {
        _collectiblePrefab = collectible;
        _collectible = Instantiate(collectible, transform.position + Vector3.up * 0.2f, transform.rotation);
    }

    public bool HasCollectible()
    {
        return _collectible != null;
    }

    public GameObject ReturnCollectiblePrefab()
    {
        return HasCollectible() ? _collectiblePrefab : null;
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }

    public void DeletePlayer()
    {
        _player = null;
    }

    public Transform ReturnPlayer()
    {
        return _player;
    }

    
}

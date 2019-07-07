using UnityEngine;

[RequireComponent(typeof(TileManager))]
[RequireComponent(typeof(BoxCollider2D))]
public class Tile : MonoBehaviour
{
    private TileValue tileValue;
    private Vector3 destination;

    private const float speed = 2f;

    private bool followMouse;
    private SpriteRenderer sr;
    private MeshRenderer childMr;
    private BoxCollider2D bc;
    private Support support;

    public void SetSupport(Support newSupport)
        => support = newSupport;

    private void Start()
    {
        followMouse = false;
        sr = GetComponent<SpriteRenderer>();
        childMr = GetComponentInChildren<MeshRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!followMouse)
            transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
        else
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.position = pos;
        }
    }

    public void SetOnTopLayer()
    {
        sr.sortingOrder = TileManager.currLayerId;
        childMr.sortingOrder = TileManager.currLayerId;
        TileManager.currLayerId++;
    }

    public void PressTile()
    {
        followMouse = true;
        SetOnTopLayer();
        support.ResetHover();
        bc.enabled = false;
    }

    public void ReleaseTile()
    {
        support.MoveTile(this);
        followMouse = false;
    }

    public void SetDestination(Vector3 dest) // The tile save it position on the support
        => destination = dest;

    // Only when on board

    private void OnMouseDown()
        => PressTile();
    private void OnMouseUp()
        => ReleaseTile();
}

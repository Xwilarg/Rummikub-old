using System.Collections.Generic;
using UnityEngine;

public class Support : MonoBehaviour
{
    [SerializeField]
    private GameObject supportTilePrefab;

    private Tile[,] supportTiles;
    private TileSelector[,] backgroundTiles;
    private TileSelector currentHover;
    private List<Tile> board;
    private float supportYLimit; // Delimitation between the support and the board

    // Support size: 2*20 + offset of 10px on each side
    private const float tileWidth = 0.75f;
    private const float tileHeight = 1.02f;

    private void Start()
    {
        currentHover = null;
        supportTiles = new Tile[2, 20];
        backgroundTiles = new TileSelector[2, 20];
        board = new List<Tile>();
        for (int i = 0; i < 2; i++)
            for (int y = 0; y < 20; y++)
            {
                GameObject go = Instantiate(supportTilePrefab, transform);
                go.transform.position = new Vector3(transform.position.x + tileWidth * (y - 9), transform.position.y + tileHeight * (1 - i));
                TileSelector ts = go.GetComponent<TileSelector>();
                ts.SetSupport(this);
                backgroundTiles[i, y] = ts;
                supportTiles[i, y] = null;
            }
        supportYLimit = backgroundTiles[0, 0].transform.position.y + tileHeight;
    }

    public void ResetHover()
        => currentHover = null;

    public void SetHover(TileSelector ts)
        => currentHover = ts;

    public void UnsetHover(TileSelector oldTs)
        => currentHover = (currentHover == oldTs ? null : currentHover);

    public void AddTile(Tile t)
    {
        for (int i = 0; i < 2; i++)
            for (int y = 0; y < 20; y++)
                if (supportTiles[i, y] == null)
                {
                    supportTiles[i, y] = t;
                    t.SetDestination(backgroundTiles[i, y].transform.position);
                    t.SetSupport(this);
                    return;
                }
    }

    public void ClickTile(TileSelector ts, bool press)
    {
        for (int i = 0; i < 2; i++)
            for (int y = 0; y < 20; y++)
            {
                if (backgroundTiles[i, y] == ts)
                {
                    if (supportTiles[i, y] != null)
                    {
                        if (press) supportTiles[i, y].PressTile();
                        else supportTiles[i, y].ReleaseTile();
                    }
                    break;
                }
            }
    }

    /// Exchange the position of 2 tiles in the support
    public void MoveTile(Tile t)
    {
        int oldX = -1, oldY = -1;
        int newX = -1, newY = -1;
        for (int i = 0; i < 2; i++)
            for (int y = 0; y < 20; y++)
            {
                if (supportTiles[i, y] == t)
                {
                    oldX = i;
                    oldY = y;
                }
                if (backgroundTiles[i, y] == currentHover)
                {
                    newX = i;
                    newY = y;
                }
            }
        // TODO: For now the program iterate in all tiles everytimes we move it
        // The tile can probably keep track if it's on the board or not, instead
        if (oldX == -1 && oldY == -1)
            t.SetDestination(t.transform.position);
        else if (currentHover == null)
            MoveBoardTile(t, oldX, oldY);
        else
            MoveSupportTile(t, oldX, oldY, newX, newY);
    }

    private void MoveBoardTile(Tile t, int oldX, int oldY)
    {
        if (t.transform.position.y < supportYLimit)
            return;
        board.Add(t);
        supportTiles[oldX, oldY] = null;
        t.SetDestination(t.transform.position);
        t.GetComponent<BoxCollider2D>().enabled = true;
        // Set position properly if there are tiles next to the current one
        foreach (float f in new[] { -1f, 1f })
        {
            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(t.transform.position.x + tileWidth * f, t.transform.position.y),
                new Vector2(t.transform.position.x + (tileWidth + 1f) * f, t.transform.position.y));
            if (hit.distance > 0f && hit.distance < tileWidth)
                Debug.Log(hit.collider.name + " ; " + hit.distance);
            Debug.DrawLine(new Vector2(t.transform.position.x + tileWidth * f, t.transform.position.y),
                new Vector2(t.transform.position.x + (tileWidth + 1f) * f, t.transform.position.y), Color.red, 10000f);
        }
    }

    private void MoveSupportTile(Tile t, int oldX, int oldY, int newX, int newY)
    {
        // Swap 2 tiles
        Tile tmp = supportTiles[newX, newY];
        supportTiles[newX, newY] = t;
        t.SetDestination(backgroundTiles[newX, newY].transform.position);
        supportTiles[oldX, oldY] = tmp;
        if (tmp != null)
        {
            tmp.SetDestination(backgroundTiles[oldX, oldY].transform.position);
            tmp.SetOnTopLayer();
        }
    }
}

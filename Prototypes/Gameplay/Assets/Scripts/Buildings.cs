using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Buildings : MonoBehaviour
{

    public Texture2D _texture;

    float _blockSize = 0.75f;
    float _wallHeight = 5.0f;
    float _obstacleHeight = 2.5f;

    List<Vector3> _wallVertices = new List<Vector3>();
    List<Vector3> _wallNormals = new List<Vector3>();
    List<Vector2> _wallUvs = new List<Vector2>();
    List<int> _wallTriangles = new List<int>();
    List<Vector3> _obstacleVertices = new List<Vector3>();
    List<Vector3> _obstacleNormals = new List<Vector3>();
    List<Vector2> _obstacleUvs = new List<Vector2>();
    List<int> _obstacleTriangles = new List<int>();

    ObstacleManager _obstacleManager;


    void Awake()
    {
        TeamManager teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        _obstacleManager = GameObject.Find("ObstacleManager").GetComponent<ObstacleManager>();
        List<Vector3> _spawnerPositions = new List<Vector3>();

        Mesh mesh = new Mesh();

        Color32[] table = _texture.GetPixels32();
        for (int x0 = 0; x0 < _texture.width; x0++)
        {
            for (int y0 = 0; y0 < _texture.height; y0++)
            {
                Color32 c = table[x0 * _texture.height + y0];
                
                if (c.g == 255) // white for air
                {
                    continue;
                }
                else if (c.g == 230) //green for obstacles
                {
                    buildObstacle(x0, y0, table, c);
                    continue;
                }
                else if (c.g == 100) // orange for spawner
                {
                    // create a spawner
                    Vector3 spawnerPos = new Vector3(_blockSize * x0, 0.0f, _blockSize * y0);
                    GameObject spawner = Instantiate(Resources.Load<GameObject>("Spawner")) as GameObject;
                    spawner.transform.position = spawnerPos;
                    teamManager._spawnerPositions.Add(spawnerPos);
                    continue;
                }

                addBlock(x0, y0, table, c, true, Vector3.zero);
            }
        }

        mesh.vertices = _wallVertices.ToArray();
        mesh.uv = _wallUvs.ToArray();
        mesh.normals = _wallNormals.ToArray();
        mesh.triangles = _wallTriangles.ToArray();

        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    void addBlock(int x0, int y0, Color32[] table, Color32 c, bool isWall, Vector3 offset)
    {
        float blockHeight = _obstacleHeight;
        if (isWall)
        {
            blockHeight = _wallHeight;
        }

        // quad each side
        if (x0 == 0 || table[(x0 - 1) * _texture.height + y0].g != c.g)
        {
            addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, 0.0f, _blockSize), new Vector3(0.0f, blockHeight, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f), isWall);
        }

        if (x0 == _texture.width - 1 || table[(x0 + 1) * _texture.width + y0].g != c.g)
        {
            addQuad(new Vector3(_blockSize * (x0 + 1), 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, blockHeight, 0.0f), new Vector3(0.0f, 0.0f, _blockSize), new Vector3(1.0f, 0.0f, 0.0f), isWall);
        }

        if (y0 == 0 || table[x0 * _texture.height + y0 - 1].g != c.g)
        {
            addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, blockHeight, 0.0f), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f), isWall);
        }

        if (y0 == _texture.height - 1 || table[x0 * _texture.height + y0 + 1].g != c.g)
        {
            addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * (y0 + 1)) + offset, new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, blockHeight, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), isWall);
        }

        // on top
        addQuad(new Vector3(_blockSize * x0, blockHeight, _blockSize * y0) + offset, new Vector3(0.0f, 0.0f, _blockSize), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), isWall);
    }

    void addQuad(Vector3 point1, Vector3 vector1, Vector3 vector2, Vector3 normal, bool isWall)
    {
        List<Vector3> vertices = null;
        List<Vector3> normals = null;
        List<Vector2> uvs = null;
        List<int> triangles = null;

        if (isWall)
        {
            vertices = _wallVertices;
            normals = _wallNormals;
            uvs = _wallUvs;
            triangles = _wallTriangles;
        }
        else
        {
            vertices = _obstacleVertices;
            normals = _obstacleNormals;
            uvs = _obstacleUvs;
            triangles = _obstacleTriangles;
        }

        triangles.Add(vertices.Count);
        triangles.Add(vertices.Count + 1);
        triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 3);
        triangles.Add(vertices.Count);

        vertices.Add(point1);
        normals.Add(normal);
        uvs.Add(Vector2.zero);

        vertices.Add(point1 + vector1);
        normals.Add(normal);
        uvs.Add(new Vector2(1.0f, 0.0f));

        vertices.Add(point1 + vector1 + vector2);
        normals.Add(normal);
        uvs.Add(new Vector2(1.0f, 1.0f));

        vertices.Add(point1 + vector2);
        normals.Add(normal);
        uvs.Add(new Vector2(0.0f, 1.0f));
    }


    void buildObstacle(int x0, int y0, Color32[] table, Color32 c)
    {
        // Instantiate obstacle
        GameObject obstacle = Instantiate(Resources.Load<GameObject>("Obstacle"), Vector3.zero, Quaternion.identity) as GameObject;

        // Obstacle mesh
        Mesh mesh = new Mesh();

        _obstacleVertices.Clear();
        _obstacleNormals.Clear();
        _obstacleUvs.Clear();
        _obstacleTriangles.Clear();

        // Find the height and the width of the obstacle
        int xMax = x0, yMax = y0;
        for (xMax = x0; xMax < _texture.width;)
        {
            xMax++;
            if (table[xMax * _texture.height + y0].g != c.g)
            {
                break;
            }
        }
        for (yMax = y0; yMax < _texture.height;)
        {
            yMax++;
            if (table[x0 * _texture.height + yMax].g != c.g)
            {
                break;
            }
        }
        obstacle.transform.position = new Vector3((x0 + xMax) * 0.5f * _blockSize, 0.0f, (y0 + yMax) * 0.5f * _blockSize);

        // Build mesh
        for (int x1 = x0; x1 < xMax; x1++)
        {
            for (int y1 = y0; y1 < yMax; y1++)
            {
                addBlock(x1, y1, table, c, false, -obstacle.transform.position);
            }
        }
        // clear obstacle pixels
        for (int x1 = x0; x1 < xMax; x1++)
        {
            for (int y1 = y0; y1 < yMax; y1++)
            {
                // clear obstacle pixels
                table[x1 * _texture.height + y1] = new Color32(255, 255, 255, 255);
            }
        }
        mesh.vertices = _obstacleVertices.ToArray();
        mesh.uv = _obstacleUvs.ToArray();
        mesh.normals = _obstacleNormals.ToArray();
        mesh.triangles = _obstacleTriangles.ToArray();
        mesh.RecalculateBounds();

        obstacle.GetComponent<MeshFilter>().sharedMesh = mesh;
        obstacle.GetComponent<MeshCollider>().sharedMesh = mesh;

        _obstacleManager.obstacles.Add(obstacle.GetComponent<Obstacle>());
    }

}
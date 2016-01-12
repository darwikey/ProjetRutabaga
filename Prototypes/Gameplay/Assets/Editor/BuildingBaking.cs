using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class BuildingBaking : MonoBehaviour
{
    // parameters
    static float _blockSize = 0.75f;
    static float _wallHeight = 5.0f;
    static float _obstacleHeight = 2.5f;

    class BuildingInfo
    {
        public GameObject _building;

        public List<Vector3> _wallVertices = new List<Vector3>();
        public List<Vector3> _wallNormals = new List<Vector3>();
        public List<Vector2> _wallUvs = new List<Vector2>();
        public List<int> _wallTriangles = new List<int>();
        public List<Vector3> _obstacleVertices = new List<Vector3>();
        public List<Vector3> _obstacleNormals = new List<Vector3>();
        public List<Vector2> _obstacleUvs = new List<Vector2>();
        public List<int> _obstacleTriangles = new List<int>();

        public Texture2D _texture;
    }

    static ObstacleManager _obstacleManager;

    [MenuItem("Building/Building baking")]
    static void bake()
    {
        BuildingInfo info = new BuildingInfo();
        info._building = GameObject.Find("Building");

        if (info._building == null)
        {
            Debug.LogError("unabe to find Building");
            return;
        }

        info._texture = info._building.GetComponent<Buildings>()._texture;
        //TeamManager teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        _obstacleManager = GameObject.Find("ObstacleManager").GetComponent<ObstacleManager>();
        GameObject zonePrefab = Resources.Load<GameObject>("zone");

        Mesh mesh = new Mesh();

        Color32[] table = info._texture.GetPixels32();
        for (int x0 = 0; x0 < info._texture.width; x0++)
        {
            for (int y0 = 0; y0 < info._texture.height; y0++)
            {
                Color32 c = table[x0 * info._texture.height + y0];

                if (c.g == 255) // white for air
                {
                    continue;
                }
                else if (c.g == 230) //green for obstacles
                {
                    buildObstacle(info, x0, y0, table, c);
                    continue;
                }
                //else if (c.g == 100) // orange for spawner
                //{
                //    // create a spawner
                //    Vector3 spawnerPos = new Vector3(_blockSize * x0, 0.0f, _blockSize * y0);
                //    GameObject spawner = Instantiate(Resources.Load<GameObject>("Spawner")) as GameObject;
                //    spawner.transform.position = spawnerPos;
                //    teamManager._spawnerPositions.Add(spawnerPos);
                //    continue;
                //}
                //else if (c.g == 50) // violet for zone
                //{
                //    GameObject zone = Instantiate(zonePrefab) as GameObject;
                //    zone.transform.position = new Vector3(_blockSize * x0, 0.0f, _blockSize * y0);
                //    continue;
                //}

                addBlock(info, x0, y0, table, c, true, Vector3.zero);
            }
        }

        mesh.vertices = info._wallVertices.ToArray();
        mesh.uv = info._wallUvs.ToArray();
        mesh.normals = info._wallNormals.ToArray();
        mesh.triangles = info._wallTriangles.ToArray();

        mesh.RecalculateBounds();

        info._building.GetComponent<MeshFilter>().sharedMesh = mesh;
        info._building.GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    static void addBlock(BuildingInfo info, int x0, int y0, Color32[] table, Color32 c, bool isWall, Vector3 offset)
    {
        float blockHeight = _obstacleHeight;
        if (isWall)
        {
            blockHeight = _wallHeight;
        }

        // quad each side
        if (x0 == 0 || table[(x0 - 1) * info._texture.height + y0].g != c.g)
        {
            addQuad(info, new Vector3(_blockSize * x0, 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, 0.0f, _blockSize), new Vector3(0.0f, blockHeight, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f), isWall);
        }

        if (x0 == info._texture.width - 1 || table[(x0 + 1) * info._texture.width + y0].g != c.g)
        {
            addQuad(info, new Vector3(_blockSize * (x0 + 1), 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, blockHeight, 0.0f), new Vector3(0.0f, 0.0f, _blockSize), new Vector3(1.0f, 0.0f, 0.0f), isWall);
        }

        if (y0 == 0 || table[x0 * info._texture.height + y0 - 1].g != c.g)
        {
            addQuad(info, new Vector3(_blockSize * x0, 0.0f, _blockSize * y0) + offset, new Vector3(0.0f, blockHeight, 0.0f), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f), isWall);
        }

        if (y0 == info._texture.height - 1 || table[x0 * info._texture.height + y0 + 1].g != c.g)
        {
            addQuad(info, new Vector3(_blockSize * x0, 0.0f, _blockSize * (y0 + 1)) + offset, new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, blockHeight, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), isWall);
        }

        // on top
        addQuad(info, new Vector3(_blockSize * x0, blockHeight, _blockSize * y0) + offset, new Vector3(0.0f, 0.0f, _blockSize), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), isWall);
    }

    static void addQuad(BuildingInfo info, Vector3 point1, Vector3 vector1, Vector3 vector2, Vector3 normal, bool isWall)
    {
        List<Vector3> vertices = null;
        List<Vector3> normals = null;
        List<Vector2> uvs = null;
        List<int> triangles = null;

        if (isWall)
        {
            vertices = info._wallVertices;
            normals = info._wallNormals;
            uvs = info._wallUvs;
            triangles = info._wallTriangles;
        }
        else
        {
            vertices = info._obstacleVertices;
            normals = info._obstacleNormals;
            uvs = info._obstacleUvs;
            triangles = info._obstacleTriangles;
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


    static void buildObstacle(BuildingInfo info, int x0, int y0, Color32[] table, Color32 c)
    {
        // Instantiate obstacle
        GameObject obstacle = Instantiate(Resources.Load<GameObject>("Obstacle"), Vector3.zero, Quaternion.identity) as GameObject;
        obstacle.transform.SetParent(info._building.transform);

        // Obstacle mesh
        Mesh mesh = new Mesh();

        info._obstacleVertices.Clear();
        info._obstacleNormals.Clear();
        info._obstacleUvs.Clear();
        info._obstacleTriangles.Clear();

        // Find the height and the width of the obstacle
        int xMax = x0, yMax = y0;
        for (xMax = x0; xMax < info._texture.width;)
        {
            xMax++;
            if (table[xMax * info._texture.height + y0].g != c.g)
            {
                break;
            }
        }
        for (yMax = y0; yMax < info._texture.height;)
        {
            yMax++;
            if (table[x0 * info._texture.height + yMax].g != c.g)
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
                addBlock(info, x1, y1, table, c, false, -obstacle.transform.position);
            }
        }
        // clear obstacle pixels
        for (int x1 = x0; x1 < xMax; x1++)
        {
            for (int y1 = y0; y1 < yMax; y1++)
            {
                // clear obstacle pixels
                table[x1 * info._texture.height + y1] = new Color32(255, 255, 255, 255);
            }
        }
        mesh.vertices = info._obstacleVertices.ToArray();
        mesh.uv = info._obstacleUvs.ToArray();
        mesh.normals = info._obstacleNormals.ToArray();
        mesh.triangles = info._obstacleTriangles.ToArray();
        mesh.RecalculateBounds();

        obstacle.GetComponent<MeshFilter>().sharedMesh = mesh;
        obstacle.GetComponent<MeshCollider>().sharedMesh = mesh;

        _obstacleManager.obstacles.Add(obstacle.GetComponent<Obstacle>());
    }
}

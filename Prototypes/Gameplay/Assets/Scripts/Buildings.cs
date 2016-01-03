using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Buildings : MonoBehaviour
{

    public Texture2D _texture;

    float _blockSize = 0.75f;
    float _wallHeight = 5.0f;
    List<Vector3> _vertices = new List<Vector3>();
    List<Vector3> _normals = new List<Vector3>();
    List<Vector2> _uvs = new List<Vector2>();
    List<int> _triangles = new List<int>();


    // Use this for initialization
    void Start()
    {
        Mesh mesh = new Mesh();

        Color32[] table = _texture.GetPixels32();
        for (int x0 = 0; x0 < _texture.width; x0++)
        {
            for (int y0 = 0; y0 < _texture.height; y0++)
            {
                Color32 c = table[x0 * _texture.height + y0];

                if (c.g == 255)
                {
                    continue;
                }

                // Quads sur les cotés
                if (x0 == 0 || table[(x0 - 1) * _texture.height + y0].g == 255)
                {
                    addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * y0), new Vector3(0.0f, 0.0f, _blockSize), new Vector3(0.0f, _wallHeight, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f));
                }

                if (x0 == _texture.width - 1 || table[(x0 + 1) * _texture.width + y0].g == 255)
                {
                    addQuad(new Vector3(_blockSize * (x0 + 1), 0.0f, _blockSize * y0), new Vector3(0.0f, _wallHeight, 0.0f), new Vector3(0.0f, 0.0f, _blockSize), new Vector3(1.0f, 0.0f, 0.0f));
                }

                if (y0 == 0 || table[x0 * _texture.height + y0 - 1].g == 255)
                {
                    addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * y0), new Vector3(0.0f, _wallHeight, 0.0f), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f));
                }

                if (y0 == _texture.height - 1 || table[x0 * _texture.height + y0 + 1].g == 255)
                {
                    addQuad(new Vector3(_blockSize * x0, 0.0f, _blockSize * (y0 + 1)), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, _wallHeight, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                }

                //quad en haut
                addQuad(new Vector3(_blockSize * x0, _wallHeight, _blockSize * y0), new Vector3(0.0f, 0.0f, _blockSize), new Vector3(_blockSize, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            }
        }

        mesh.vertices = _vertices.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.normals = _normals.ToArray();
        mesh.triangles = _triangles.ToArray();

        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    void addQuad(Vector3 fPoint1, Vector3 fVector1, Vector3 fVector2, Vector3 fNormal)
    {
        _triangles.Add(_vertices.Count);
        _triangles.Add(_vertices.Count + 1);
        _triangles.Add(_vertices.Count + 2);
        _triangles.Add(_vertices.Count + 2);
        _triangles.Add(_vertices.Count + 3);
        _triangles.Add(_vertices.Count);

        _vertices.Add(fPoint1);
        _normals.Add(fNormal);
        _uvs.Add(Vector2.zero);

        _vertices.Add(fPoint1 + fVector1);
        _normals.Add(fNormal);
        _uvs.Add(new Vector2(1.0f, 0.0f));

        _vertices.Add(fPoint1 + fVector1 + fVector2);
        _normals.Add(fNormal);
        _uvs.Add(new Vector2(1.0f, 1.0f));

        _vertices.Add(fPoint1 + fVector2);
        _normals.Add(fNormal);
        _uvs.Add(new Vector2(0.0f, 1.0f));
    }


    // Update is called once per frame
    void Update()
    {

    }
}
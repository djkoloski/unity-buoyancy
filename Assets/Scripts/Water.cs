using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
	public float angle;
	public float amplitude;
	public float period;
	public float phase;
	public float speed;
}

public class Water : MonoBehaviour
{
	[SerializeField]
	private List<Wave> _waves;
	[SerializeField]
	private Vector2 _size;
	[SerializeField]
	private int _subdivisions;

	private Transform _transform;
	private MeshFilter _meshFilter;
	private Mesh _mesh;

	public void Awake()
	{
		_transform = GetComponent<Transform>();
		_meshFilter = GetComponent<MeshFilter>();

		_mesh = new Mesh();
		_meshFilter.sharedMesh = _mesh;
	}
	public void Update()
	{
		Vector3[] vertices = new Vector3[(_subdivisions + 1) * (_subdivisions + 1)];
		Vector2[] uvs = new Vector2[vertices.Length];

		for (int y = 0, i = 0; y <= _subdivisions; ++y)
		{
			for (int x = 0; x <= _subdivisions; ++x, ++i)
			{
				Vector2 position = Vector2.Scale(new Vector2((float)x / _subdivisions, (float)y / _subdivisions), _size);
				vertices[i] = new Vector3(position.x, GetHeightAt(position), position.y);
				uvs[i] = position;
			}
		}

		int[] tris = new int[6 * _subdivisions * _subdivisions];
		for (int y = 0, i = 0; y < _subdivisions; ++y)
		{
			for (int x = 0; x < _subdivisions; ++x, ++i)
			{
				int ll = y * (_subdivisions + 1) + x;
				tris[6 * i] = ll;
				tris[6 * i + 1] = ll + _subdivisions + 1;
				tris[6 * i + 2] = ll + _subdivisions + 2;
				tris[6 * i + 3] = ll + _subdivisions + 2;
				tris[6 * i + 4] = ll + 1;
				tris[6 * i + 5] = ll;
			}
		}

		_mesh.vertices = vertices;
		_mesh.uv = uvs;
		_mesh.triangles = tris;
		_mesh.RecalculateNormals();
	}
	public float GetHeightAt(Vector2 position)
	{
		float height = _transform.position.y;
		foreach (Wave wave in _waves)
		{
			Vector2 direction = new Vector2(Mathf.Cos(wave.angle), Mathf.Sin(wave.angle));
			height += wave.amplitude * Mathf.Sin(wave.period * Vector2.Dot(position, direction) + wave.speed * Time.realtimeSinceStartup + wave.phase);
		}
		return height;
	}
}
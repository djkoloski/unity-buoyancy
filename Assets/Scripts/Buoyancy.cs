using UnityEngine;

public class Buoyancy : MonoBehaviour
{
	[SerializeField]
	private Water _water;

	private Transform _transform;
	private MeshFilter _meshFilter;
	private Rigidbody _rigidbody;

	public void Awake()
	{
		_transform = GetComponent<Transform>();
		_meshFilter = GetComponent<MeshFilter>();
		_rigidbody = GetComponent<Rigidbody>();
	}
	public void Update()
	{
		Vector3[] verts = _meshFilter.sharedMesh.vertices;
		int[] tris = _meshFilter.sharedMesh.triangles;

		for (int i = 0; i < tris.Length; i += 3)
		{
			int ai = tris[i];
			int bi = tris[i + 1];
			int ci = tris[i + 2];

			Vector3 a = _transform.TransformPoint(verts[ai]);
			Vector3 b = _transform.TransformPoint(verts[bi]);
			Vector3 c = _transform.TransformPoint(verts[ci]);

			Vector3 centroid = (a + b + c) / 3.0f;
			Vector3 n = Vector3.Cross(b - a, c - a);
			Vector3 normal = n.normalized;
			float area = n.magnitude / 2.0f;

			Vector3 f = -normal * area * Pressure(centroid.y - _water.GetHeightAt(new Vector2(centroid.x, centroid.z)));

			_rigidbody.AddForceAtPosition(f, centroid);
		}
	}

	private float Pressure(float y)
	{
		const float kAtmosphericPressure = 101.3250f;
		const float kWaterDensity = 1.0f;
		const float kGravitationalAcceleration = -9.8f;

		if (y >= 0.0f)
		{
			return kAtmosphericPressure;
		}
		else
		{
			return kAtmosphericPressure + kWaterDensity * kGravitationalAcceleration * y;
		}
	}
}
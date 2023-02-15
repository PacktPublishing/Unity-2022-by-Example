using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Unity.Splines.Examples
{
	// Example showing how to extend Spline class through inheritance.
	[Serializable]
	public class RollercoasterTrack : Spline
	{
		const string k_GeneratedMeshDirectory = "Assets/Generated/Rollercoaster";

		[SerializeField, Range(.1f, 10f)]
		float m_TrackWidth = 1f;

		[SerializeField]
		int m_TracksPerMeter = 2;

		[SerializeField]
		Mesh m_Mesh;

		public Mesh mesh
        {
            get
            {
                if (m_Mesh != null)
                    return m_Mesh;

                m_Mesh = new Mesh();

				// In the editor it's usually convenient to save the mesh as an asset rather than generate it on the
				// fly any time the scene is loaded.
#if UNITY_EDITOR
                Directory.CreateDirectory(k_GeneratedMeshDirectory);
                var path = AssetDatabase.GenerateUniqueAssetPath($"{k_GeneratedMeshDirectory}/Track.asset");
                AssetDatabase.CreateAsset(m_Mesh, path);
                AssetDatabase.ImportAsset(path);
#endif
                return m_Mesh;
            }
        }

		// When working in the editor, instead of rebuilding the mesh any time the spline changes (which can be often
		// especially when moving knots), we'll schedule updates once per-frame.
#if UNITY_EDITOR
		[NonSerialized]
		bool m_RebuildScheduled;
#endif

		protected override void OnSplineChanged()
		{
			base.OnSplineChanged();

#if UNITY_EDITOR
			if (m_RebuildScheduled)
				return;
			m_RebuildScheduled = true;
			EditorApplication.delayCall += () =>
			{
				RebuildTracks();
				m_RebuildScheduled = false;
			};
#else
			RebuildTracks();
#endif
		}

		// Generate a set of ties along the spline path.
		public void RebuildTracks()
		{
			using var spline = new NativeSpline(this);
			var len = spline.GetLength();
			var tieCount = (int) (len * m_TracksPerMeter);

			mesh.Clear();

			Vector3[] positions = new Vector3[tieCount * 4];
			Vector3[] normals = new Vector3[tieCount * 4];
			int[] indices = new int[tieCount * 6];

			for (int i = 0; i < tieCount; i++)
			{
				float t = (float) i / (tieCount - 1);

				var position = SplineUtility.EvaluatePosition(spline, t);
				var forward = SplineUtility.EvaluateTangent(spline, t);
				var tangent = (quaternion) Quaternion.LookRotation(forward);

				int a = i * 4 + 0,
					b = i * 4 + 1,
					c = i * 4 + 2,
					d = i * 4 + 3;

				positions[a] = position + math.mul(tangent, new float3(-m_TrackWidth * .5f, 0f, -0.25f));
				positions[b] = position + math.mul(tangent, new float3( m_TrackWidth * .5f, 0f, -0.25f));
				positions[c] = position + math.mul(tangent, new float3(-m_TrackWidth * .5f, 0f,  0.25f));
				positions[d] = position + math.mul(tangent, new float3( m_TrackWidth * .5f, 0f,  0.25f));

				normals[a] = Vector3.up;
				normals[b] = Vector3.up;
				normals[c] = Vector3.up;
				normals[d] = Vector3.up;

				indices[i*6+0] = c;
				indices[i*6+1] = b;
				indices[i*6+2] = a;

				indices[i*6+3] = b;
				indices[i*6+4] = c;
				indices[i*6+5] = d;
			}

			mesh.vertices = positions;
			mesh.normals = normals;
			mesh.subMeshCount = 1;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}
	}
}
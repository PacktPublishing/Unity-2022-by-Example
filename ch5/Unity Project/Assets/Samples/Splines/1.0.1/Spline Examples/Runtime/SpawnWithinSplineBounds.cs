using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Splines;
#endif
using Interpolators = UnityEngine.Splines.Interpolators;
using Random = UnityEngine.Random;

namespace Unity.Splines.Examples
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class SpawnWithinSplineBounds : MonoBehaviour
    {
        struct SpawnPoint
        {
            public float3 pos;
            public float3 right;
            public float3 up;
        }

        [SerializeField] 
        SplineContainer m_SplineContainer;
        public SplineContainer splineContainer => m_SplineContainer;
        
        [SerializeField] 
        Transform m_SpawnContainer;
        [SerializeField]
        int m_MaxIterations;

        [Header("Spawning")]
        [SerializeField]
        List<GameObject> m_Prefabs;
        [SerializeField] 
        float m_SpawnSpacing;
        [SerializeField] 
        [Range(0, 1)]
        float m_SpawnChance;
        [SerializeField]
        List<GameObject> m_BorderPrefabs;
        [SerializeField] 
        float m_BorderSpawnSpacing;
        [SerializeField]
        [Range(0, 1)]
        float m_BorderSpawnChance;
        [SerializeField]
        SplineData<float> m_SpawnBorderData;
        public SplineData<float> spawnBorderData => m_SpawnBorderData;
        
        [Header("Randomization")]
        [SerializeField] 
        int m_RandomSeed;
        [SerializeField] 
        Vector2 m_RotationRandomRange;
        
        int m_Iterations;
        List<Vector2> m_SplineSegments = new List<Vector2>();
        List<Vector2> m_ParentPointSegments = new List<Vector2>();
        
        const int k_SegmentsPerCurve = 10;
        
        void OnEnable()
        {
#if UNITY_EDITOR
            EditorSplineUtility.afterSplineWasModified += OnSplineModified;
            m_SpawnBorderData.changed += OnSpawnBorderDataChanged;
#endif
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            EditorSplineUtility.afterSplineWasModified -= OnSplineModified;
            m_SpawnBorderData.changed -= OnSpawnBorderDataChanged;
#endif
        }

        void OnValidate()
        { 
#if UNITY_EDITOR
            if (m_SplineContainer != null && !EditorApplication.isPlayingOrWillChangePlaymode) 
                EditorApplication.delayCall += () => OnSplineModified(m_SplineContainer.Spline);
#endif
        }
        
        void CleanUp()
        {
            if (m_SpawnContainer == null)
                return;
            
            for (int i = m_SpawnContainer.childCount - 1; i >= 0; --i)
            {
                var child = m_SpawnContainer.GetChild(i);
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }

        void OnSplineModified(Spline spline)
        {
            if (spline == m_SplineContainer.Spline && m_SpawnContainer != null)
            {
                CleanUp();
                BuildSplineSegments();
                Random.InitState(m_RandomSeed);

                if (m_Prefabs.Count > 0)
                    SpawnObjectsWithinSpline(m_SplineContainer, m_SpawnChance, m_SpawnSpacing, false);

                if (m_BorderPrefabs.Count > 0)
                    SpawnObjectsWithinSpline(m_SplineContainer, m_BorderSpawnChance, m_BorderSpawnSpacing, true);
            }
        }

        void SpawnObjectsWithinSpline(SplineContainer splineContainer, float spawnChance, float spawnSpacing, bool spawnOnBorder)
        {
            var spline = splineContainer.Spline;
            var splineLen = spline.GetLength();
            var points = new List<SpawnPoint>();
            var splineTime = 0f;
            var spawnCount = Mathf.CeilToInt(splineLen / spawnSpacing);
            var splineXform = splineContainer.transform;
            for (int i = 0; i < spawnCount; ++i)
            {
                // Here we do not need to manually transform the output vectors as all SplineContainer's evaluation methods transform the results to world space.
                splineContainer.Evaluate(splineTime, out var _, out var dir, out var up);
                
                // Spline's evaluation methods return results in spline space therfore manual transforming to world space is required.
                var pos = splineXform.TransformPoint(spline.GetPointAtLinearDistance(splineTime, spawnSpacing, out splineTime));
                var right = splineXform.TransformDirection(Vector3.ProjectOnPlane(math.cross(math.normalize(dir), up), up));
                var spawnBorder = m_SpawnBorderData.Evaluate(spline, splineTime, PathIndexUnit.Normalized, new Interpolators.LerpFloat());

                if (spawnBorder <= spawnSpacing * 0.5f)
                {
                    if (!spawnOnBorder)
                        SpawnRandomPrefab(m_Prefabs, pos, -right, up, spawnChance);
                }
                else if (spawnOnBorder)
                    SpawnRandomPrefab(m_BorderPrefabs, pos, -right, up, spawnChance);

                points.Add(new SpawnPoint() { pos = pos, right = right, up = up });
            }

            m_Iterations = 1;
            SpawnObjectsForPoints(points, spawnChance, spawnSpacing, spawnOnBorder);
        }

        void SpawnObjectsForPoints(List<SpawnPoint> points, float spawnChance, float spawnSpacing, bool spawnOnBorder)
        {
            if (m_Iterations == m_MaxIterations)
                return;

            // Backup parent points
            var parentPoints = new List<SpawnPoint>(points);
            
            // Offset all child points along right vector
            for (int i = 0; i < points.Count; i++)
            {
                var splinePoint = points[i];
                var nextIdx = (i == points.Count - 1 ? 0 : i + 1);
                var nextPoint = points[nextIdx];
                var right = (float3) Vector3.Slerp(splinePoint.right, nextPoint.right, 0.5f);
                var up = (float3) Vector3.Slerp(splinePoint.up, nextPoint.up, 0.5f);
                var pos = math.lerp(splinePoint.pos, nextPoint.pos, 0.5f);

                splinePoint.pos = pos + right * spawnSpacing;
                splinePoint.right = right;
                splinePoint.up = up;
                
                points[i] = splinePoint;
            }

            var pointsToRemove = new SortedSet<int>();
            var spawnedPoints = new List<SpawnPoint>();

            // Check if point should be discard - spawn otherwise
            for (int i = 0; i < points.Count; ++i)
            {
                var discardPoint = false;
                var pointWithinBorder = false;
                var pointSplineSpace = m_SplineContainer.transform.InverseTransformPoint(points[i].pos);
                
                // Check against border
                var dist = SplineUtility.GetNearestPoint(m_SplineContainer.Spline, pointSplineSpace, out var _, out var splineTime);
                var spawnOffset = m_SpawnBorderData.Evaluate(m_SplineContainer.Spline, splineTime, PathIndexUnit.Normalized, new Interpolators.LerpFloat());
                if (dist < spawnOffset)
                    pointWithinBorder = true;

                // Check against child points
                for (int spawnedPointIdx = spawnedPoints.Count - 1; spawnedPointIdx >= 0; --spawnedPointIdx)
                {
                    if (math.length(points[i].pos - spawnedPoints[spawnedPointIdx].pos) < spawnSpacing)
                    {
                        discardPoint = true;
                        pointsToRemove.Add(i);

                        break;
                    }
                }
                
                // Check against parent points
                if (!discardPoint)
                {
                    var nextIdx = i == points.Count - 1 ? 0 : i + 1;
                    while (i != nextIdx)
                    {
                        if (math.length(points[i].pos - parentPoints[nextIdx].pos) < spawnSpacing * 0.9f)
                        {
                            discardPoint = true;
                            pointsToRemove.Add(i);
                            
                            break;
                        }

                        nextIdx = nextIdx == points.Count - 1 ? 0 : nextIdx + 1;
                    }
                }

                // Ensure point is within parent points bounds
                if (!discardPoint)
                {
                    m_ParentPointSegments.Clear();
                    foreach (var parentPoint in parentPoints)
                        m_ParentPointSegments.Add(new Vector2(parentPoint.pos.x, parentPoint.pos.z));

                    discardPoint = !PointInsidePolygon(new Vector2(points[i].pos.x, points[i].pos.z), m_ParentPointSegments);
                }
                
                // Ensure point is roughly within spline bounds
                if (!discardPoint)
                    discardPoint = !PointInsidePolygon(new Vector2(pointSplineSpace.x, pointSplineSpace.z), m_SplineSegments);

                if (!discardPoint)
                {
                    if (!pointWithinBorder)
                    {
                        if (!spawnOnBorder)
                            SpawnRandomPrefab(m_Prefabs, points[i].pos, -points[i].right, points[i].up, spawnChance);
                    }
                    else if (spawnOnBorder)
                        SpawnRandomPrefab(m_BorderPrefabs, points[i].pos, -points[i].right, points[i].up, spawnChance);
                    spawnedPoints.Add(points[i]);
                }
            }

            m_Iterations++;
            foreach (var point in pointsToRemove.Reverse())
                points.RemoveAt(point);

            if (points.Count == 0)
                return;

            SpawnObjectsForPoints(points, spawnChance, spawnSpacing, spawnOnBorder);
        }

        void SpawnRandomPrefab(List<GameObject> prefabs, Vector3 position, Vector3 forward, Vector3 up, float spawnChance)
        {
            if (Random.Range(0f, 1f) > spawnChance)
                return;
            
            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            var go = Instantiate(prefab, position, quaternion.identity);
            go.transform.rotation = Quaternion.LookRotation(forward, up) * Quaternion.AngleAxis(Random.Range(m_RotationRandomRange.x, m_RotationRandomRange.y), Vector3.up);
            go.transform.SetParent(m_SpawnContainer, true);
        }

        void OnSpawnBorderDataChanged()
        {
            OnSplineModified(m_SplineContainer.Spline);
        }

        void BuildSplineSegments()
        {
            m_SplineSegments.Clear();

            var spline = m_SplineContainer.Spline;
            var curveCount = spline.Closed ? spline.Count : spline.Count - 1;
            var stepSize = 1f / k_SegmentsPerCurve;

            for (int curveIndex = 0; curveIndex < curveCount; ++curveIndex)
            {
                for (int step = 0; step < k_SegmentsPerCurve; ++step)
                {
                    var splineTime = spline.CurveToSplineT(curveIndex + step * stepSize);
                    var pos = spline.EvaluatePosition(splineTime);

                    m_SplineSegments.Add(new Vector2(pos.x, pos.z));
                }
            }
        }
        
        bool PointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            Vector2 p1, p2;
            p1 = polygon[0];
            var counter = 0;
            for (int i = 1; i <= polygon.Count; i++)
            { 
                p2 = polygon[i % polygon.Count];
                if (point.y > Mathf.Min(p1.y, p2.y))
                {
                    if (point.y <= Mathf.Max(p1.y, p2.y))
                    {
                        if (point.x <= Mathf.Max(p1.x, p2.x))
                        {
                            if (p1.y != p2.y)
                            {
                                var xinters = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0)
                return false;
            else
                return true;
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Rollercoaster : MonoBehaviour, ISplineProvider
{
	[SerializeField]
	RollercoasterTrack m_Track;

	[SerializeField]
	Transform m_Cart;

	[SerializeField]
	float m_Speed = .314f;
	
	public IEnumerable<Spline> Splines => new[] { m_Track };

	void Update()
	{
		var trs = transform;
		var t = math.frac(Time.time * m_Speed);
		m_Cart.position = trs.TransformPoint(m_Track.EvaluatePosition(t));
		m_Cart.rotation = Quaternion.LookRotation(trs.TransformDirection(m_Track.EvaluateTangent(t)));
	}
}

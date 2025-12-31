using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class XRay_Movement : MonoBehaviour
{
    public List<Transform> m_Points;

    public float m_Velocity = 10f;
    public float m_Distance = 0.1f;

    private int m_Index = 0;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Transform trajectory = m_Points[m_Index];

        // Move to the current point
        transform.position = Vector3.MoveTowards(transform.position, trajectory.position, m_Velocity * Time.deltaTime);

        // Check if it arrived the current point
        if (Vector3.Distance(transform.position, trajectory.position) <= m_Distance)
        {
            m_Index++;
            // If reached the last point, start again
            if (m_Index >= m_Points.Count) m_Index = 0;
        }
    }
}

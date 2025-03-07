using Oculus.Interaction;
using System;
using UnityEngine;

public class  ClapEventArgs
{
    public Vector3 clapPos;

    public Vector3 velocityL;
    public Vector3 velocityR;
}


public class ClapRecognizer : MonoBehaviour
{
    [SerializeField] private HandVisual m_HandVisual_L;
    [SerializeField] private HandVisual m_HandVisual_R;

    private VelocityTracker velocityTracker_L;
    private VelocityTracker velocityTracker_R;

    [SerializeField] private float m_ClapDistThreshold = 0.05f;
    [SerializeField] private float m_ClapSpeedThreshold = 0.3f;
    [SerializeField] private float m_ClapDotThreshold = -0.5f;

    private bool hasClapped = false;

    public event Action<ClapEventArgs> OnClap;
    
    void Start()
    {
        velocityTracker_L = gameObject.AddComponent<VelocityTracker>();
        var palmL = m_HandVisual_L.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm);
        velocityTracker_L.Init(palmL);

        velocityTracker_R = gameObject.AddComponent<VelocityTracker>();
        var palmR = m_HandVisual_R.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm);
        velocityTracker_R.Init(palmR);   
    }

    private void Update()
    {
        CheckClap();
    }

    private void CheckClap()
    {
        CheckResetClap();

        if (!hasClapped && IsClap())
        {
            hasClapped = true;
            //invoke clap event!

            ClapEventArgs args = new ClapEventArgs();
            args.clapPos = GetClapPosition();
            args.velocityL = velocityTracker_L.GetAverageVelocity();
            args.velocityR = velocityTracker_R.GetAverageVelocity();
            OnClap?.Invoke(args);
        }
    }

    private void CheckResetClap()
    {
        if(hasClapped)
        {
            float dist = GetDistanceBtwPalms();
            if(dist >= m_ClapDistThreshold * 3f)
            {
                hasClapped = false;
            }
        }
    }

    private bool IsClap()
    {
        //distance check
        float dist = GetDistanceBtwPalms();
        if (dist > m_ClapDistThreshold) return false;

        //velocity check
        Vector3 velocityL =  velocityTracker_L.GetAverageVelocity();
        Vector3 velocityR = velocityTracker_R.GetAverageVelocity();

        if (velocityL.magnitude < m_ClapSpeedThreshold || velocityR.magnitude < m_ClapSpeedThreshold) return false;

        //dot check
        if(Vector3.Dot(velocityL.normalized, velocityR.normalized) > m_ClapDotThreshold) return false;

        return true;
    }


    private float GetDistanceBtwPalms()
    {
        return Vector3.Distance(m_HandVisual_L.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm).position, m_HandVisual_R.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm).position);
    }

    public Vector3 GetClapPosition()
    {
        return Vector3.Lerp(m_HandVisual_L.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm).position, m_HandVisual_R.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandPalm).position, 0.5f);
    }


}

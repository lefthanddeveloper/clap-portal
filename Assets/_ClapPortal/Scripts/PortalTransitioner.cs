using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PortalTransitioner : MonoBehaviour
{
    private ClapRecognizer clapRecognizer;

    [SerializeField] private Transform m_Mask1Sphere;
    [SerializeField] private Transform m_Mask0Sphere;
    [SerializeField] private float m_TransitionDuration = 3f;
    private Coroutine transitionCor = null;

    private bool isVR = false;
    void Start()
    {
        clapRecognizer = GetComponent<ClapRecognizer>();
        clapRecognizer.OnClap += OnClap;

        m_Mask0Sphere.localScale = Vector3.zero;
        m_Mask1Sphere.localScale = Vector3.zero;
    }

    private void OnClap(ClapEventArgs args)
    {
        Debug.Log($"clapPos: {args.clapPos}");
        Debug.Log($"speed L:  {args.velocityL.magnitude}");
        Debug.Log($"speed R: {args.velocityR.magnitude}");

        if (transitionCor != null) return;

        if(!isVR)
        {
            m_Mask0Sphere.localScale = Vector3.zero;

            //place transition object
            m_Mask1Sphere.position = args.clapPos; 
            transitionCor = StartCoroutine(TransitionCor(m_Mask1Sphere, 0f, 3f, m_TransitionDuration, () =>
            {
                isVR = true;
            }));
        }
        else //vr
        {
            m_Mask0Sphere.position = args.clapPos;

            transitionCor = StartCoroutine(TransitionCor(m_Mask0Sphere, 0f, 2.5f, m_TransitionDuration, () =>
            {
                m_Mask1Sphere.localScale = Vector3.zero;
                m_Mask0Sphere.localScale = Vector3.zero;
                isVR = false;
            }));
        }
    }

    private IEnumerator TransitionCor(Transform targetTr, float startScale, float targetScale, float duration, Action done = null)
    {
        float timePassed = 0f;
        float ratio = 0f;
        while(ratio < 1f)
        {
            timePassed += Time.deltaTime;
            ratio = timePassed/ duration;

            float scale = Mathf.Lerp(startScale, targetScale, ratio);
            targetTr.localScale = Vector3.one * scale;  
            yield return null;
        }
        targetTr.localScale = Vector3.one * targetScale;
        done?.Invoke();
        transitionCor = null;
    }


    
}

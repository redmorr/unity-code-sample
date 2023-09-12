using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Caster : MonoBehaviour
{
    public MeshRenderer mr1;
    public MeshRenderer mr2;
    public Color success1;
    public Color success2;
    public Color error1;
    public Color error2;
    public Transform ghost1;
    public Transform ghost2;

    private void OnEnable()
    {
        mr1 = ghost1.GetComponent<MeshRenderer>();
        mr2 = ghost2.GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        Vector3 firstCastOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 firstCapsuleUp = firstCastOrigin + Vector3.up * 0.5f;
        Vector3 firstCapsuleDown = firstCastOrigin - Vector3.up * 0.5f;

        Vector3 secondCastOrigin;
        Vector3 secondCapsuleUp;
        Vector3 secondCapsuleDown;
        
        RaycastHit hit;
        
        if (Physics.CapsuleCast(firstCapsuleUp, firstCapsuleDown, 0.5f, transform.forward, out  hit, 1f))
        {
            mr2.enabled = false;
            mr1.sharedMaterial.color = success1;
            secondCastOrigin = firstCastOrigin + transform.forward * hit.distance;
        }
        else
        {
            mr2.enabled = true;
            mr1.sharedMaterial.color = error1;
            secondCastOrigin = firstCastOrigin + transform.forward;
        }

        ghost1.position = secondCastOrigin;
        secondCapsuleUp = secondCastOrigin + Vector3.up * 0.5f;
        secondCapsuleDown = secondCastOrigin - Vector3.up * 0.5f;
        
        if (Physics.CapsuleCast(secondCapsuleUp, secondCapsuleDown, 0.5f, Vector3.down, out hit, 2f))
        {
            mr2.sharedMaterial.color = success2;
            ghost2.position = secondCastOrigin + Vector3.down * hit.distance;
        }
        else
        {
            mr2.sharedMaterial.color = error2;
            ghost2.position = secondCastOrigin + Vector3.down * 2f;
        }
    }

    private void OnDrawGizmos()
    {
        // Vector3 pos1 = transform.position + Vector3.up * 0.5f;
        // Vector3 pos2 = transform.position - Vector3.up * 0.5f;
        // Gizmos.DrawSphere(pos1, 0.5f);
        // Gizmos.DrawSphere(pos2, 0.5f);
    }
}
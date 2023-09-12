using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private IDamagable damagableImplementation;
    //TODO: implement PlayerHealth
    
    public void DealDamage(float damagePoints, Vector3 knockbackForce)
    {
        Debug.Log(damagePoints.ToString());
    }
}
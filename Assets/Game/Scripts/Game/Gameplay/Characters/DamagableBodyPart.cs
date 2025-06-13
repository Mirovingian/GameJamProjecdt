using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableBodyPart : MonoBehaviour, IEntity
{
    public MonoBehaviour controller;
    private IEntityController Controller => controller as IEntityController;
    public void TakeDamage(int amount)
    {
        if (Controller != null)
            Controller.ChangeHealth(amount);
        else
            Debug.LogError("Controller can't serialize IEntityController!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// Здоровье юнита
public class UnitHP : MonoBehaviour
{
    [SerializeField]
    private float m_CurrentHealth;                       // текущее количество здоровья  
    
    public static event EventHandler IAmDead;            // сообщу всем, чтоб меня в качестве цели больше не использовали        
    
    public void SetStartHealth(float startHealth)        // Установить начальное здоровье
    {
        m_CurrentHealth = startHealth;                                  
    }

    public bool TakeDamage(float amount)                // Принять удар
    {                                                   // возвращает true, если после текущего удара герой умер
        m_CurrentHealth -= amount;                          

        if (m_CurrentHealth <= 0f) 
        {
            Debug.Log(this.name + ": I'm dead");
            IAmDead?.Invoke(this.gameObject, EventArgs.Empty);       
            return true;
        }
        else return false;
    }
}


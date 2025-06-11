using UnityEngine;

// ESTE SCRIPT NO VA ANEXADO A NINGUN GAMEOBJECT
// SE UTILIZA PARA DEFINIR UNA INTERFAZ QUE DEBE SER IMPLEMENTADA POR LOS OBJETOS QUE HACEN DAÑO

/// <summary>
/// Interface for components that deal damage.
/// </summary>
public interface IDamageDealer
{
    /// <summary>
    /// Amount of damage this object inflicts.
    /// </summary>
    float Damage { get; }
}


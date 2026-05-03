using UnityEngine;
using UnityEngine.UI;

public abstract class PowerUpBase : MonoBehaviour
{

    public bool IsUsed;

    public System.Action OnUse { get; set; }

    [Header("UI")]

    public Texture iconTexture;

    public abstract void ActivatePowerUp(GameObject player);
}

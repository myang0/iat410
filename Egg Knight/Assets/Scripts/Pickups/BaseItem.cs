using System;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public string DisplayName;
    public string Description;

    public Item InventoryKey;

    public static event EventHandler<InventoryAddEventArgs> OnInventoryAdd;
    public static event EventHandler<ItemTextEventArgs> OnItemTextDisplay;

    public event EventHandler OnPickup;

    private float _origX;
    private float _origY;

    protected virtual void Awake() {
        _origX = transform.position.x;
        _origY = transform.position.y;
    }

    protected virtual void Update() {
        transform.position = new Vector3(
            transform.position.x,
            _origY + (Mathf.Sin(Time.time) * 0.15f),
            transform.position.z
        );
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PickUp();
        }
    }

    protected virtual void PickUp() {
        OnInventoryAdd?.Invoke(this, new InventoryAddEventArgs(InventoryKey));
        OnItemTextDisplay?.Invoke(this, new ItemTextEventArgs(DisplayName, Description));
        OnPickup?.Invoke(this, EventArgs.Empty);
        
        Destroy(gameObject);
    }
}

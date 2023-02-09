using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Framework/Inventory/Item", order = 0)]
public class ItemData : ScriptableObject {
    public ItemType itemType;
    public MotivationStatName motivationStatName;
    public int motivationStatVal;
    public bool transmutes;
    public ItemData transmutesTo;
    public bool destroyOnUse;

    public string performString;

    public int useCost;

}
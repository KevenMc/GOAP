using UnityEngine;

[CreateAssetMenu(fileName = "Station", menuName = "Framework/Stations/Station", order = 0)]
public class StationData : ScriptableObject {
    public ItemData returnItem;
    public ItemType neededItemType;
    public ItemType returnItemType;
    public MotivationStatName motivationStatName;
}
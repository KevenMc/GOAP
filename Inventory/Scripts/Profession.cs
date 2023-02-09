using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "Profession", menuName = "Framework/Profession", order = 0)]
public class Profession : ScriptableObject {
    public List<Interaction> recipes;
    public List<Profession> incompatibleProfessions;
    
}
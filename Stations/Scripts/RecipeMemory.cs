using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "RecipeMemory", menuName = "Framework/Recipes/Recipe Memory", order = 0)]
public class RecipeMemory : ScriptableObject
{
    public List<Interaction> recipes;
}
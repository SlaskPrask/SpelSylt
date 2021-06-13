using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Drop Table", menuName = "Drop Table")]
public class DropTable : ScriptableObject
{
    [HideInInspector]
    public ushort totalDropChance;
    public List<DropData> dropTable;

    public PowerUp_Scriptable CalculateDrop()
    {
        //Calculate what drop to get :)
        int result = Random.Range(0, totalDropChance);

        for (int i = 0, dropCounter = 0; i < dropTable.Count; i++)
        {
            dropCounter += dropTable[i].dropChance;
            if (result < dropCounter)
            {
                return dropTable[i].powerUp;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct DropData
{
    public PowerUp_Scriptable powerUp;
    public ushort dropChance;
}

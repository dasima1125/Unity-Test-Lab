[System.Serializable]
public class InventoryItem
{
    public int ID;
    public int Quantity;
    public InventoryItem(int id, int quantity)
    {
        ID = id;
        Quantity = quantity;
    }
}
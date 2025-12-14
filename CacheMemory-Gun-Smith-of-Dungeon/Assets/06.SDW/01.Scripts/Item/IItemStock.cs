namespace _06.SDW._01.Scripts.Item
{
    public interface IItemStock
    {
        int GetCount(string itemKey);
        bool Has(string itemKey, int amount);
        bool TryConsume(string itemKey, int amount);
        void Add(string itemKey, int amount);
    }
}
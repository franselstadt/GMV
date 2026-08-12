namespace gmvTM.Domain.Items.Interfaces
{
    public interface IBaseItem : IItem
    {
        bool IsNew
        {
            get;
        }
    }
}

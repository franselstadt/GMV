namespace gmvTM.Domain.Items.Interfaces
{
    public interface IItem
    {
        int ID
        {
            get;
            set;
        }

        string TableName
        {
            get;
        }
    }
}

using gmvTM.Domain.Items;

namespace gmvTM.Domain
{
    public static class StopPlanFactory
    {
        public static StopPlanItem CreateItem(int stopID, int sequence, int arrivalSeconds, int id = 0)
        {
            return new StopPlanItem
            {
                ID = id,
                StopID = stopID,
                Sequence = sequence,
                ArrivalSeconds = arrivalSeconds
            };
        }
    }
}

using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class RouteItem : BaseItem
    {
        private string shortName = null!;
        private string longName = null!;
        private string? color;
        private string encodedPolyline = null!;

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.Routes; }
        }

        public string ShortName
        {
            get { return this.shortName; }
            set { this.shortName = value; }
        }

        public string LongName
        {
            get { return this.longName; }
            set { this.longName = value; }
        }

        public string? Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        public string EncodedPolyline
        {
            get { return this.encodedPolyline; }
            set { this.encodedPolyline = value; }
        }
    }
}

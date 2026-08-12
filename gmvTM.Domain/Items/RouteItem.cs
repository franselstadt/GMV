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
            get { return gmvDomain.Tables.Routes; }
        }

        [TableDefinition(MaxLength = 32, IsRequired = true)]
        public string ShortName
        {
            get { return this.shortName; }
            set { this.shortName = value; }
        }

        [TableDefinition(MaxLength = 256, IsRequired = true)]
        public string LongName
        {
            get { return this.longName; }
            set { this.longName = value; }
        }

        [TableDefinition(MaxLength = 32)]
        public string? Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        [TableDefinition(IsRequired = true)]
        public string EncodedPolyline
        {
            get { return this.encodedPolyline; }
            set { this.encodedPolyline = value; }
        }
    }
}

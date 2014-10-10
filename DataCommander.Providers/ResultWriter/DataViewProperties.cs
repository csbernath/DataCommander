namespace DataCommander.Providers
{
    internal sealed class DataViewProperties
    {
        private string rowFilter;
        private string sort;

        public string RowFilter
        {
            get
            {
                return this.rowFilter;
            }
            set
            {
                this.rowFilter = value;
            }
        }

        public string Sort
        {
            get
            {
                return this.sort;
            }
            set
            {
                this.sort = value;
            }
        }
    }
}
namespace DataCommander.Providers.Odp
{

    internal sealed class OracleName
    {
        private string owner;
        private string name;

        public OracleName( string userId, string name )
        {
            if (name != null)
            {
                string[] items = name.Split( '.' );

                if (items.Length > 1)
                {
                    this.owner = items[ 0 ];
                    this.name = items[ 1 ];
                }
                else
                {
                    owner = userId;
                    this.name = name;
                }
            }
            else
            {
                owner = userId;
                this.name = name;
            }
        }

        public string Owner
        {
            get
            {
                return this.owner;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}
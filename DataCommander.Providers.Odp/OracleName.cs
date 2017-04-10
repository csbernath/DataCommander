namespace DataCommander.Providers.Odp
{

    internal sealed class OracleName
    {
        private readonly string owner;
        private readonly string name;

        public OracleName( string userId, string name )
        {
            if (name != null)
            {
                var items = name.Split( '.' );

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

        public string Owner => this.owner;

        public string Name => this.name;
    }
}
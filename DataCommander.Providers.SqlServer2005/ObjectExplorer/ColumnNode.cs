namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal enum SqlServerSystemType
    {
        Image = 34,
        Text = 35,
        UniqueIdentifier = 36,
        TinyInt = 48,
        SmallInt = 52,
        Int = 56,
        SmallDateTime = 58,
        Real = 59,
        Money = 60,
        DateTime = 61,
        Float = 62,
        SqlVariant = 98,
        NText = 99,
        Bit = 104,
        Decimal = 106,
        Numeric = 108,
        SmallMoney = 122,
        BigInt = 127,
        VarBinary = 165,
        VarChar = 167,
        Binary = 173,
        Char = 175,
        Timestamp = 189,
        NVarChar = 231,
        NChar = 239,
        Xml = 241,
        Sysname = 231,
    }

    internal sealed class ColumnNode : ITreeNode
    {
        private readonly string columnName;
        private readonly byte systemTypeId;
        private readonly short maxLength;
        private readonly byte precision;
        private readonly byte scale;
        private readonly bool isNullable;
        private readonly string userTypeName;
        private bool isPrimaryKey;
        private bool isForeignKey;

        public ColumnNode(
            int id,
            string columnName,
            byte systemTypeId,            
            short maxLength,
            byte precision,
            byte scale,
            bool isNullable,
            string userTypeName )
        {
            this.Id = id;
            this.columnName = columnName;
            this.systemTypeId = systemTypeId;
            this.maxLength = maxLength;
            this.precision = precision;
            this.scale = scale;
            this.isNullable = isNullable;
            this.userTypeName = userTypeName;
        }

        public int Id { get; }

        public bool IsPrimaryKey
        {
            set
            {
                this.isPrimaryKey = value;
            }
        }

        public bool IsForeignKey
        {
            set
            {
                this.isForeignKey = value;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                string typeName;
                var systemType = (SqlServerSystemType)(int)this.systemTypeId;
                switch (systemType)
                {
                    case SqlServerSystemType.Char:
                    case SqlServerSystemType.VarBinary:
                    case SqlServerSystemType.VarChar:
                        var maxLengthString = this.maxLength >= 0 ? this.maxLength.ToString() : "max";
                        typeName = $"{this.userTypeName}({maxLengthString})";
                        break;

                    case SqlServerSystemType.NChar:
                    case SqlServerSystemType.NVarChar:
                        maxLengthString = this.maxLength >= 0 ? (this.maxLength/2).ToString() : "max";
                        typeName = $"{this.userTypeName}({maxLengthString})";
                        break;

                    case SqlServerSystemType.Decimal:
                    case SqlServerSystemType.Numeric:
                        if (this.scale == 0)
                        {
                            typeName = $"{this.userTypeName}({this.precision})";
                        }
                        else
                        {
                            typeName = $"{this.userTypeName}({this.precision},{this.scale})";
                        }
                        break;

                    default:
                        typeName = this.userTypeName;
                        break;
                }

                return
                    $"{this.columnName} ({(this.isPrimaryKey ? "PK, " : null)}{(this.isForeignKey ? "FK, " : null)}{typeName}, {(this.isNullable ? "null" : "not null")})";
            }
        }

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return null;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}
namespace DataCommander.Providers.Tfs
{
    using System.Collections.Generic;

    internal static class TfsDataReaderFactory
    {
        public delegate TfsDataReader CreateDataReader(TfsCommand command);

        public static void Add(
            string name,
            TfsParameterCollection parameters,
            CreateDataReader createDataReader)
        {
            var info = new DataReaderInfo(name, parameters, createDataReader);
            Dictionary.Add(name, info);
        }

        public static SortedDictionary<string, DataReaderInfo> Dictionary { get; } = new SortedDictionary<string, DataReaderInfo>();

        public sealed class DataReaderInfo
        {
            private string name;

            public DataReaderInfo(
                string name,
                TfsParameterCollection parameters,
                CreateDataReader createDataReader)
            {
                this.name = name;
                Parameters = parameters;
                CreateDataReader = createDataReader;
            }

            public TfsParameterCollection Parameters { get; }

            public CreateDataReader CreateDataReader { get; }
        }
    }
}
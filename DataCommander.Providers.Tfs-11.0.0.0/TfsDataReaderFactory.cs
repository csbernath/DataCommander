namespace DataCommander.Providers.Tfs
{
    using System.Collections.Generic;

    internal static class TfsDataReaderFactory
    {
        private static readonly SortedDictionary<string, DataReaderInfo> dictionary = new SortedDictionary<string, DataReaderInfo>();
        
        public delegate TfsDataReader CreateDataReader(TfsCommand command);

        public static void Add(
            string name,
            TfsParameterCollection parameters,
            CreateDataReader createDataReader)
        {
            var info = new DataReaderInfo(name, parameters, createDataReader);
            dictionary.Add(name, info);
        }

        public static SortedDictionary<string, DataReaderInfo> Dictionary => dictionary;

        public sealed class DataReaderInfo
        {
            private string name;
            private readonly TfsParameterCollection parameters;
            private readonly CreateDataReader createDataReader;

            public DataReaderInfo(
                string name,
                TfsParameterCollection parameters,
                CreateDataReader createDataReader)
            {
                this.name = name;
                this.parameters = parameters;
                this.createDataReader = createDataReader;
            }

            public TfsParameterCollection Parameters => this.parameters;

            public CreateDataReader CreateDataReader => this.createDataReader;
        }
    }
}
namespace DataCommander.Providers.Tfs
{
    using System.Collections.Generic;

    internal static class TfsDataReaderFactory
    {
        private static SortedDictionary<string, DataReaderInfo> dictionary = new SortedDictionary<string, DataReaderInfo>();
        
        public delegate TfsDataReader CreateDataReader(TfsCommand command);

        public static void Add(
            string name,
            TfsParameterCollection parameters,
            CreateDataReader createDataReader)
        {
            DataReaderInfo info = new DataReaderInfo(name, parameters, createDataReader);
            dictionary.Add(name, info);
        }

        public static SortedDictionary<string, DataReaderInfo> Dictionary
        {
            get
            {
                return dictionary;
            }
        }

        public sealed class DataReaderInfo
        {
            private string name;
            private TfsParameterCollection parameters;
            private CreateDataReader createDataReader;

            public DataReaderInfo(
                string name,
                TfsParameterCollection parameters,
                CreateDataReader createDataReader)
            {
                this.name = name;
                this.parameters = parameters;
                this.createDataReader = createDataReader;
            }

            public TfsParameterCollection Parameters
            {
                get
                {
                    return this.parameters;
                }
            }

            public CreateDataReader CreateDataReader
            {
                get
                {
                    return this.createDataReader;
                }
            }
        }
    }
}
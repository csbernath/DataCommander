namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

	internal sealed class TfsDownloadDataReader : TfsDataReader
	{
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private TfsCommand command;
		private Item item;
		private string localPath;
		private bool first = true;
		private int recordsAffected;
		private Queue<Item> queue = new Queue<Item>();

		public TfsDownloadDataReader( TfsCommand command )
		{
            Contract.Requires(command != null);
			this.command = command;
		}

		public override int FieldCount
		{
			get
			{
				return 4;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
		}

		public override DataTable GetSchemaTable()
		{
			DataTable table = CreateSchemaTable();
			AddSchemaRowString( table, "ServerItem", false );
			AddSchemaRowInt32( table, "ItemType", false );
			AddSchemaRowDateTime( table, "CheckinDate", false );
			AddSchemaRowInt32( table, "ContentLength", false );
			return table;
		}

		public override bool Read()
		{
			bool read;

			if (this.command.Cancelled)
			{
				read = false;
			}
			else
			{
				if (first)
				{
					first = false;
					string serverPath = (string) command.Parameters[ "serverPath" ].Value;
					this.item = command.Connection.VersionControlServer.GetItem( serverPath );
					this.localPath = Database.GetValueOrDefault<string>( command.Parameters[ "localPath" ].Value );

					if (this.localPath == null)
					{
						this.localPath = Path.GetTempPath();
						this.localPath = Path.Combine( this.localPath, DateTime.Now.ToString( "yyyyMMdd HHmmss.fff" ) );

						switch (item.ItemType)
						{
							case ItemType.File:
							case ItemType.Folder:
								string name = VersionControlPath.GetFileName( this.item.ServerItem );
								this.localPath = Path.Combine( localPath, name );
								break;

							default:
								throw new NotImplementedException();
						}
					}

					var queryForm = (QueryForm) Application.Instance.MainForm.ActiveMdiChild;
                    queryForm.AddInfoMessage( new InfoMessage( OptimizedDateTime.Now, InfoMessageSeverity.Information, string.Format( "localPath: {0}", localPath ) ) );


					if (!VersionControlPath.IsValidPath( serverPath ))
					{
						throw new ArgumentException( string.Format( "The parameter serverPath '{0}' is invalid.", serverPath ) );
					}

					this.queue.Enqueue( item );
				}

				if (this.queue.Count > 0)
				{
					Item current = this.queue.Dequeue();
					object[] values = new object[ 4 ];
					values[ 0 ] = current.ServerItem;
					values[ 1 ] = current.ItemType;
					values[ 2 ] = current.CheckinDate;
					values[ 3 ] = current.ContentLength;
					this.Values = values;
					string name = VersionControlPath.GetFileName( current.ServerItem );
					string path;

                    if (item.ServerItem.Length + 1 <= current.ServerItem.Length)
                    {
                        string secondPath = current.ServerItem.Substring( item.ServerItem.Length + 1 );
                        secondPath = secondPath.Replace( VersionControlPath.Separator, Path.DirectorySeparatorChar );
                        path = Path.Combine( localPath, secondPath );
                    }
                    else
                    {
                        path = this.localPath;
                    }

					switch (current.ItemType)
					{
						case ItemType.File:
							log.Write( LogLevel.Trace,  "Downloading {0}...", current.ServerItem );
							current.DownloadFile( path );
							DateTime checkingDate = current.CheckinDate;
							FileInfo fileInfo = new FileInfo( path );
							fileInfo.LastWriteTime = checkingDate;
							fileInfo.Attributes = FileAttributes.ReadOnly;
							break;

						case ItemType.Folder:
							if (!Directory.Exists( path ))
							{
								DirectoryInfo directoryInfo = Directory.CreateDirectory( path );

								if (!directoryInfo.Exists)
								{
									throw new ArgumentException( string.Format( "The directory '{0}' does not exist.", path ) );
								}
							}

							ItemSet itemSet = current.VersionControlServer.GetItems( current.ServerItem, RecursionType.OneLevel );

							foreach (Item childItem in itemSet.Items.Skip( 1 ))
							{
								this.queue.Enqueue( childItem );
							}

							break;

						default:
							throw new NotImplementedException();
					}

					read = true;
					this.recordsAffected++;
				}
				else
				{
					read = false;
				}
			}

			return read;
		}
	}
}
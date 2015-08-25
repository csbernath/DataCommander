namespace DataCommander.Foundation.Web
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Web;
    using System.Web.SessionState;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;

    /// <exclude/>
    public static class SessionMonitor
    {
        private static IndexableCollection<SessionData> sessions;
        private static LinkedListIndex<SessionData> linkedListIndex;
        private static UniqueIndex<String, SessionData> idIndex;

        private static List<SessionData> history = new List<SessionData>();

        static SessionMonitor()
        {
            linkedListIndex = new LinkedListIndex<SessionData>( "LinkedList" );
            idIndex = new UniqueIndex<String, SessionData>(
                "Id",
                delegate( SessionData sessionData, out String sessionId )
                {
                    sessionId = sessionData.SessionId;
                    return true;
                },
                false );

            sessions = new IndexableCollection<SessionData>( linkedListIndex );
            sessions.Indexes.Add( idIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public static String State
        {
            get
            {
                var table = new DataTable();
                table.Locale = CultureInfo.InvariantCulture;
                table.Columns.Add( "SessionID" );
                table.Columns.Add( "UserName" );
                table.Columns.Add( "HostName" );
                table.Columns.Add( "StartTime" );
                table.Columns.Add( "EndTime" );

                lock (history)
                {
                    AddRows( history, table.Rows );
                }

                lock (sessions)
                {
                    AddRows( sessions, table.Rows );
                }

                return "SessionMonitor.State:\r\n" + table.ToStringTable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SessionStart()
        {
            var context = HttpContext.Current;
            String id = context.Session.SessionID;
            String userName = context.User.Identity.Name;
            String hostName = context.Request.UserHostName;
            var data = new SessionData( id, userName, hostName );

            lock (sessions)
            {
                SessionData prev;
                if (idIndex.TryGetValue( id, out prev ))
                {
                    sessions.Remove( prev );

                    lock (history)
                    {
                        history.Add( prev );
                    }
                }

                sessions.Add( data );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public static void SessionEnd( HttpSessionState session )
        {
            String id = session.SessionID;
            SessionData sessionData;
            if (idIndex.TryGetValue( id, out sessionData ))
            {
                sessionData.EndTime = OptimizedDateTime.Now;

                lock (sessions)
                {
                    sessions.Remove( sessionData );
                }

                lock (history)
                {
                    history.Add( sessionData );
                }
            }
            else
            {
                ApplicationLog.Instance.Write( LogLevel.Error, false, "Session not found. SessionID: {0}", id );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            lock (history)
            {
                history.Clear();
            }
        }

        private static void AddRows( IEnumerable<SessionData> enumerable, DataRowCollection rows )
        {
            foreach (var session in enumerable)
            {
                Object endTime = null;

                if (session.EndTime != DateTime.MinValue)
                {
                    endTime = session.EndTime.ToString( "yyyy.MM.dd HH:mm:ss.fff" );
                }

                rows.Add( new Object[]
                {                
                    session.SessionId,
                    session.UserName,
                    session.HostName,
                    session.StartTime.ToString("yyyy.MM.dd HH:mm:ss.fff"),
                    endTime
                } );
            }
        }

        private sealed class SessionData
        {
            private String sessionId;
            private String userName;
            private String hostName;
            private DateTime startTime = OptimizedDateTime.Now;
            private DateTime endTime;

            public SessionData(
                String sessionId,
                String userName,
                String hostName )
            {
                this.sessionId = sessionId;
                this.userName = userName;
                this.hostName = hostName;
            }

            public String SessionId
            {
                get
                {
                    return this.sessionId;
                }
            }

            public String UserName
            {
                get
                {
                    return this.userName;
                }
            }

            public String HostName
            {
                get
                {
                    return this.hostName;
                }
            }

            public DateTime StartTime
            {
                get
                {
                    return this.startTime;
                }
            }

            public DateTime EndTime
            {
                get
                {
                    return this.endTime;
                }

                set
                {
                    this.endTime = value;
                }
            }
        }
    }
}
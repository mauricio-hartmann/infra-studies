using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace IS.Core.Logging
{
    public sealed class UtcTimestampColumnWriter : ColumnWriterBase
    {
        public UtcTimestampColumnWriter() : base(NpgsqlDbType.TimestampTz)
        {
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            return logEvent.Timestamp.UtcDateTime;
        }
    }
}

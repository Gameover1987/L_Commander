using System;

namespace L_Commander.Common.Extensions
{
    public static class ObjectExtension
    {
        public static int ToInt(this object obj)
        {
            return int.Parse(obj.ToString());
        }

        public static int? ToIntOrNull(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            return obj.ToInt();
        }

        public static float ToFloat(this object obj)
        {
            return float.Parse(obj.ToString());
        }

        public static decimal ToDecimal(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;

            return decimal.Parse(obj.ToString());
        }

        public static decimal? ToDecimalOrNull(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            return obj.ToDecimal();
        }

        public static DateTime ToDateTime(this object obj)
        {
            return DateTime.Parse(obj.ToString());
        }

        public static DateTime? ToDateTimeOrNull(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            return obj.ToDateTime();
        }

        public static bool ToBool(this object obj)
        {
            if (bool.TryParse(obj.ToString(), out var boolValue))
            {
                return boolValue;
            }

            return false;
        }

        public static Guid ToGuid(this object obj)
        {
            return Guid.Parse(obj.ToString());
        }

        public static string ToYesNo(this bool boolValue)
        {
            if (boolValue)
                return "Да";

            return "Нет";
        }

        public static object GetValueOrDbNull(this object obj)
        {
            if (obj == null)
                return DBNull.Value;

            if (obj.ToString().IsNullOrWhiteSpace())
                return DBNull.Value;

            if (obj.SerializeToJson().IsNullOrEmpty())
                return DBNull.Value;

            return obj;
        }
    }
}

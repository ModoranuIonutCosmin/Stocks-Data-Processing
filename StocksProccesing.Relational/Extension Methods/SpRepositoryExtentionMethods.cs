using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.DataAccess;

namespace StocksProccesing.Relational.Extension_Methods;

public static class SprocRepositoryExtensions
{
    public static DbCommand LoadStoredProcedure(this StocksMarketContext context, string Sp_NotificationCounter)
    {
        var cmd = context.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = Sp_NotificationCounter;
        cmd.CommandType = CommandType.StoredProcedure;
        return cmd;
    }

    public static DbCommand WithSqlParams(this DbCommand cmd, params (string, object)[] nameValues)
    {
        foreach (var pair in nameValues)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = pair.Item1;
            param.Value = pair.Item2 ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        return cmd;
    }

    public static IList<T> ExecuteStoredProcedure<T>(this DbCommand command) where T : class
    {
        using (command)
        {
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();
            try
            {
                using var reader = command.ExecuteReader();
                return reader.MapToList<T>();
            }
            finally
            {
                command.Connection.Close();
            }
        }
    }

    public static async Task<IList<T>> ExecuteStoredProcedureAsync<T>(this DbCommand command) where T : class
    {
        using (command)
        {
            if (command.Connection.State == ConnectionState.Closed)
                await command.Connection.OpenAsync();
            try
            {
                using var reader = command.ExecuteReader();
                return reader.MapToList<T>();
            }
            finally
            {
                command.Connection.Close();
            }
        }
    }

    private static IList<T> MapToList<T>(this DbDataReader dr)
    {
        var objList = new List<T>();
        var props = typeof(T).GetRuntimeProperties();

        var colMapping = dr.GetColumnSchema()
            .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
            .ToDictionary(key => key.ColumnName.ToLower());

        if (dr.HasRows)
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var prop in props)
                {
                    if (!prop.CanWrite)
                        continue;

                    var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                }

                objList.Add(obj);
            }

        return objList;
    }
}
using Dapper;
using MySqlConnector;
using System.Data;

namespace resume_service_backend.Repositories
{
    public abstract class AbstractRepository
    {
        protected readonly string _connectionString;

        protected AbstractRepository(string connectionString)
        {
            _connectionString = connectionString;
        }   

        /// <summary>
        /// ЕДИНСТВЕННЫЙ метод, который управляет соединением и транзакцией.
        /// Принимает функцию, которая будет выполняться внутри транзакции.
        /// </summary>
        private async Task<T> ExecuteInTransactionAsync<T>(
            Func<MySqlConnection, MySqlTransaction, Task<T>> action)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = await action(connection, transaction);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Выполняет хранимую процедуру без возврата данных (INSERT, UPDATE, DELETE)
        /// </summary>
        protected async Task ExecuteProcAsync(string storedProc, object parameters)
        {
            await ExecuteInTransactionAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(
                    storedProc,
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
                return true; // заглушка, т.к. Task<T> требует возврата
            });
        }

        /// <summary>
        /// Выполняет хранимую процедуру, возвращающую список объектов
        /// </summary>
        protected async Task<List<T>> QueryProcAsync<T>(string storedProc, object? parameters = null) 
            where T : class
        {
            return await ExecuteInTransactionAsync(async (connection, transaction) =>
            {
                var result = await connection.QueryAsync<T>(
                    storedProc,
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
                return result.AsList();
            });
        }

        /// <summary>
        /// Выполняет хранимую процедуру, возвращающую один объект или null
        /// </summary>
        protected async Task<T?> QuerySingleProcAsync<T>(string storedProc, object? parameters = null) 
            where T : class
        {
            return await ExecuteInTransactionAsync(async (connection, transaction) =>
            {
                return await connection.QueryFirstOrDefaultAsync<T>(
                    storedProc,
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
            });
        }
    }
}
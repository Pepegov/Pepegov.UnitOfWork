using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Database
{
    /// <summary>
    /// Extensions for <see cref="IMongoClient"/>
    /// </summary>
    public static class MongoClientExtension
    {
        /// <summary>
        /// Tests that a transaction available in MongoDb replica set
        /// </summary>
        /// <param name="mongoClient"></param>
        /// <param name="cancellation"></param>
        public static async Task<bool> IsEnsureTransactionReadyAsync(
            this IMongoClient mongoClient,
            CancellationToken cancellation = default)
        {
            var result = false;
            var database = mongoClient.GetDatabase("__empty-db");
            try
            {
                while (true)
                {

                    _ = database.GetCollection<Empty>("__empty");
                    await database.DropCollectionAsync("__empty", cancellation);

                    var session = await mongoClient.StartSessionAsync(cancellationToken: cancellation);

                    try
                    {
                        session.StartTransaction();
                        await session.AbortTransactionAsync(cancellation);
                        result = true;
                    }
                    finally
                    {
                        session.Dispose();
                    }

                    break;
                }
            }
            finally
            {
                await mongoClient.DropDatabaseAsync("__empty-db", cancellation).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Tests that a transaction available in MongoDb replica set
        /// </summary>
        /// <param name="mongoClient"></param>
        public static bool IsEnsureTransactionReady(this IMongoClient mongoClient)
        {
            var result = false;
            var database = mongoClient.GetDatabase("__empty-db");
            try
            {
                while (true)
                {

                    _ = database.GetCollection<Empty>("__empty");
                    database.DropCollection("__empty");

                    var session = mongoClient.StartSession();

                    try
                    {
                        session.StartTransaction();
                        session.AbortTransaction();
                        result = true;
                    }
                    finally
                    {
                        session.Dispose();
                    }

                    break;
                }
            }
            finally
            {
                mongoClient.DropDatabase("__empty-db");
            }

            return result;
        }

        private sealed class Empty
        {
            public int Id { get; set; }
        }
    }
}

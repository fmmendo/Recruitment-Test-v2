using JG.FinTechTest.Model;
using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest.Repository
{
    public class DeclarationRepository : IRepository<GiftAidDeclaration>
    {
        private LiteDatabase _db;
        private LiteCollection<GiftAidDeclaration> _declarations;

        public DeclarationRepository(IOptions<AppSettings> settings)
        {
            _db = new LiteDatabase(settings.Value.DbLocation);
            _declarations = (LiteCollection<GiftAidDeclaration>)_db.GetCollection<GiftAidDeclaration>(settings.Value.DonationsTable);
        }

        public Result<int> InsertRecord(GiftAidDeclaration d)
        {
            try
            {
                _declarations.Insert(d);
            }
            catch (LiteException le)
            {
                return Result.Fail(default(int), le.Message, ErrorType.FailedToInsertToDatabase);
            }

            return Result.Ok(d.Id);
        }

        public Result<GiftAidDeclaration> ReadRecord(int id)
        {
            var donation = _declarations.FindById(id);

            if (donation == null)
                return Result.Fail<GiftAidDeclaration>(null, $"Could not find donation with id: {id}.", ErrorType.DonationNotFound);

            return Result.Ok(donation);
        }

        public Result UpdateRecord(GiftAidDeclaration d)
        {
            var donation = _declarations.FindById(d.Id);

            if (donation == null)
                return Result.Fail<GiftAidDeclaration>(null, $"Could not find donation with id: {d.Id} to update.", ErrorType.DonationNotFound);

            _declarations.Update(d);

            return Result.Ok();
        }
    }

    public interface IRepository<T>
    {
        Result<int> InsertRecord(T p);
        Result<T> ReadRecord(int id);
        Result UpdateRecord(T p);
    }
}

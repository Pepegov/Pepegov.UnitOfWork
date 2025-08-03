using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework;

namespace TelecomService.QRON.Payment.Fiscal.Test.Core.Specimens;

public sealed class UnitOfWorkSpecimenBuilder : ISpecimenBuilder
{
    private readonly DbContext _context;

    public UnitOfWorkSpecimenBuilder(DbContext context)
    {
        _context = context;
    }

    public object Create(object request, ISpecimenContext context)
    {
        //If requested is not match required type then skip this builder
        if (request is not Type type || type != typeof(IUnitOfWorkManager))
        {
            return new NoSpecimen(); // indicate that current specimen builder cannot create requested object
        }

        var instances = new IUnitOfWorkInstance[]
        {
            new UnitOfWorkEntityFrameworkInstance<DbContext>(_context)
        };

        return new UnitOfWorkManager(instances);
    }
}
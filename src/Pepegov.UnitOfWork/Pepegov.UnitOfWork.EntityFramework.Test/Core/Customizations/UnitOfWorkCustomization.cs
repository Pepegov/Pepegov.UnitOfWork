using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TelecomService.QRON.Payment.Fiscal.Test.Core.Specimens;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Core.Customizations;

/// <summary>
/// A Customization that add UnitOfWork builder to Fixture
/// </summary>
public sealed class UnitOfWorkCustomization(DbContext context) : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new UnitOfWorkSpecimenBuilder(context));
    }
}
using AutoFixture;
using Pepegov.UnitOfWork.EntityFramework.Test.Core.Specimens;
using TelecomService.QRON.Payment.Fiscal.Test.Core.Specimens;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Core.Customizations;

public class LoggerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new LoggerSpecimenBuilder());
    }
}
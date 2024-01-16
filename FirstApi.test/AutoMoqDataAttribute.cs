using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FirstApi.test;


    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => 
                new Fixture()
                    .Customize(new AutoMoqCustomization())) 
        {
        }
    }

public class OmitAutoPropCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        if (fixture == null) throw new ArgumentNullException(nameof(fixture));
        fixture.Customize<BindingInfo>(cc => cc.OmitAutoProperties());
    }
}


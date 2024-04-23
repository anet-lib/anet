using Anet.Atrributes;
using Anet.Utilities;

namespace Tests
{
    public class UtilityTests
    {
        [Fact]
        public void EnumUtil_GetSelectOptions_Test()
        {
            var options = EnumUtil.GetSelectOptions<Enum1>();

            Assert.Equal(2, options.Count());
        }
    }

    public enum Enum1
    {
        [Display("Foo display text")] Foo,
        [Display("Bar display text")] Bar
    }
}

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Anet.Tests
{
    public class IdGenTests
    {
        [Fact]
        public void GeneratedIds_Not_Duplication()
        {
            IdGen.SetInstace(new IdGenerator(15));

            var idList = new List<long>();

            for (int i= 0; i < 10000; i++)
            {
                var id = IdGen.NewId();
                idList.Add(id);
            }

            var originalCount = idList.Count;
            var distinctCount = idList.Distinct().Count();

            Assert.Equal(originalCount, distinctCount);
        }
    }
}

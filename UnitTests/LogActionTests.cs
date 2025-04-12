using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json;
using RetryMachine;
using RetryMachine.Api.Actions;
using RetryMachine.Api.Service;
using RetryMachine.Api.Sevices;

namespace UnitTests
{
    [TestClass]
    public class LogActionTests
    {
        private ILogService _logService;

        [TestInitialize]
        public void TestInitialize()
        {
            _logService = A.Fake<ILogService>();
        }

        [TestMethod]
        public async Task Stuff_ThatMatches()
        {
            var service = new LogAction(_logService);

            await service.Perform(JsonConvert.SerializeObject(new LogSettings { AccountId = 11, Template = "" }));

            A.CallTo(() => _logService.WriteLog(11,"")).MustHaveHappened();
        }

    }
}

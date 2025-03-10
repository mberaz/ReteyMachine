using FakeItEasy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RetryMachine;
using RetryMachine.Api.Actions;
using RetryMachine.Api.Service;

namespace UnitTests
{
    [TestClass]
    public sealed class RandomServiceTests
    {
        private IRetryMachineRunner _retryMachine;

        [TestInitialize]
        public void TestInitialize()
        {
            _retryMachine = A.Fake<IRetryMachineRunner>();
        }

        [TestMethod]
        public async Task Stuff_ThatMatches()
        {
            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>._)).Returns(Task.CompletedTask);
            var service = new RandomService(_retryMachine);

            await service.Stuff();

            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>.That.
                Matches(l => MatchTaskList(l)))).MustHaveHappened();
        }

        [TestMethod]
        public async Task Stuff_GetArgs()
        {
            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>._)).Returns(Task.CompletedTask);
            var service = new RandomService(_retryMachine);

            await service.Stuff();

            var list = GetArgs(_retryMachine);

            MatchTaskList(list);
        }

        private bool MatchTaskList(List<RetryCreate> list)
        {
            var autoLogsTask = list.FirstOrDefault(l => l.TaskName == AutoLogAction.ActionName);
            Assert.IsNotNull(autoLogsTask);
            Assert.AreEqual(autoLogsTask.Order, 1);
            var autoLogsSettings = JObject.Parse(autoLogsTask.Settings);
            Assert.AreEqual(autoLogsSettings.Value<int>("AccountHolderId"), 11);

            var ualTask = list.FirstOrDefault(l => l.TaskName == UserActionLogAction.ActionName);
            Assert.IsNotNull(ualTask);
            Assert.AreEqual(ualTask.Order, 2);
            var ualSettings = JsonConvert.DeserializeObject<UserActionLogSettings>(ualTask.Settings);
            Assert.AreEqual(ualSettings.ActionId, 11);

            return true;
        }

        private static List<RetryCreate> GetArgs(IRetryMachineRunner retryMachine)
        {
            var calls = Fake.GetCalls(retryMachine).ToList();

            return calls.First().Arguments.Get<List<RetryCreate>>("tasks");
        }
    }
}

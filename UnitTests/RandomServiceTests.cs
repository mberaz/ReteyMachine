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
            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>._, A<string>._, A<string>._))
                .Returns(Task.CompletedTask);

            var service = new RandomService(_retryMachine);

            await service.Stuff();

            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>.That.
                Matches(l => MatchTaskList(l)), A<string>._, A<string>._)).MustHaveHappened();
        }

        [TestMethod]
        public async Task Stuff_GetArgs()
        {
            A.CallTo(() => _retryMachine.CreateTasks(A<List<RetryCreate>>._, A<string>._, A<string>._)).Returns(Task.CompletedTask);
            var service = new RandomService(_retryMachine);

            await service.Stuff();

            var list = GetArgs(_retryMachine);

            MatchTaskList(list);
        }

        private bool MatchTaskList(List<RetryCreate>? list)
        {
            var LogsTask = list.FirstOrDefault(l => l.TaskName == LogAction.ActionName);
            Assert.IsNotNull(LogsTask);
            Assert.AreEqual(LogsTask.Order, 1);
            var LogsSettings = JObject.Parse(LogsTask.Settings);
            Assert.AreEqual(LogsSettings.Value<int>("AccountId"), 11);

            var monitorTask = list.FirstOrDefault(l => l.TaskName == MonitoringAction.ActionName);
            Assert.IsNotNull(monitorTask);
            Assert.AreEqual(monitorTask.Order, 2);
            var monitorSettings = JsonConvert.DeserializeObject<MonitoringSettings>(monitorTask.Settings);
            Assert.AreEqual(monitorSettings.ActionId, 11);

            return true;
        }

        private static List<RetryCreate>? GetArgs(IRetryMachineRunner retryMachine)
        {
            var calls = Fake.GetCalls(retryMachine).ToList();

            return calls.First().Arguments.Get<List<RetryCreate>>("tasks");
        }
    }
}

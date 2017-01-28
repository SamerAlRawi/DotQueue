using System.Linq;
using System.Net.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using DotQueue.HostLib.IOC;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class ApiHostConfigurationBuilderTests
    {
        private ApiHostConfigurationBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new ApiHostConfigurationBuilder();
        }

        [Test]
        public void BuildHostConfiguration_Sets_Port_And_MessageSize()
        {
            var port = 1001;
            var maxMessageSize = 123456;
            var config = _builder.BuildHostConfiguration(port, maxMessageSize);

            Assert.That(config.BaseAddress.Port, Is.EqualTo(port));
            Assert.That(config.MaxReceivedMessageSize, Is.EqualTo(maxMessageSize));
        }

        [Test]
        public void BuildHostConfiguration_Sets_AssemblyResolver()
        {
            var config = _builder.BuildHostConfiguration(1, 1);

            Assert.That(config.Services.GetService(typeof(IAssembliesResolver)), Is.InstanceOf<CustomAssemblyResolver>());
        }

        [Test]
        public void BuildHostConfiguration_Sets_DependencyResolver()
        {
            var config = _builder.BuildHostConfiguration(1, 1);

            Assert.That(config.DependencyResolver, Is.InstanceOf<DotQueueIoCContainer>());
        }
        [Test]
        public void BuildHostConfiguration_Sets_Routes()
        {
            var config = _builder.BuildHostConfiguration(1, 1);

            config.Routes.Single(route => route.RouteTemplate == "Api/{controller}/{id}");
            config.Routes.Single(route => route.RouteTemplate == "Api/{controller}/{action}");
            config.Routes.Single(route => route.RouteTemplate == "Api/{controller}"
                        && (route.Constraints["httpMethod"] as HttpMethodConstraint).AllowedMethods[0] == HttpMethod.Get);
            config.Routes.Single(route => route.RouteTemplate == "Api/{controller}"
                        && (route.Constraints["httpMethod"] as HttpMethodConstraint).AllowedMethods[0] == HttpMethod.Post);
        }
    }
}

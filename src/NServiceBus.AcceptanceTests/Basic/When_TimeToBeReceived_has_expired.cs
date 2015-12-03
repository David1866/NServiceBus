﻿namespace NServiceBus.AcceptanceTests.Basic
{
    using System;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NUnit.Framework;

    public class When_TimeToBeReceived_has_expired : NServiceBusAcceptanceTest
    {
        [Test, Ignore("The TTL will only be started at the moment the timeoutmanager sends the message back, still giving the test a second to receive it")]
        public void Message_should_not_be_received()
        {
            var context = new Context();
            Scenario.Define(context)
                    .WithEndpoint<Endpoint>(b => b.Given((bus, c) => bus.Defer(TimeSpan.FromSeconds(5), new MyMessage())))
                    .Run(TimeSpan.FromSeconds(10));
            Assert.IsFalse(context.WasCalled);
        }

        public class Context : ScenarioContext
        {
            public bool WasCalled { get; set; }
        }
        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(c => c.Transactions().Disable()); //transactional msmq with ttbr not supported
            }
            public class MyMessageHandler : IHandleMessages<MyMessage>
            {
                public Context Context { get; set; }

                public IBus Bus { get; set; }

                public void Handle(MyMessage message)
                {
                    Context.WasCalled = true;
                }
            }
        }

        [Serializable]
        [TimeToBeReceived("00:00:01")]
        public class MyMessage : IMessage
        {
        }
    }
}
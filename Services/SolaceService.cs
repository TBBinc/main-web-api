using SolaceSystems.Solclient.Messaging;
using SiteWebApi.Interfaces;
using System;
using System.Text;

namespace SiteWebApi.Services
{
    public class SolaceService : ISolaceService
    {
        private static readonly SolaceService _SolaceService = new SolaceService();
        protected readonly ContextFactoryProperties _cfp;
        private SessionProperties sessionProps;
        private ISession session;

        private IContext context;
        public SolaceService(){
            _cfp = new ContextFactoryProperties()
            {
                SolClientLogLevel = SolLogLevel.Warning
            };
            _cfp.LogToConsoleError();
            ContextFactory.Instance.Init(_cfp);
            sessionProps = new SessionProperties(){
                Host = Environment.GetEnvironmentVariable("SolaceHost"),
                VPNName = Environment.GetEnvironmentVariable("SolaceVPNName"),
                UserName = Environment.GetEnvironmentVariable("SolaceUserName"),
                Password = Environment.GetEnvironmentVariable("SolacePassword"),
                //SSLTrustStoreDir = Environment.GetEnvironmentVariable("TRUST_STORE")
                
            };


            context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);

            session = context.CreateSession(sessionProps, null, null);

            ReturnCode returnCode = session.Connect();

            if (returnCode == ReturnCode.SOLCLIENT_OK)
            {
                Console.WriteLine("Session successfully connected.");
            }
            else
            {
                Console.WriteLine("Error connecting, return code: {0}", returnCode);
            }
        }
        public virtual void PublishMessage(string messageText)
        {
            // Create the message
            using (IMessage message = ContextFactory.Instance.CreateMessage())
            {
                message.Destination = ContextFactory.Instance.CreateTopic("TBBinc/signal");
                // Create the message content as a binary attachment
                message.BinaryAttachment = Encoding.ASCII.GetBytes(messageText);

                // Publish the message to the topic on the Solace messaging router
                Console.WriteLine("Publishing message...");
                ReturnCode returnCode = session.Send(message);
                if (returnCode == ReturnCode.SOLCLIENT_OK)
                {
                    Console.WriteLine("Done.");
                }
                else
                {
                    Console.WriteLine("Publishing failed, return code: {0}", returnCode);
                }
            }
        }
    }
}
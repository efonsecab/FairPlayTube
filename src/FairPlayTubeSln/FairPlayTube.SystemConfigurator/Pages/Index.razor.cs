using FairPlayTube.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.SystemConfigurator.Pages
{
    public partial class Index
    {
        public string ErrorMessage { get; private set; }
        public List<Controller> Controllers = new List<Controller>();

        protected override void OnInitialized()
        {
            try
            {
                var assembly = typeof(VideoController).Assembly;
                var types = assembly.GetTypes().Where(p=>p.Name.EndsWith("Controller"));
                foreach (var singleType in types)
                {
                    Controller controller = new Controller()
                    {
                        Name = singleType.Name
                    };
                    var endpoints = singleType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                    foreach (var singleEndpoint in endpoints)
                    {
                        controller.Endpoints.Add(new Endpoint() 
                        {
                            Name = singleEndpoint.Name
                        });
                    }
                    this.Controllers.Add(controller);
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {

            }
        }
    }

    public class Controller
    {

        public string Name { get; set; }
        public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
    public class Endpoint
    {
        public string Name { get; set; }
    }
}

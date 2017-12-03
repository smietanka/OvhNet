using Newtonsoft.Json;
using OvhWrapper.Types.OpenStack;
using OvhWrapper.Types.OpenStack.Models;
using RestSharp;
using System.Linq;

namespace OvhWrapper
{
    public class OvhStorage
    {
        private string UserName { get; set; }
        private string Password { get; set; }
        private string TenantId { get; set; }
        private string Region { get; set; }

        //manage container
        public Container Container { get; set; }
        public ContainerObject ContainerObject { get; set; }

        //Connection details
        private AccessDetail Access { get; set; }
        private ServiceDetail ObjectStorageService { get; set; }
        private EndpointDetail CurrentRegionEndpoint { get; set; }

        //Includes
        private readonly RestClient Client = new RestClient("https://auth.cloud.ovh.net/v2.0");
        /// <summary>
        /// Initialize object storage openstack api connection.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="tenantId"></param>
        /// <param name="region">For example: GRA1</param>
        public OvhStorage(string userName, string password, string tenantId, string region)
        {
            UserName = userName;
            Password = password;
            TenantId = tenantId;
            Region = region;
            Connection();
        }

        /// <summary>
        /// Make connection to OpenStack
        /// </summary>
        private void Connection()
        {
            var requestBody = new
            {
                auth = new
                {
                    passwordCredentials = new
                    {
                        username = UserName,
                        password = Password
                    },
                    tenantId = TenantId
                }
            };

            var request = new RestRequest("/tokens", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(requestBody);
            var response = Client.Execute(request);

            if(ResponseError(response, true))
            {
                AccessRoot accesTo = JsonConvert.DeserializeObject<AccessRoot>(response.Content);
                Access = accesTo.Access;
                var tempService = Access.ServiceCatalog.Where(z => z.Type.Equals("object-store")).FirstOrDefault();
                if (tempService != null)
                {
                    ObjectStorageService = tempService;
                }
                else throw new System.Exception("Cannot find object storage service in access");
                var tempRegion = ObjectStorageService.Endpoints.Where(z => z.Region.Equals(Region)).FirstOrDefault();
                if (tempRegion != null)
                {
                    CurrentRegionEndpoint = tempRegion;
                }
                else throw new System.Exception("Cannot find endpoint in storage service");
            }

            //Initialize manages
            Container = new Container(Access, CurrentRegionEndpoint);
            ContainerObject = new ContainerObject(Access, CurrentRegionEndpoint);
        }
        
        public static bool ResponseError(IRestResponse response, bool throwException = false)
        {
            if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
            {
                if(throwException)
                {
                    throw new System.Exception(string.Format("Error. {0} {1}", (int)response.StatusCode, response.Content));
                }
                return false;
            }
            return true;
        }
    }
}
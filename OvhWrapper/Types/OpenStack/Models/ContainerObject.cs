using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Extensions;

namespace OvhWrapper.Types.OpenStack.Models
{
    public class ContainerObject
    {
        private AccessDetail Access { get; set; }
        private EndpointDetail Endpoint { get; set; }

        //Includes
        private RestClient Client { get; set; }
        public ContainerObject(AccessDetail access, EndpointDetail endpoint)
        {
            Access = access;
            Endpoint = endpoint;
            Client = new RestClient(Endpoint.PublicUrl);
        }

        /// <summary>
        /// Insert file into container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Set(string containerName, string fileName, string path)
        {
            var fileBytes = File.ReadAllBytes(path);

            var request = new RestRequest("/{containerName}/{fileName}", Method.PUT);
            request.AddUrlSegment("containerName", containerName);
            request.AddUrlSegment("fileName", fileName);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-Auth-Token", Access.Token.Id);
            request.AddFileBytes(fileName, fileBytes, fileName);
            var response = Client.Execute(request);
            return OvhStorage.ResponseError(response);
        }

        /// <summary>
        /// Insert file into container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Set(string containerName, string fileName, Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var byteee = ms.ToArray();
                var request = new RestRequest("/{containerName}/{fileName}", Method.PUT);
                request.AddUrlSegment("containerName", containerName);
                request.AddUrlSegment("fileName", fileName);

                request.AddHeader("Accept", "application/json");
                request.AddHeader("X-Auth-Token", Access.Token.Id);
                request.AddFileBytes(fileName, byteee, fileName);
                var response = Client.Execute(request);
                return OvhStorage.ResponseError(response);
            }
        }

        /// <summary>
        /// Delete file from container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Delete(string containerName, string fileName)
        {
            var request = new RestRequest("/{containerName}/{fileName}", Method.DELETE);
            request.AddUrlSegment("containerName", containerName);
            request.AddUrlSegment("fileName", fileName);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-Auth-Token", Access.Token.Id);

            var response = Client.Execute(request);
            return OvhStorage.ResponseError(response);
        }

        /// <summary>
        /// Get meta datas about file
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<ObjectMetaData> Info(string containerName, string fileName)
        {
            var request = new RestRequest("/{containerName}/{fileName}", Method.HEAD);
            request.AddUrlSegment("containerName", containerName);
            request.AddUrlSegment("fileName", fileName);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-Auth-Token", Access.Token.Id);
            var response = Client.Execute(request);
            OvhStorage.ResponseError(response);
            return response.Headers.Select(z => new ObjectMetaData() { ContentType = z.ContentType, Name = z.Name, Type = z.Type, Value = z.Value }).ToList();
        }

        /// <summary>
        /// Copy file from source container to destination container
        /// </summary>
        /// <param name="containerName">Source container</param>
        /// <param name="fileName">File name</param>
        /// <param name="containerNameToPaste">Destination container</param>
        /// <returns></returns>
        public bool Copy(string containerName, string fileName, string containerNameToPaste)
        {
            var request = new RestRequest("/{containerName}/{fileName}", Method.PUT);
            request.AddUrlSegment("containerName", containerNameToPaste);
            request.AddUrlSegment("fileName", fileName);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-Auth-Token", Access.Token.Id);
            request.AddHeader("X-Copy-From", string.Format("{0}/{1}", containerName, fileName));
            request.AddHeader("Content-Length", "0");

            var response = Client.Execute(request);
            return OvhStorage.ResponseError(response);
        }

        /// <summary>
        /// Get file from container as byte[]
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] Get(string containerName, string fileName)
        {
            var request = new RestRequest("/{containerName}/{fileName}", Method.GET);
            request.AddUrlSegment("containerName", containerName);
            request.AddUrlSegment("fileName", fileName);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("X-Auth-Token", Access.Token.Id);
            var response = Client.Execute(request);
            OvhStorage.ResponseError(response);
            return response.RawBytes;
        }
    }
}

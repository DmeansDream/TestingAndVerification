using System;
using System.Collections;
using System.Text;
using NUnit.Framework;
using ScriptsToTest;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.Network
{
    public class FailingNetworkTests
    {
        private const string localhost = "https://127.0.0.1:7146";
        
        [UnityTest, Order(1)]
        public IEnumerator CreateRegionDataFails()
        {
            MapRegionData region = new MapRegionData()
            {
                RegionId = 0,
                RegionName = "Raya Lukaria",
                RegionBiome = "SunnyPlain"
            };
        
            var jsonToSend  = JsonConvert.SerializeObject(region);
            Debug.Log(jsonToSend);

            UnityWebRequest result = null;
            yield return Post(localhost + "/api/mapdata", jsonToSend, request =>
            {
                result = request;
            });

            Assert.AreEqual(401, result.responseCode);
        
            result.Dispose();
        }

        [UnityTest, Order(2)]
        public IEnumerator PlayerCharacterCreationWrongIdFails()
        {
            PlayerData dataToFail = new PlayerData()
            {
                ID = 100,
                Name = "Neal",
                MaxHealth = 100,
                Health = 0,
                Damage = 3
            };
            
            var jsonToSend  = JsonConvert.SerializeObject(dataToFail);
            Debug.Log(jsonToSend);

            UnityWebRequest result = null;
            yield return Post(localhost + "/api/pcdata", jsonToSend, request =>
            {
                result = request;
            });

            Assert.AreEqual(400, result.responseCode);
        
            result.Dispose();
        }
        
        [UnityTest, Order(3)]
        public IEnumerator PlayerGetBadRequest()
        {
            int idToFind = -1;
            UnityWebRequest req = UnityWebRequest.Get(localhost + $"/api/pcdata/{idToFind}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();
            
            Assert.That(req.responseCode, Is.EqualTo(400));
            
            req.Dispose();
        }
        
        [UnityTest, Order(4)]
        public IEnumerator PlayerGetNotFound()
        {
            int idToFind = 393;
            UnityWebRequest req = UnityWebRequest.Get(localhost + $"/api/pcdata/{idToFind}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();
            
            Assert.That(req.responseCode, Is.EqualTo(404));
            
            req.Dispose();
        }
        
        [UnityTest, Order(5)]
        public IEnumerator DeletePlayerCharacterDataFail()
        {
            int idToDelete = 393;
            UnityWebRequest req = UnityWebRequest.Delete(localhost + $"/api/pcdata/{idToDelete}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();
            
            Assert.That(req.responseCode, Is.EqualTo(404));
            
            req.Dispose();
        }
        
        IEnumerator Post(string url, string bodyJsonString, Action<UnityWebRequest> callback)
        {
            var request = new UnityWebRequest(url, "POST");
        
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.certificateHandler = new AcceptAllCertificates();
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);

            callback.Invoke(request);
        }
    
        IEnumerator Put(string url, string bodyJsonString, Action<UnityWebRequest> callback)
        {
            var request = new UnityWebRequest(url, "PUT");
        
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.certificateHandler = new AcceptAllCertificates();
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);

            callback.Invoke(request);
        }
    
        public class AcceptAllCertificates : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true; 
            }
        }
        
    }
}
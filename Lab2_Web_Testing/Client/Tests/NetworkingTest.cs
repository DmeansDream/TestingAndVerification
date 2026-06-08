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
    [TestFixture]

    public class NetworkingTest
    {
        private static PlayerCharacter pcUnderTest;
        private static int createdPcID;
        private const string localhost = "https://127.0.0.1:7146";

        [OneTimeSetUp]
        public void Setup()
        {
            string Name = "Ben";
            int MaxHealth = 100;
            int Health = 100;
            int Damage = 20;

            pcUnderTest = new PlayerCharacter(Name, MaxHealth, Health, Damage);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            pcUnderTest = null;
        }

        [UnityTest, Order(1)]
        public IEnumerator RegisterNewPlayerData()
        {
            var jsonToSend = JsonConvert.SerializeObject(pcUnderTest.data);
            Debug.Log(jsonToSend);

            UnityWebRequest result = null;
            yield return Post(localhost + "/api/pcdata", jsonToSend, request => { result = request; });

            Assert.AreEqual(201, result.responseCode);

            string jsonResponse = result.downloadHandler.text;
            PlayerData dataReceived = JsonConvert.DeserializeObject<PlayerData>(jsonResponse);

            if (dataReceived == null || dataReceived.ID == 0)
                Assert.Fail();

            createdPcID = dataReceived.ID;

            result.Dispose();
        }

        [UnityTest, Order(2)]
        public IEnumerator GetPlayerData()
        {
            UnityWebRequest req = UnityWebRequest.Get(localhost + $"/api/pcdata/{createdPcID}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();

            Assert.AreEqual(200, req.responseCode);

            string jsonResponse = req.downloadHandler.text;
            PlayerData dataReceived = JsonConvert.DeserializeObject<PlayerData>(jsonResponse);
            if (dataReceived == null || dataReceived.ID == 0)
                Assert.Fail();

            Assert.That(dataReceived.Name, Is.EqualTo(pcUnderTest.data.Name));

            pcUnderTest.data.ID = dataReceived.ID;

            req.Dispose();
        }

        [UnityTest, Order(3)]
        public IEnumerator UpdatePlayerStats()
        {
            int damageTaken = 2;
            int lvlHpIncrease = 5;
            int lvlDmgIncrease = 3;

            pcUnderTest.TakeDamage(damageTaken);
            pcUnderTest.LevelUp(lvlHpIncrease, lvlDmgIncrease);
            pcUnderTest.TakeDamage(damageTaken);
            pcUnderTest.TakeDamage(damageTaken);

            PlayerUpdateDTO dtoToSend = new PlayerUpdateDTO()
            {
                ID = pcUnderTest.data.ID,
                MaxHealth = pcUnderTest.data.MaxHealth,
                Health = pcUnderTest.data.Health,
                Damage = pcUnderTest.data.Damage
            };

            var jsonToSend = JsonConvert.SerializeObject(dtoToSend);
            Debug.Log(jsonToSend);

            UnityWebRequest result = null;
            yield return Put(localhost + $"/api/pcdata/{createdPcID}", jsonToSend, request => { result = request; });

            Assert.AreEqual(200, result.responseCode);

            result.Dispose();
        }

        [UnityTest, Order(4)]
        public IEnumerator GetRegionData()
        {
            int regionId = 1;
            string expectedRegion = "Leyndel";
            UnityWebRequest req = UnityWebRequest.Get(localhost + $"/api/mapdata/{regionId}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();

            Assert.AreEqual(200, req.responseCode);

            string jsonResponse = req.downloadHandler.text;
            MapRegionData dataReceived = JsonConvert.DeserializeObject<MapRegionData>(jsonResponse);
            if (dataReceived == null || dataReceived.RegionId == 0)
                Assert.Fail();

            Assert.That(dataReceived.RegionName, Is.EqualTo(expectedRegion));

            req.Dispose();
        }
        
        [UnityTest, Order(5)]
        public IEnumerator DeletePlayerCharacterData()
        {
            UnityWebRequest req = UnityWebRequest.Delete(localhost + $"/api/pcdata/{createdPcID}");
            req.certificateHandler = new AcceptAllCertificates();
            req.timeout = 5;

            yield return req.SendWebRequest();
            
            Assert.That(req.responseCode, Is.EqualTo(204));
            
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

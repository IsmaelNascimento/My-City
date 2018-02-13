using System.Collections;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.Networking;
using System.Text;
using System;

namespace PushStart.MyCity
{
    public class Request : MonoBehaviour
    {        
        // Routes
        private const string api = "http://dev.pushstart.com.br/desafio/public/api";
        private const string pathLogin = "/auth/login";
        private const string pathStatus = "/status";

        private static string loadDataUser;
        private User user;

        public static string callbackRequest;
        
        void Start()
        {
            user = new User();

            //StartCoroutine(RequestData_Coroutine());
            
            // Tests persistet data
            //JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("dataUser"), user); //Carrega sempre o ulimo valor em dinheiro do usuario;
        }

        public static IEnumerator RequestData_Coroutine(string username, string password)
        {
            WWWForm data = new WWWForm();

            data.AddField("username", username);
            data.AddField("password", GetHashSha256(password)); // Replace 'senha' inputField password 

            //Request Datas
            UnityWebRequest requestToken = UnityWebRequest.Post((api + pathLogin), data);

            yield return requestToken.Send();

            if (requestToken.isNetworkError || requestToken.isHttpError)
            {
                callbackRequest = "error";
                print("Verifique sua internet. Error:: " + requestToken.error);

                if (!string.IsNullOrEmpty(requestToken.downloadHandler.text))
                {
                    print("Usuario ou senha invalida. Error:: " + requestToken.downloadHandler.text);
                }
            }
            else
            {
                var dataAsJson = requestToken.downloadHandler.text;
                //print(dataAsJson);

                DataRequest dataRequest = JsonUtility.FromJson<DataRequest>(dataAsJson);
                //print("token:: " + dataRequest.token);

                //Resquest User
                UnityWebRequest requestProfile = UnityWebRequest.Get((api + pathStatus));

                requestProfile.SetRequestHeader("X-Authorization", dataRequest.token);

                yield return requestProfile.Send();

                if (requestProfile.isNetworkError || requestProfile.isHttpError)
                {
                    print(requestProfile.error);
                    callbackRequest = "error";
                }
                else
                {
                    var jsonUser = requestProfile.downloadHandler.text;
                    loadDataUser = jsonUser;

                    callbackRequest = "sucess";

                    // Tests persist data

                    //JsonUtility.FromJsonOverwrite(jsonUser, user);

                    //if (user.money != 0)
                    //{
                    //    JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("dataUser"), user);

                    //    user.money += 100;
                    //    PlayerPrefs.SetString("dataUser", JsonUtility.ToJson(user)); // Salva sempre o ulimo valor em dinheiro do usuario;

                    //    print("Last value user.money:: " + user.money);
                    //}
                }
            }
        }

        private static string GetHashSha256(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                var resultFinal = "";
                var newValue = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                foreach (var b in newValue)
                {
                    resultFinal += (b.ToString("x2"));
                }

                return resultFinal;
            }
        }
        
        public string LoadDataUser()
        {
            //print("loadDataUser:: " + loadDataUser);
            return loadDataUser;
        }
    }

    public class DataRequest
    {
        public Profile profile;
        public string token;
        public int expires;
    }

    public class Profile
    {
        public string name;
        public string type;
    }
}
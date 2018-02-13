using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PushStart.MyCity
{
    public class DataSave : MonoBehaviour
    {
        public static string jsonDataSaveBuildings;
        public static string jsonDataSaveUser;


        public static void SaveAll()
        {
            SaveDataUser();
            SaveDataBuildings();
        }

        public static void SaveDataUser()
        {
            jsonDataSaveUser = JsonUtility.ToJson(GameMaster.Instance.userDetails);

            PlayerPrefs.SetString("jsonDataSaveUser", jsonDataSaveUser);
        }

        public static void SaveDataBuildings()
        {
            var buildingsRoot = GameObject.FindGameObjectWithTag("Buildings");
            var buildings = new Buildings();
            buildings.buildings = new List<Building>();

            for (int i = 0; i < buildingsRoot.transform.childCount; i++)
            {
                var type = "";
                var position = new Vector3();

                if (buildingsRoot.transform.GetChild(i).GetComponent<Houses>())
                {
                    type = "houses";
                    position = buildingsRoot.transform.GetChild(i).position;
                }
                else if(buildingsRoot.transform.GetChild(i).GetComponent<Factory>())
                {
                    type = "factory";
                    position = buildingsRoot.transform.GetChild(i).position;
                }
                else if (buildingsRoot.transform.GetChild(i).GetComponent<Mall>())
                {
                    type = "mall";
                    position = buildingsRoot.transform.GetChild(i).position;
                }
                else if (buildingsRoot.transform.GetChild(i).GetComponent<Park>())
                {
                    type = "park";
                    position = buildingsRoot.transform.GetChild(i).position;
                }
                else if (buildingsRoot.transform.GetChild(i).GetComponent<Farm>())
                {
                    type = "farm";
                    position = buildingsRoot.transform.GetChild(i).position;
                }

                buildings.buildings.Add(new Building(type, position));
            }

            jsonDataSaveBuildings = JsonUtility.ToJson(buildings);

            PlayerPrefs.SetString("jsonDataSaveBuildings", jsonDataSaveBuildings);
        }

        public static void LoadAll(User user)
        {
            var jsonDataSaveBuildings = PlayerPrefs.GetString("jsonDataSaveBuildings");
            var jsonDataSaveUser = PlayerPrefs.GetString("jsonDataSaveUser");
            var buildingsRoot = GameObject.FindGameObjectWithTag("Buildings");
            var buildings = new Buildings();


            JsonUtility.FromJsonOverwrite(jsonDataSaveUser, user); // Load Data User
            JsonUtility.FromJsonOverwrite(jsonDataSaveBuildings, buildings); // Load Data Buildings

            for (int i = 0; i < buildings.buildings.Count; i++)
            {
                var building = new GameObject();
                switch (buildings.buildings[i].type)
                {
                    case "houses":
                        building = Instantiate(Resources.Load<GameObject>("Buildings/HOUSES"), buildings.buildings[i].position, Quaternion.identity, buildingsRoot.transform);
                        building.GetComponent<Houses>().LoadNow = true;
                        break;
                    case "factory":
                        building = Instantiate(Resources.Load<GameObject>("Buildings/FACTORY"), buildings.buildings[i].position, Quaternion.identity, buildingsRoot.transform);
                        building.GetComponent<Factory>().LoadNow = true;
                        break;
                    case "mall":
                        building = Instantiate(Resources.Load<GameObject>("Buildings/MALL"), buildings.buildings[i].position, Quaternion.identity, buildingsRoot.transform);
                        building.GetComponent<Mall>().LoadNow = true;
                        break;
                    case "park":
                        building = Instantiate(Resources.Load<GameObject>("Buildings/PARK"), buildings.buildings[i].position, Quaternion.identity, buildingsRoot.transform);
                        building.GetComponent<Park>().LoadNow = true;
                        break;
                    case "farm":
                        building = Instantiate(Resources.Load<GameObject>("Buildings/FARM"), buildings.buildings[i].position, Quaternion.identity, buildingsRoot.transform);
                        building.GetComponent<Farm>().LoadNow = true;
                        break;
                }
            }
        }
    }

    [Serializable]
    public class Building
    {
        public string type;
        public Vector3 position;

        public Building(string type, Vector3 position)
        {
            this.type = type;
            this.position = position;
        }
    }

    [Serializable]
    public class Buildings
    {
        public List<Building> buildings;
    }
}
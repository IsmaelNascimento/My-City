using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RTS_Cam;

namespace PushStart.MyCity
{
    public class GameMaster : MonoBehaviour
    {
        #region Variables
        private static GameMaster m_Instance;
        public static GameMaster Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType(typeof(GameMaster)) as GameMaster;
                }

                return m_Instance;
            }
        }

        private bool m_Pause;
        public bool Pause
        {
            get
            {
                return m_Pause;
            }
            set
            {
                m_Pause = value;

                if (m_Pause)
                {
                    Camera.main.GetComponent<RTS_Camera>().enabled = false;
                }
                else
                {
                    Camera.main.GetComponent<RTS_Camera>().enabled = true;
                }
            }
        }

        [Header("Prefab Buildings")]
        [SerializeField] private Transform m_Houses;
        [SerializeField] private Transform m_Factory;
        [SerializeField] private Transform m_Mall;
        [SerializeField] private Transform m_Park;
        [SerializeField] private Transform m_Farm;
        [Space(10)]

        [Header("Image Blocks Buildings")]
        [SerializeField] private GameObject m_ImageBlockHouses;
        [SerializeField] private GameObject m_ImageBlockFactory;
        [SerializeField] private GameObject m_ImageBlockMall;
        [SerializeField] private GameObject m_ImageBlockPark;
        [SerializeField] private GameObject m_ImageBlockFarm;
        [Space(10)]

        [Header("Buttons Buildings")]
        [SerializeField] private EventTrigger m_ButtonBlockHouses;
        [SerializeField] private EventTrigger m_ButtonBlockFactory;
        [SerializeField] private EventTrigger m_ButtonBlockMall;
        [SerializeField] private EventTrigger m_ButtonBlockPark;
        [SerializeField] private EventTrigger m_ButtonBlockFarm;
        [Space(10)]

        [Header("Popup Notification")]
        [SerializeField] private GameObject m_PopupNotifications;
        [SerializeField] private Text m_TextTitle;
        [SerializeField] private Text m_TextDescription;
        [Space(10)]

        [Header("UI User")]
        [SerializeField] private Text m_Nickname;
        [SerializeField] private Text m_CountMoneyUser;
        public RectTransform imageMoney;
        [SerializeField] private InputField m_NewNickname;
        [SerializeField] private GameObject m_PanelNewNickname;
        [Space(10)]

        [Header("Others")]
        [SerializeField] private Transform m_Buildings;
        [Space(10)]

        [Header("Sounds")]
        public AudioSource finishBuilding;
        public AudioSource moneyGenerated;
        public AudioSource newBuilding;
        public AudioSource notDoBuilding;


        [HideInInspector] public User userDetails;
        private Request requestData;
        #endregion

        private void Start()
        {
            userDetails = new User();
            requestData = new Request();

            JsonUtility.FromJsonOverwrite(requestData.LoadDataUser(), userDetails);
            

            if (PlayerPrefs.GetInt("newNickname") != 1)
            {
                StartCoroutine(ValidationCreateBuilding_Coroutine());
                m_PanelNewNickname.SetActive(true);
            }
            else
            {
                StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => DataSave.LoadAll(userDetails)));
                m_PanelNewNickname.SetActive(false);
            }
        }

        #region Sounds
        public void FxFinishBuilding()
        {
            finishBuilding.Play();
        }

        public void FxMoneyGenerated()
        {
            moneyGenerated.Play();
        }

        public void FxNewBuilding()
        {
            newBuilding.Play();
        }

        public void FxNotDoBuilding()
        {
            notDoBuilding.Play();
        }
        #endregion

        #region Methods UI
        public void OnButtonConfirmNameClicked(GameObject buttonConfirm)
        {
            userDetails.nickname = m_NewNickname.text;
            UpdateDataVisualUser();
            buttonConfirm.SetActive(false);
        }

        public void OnButtonConfirmNameAudioClicked(AudioSource fxClick)
        {
            PlayerPrefs.SetInt("newNickname", 1);
            fxClick.Play();
        }

        public void PointerDown(string building)
        {
            PopupInformationBuilding(building);
        }

        public void PointerUp(string building)
        {
            m_PopupNotifications.GetComponent<Animator>().Play("Popup Effect Exit");
        }

        public void BeginDrag(string building)
        {
            Camera.main.GetComponent<RTS_Camera>().enabled = false;
            m_PopupNotifications.GetComponent<Animator>().Play("Popup Effect Exit");

            switch (building.ToLower())
            {
                case "houses":
                    StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => CreateNewHouses(beginDrag: true)));
                    break;
                case "factory":
                    StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => CreateNewFactory(beginDrag: true)));
                    break;
                case "mall":
                    StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => CreateNewMall(beginDrag: true)));
                    break;
                case "park":
                    StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => CreateNewPark(beginDrag: true)));
                    break;
                case "farm":
                    StartCoroutine(ValidationCreateBuilding_Coroutine(checkedValidation: () => CreateNewFarm(beginDrag: true)));
                    break;
            }
        }

        public void EndDrag(string building)
        {
            Camera.main.GetComponent<RTS_Camera>().enabled = true;

            switch (building.ToLower())
            {
                case "houses":
                    CreateNewHouses(endDrag: true);
                    break;
                case "factory":
                    CreateNewFactory(endDrag: true);
                    break;
                case "mall":
                    CreateNewMall(endDrag: true);
                    break;
                case "park":
                    CreateNewPark(endDrag: true);
                    break;
                case "farm":
                    CreateNewFarm(endDrag: true);
                    break;
            }
        }

        private void UpdateDataVisualUser()
        {
            m_Nickname.text = userDetails.nickname;
            m_CountMoneyUser.text = userDetails.money.ToString();

            DataSave.SaveDataUser();
        }
        #endregion

        #region Methods Create Buildings
        private void CreateNewHouses(bool beginDrag = false, bool endDrag = false)
        {
            var newHouses = Instantiate(m_Houses, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)), Quaternion.identity, m_Buildings);
            newHouses.position = new Vector3(newHouses.position.x, newHouses.position.y, -9);

            if (beginDrag)
            {
                newHouses.GetComponent<Houses>().CanMove = true;
            }
            else if (endDrag)
            {
                foreach (Houses house in FindObjectsOfType(typeof(Houses)))
                {
                    if (house.CanMove)
                    {
                        Destroy(house.gameObject);
                    }
                }

                newHouses.GetComponent<Houses>().CanMove = false;
                newHouses.GetComponent<Houses>().BuildNow = true;

                StartCoroutine(CreateNewBuilding_Coroutine("houses", newHouses));
            }
        }

        private void CreateNewFactory(bool beginDrag = false, bool endDrag = false)
        {
            var newFactory = Instantiate(m_Factory, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)), Quaternion.identity, m_Buildings);
            newFactory.position = new Vector3(newFactory.position.x, newFactory.position.y, -9);

            if (beginDrag)
            {
                newFactory.GetComponent<Factory>().CanMove = true;
            }
            else if (endDrag)
            {
                foreach (Factory factory in FindObjectsOfType(typeof(Factory)))
                {
                    if (factory.CanMove)
                    {
                        Destroy(factory.gameObject);
                    }
                }

                newFactory.GetComponent<Factory>().CanMove = false;
                newFactory.GetComponent<Factory>().BuildNow = true;

                StartCoroutine(CreateNewBuilding_Coroutine("factory", newFactory));
            }
        }

        private void CreateNewMall(bool beginDrag = false, bool endDrag = false)
        {
            var newMall = Instantiate(m_Mall, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)), Quaternion.identity, m_Buildings);
            newMall.position = new Vector3(newMall.position.x, newMall.position.y, -9);

            if (beginDrag)
            {
                newMall.GetComponent<Mall>().CanMove = true;
            }
            else if (endDrag)
            {
                foreach (Mall mall in FindObjectsOfType(typeof(Mall)))
                {
                    if (mall.CanMove)
                    {
                        Destroy(mall.gameObject);
                    }
                }

                newMall.GetComponent<Mall>().CanMove = false;
                newMall.GetComponent<Mall>().BuildNow = true;

                StartCoroutine(CreateNewBuilding_Coroutine("mall", newMall));
            }
        }

        private void CreateNewPark(bool beginDrag = false, bool endDrag = false)
        {
            var newPark = Instantiate(m_Park, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)), Quaternion.identity, m_Buildings);
            newPark.position = new Vector3(newPark.position.x, newPark.position.y, -9);

            if (beginDrag)
            {
                newPark.GetComponent<Park>().CanMove = true;
            }
            else if (endDrag)
            {
                foreach (Park park in FindObjectsOfType(typeof(Park)))
                {
                    if (park.CanMove)
                    {
                        Destroy(park.gameObject);
                    }
                }

                newPark.GetComponent<Park>().CanMove = false;
                newPark.GetComponent<Park>().BuildNow = true;

                StartCoroutine(CreateNewBuilding_Coroutine("park", newPark));
            }
        }

        private void CreateNewFarm(bool beginDrag = false, bool endDrag = false)
        {
            var newFarm = Instantiate(m_Farm, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)), Quaternion.identity, m_Buildings);
            newFarm.position = new Vector3(newFarm.position.x, newFarm.position.y, -9);

            if (beginDrag)
            {
                newFarm.GetComponent<Farm>().CanMove = true;
            }
            else if (endDrag)
            {
                foreach (Farm farm in FindObjectsOfType(typeof(Farm)))
                {
                    if (farm.CanMove)
                    {
                        Destroy(farm.gameObject);
                    }
                }

                newFarm.GetComponent<Farm>().CanMove = false;
                newFarm.GetComponent<Farm>().BuildNow = true;

                StartCoroutine(CreateNewBuilding_Coroutine("farm", newFarm));
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator CreateNewBuilding_Coroutine(string typeBuilding, Transform building)
        {
            yield return new WaitForSeconds(.1f);

            if(building == null)
            {
                yield break;
            }

            switch (typeBuilding.ToLower())
            {
                case "houses":
                    building.GetComponent<Houses>().BuildNow = false;
                    building.GetComponent<Houses>().ActiveTimeBuilding();
                    RemoveMoneyUser(building.GetComponent<Houses>().valueBuildidng);
                    FxNewBuilding();
                    break;
                case "factory":
                    building.GetComponent<Factory>().BuildNow = false;
                    building.GetComponent<Factory>().ActiveTimeBuilding();
                    RemoveMoneyUser(building.GetComponent<Factory>().valueBuildidng);
                    FxNewBuilding();
                    break;
                case "mall":
                    building.GetComponent<Mall>().BuildNow = false;
                    building.GetComponent<Mall>().ActiveTimeBuilding();
                    RemoveMoneyUser(building.GetComponent<Mall>().valueBuildidng);
                    FxNewBuilding();
                    break;
                case "park":
                    building.GetComponent<Park>().BuildNow = false;
                    building.GetComponent<Park>().ActiveTimeBuilding();
                    RemoveMoneyUser(building.GetComponent<Park>().valueBuildidng);
                    FxNewBuilding();
                    break;
                case "farm":
                    building.GetComponent<Farm>().BuildNow = false;
                    building.GetComponent<Farm>().ActiveTimeBuilding();
                    RemoveMoneyUser(building.GetComponent<Farm>().valueBuildidng);
                    FxNewBuilding();
                    break;
                default:
                    Debug.LogError("building not found");
                    break;
            }

            DataSave.SaveAll();
        }

        public IEnumerator ValidationCreateBuilding_Coroutine(IEnumerator loadData = null, Action checkedValidation = null)
        {
            if(loadData != null)
            {
                yield return loadData;

                JsonUtility.FromJsonOverwrite(requestData.LoadDataUser(), userDetails);
                UpdateDataVisualUser();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            if(userDetails.money < m_Houses.GetComponent<Houses>().valueBuildidng)
            {
                m_ButtonBlockHouses.GetComponent<EventTrigger>().enabled = false;
                m_ImageBlockHouses.SetActive(true);
            }
            else
            {
                m_ButtonBlockHouses.GetComponent<EventTrigger>().enabled = true;
                m_ImageBlockHouses.SetActive(false);
            }

            if (userDetails.money < m_Factory.GetComponent<Factory>().valueBuildidng)
            {
                m_ButtonBlockFactory.GetComponent<EventTrigger>().enabled = false;
                m_ImageBlockFactory.SetActive(true);
            }
            else
            {
                m_ButtonBlockFactory.GetComponent<EventTrigger>().enabled = true;
                m_ImageBlockFactory.SetActive(false);
            }

            if (userDetails.money < m_Mall.GetComponent<Mall>().valueBuildidng)
            {
                m_ButtonBlockMall.GetComponent<EventTrigger>().enabled = false;
                m_ImageBlockMall.SetActive(true);
            }
            else
            {
                m_ButtonBlockMall.GetComponent<EventTrigger>().enabled = true;
                m_ImageBlockMall.SetActive(false);
            }

            if (userDetails.money < m_Park.GetComponent<Park>().valueBuildidng)
            {
                m_ButtonBlockPark.GetComponent<EventTrigger>().enabled = false;
                m_ImageBlockPark.SetActive(true);
            }
            else
            {
                m_ButtonBlockPark.GetComponent<EventTrigger>().enabled = true;
                m_ImageBlockPark.SetActive(false);
            }

            if (userDetails.money < m_Farm.GetComponent<Farm>().valueBuildidng)
            {
                m_ButtonBlockFarm.GetComponent<EventTrigger>().enabled = false;
                m_ImageBlockFarm.SetActive(true);
            }
            else
            {
                m_ButtonBlockFarm.GetComponent<EventTrigger>().enabled = true;
                m_ImageBlockFarm.SetActive(false);
            }

            if (checkedValidation != null)
            {
                checkedValidation();
                UpdateDataVisualUser();
            }
        }
        #endregion

        private void PopupInformationBuilding(string building)
        {
            m_PopupNotifications.SetActive(true);
            m_PopupNotifications.GetComponent<Animator>().Play("Popup Effect");

            switch (building.ToLower())
            {
                case "houses":
                    m_TextTitle.text = "HOUSES";
                    m_TextDescription.text = string.Format("valueBuildidng:\n {0}\n timeBuildidng:\n {1}\n timeGenerateMoney:\n {2}\n valueGeneratedMoney:\n {3}",
                        m_Houses.GetComponent<Houses>().valueBuildidng, m_Houses.GetComponent<Houses>().timeBuildidng, m_Houses.GetComponent<Houses>().timeGenerateMoney, m_Houses.GetComponent<Houses>().valueGeneratedMoney);
                    break;
                case "factory":
                    m_TextTitle.text = "FACTORY";
                    m_TextDescription.text = string.Format("valueBuildidng:\n {0}\n timeBuildidng:\n {1}\n timeGenerateMoney:\n {2}\n valueGeneratedMoney:\n {3}",
                        m_Factory.GetComponent<Factory>().valueBuildidng, m_Factory.GetComponent<Factory>().timeBuildidng, m_Factory.GetComponent<Factory>().timeGenerateMoney, m_Factory.GetComponent<Factory>().valueGeneratedMoney);
                    break;
                case "mall":
                    m_TextTitle.text = "MALL";
                    m_TextDescription.text = string.Format("valueBuildidng:\n {0}\n timeBuildidng:\n {1}\n timeGenerateMoney:\n {2}\n valueGeneratedMoney:\n {3}",
                        m_Mall.GetComponent<Mall>().valueBuildidng, m_Mall.GetComponent<Mall>().timeBuildidng, m_Mall.GetComponent<Mall>().timeGenerateMoney, m_Mall.GetComponent<Mall>().valueGeneratedMoney);
                    break;
                case "park":
                    m_TextTitle.text = "PARK";
                    m_TextDescription.text = string.Format("valueBuildidng:\n {0}\n timeBuildidng:\n {1}\n timeGenerateMoney:\n {2}\n valueGeneratedMoney:\n {3}",
                        m_Park.GetComponent<Park>().valueBuildidng, m_Park.GetComponent<Park>().timeBuildidng, m_Park.GetComponent<Park>().timeGenerateMoney, m_Park.GetComponent<Park>().valueGeneratedMoney);
                    break;
                case "farm":
                    m_TextTitle.text = "FARM";
                    m_TextDescription.text = string.Format("valueBuildidng:\n {0}\n timeBuildidng:\n {1}\n timeGenerateMoney:\n {2}\n valueGeneratedMoney:\n {3}",
                        m_Farm.GetComponent<Farm>().valueBuildidng, m_Farm.GetComponent<Farm>().timeBuildidng, m_Farm.GetComponent<Farm>().timeGenerateMoney, m_Farm.GetComponent<Farm>().valueGeneratedMoney);
                    break;
            }
        }

        public void AddMoneyUser(int money = 0)
        {
            userDetails.money += money;
            UpdateDataVisualUser();
            StartCoroutine(ValidationCreateBuilding_Coroutine());
        }

        public void RemoveMoneyUser(int money = 0)
        {
            userDetails.money -= money;
            UpdateDataVisualUser();
            StartCoroutine(ValidationCreateBuilding_Coroutine());
        }
    }
}
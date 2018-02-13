using System.Collections;
using UnityEngine;

namespace PushStart.MyCity
{
    public abstract class Building<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables
        private bool m_CanMove;
        private bool m_BuildNow;
        private bool m_BuildInvalid;
        private bool m_LoadNow;

        // Propertis
        public bool CanMove
        {
            get
            {
                return m_CanMove;
            }
            set
            {
                m_CanMove = value;
            }
        }
        public bool BuildNow
        {
            get
            {
                return m_BuildNow;
            }
            set
            {
                m_BuildNow = value;
            }
        }
        public bool LoadNow
        {
            get
            {
                return m_LoadNow;
            }
            set
            {
                m_LoadNow = value;
            }
        }

        [Header("Values Building")]
        public int valueBuildidng;
        public float timeBuildidng; // Seconds
        public float timeGenerateMoney; // Seconds
        public int valueGeneratedMoney;

        [Space(10)]
        [Header("Others")]
        [SerializeField] private GameObject m_TimeBuildingVisual;
        [SerializeField] private GameObject m_MoneyGenerator;
        [SerializeField] private GameObject m_MoneyGenerated;
        #endregion

        #region Methods MonoBehaviour
        void Start()
        {
            if (LoadNow)
            {
                ActiveTimeGenerateMoney();
            }

            if (!CanMove)
            {
                //print("Você quer mesmo essa construção neste local ? ");
                // Aparecer icones de confirmação
            }
        }

        void Update()
        {
            if (CanMove)
            {
                FollowMouse();
            }
        }
        
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (CanMove)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                if (collision.GetComponent<Houses>() || collision.GetComponent<Factory>() || collision.GetComponent<Mall>() || collision.GetComponent<Park>() || collision.GetComponent<Farm>())
                {
                    m_BuildInvalid = true;
                }
                else
                {
                    m_BuildInvalid = false;
                }

                if (m_BuildInvalid && BuildNow)
                {
                    Instantiate(Resources.Load<GameObject>("ParticleDestroyBuilding"), transform.position, Quaternion.identity);
                    GameMaster.Instance.FxNotDoBuilding();
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        #endregion

        private void FollowMouse()
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, -9);
        }

        public void ActiveTimeBuilding()
        {
            StartCoroutine(ActiveTimeBuilding_Coroutine());
        }

        #region Coroutinas
        private IEnumerator ActiveTimeBuilding_Coroutine()
        {
            m_TimeBuildingVisual.SetActive(true);

            yield return new WaitForSeconds(timeBuildidng);

            GameMaster.Instance.FxFinishBuilding();
            Instantiate(Resources.Load<GameObject>("ParticleBuilding"), transform.position, Quaternion.identity, transform);
            m_TimeBuildingVisual.SetActive(false);
            ActiveTimeGenerateMoney();
        }
        
        private IEnumerator ActiveTimeGenerateMoney_Coroutine()
        {
            m_MoneyGenerator.SetActive(true);
            yield return new WaitForSeconds(timeGenerateMoney);
            GetMoneyGenerated();
        }
        #endregion

        private void ActiveTimeGenerateMoney()
        {
            StartCoroutine(ActiveTimeGenerateMoney_Coroutine());
        }

        public void GetMoneyGenerated()
        {
            if (GetComponent<Houses>())
            {
                GameMaster.Instance.AddMoneyUser(GetComponent<Houses>().valueGeneratedMoney);
            }
            else if (GetComponent<Factory>())
            {
                GameMaster.Instance.AddMoneyUser(GetComponent<Factory>().valueGeneratedMoney);
            }
            else if (GetComponent<Mall>())
            {
                GameMaster.Instance.AddMoneyUser(GetComponent<Mall>().valueGeneratedMoney);
            }
            else if (GetComponent<Park>())
            {
                GameMaster.Instance.AddMoneyUser(GetComponent<Park>().valueGeneratedMoney);
            }
            else if (GetComponent<Farm>())
            {
                GameMaster.Instance.AddMoneyUser(GetComponent<Farm>().valueGeneratedMoney);
            }

            m_MoneyGenerated.SetActive(true);

            ActiveTimeGenerateMoney();
        }
    }
}
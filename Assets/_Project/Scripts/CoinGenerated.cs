using UnityEngine;
using DG.Tweening;
using RTS_Cam;

namespace PushStart.MyCity
{
    public class CoinGenerated : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.localScale = Vector3.one;
            Camera.main.GetComponent<RTS_Camera>().enabled = false;

            var positionInitial = transform.position;
            var positionCoinUI = Camera.main.ScreenToWorldPoint(GameMaster.Instance.imageMoney.position);

            transform.DOMove(positionCoinUI, 1f);
            transform.DOScale(0, 2f).OnComplete(() => {
                GameMaster.Instance.FxMoneyGenerated();
                transform.position = positionInitial;
                gameObject.SetActive(false);
                Camera.main.GetComponent<RTS_Camera>().enabled = true;
            });
        }
    }
}
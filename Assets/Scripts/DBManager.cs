using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System.Linq;
using UnityEngine.UI;

public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    public GameObject rankingRowDataOdd;
    public GameObject rankingRowDataEven;
    public GameObject rankingBox;
    public Button confirmNewNameButton;
    public GameObject cannotUseThatNameText;

    private GameObject rowOdd;
    private GameObject rowEven;

    string dataStoreClassName = "Score";
    string playerColumnName = "player";
    string scoreColumnName = "score";
    string datetimeColumnName = "updateDate";

    /* ハイスコアをデータストアに保存 */
    public void SaveHighScore(string player, int score)
    {
        // データストアにデータを保存
        NCMBObject store = new NCMBObject(dataStoreClassName);
        store["player"] = player;
        store["score"] = score;
        store.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("保存失敗: " + e.ErrorMessage);
            }
            else
            {
                Debug.Log("保存成功");
            }
        });
    }

    /* 既に存在しているプレイヤー名を弾く */
    public void PlayerNameIsOK(string player)
    {
        //データストアの"Score"クラスから検索
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(dataStoreClassName);
        query.WhereEqualTo(playerColumnName, player);
        query.CountAsync((int count, NCMBException e) =>
        {
           if (count < 1)
            {
                confirmNewNameButton.interactable = true;
                cannotUseThatNameText.SetActive(false);

            }
            else
            {
                confirmNewNameButton.interactable = false;
                cannotUseThatNameText.SetActive(true);
            }
        });
    }

    /* TOP30グローバルランク取得 */
    public void LoadTop30Score()
    {
        //データストアの"Score"クラスから検索
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(dataStoreClassName);

        //"Score"クラスのkeyカラムを降順に並び替え
        query.OrderByDescending(scoreColumnName);

        //実際に取得する
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得失敗: " + e.ErrorMessage);
            }
            else
            {
                List<NCMBObject> newObjList = new List<NCMBObject>();
                List<string> playerNameList = new List<string>();
                for (int i = 0; i < objList.Count; i++)
                {
                    if (!playerNameList.Contains(objList[i][playerColumnName]))
                    {
                        playerNameList.Add((string)objList[i][playerColumnName]);
                        newObjList.Add(objList[i]);
                    }

                }

                for (int i = 0; i < newObjList.Count; i++)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        rowEven = Instantiate(rankingRowDataEven);
                        rowEven.transform.SetParent(rankingBox.transform, false);
                        rowEven.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                        rowEven.transform.GetChild(1).GetComponent<Text>().text = (string)newObjList[i][playerColumnName];
                        rowEven.transform.GetChild(2).GetComponent<Text>().text = (string)newObjList[i][scoreColumnName].ToString();
                        rowEven.transform.GetChild(3).GetComponent<Text>().text = newObjList[i].UpdateDate.ToString();
                    }
                    else
                    {
                        rowOdd = Instantiate(rankingRowDataOdd);
                        rowOdd.transform.SetParent(rankingBox.transform, false);
                        rowOdd.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                        rowOdd.transform.GetChild(1).GetComponent<Text>().text = (string)newObjList[i][playerColumnName];
                        rowOdd.transform.GetChild(2).GetComponent<Text>().text = (string)newObjList[i][scoreColumnName].ToString();
                        rowOdd.transform.GetChild(3).GetComponent<Text>().text = newObjList[i].UpdateDate.ToString();
                    }
                }
            }
        });
    }
}
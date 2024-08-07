using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SamegameDirector : MonoBehaviour
{
    //変数の設定
    [SerializeField] List<GameObject> prefubCats; //ネコのプレハブ
    [SerializeField] float gameTimer; //タイマー
    [SerializeField] int fielditemCountMax;//フィールドに表示されるアイテム(Cat)の総数
    [SerializeField] int deleteItemCount;//削除できるアイテム(Cat)の数

    //UIに関する変数
    [SerializeField] TextMeshProUGUI scoreText;//スコア
    [SerializeField] TextMeshProUGUI timerText;//ゲーム時間
    [SerializeField] GameObject finishPanel;//ゲーム終了時に出すパネル
    [SerializeField] GameObject retryButton;//ゲーム終了時に出すパネル
    //[SerializeField] GameObject CatPoof;//爆発エフェクト

    //bgm,seに関する変数
    [SerializeField] AudioClip BublesSE;//ネコを消した時の音
    [SerializeField] AudioClip BGM;//BGM

    //ゲーム内で使用するものの変数
    List<GameObject> cats;//アイテム(Cat)のリスト
    int gameScore;//スコアの変数
    AudioSource audioSource;//サウンド再生用

    //ライン
    List<GameObject> lineCats;
    LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        /*全アイテム
         アイテムのプレハブが生成されるたびにリストに追加*/
        cats = new List<GameObject>();

        //リザルト画面を最初は非表示に設定
        finishPanel.SetActive(false);

        //アイテム生成関数呼び出し
        SpawnItem(fielditemCountMax);

        //ライン初期化
        lineCats = new List<GameObject>();

        //連結したアイテム上のライン
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //タイマーを更新
        gameTimer -= Time.deltaTime;
        timerText.text = "" + (int)gameTimer;

        //ゲーム終了
        if (0 > gameTimer)
        {
            //リザルト画面を表示
            finishPanel.SetActive(true);

            //Update関数に入らないようにする
            enabled = false;

            //Updateから抜ける
            return;
        }

        //タッチ開始から指を離すまで
        if (Input.GetMouseButtonDown(0))//タッチ開始
        {
            GameObject hitCat = GetHitCat(false);

            //一度リストのネコをクリアする
            lineCats.Clear();

            //当たり判定があったらリストに追加
            if (hitCat)
            {
                lineCats.Add(hitCat);
                hitCat.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 0, 0), 0.5f).SetLoops(-1, LoopType.Yoyo);

                hitCat.transform.localScale = new Vector3(2, 2, 2);
            }
        }

        else if (Input.GetMouseButton(0))//押したままの状態
        {
            GameObject hitCat = GetHitCat(true);

            if (lineCats.Count > 0)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                lineCats[0].transform.position = worldPoint;
            }

            //当たり判定があったら
            if (hitCat && lineCats.Count > 0)
            {
                //距離を測る
                GameObject pre = lineCats[lineCats.Count - 1];
                float distance = Vector2.Distance(hitCat.transform.position, pre.transform.position);


                //画像で判定
                bool isSameColor = hitCat.GetComponent<SpriteRenderer>().sprite == pre.GetComponent<SpriteRenderer>().sprite;

                //色が同じ&&リストに追加していない
                if (isSameColor && !lineCats.Contains(hitCat))
                {
                    hitCat.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 0, 0), 0.5f).SetLoops(-1, LoopType.Yoyo);

                    //ラインに追加
                    lineCats.Add(hitCat);
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))//指を離したら
        {
            //削除されたアイテムをクリア
            cats.RemoveAll(item => item == null);

            //アイテムを削除
            DeleteItems(lineCats);

            //ラインをクリア
            lineRenderer.positionCount = 0;
            lineCats.Clear();
        }

        //ライン描画処理
        //if (lineCats.Count > 1)
        //{
        //    //頂点数
        //    lineRenderer.positionCount = lineCats.Count;

        //    //ラインのポジション
        //    for (int i = 0; i < lineCats.Count; i++)
        //    {
        //        lineRenderer.SetPosition(i, lineCats[i].transform.position + new Vector3(0, 0, 1));
        //    }
        //}
    }

    //アイテムを生成する関数
    void SpawnItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //色をランダムにする
            int rnd = Random.Range(0, prefubCats.Count);

            //場所をランダムにする
            float x = Random.Range(-7.0f, 0.0f);
            float y = Random.Range(-7.0f, 0.0f);

            //アイテム生成
            GameObject cat = Instantiate(prefubCats[rnd], new Vector3(x, 10 + y, 0), Quaternion.identity);

            //内部データ追加
            cats.Add(cat);
        }
    }

    //引数と同じ色のアイテムを削除する
    void DeleteItems(List<GameObject> checkitems)
    {
        //削除可能数に達していなかったら削除しない
        if (checkitems.Count < deleteItemCount) return;

        //削除してスコアに加算する
        List<GameObject> destroyItems = new List<GameObject>();

        foreach (var item in checkitems)
        {
            //被っていない削除したアイテムをカウント
            if (!destroyItems.Contains(item))
            {
                destroyItems.Add(item);
            }

            //削除する
            Destroy(item);

            //爆発エフェクト生成
            //CatPoof =Instantiate(GetComponent<GameObject>());

            //削除した分生成して、スコアを加算
            SpawnItem(destroyItems.Count / 5);
            gameScore += destroyItems.Count * 10;

            //スコアの表示を更新
            scoreText.text = "" + gameScore;
        }
    }

    //同じ色かつ距離が近いアイテムのリストを返す
    List<GameObject> GetSameItems(GameObject taget)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (var item in cats)
        {
            //アイテムが無い/同じアイテム/違う色/という場合はスキップする

            //繋げるアイテムとタッチしたアイテムが同じ物かの判定
            if (!item || taget == item) continue;

            //同じスプライトかどうかで判定する
            if (item.GetComponent<SpriteRenderer>().sprite != taget.GetComponent<SpriteRenderer>().sprite)
            {
                continue;
            }

            //条件をチェックし、全ての条件が合っている(同じ色で距離が近い)アイテムを追加
            ret.Add(item);
        }

        return ret;//retリストに返す
    }

    //引数と同じ色のアイテムを探す
    void CheckItems(GameObject target)
    {
        //このアイテムと同じ色を追加する
        List<GameObject> chekItems = new List<GameObject>();

        //自分を追加
        chekItems.Add(target);

        //チェック済のアイテムのインデックス
        int checkIndex = 0;

        //checkItemsの最大値までループ
        while (checkIndex < chekItems.Count)
        {
            //隣接する同じ色を取得
            List<GameObject> sameItems = GetSameItems(chekItems[checkIndex]);

            /*チェック済のインデックスを進める
            　※インデックスは番号みたいなもの
            　今回は番号を進めるという意味　　*/
            checkIndex++;

            //まだ追加されていないアイテムを追加する
            foreach (var item in sameItems)
            {
                if (chekItems.Contains(item)) continue;
                chekItems.Add(item);
            }
        }
        DeleteItems(chekItems);
    }

    //マウスポジションに当たり判定があったアイテムを返す
    GameObject GetHitCat(bool iskeepPushing)
    {
        GameObject ret = null;

        //スクリーン座標からワールド座標に変換
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero);
        //RaycastHit2D hit = Physics2D.CircleCast(worldPoint,1, Vector2.zero);

        RaycastHit2D raycastHit2D;

        if (iskeepPushing)
        {
            RaycastHit2D hit = Physics2D.CircleCast(worldPoint, 0.3f, Vector2.zero);
            raycastHit2D = hit;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            raycastHit2D = hit;
        }

        //当たり判定があったら
        if (raycastHit2D)
        {
            //当たり判定があったオブジェクトからSpriteRendererを取得
            SpriteRenderer spriteRenderer = raycastHit2D.collider.gameObject.GetComponent<SpriteRenderer>();

            //画像が設定されていたら、ネコのピースと判定する
            if (spriteRenderer)
            {
                ret = raycastHit2D.collider.gameObject;
            }
        }

        if (ret != null && ret.tag != "cat")
        {
            ret = null;
        }

        return ret;
    }

    //リトライボタンを押されたら
    public void OnClicRetry()
    {
        SceneManager.LoadScene("Samegame");
    }
}

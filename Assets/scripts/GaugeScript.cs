using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour
{
    //スキルが使えるようになるまでのゲージの変数
    [SerializeField] float gaugeLimit;

    /*経過時間保持の変数
    ※仮で制限時間式とする。根幹を作成する際にピースを消した数に対応させる。*/
    float seconds = 0;//後で[deretePace]にする

    // Start is called before the first frame update
   /* void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        //ゲージの更新関数を呼び出す
        updateGauge();
    }

    void updateGauge()
    {
        /*経過時間を取得
         ※後でピースを消した数を取得させる。*/
        seconds += Time.deltaTime;

        /*経過時間を、制限時間で割る
         タイマーのプログラムは後でチャレンジモードの制限時間で応用する。*/
        float timer = seconds / gaugeLimit;

        //確認用にコンソールに表示する
        Debug.Log(timer);
    }
}

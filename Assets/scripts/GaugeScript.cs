using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour
{
    //�X�L�����g����悤�ɂȂ�܂ł̃Q�[�W�̕ϐ�
    [SerializeField] float gaugeLimit;

    /*�o�ߎ��ԕێ��̕ϐ�
    �����Ő������Ԏ��Ƃ���B�������쐬����ۂɃs�[�X�����������ɑΉ�������B*/
    float seconds = 0;//���[deretePace]�ɂ���

    // Start is called before the first frame update
   /* void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        //�Q�[�W�̍X�V�֐����Ăяo��
        updateGauge();
    }

    void updateGauge()
    {
        /*�o�ߎ��Ԃ��擾
         ����Ńs�[�X�������������擾������B*/
        seconds += Time.deltaTime;

        /*�o�ߎ��Ԃ��A�������ԂŊ���
         �^�C�}�[�̃v���O�����͌�Ń`�������W���[�h�̐������Ԃŉ��p����B*/
        float timer = seconds / gaugeLimit;

        //�m�F�p�ɃR���\�[���ɕ\������
        Debug.Log(timer);
    }
}

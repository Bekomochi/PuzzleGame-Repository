using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SamegameDirector : MonoBehaviour
{
    //�ϐ��̐ݒ�
    [SerializeField] List<GameObject> prefubCats; //�l�R�̃v���n�u
    [SerializeField] float gameTimer; //�^�C�}�[
    [SerializeField] int fielditemCountMax;//�t�B�[���h�ɕ\�������A�C�e��(Cat)�̑���
    [SerializeField] int deleteItemCount;//�폜�ł���A�C�e��(Cat)�̐�

    //UI�Ɋւ���ϐ�
    [SerializeField] TextMeshProUGUI scoreText;//�X�R�A
    [SerializeField] TextMeshProUGUI timerText;//�Q�[������
    [SerializeField] GameObject finishPanel;//�Q�[���I�����ɏo���p�l��
    [SerializeField] GameObject retryButton;//�Q�[���I�����ɏo���p�l��
    //[SerializeField] GameObject CatPoof;//�����G�t�F�N�g

    //bgm,se�Ɋւ���ϐ�
    [SerializeField] AudioClip BublesSE;//�l�R�����������̉�
    [SerializeField] AudioClip BGM;//BGM

    //�Q�[�����Ŏg�p������̂̕ϐ�
    List<GameObject> cats;//�A�C�e��(Cat)�̃��X�g
    int gameScore;//�X�R�A�̕ϐ�
    AudioSource audioSource;//�T�E���h�Đ��p

    //���C��
    List<GameObject> lineCats;
    LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        /*�S�A�C�e��
         �A�C�e���̃v���n�u����������邽�тɃ��X�g�ɒǉ�*/
        cats = new List<GameObject>();

        //���U���g��ʂ��ŏ��͔�\���ɐݒ�
        finishPanel.SetActive(false);

        //�A�C�e�������֐��Ăяo��
        SpawnItem(fielditemCountMax);

        //���C��������
        lineCats = new List<GameObject>();

        //�A�������A�C�e����̃��C��
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //�^�C�}�[���X�V
        gameTimer -= Time.deltaTime;
        timerText.text = "" + (int)gameTimer;

        //�Q�[���I��
        if (0 > gameTimer)
        {
            //���U���g��ʂ�\��
            finishPanel.SetActive(true);

            //Update�֐��ɓ���Ȃ��悤�ɂ���
            enabled = false;

            //Update���甲����
            return;
        }

        //�^�b�`�J�n����w�𗣂��܂�
        if (Input.GetMouseButtonDown(0))//�^�b�`�J�n
        {
            GameObject hitCat = GetHitCat(false);

            //��x���X�g�̃l�R���N���A����
            lineCats.Clear();

            //�����蔻�肪�������烊�X�g�ɒǉ�
            if (hitCat)
            {
                lineCats.Add(hitCat);
                hitCat.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 0, 0), 0.5f).SetLoops(-1, LoopType.Yoyo);

                hitCat.transform.localScale = new Vector3(2, 2, 2);
            }
        }

        else if (Input.GetMouseButton(0))//�������܂܂̏��
        {
            GameObject hitCat = GetHitCat(true);

            if (lineCats.Count > 0)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                lineCats[0].transform.position = worldPoint;
            }

            //�����蔻�肪��������
            if (hitCat && lineCats.Count > 0)
            {
                //�����𑪂�
                GameObject pre = lineCats[lineCats.Count - 1];
                float distance = Vector2.Distance(hitCat.transform.position, pre.transform.position);


                //�摜�Ŕ���
                bool isSameColor = hitCat.GetComponent<SpriteRenderer>().sprite == pre.GetComponent<SpriteRenderer>().sprite;

                //�F������&&���X�g�ɒǉ����Ă��Ȃ�
                if (isSameColor && !lineCats.Contains(hitCat))
                {
                    hitCat.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 0, 0), 0.5f).SetLoops(-1, LoopType.Yoyo);

                    //���C���ɒǉ�
                    lineCats.Add(hitCat);
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))//�w�𗣂�����
        {
            //�폜���ꂽ�A�C�e�����N���A
            cats.RemoveAll(item => item == null);

            //�A�C�e�����폜
            DeleteItems(lineCats);

            //���C�����N���A
            lineRenderer.positionCount = 0;
            lineCats.Clear();
        }

        //���C���`�揈��
        //if (lineCats.Count > 1)
        //{
        //    //���_��
        //    lineRenderer.positionCount = lineCats.Count;

        //    //���C���̃|�W�V����
        //    for (int i = 0; i < lineCats.Count; i++)
        //    {
        //        lineRenderer.SetPosition(i, lineCats[i].transform.position + new Vector3(0, 0, 1));
        //    }
        //}
    }

    //�A�C�e���𐶐�����֐�
    void SpawnItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //�F�������_���ɂ���
            int rnd = Random.Range(0, prefubCats.Count);

            //�ꏊ�������_���ɂ���
            float x = Random.Range(-7.0f, 0.0f);
            float y = Random.Range(-7.0f, 0.0f);

            //�A�C�e������
            GameObject cat = Instantiate(prefubCats[rnd], new Vector3(x, 10 + y, 0), Quaternion.identity);

            //�����f�[�^�ǉ�
            cats.Add(cat);
        }
    }

    //�����Ɠ����F�̃A�C�e�����폜����
    void DeleteItems(List<GameObject> checkitems)
    {
        //�폜�\���ɒB���Ă��Ȃ�������폜���Ȃ�
        if (checkitems.Count < deleteItemCount) return;

        //�폜���ăX�R�A�ɉ��Z����
        List<GameObject> destroyItems = new List<GameObject>();

        foreach (var item in checkitems)
        {
            //����Ă��Ȃ��폜�����A�C�e�����J�E���g
            if (!destroyItems.Contains(item))
            {
                destroyItems.Add(item);
            }

            //�폜����
            Destroy(item);

            //�����G�t�F�N�g����
            //CatPoof =Instantiate(GetComponent<GameObject>());

            //�폜�������������āA�X�R�A�����Z
            SpawnItem(destroyItems.Count / 5);
            gameScore += destroyItems.Count * 10;

            //�X�R�A�̕\�����X�V
            scoreText.text = "" + gameScore;
        }
    }

    //�����F���������߂��A�C�e���̃��X�g��Ԃ�
    List<GameObject> GetSameItems(GameObject taget)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (var item in cats)
        {
            //�A�C�e��������/�����A�C�e��/�Ⴄ�F/�Ƃ����ꍇ�̓X�L�b�v����

            //�q����A�C�e���ƃ^�b�`�����A�C�e�������������̔���
            if (!item || taget == item) continue;

            //�����X�v���C�g���ǂ����Ŕ��肷��
            if (item.GetComponent<SpriteRenderer>().sprite != taget.GetComponent<SpriteRenderer>().sprite)
            {
                continue;
            }

            //�������`�F�b�N���A�S�Ă̏����������Ă���(�����F�ŋ������߂�)�A�C�e����ǉ�
            ret.Add(item);
        }

        return ret;//ret���X�g�ɕԂ�
    }

    //�����Ɠ����F�̃A�C�e����T��
    void CheckItems(GameObject target)
    {
        //���̃A�C�e���Ɠ����F��ǉ�����
        List<GameObject> chekItems = new List<GameObject>();

        //������ǉ�
        chekItems.Add(target);

        //�`�F�b�N�ς̃A�C�e���̃C���f�b�N�X
        int checkIndex = 0;

        //checkItems�̍ő�l�܂Ń��[�v
        while (checkIndex < chekItems.Count)
        {
            //�אڂ��铯���F���擾
            List<GameObject> sameItems = GetSameItems(chekItems[checkIndex]);

            /*�`�F�b�N�ς̃C���f�b�N�X��i�߂�
            �@���C���f�b�N�X�͔ԍ��݂����Ȃ���
            �@����͔ԍ���i�߂�Ƃ����Ӗ��@�@*/
            checkIndex++;

            //�܂��ǉ�����Ă��Ȃ��A�C�e����ǉ�����
            foreach (var item in sameItems)
            {
                if (chekItems.Contains(item)) continue;
                chekItems.Add(item);
            }
        }
        DeleteItems(chekItems);
    }

    //�}�E�X�|�W�V�����ɓ����蔻�肪�������A�C�e����Ԃ�
    GameObject GetHitCat(bool iskeepPushing)
    {
        GameObject ret = null;

        //�X�N���[�����W���烏�[���h���W�ɕϊ�
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

        //�����蔻�肪��������
        if (raycastHit2D)
        {
            //�����蔻�肪�������I�u�W�F�N�g����SpriteRenderer���擾
            SpriteRenderer spriteRenderer = raycastHit2D.collider.gameObject.GetComponent<SpriteRenderer>();

            //�摜���ݒ肳��Ă�����A�l�R�̃s�[�X�Ɣ��肷��
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

    //���g���C�{�^���������ꂽ��
    public void OnClicRetry()
    {
        SceneManager.LoadScene("Samegame");
    }
}

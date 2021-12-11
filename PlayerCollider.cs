using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    private Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //lynn添加 玩家碰到怪物受伤 2021/12/10
    private void OnCollisionEnter2D(Collision2D other)
    {
        //如果碰到怪物  暂时无敌1s
        if (other.gameObject.CompareTag("Monster"))
        {
            
            //other.gameObject.GetComponent<dropItems>().Drop();
            DamageByMonster(other.gameObject.GetComponent<Monster>().damage);
            UserInfo.Instance.isNB = true;
            Invoke("Reset_NB", 0.8f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Item_SpeedUp"))
        {
            //捡到加速道具，获得10s加速效果 //修改为固定值UserInfo.Instance.Speed = 16； 不然会一直叠加上去。2021/12/10
            UserInfo.Instance.Speed = 16; 
            //更新玩家移动速度
            gameObject.GetComponent<PlayerMove>().Update_Speed();
            Destroy(other.gameObject);
            Invoke("Reset_Speed", 10);
        }
        else if (other.CompareTag("Item_HealthUp")) {
            //捡到回血道具 血量回1
            if (UserInfo.Instance.health != 5) {
                GameObject.Find("UserInfo").GetComponent<UserInfo>().recover_health();
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Item_Shiled")) {
            //捡到无敌道具 玩家无敌6s
            UserInfo.Instance.isNB = true;
            Invoke("Reset_NB", 6);
            Destroy(other.gameObject);
        }
    }

    //重置玩家速度
    public void Reset_Speed()
    {
        UserInfo.Instance.Speed = 8;
        gameObject.GetComponent<PlayerMove>().Update_Speed();
    }

    //重置玩家无敌状态
    public void Reset_NB()
    {
        UserInfo.Instance.isNB = false;
    }

    //玩家收到伤害对于血量和护甲的减少反馈
    public void DamageByMonster(int damage)
    {
        //先判断是否处于无敌状态
        if (!UserInfo.Instance.isNB)
        {
            //m_animator.SetInteger("AnimState", 9);
            m_animator.SetTrigger("Hurt");
            
            //后续根据怪物的攻击力调正减少值
            if (UserInfo.Instance.armor >= damage)
            {
                UserInfo.Instance.armor -= damage;
                UserInfo.Instance.healthOrarmor_update();
            }
            else if (UserInfo.Instance.armor != 0)
            {
                int temp = (int)(damage - UserInfo.Instance.armor);
                UserInfo.Instance.armor = 0;
                if(UserInfo.Instance.health>temp)//玩家血量归0后，无法移动，进入死亡状态，弹出game over 然后重新开始
                    UserInfo.Instance.health -= temp;
                else
                {
                    UserInfo.Instance.health = 0;
                    //进入死亡状态
                    //开始
                    m_animator.SetTrigger("Death");
                }
                UserInfo.Instance.healthOrarmor_update();
            }
        }
    }
}
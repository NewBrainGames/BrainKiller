using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverbotAnimatorController : MonoBehaviour
{
    public float moveSpeed;//ǰ���ƶ��ٶ�
    public bool alerted;//׷��������Լ��ƶ�ʱ
    public bool death;//����ʱ
    public Animator _hovertAni;
    // Start is called before the first frame update
    void Start()
    {
        _hovertAni = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        setPara();
    }

    private void setPara()
    {
        if (_hovertAni == null)
            return;
        _hovertAni.SetFloat("MoveSpeed", moveSpeed);
        _hovertAni.SetBool("Alerted",alerted);
        _hovertAni.SetBool("Death", death);
    }

    public void TriggerAttack()//����
    {
        if (_hovertAni == null)
            return;
        _hovertAni.SetTrigger("Attack");
    }

    public void TriggerOnDamage()
    {
        if (_hovertAni == null)
            return;
        _hovertAni.SetTrigger("OnDamaged");
    }
}

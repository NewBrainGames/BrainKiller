using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverbotAnimatorController : MonoBehaviour
{
    public float moveSpeed;//前后移动速度
    public bool alerted;//追击玩家事以及移动时
    public bool death;//死亡时
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

    public void TriggerAttack()//开火
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

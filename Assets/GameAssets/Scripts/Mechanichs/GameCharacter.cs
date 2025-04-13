using System.Collections;
using System;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private float health; 
    private float attack; 
    private int movRange;
    private int attackRange;
    private int team; 

    private bool isWalking = false;
    private float speed = 2f;  
    private GridCell currentCell;
    private bool isInIdleAnim = false;
    public static event Action OnAnimEnd; 
    [SerializeField] private GameObject flagmarkerObject;
    private bool isCarryingFlag = false;   
    
    public void InitChar(float newHealth, float newAttack, int newMovRange, int newAttackRange, Sprite sprite, RuntimeAnimatorController animator, int newTeam, GridCell newCell)
    {
        Debug.Log(GetComponentInChildren<SpriteRenderer>());
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        GetComponentInChildren<Animator>().runtimeAnimatorController  = animator;
        
        SetHealth(newHealth);
        SetAttack(newAttack);
        SetMovRange(newMovRange);
        SetAttackRange(newAttackRange);
        SetCell(newCell);
        SetTeam(newTeam);
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public void SetAttack(float newAttack)
    {
        attack = newAttack; 
    }

    public void SetMovRange(int newMovRange)
    {
        movRange = newMovRange; 
    }

    public void SetAttackRange(int newAttackRange)
    {
        attackRange = newAttackRange; 
    }
    public void SetTeam(int newTeam)
    {
        team = newTeam; 
    }
    public int GetMovRange()
    {
        return movRange;
    }
    public void GetDamage(float damage)
    {
        GetComponentInChildren<Animator>().SetTrigger("TakeHitAnim"); 
        health -= damage;
        
    }

    public float GetHealth(){
        return health;
    }
    public int GetTeam()
    {
        return team; 
    }
    public void Dead()
    {
        SetTriggerAnim("DeathAnim");
        gameObject.SetActive(false); 
    }

    public void Attack(GameCharacter targetChar)
    {
        targetChar.GetDamage(attack);
        SetTriggerAnim("AttackAnim");
    }

    public void SetCell(GridCell cell){
        currentCell =  cell;
        Debug.Log(currentCell.gridIndex);
    }

    public void Walk(GridCell destCell)
    {
        currentCell = destCell;
        StartCoroutine(WalkToPoint(destCell.GetComponent<Transform>().position));
    }
    public void SetTriggerAnim(string triggerName)
    {
        isInIdleAnim = false; 
        GetComponentInChildren<Animator>().SetTrigger("AttackAnim"); 
    }
    public void AnimationEnd()
    {
            OnAnimEnd?.Invoke();
            isInIdleAnim = true;    
    }

    IEnumerator WalkToPoint(Vector3 destination)
    {
        isWalking = true;
        GetComponentInChildren<Animator>().SetBool("isWalking", true); 

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            this.GetComponent<Transform>().position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        this.GetComponent<Transform>().position = destination;
        GetComponentInChildren<Animator>().SetBool("isWalking", false); 
        isWalking = false;
    }

    private void SetActivateFlag(bool var)
    {
        flagmarkerObject.SetActive(var); 
    }


    public void SetIsCarryingFlag(bool var)
    {
        isCarryingFlag = var; 
        SetActivateFlag(var); 
    }

    public bool GetIsCarryingFlag()
    {
        return isCarryingFlag;
    }
}

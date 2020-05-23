/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Actor : MonoBehaviour {


  public Animator baseAnim;
  public Rigidbody body;
  public SpriteRenderer shadowSprite;

  public float speed = 2;

  protected Vector3 frontVector;
  public bool isGrounded;

  public SpriteRenderer baseSprite;

  public bool isAlive = true;

  public float maxLife = 100.0f;
  public float currentLife = 100.0f;

  public AttackData normalAttack;

  protected Coroutine knockdownRoutine;
  public bool isKnockedOut;

  public GameObject hitSparkPrefab;

  public LifeBar lifeBar;
  public Sprite actorThumbnail;

  public GameObject hitValuePrefab;

  public AudioClip deathClip;
  public AudioClip hitClip;
  public AudioSource audioSource;
  
  protected ActorCollider actorCollider;

  
  protected bool canFlinch = true;


  protected virtual void Start() {
    currentLife = maxLife;
    isAlive = true;
    baseAnim.SetBool("IsAlive", isAlive);

    actorCollider = GetComponent<ActorCollider> ();
    actorCollider.SetColliderStance (true);
  }

  public virtual void Update() {
    Vector3 shadowSpritePosition = shadowSprite.transform.position;
    shadowSpritePosition.y = 0;
    shadowSprite.transform.position = shadowSpritePosition;
  }

  protected  virtual void OnCollisionEnter(Collision collision) {
    if (collision.collider.name == "Floor") {
      isGrounded = true;
      baseAnim.SetBool("IsGrounded", isGrounded);
      DidLand();
    } 
  }

  protected virtual void OnCollisionExit(Collision collision) {
    if (collision.collider.name == "Floor") {
      isGrounded = false;
      baseAnim.SetBool("IsGrounded", isGrounded);
    }
  }

  protected virtual void DidLand()
  {
  }


  public void FlipSprite(bool isFacingLeft) {
    if (isFacingLeft) {
      frontVector = new Vector3(-1, 0, 0);
      transform.localScale = new Vector3(-1, 1, 1);
    } else {
      frontVector = new Vector3(1, 0, 0);
      transform.localScale = new Vector3(1, 1, 1);
    }
  }

  public virtual void Attack() {
    baseAnim.SetTrigger("Attack");
  }

  public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector) {
    Actor actor = collider.GetComponent<Actor>();
     if (actor != null && actor.CanBeHit() && collider.tag != gameObject.tag) {
      if (collider.attachedRigidbody != null) {
        HitActor(actor, hitPoint, hitVector);
      }
    } 
  }

  protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector) {
     actor.EvaluateAttackData(normalAttack, hitVector, hitPoint);
     PlaySFX (hitClip);
  }

  protected virtual void Die() {
    if (knockdownRoutine != null) {
      StopCoroutine(knockdownRoutine);
    }

    isAlive = false;
    baseAnim.SetBool("IsAlive", isAlive);
    StartCoroutine(DeathFlicker());
    PlaySFX (deathClip);
    actorCollider.SetColliderStance (false);
  }

  protected virtual void SetOpacity(float value) {
    Color color = baseSprite.color;
    color.a = value;
    baseSprite.color = color;
  }
  
  private IEnumerator DeathFlicker() {
    int i = 5;
    while (i > 0) {
      SetOpacity(0.5f);
      yield return new WaitForSeconds(0.1f);
      SetOpacity(1.0f);
      yield return new WaitForSeconds(0.1f);
       i--; 
    }
  }

   public virtual void TakeDamage(float value, Vector3 hitVector, bool knockdown = false) {
    FlipSprite(hitVector.x > 0);
    currentLife -= value;
    if (isAlive && currentLife <= 0) {
      Die();
    } else if (knockdown) {
      if (knockdownRoutine == null) {
        Vector3 pushbackVector = (hitVector + Vector3.up*0.75f).normalized;
        body.AddForce (pushbackVector* 250 );
        knockdownRoutine = StartCoroutine(KnockdownRoutine());
      }
    } else if (canFlinch) {
      baseAnim.SetTrigger("IsHurt");
    }

    lifeBar.EnableLifeBar(true); // 1
    lifeBar.SetProgress(currentLife / maxLife); // 2
    Color color = baseSprite.color; // 3
    if (currentLife < 0) { // 4
      color.a = 0.75f;
    }
    lifeBar.SetThumbnail(actorThumbnail, color); // 5

  }

  public virtual bool CanWalk() {
    return true;
  }

  public virtual void FaceTarget(Vector3 targetPoint) {
    FlipSprite(transform.position.x - targetPoint.x > 0);
  }

  public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint) {
    body.AddForce(data.force * hitVector);
    TakeDamage(data.attackDamage, hitVector, data.knockdown);
    ShowHitEffects(data.attackDamage, hitPoint);
  }

  public void DidGetUp() {
    isKnockedOut = false;
  }

  public bool CanBeHit() {
    return isAlive && !isKnockedOut;
  }

  protected virtual IEnumerator KnockdownRoutine() {
    isKnockedOut = true;
    baseAnim.SetTrigger("Knockdown");
    
    actorCollider.SetColliderStance (false);
    yield return new WaitForSeconds (1.0f);
    actorCollider.SetColliderStance (true);

    baseAnim.SetTrigger ("GetUp");
    knockdownRoutine = null;
  }

  protected void ShowHitEffects(float value, Vector3 position) {
    GameObject sparkObj = Instantiate(hitSparkPrefab);
    sparkObj.transform.position = position;

    GameObject obj = Instantiate(hitValuePrefab);
    obj.GetComponent<Text>().text = value.ToString();
    obj.GetComponent<DestroyTimer>().EnableTimer(1.0f);
    
    GameObject canvas =
    GameObject.FindGameObjectWithTag("WorldCanvas");
    obj.transform.SetParent(canvas.transform);
    obj.transform.localRotation = Quaternion.identity;
    obj.transform.localScale = Vector3.one;
    obj.transform.position = position;

  }

  public void PlaySFX(AudioClip clip) {
    audioSource.PlayOneShot (clip);
  }
}

[System.Serializable]
public class AttackData {
  public float attackDamage = 10;
  public float force = 50;
  public bool knockdown = false;
}


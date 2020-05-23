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
using UnityEngine;
public class Robot : Enemy {
  
  public RobotColor color;
  
  public SpriteRenderer smokeSprite;
  public SpriteRenderer beltSprite;

  public void SetColor(RobotColor color) {
    this.color = color;
    switch (color) {
      case RobotColor.Colorless:
        baseSprite.color = Color.white;
        smokeSprite.color = Color.white;
        beltSprite.color = Color.white;
        maxLife = 50.0f;
        normalAttack.attackDamage = 2;
        break;
      case RobotColor.Copper:
        baseSprite.color = new Color(1.0f, 0.75f, 0.62f);
        smokeSprite.color = new Color(0.38f, 0.63f, 1.0f);
        beltSprite.color = new Color(0.86f, 0.85f, 0.71f);
         maxLife = 100.0f;
        normalAttack.attackDamage = 4;
        break;
      case RobotColor.Silver:
        baseSprite.color = Color.white;
        smokeSprite.color = new Color(0.38f, 1.0f, 0.5f);
        beltSprite.color = new Color(0.5f, 0.5f, 0.5f);
        maxLife = 125.0f;
        normalAttack.attackDamage = 5;
        break;
      case RobotColor.Gold:
        baseSprite.color = new Color(0.91f, 0.7f, 0.0f);
        smokeSprite.color = new Color(0.42f, 0.15f, 0.10f);
        beltSprite.color = new Color(0.86f, 0.5f, 0.32f);
        maxLife = 150.0f;
        normalAttack.attackDamage = 6;
        break;
      case RobotColor.Random:
        baseSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
        smokeSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
        beltSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
        maxLife = Random.Range(100, 250);
        normalAttack.attackDamage = Random.Range(4, 10);
        break;
    }
    currentLife = maxLife;
  }

  [ContextMenu("Color: Copper")]
  void SetToCopper() {
    SetColor (RobotColor.Copper);
  }

  [ContextMenu("Color: Silver")]
  void SetToSilver() {
    SetColor (RobotColor.Silver);
  }

  [ContextMenu("Color: Gold")]
  void SetToGold() {
    SetColor (RobotColor.Gold);
  }

  [ContextMenu("Color: Random")]
  void SetToRandom() {
    SetColor (RobotColor.Random);
  }

  protected override IEnumerator KnockdownRoutine() {
    isKnockedOut = true;
    baseAnim.SetTrigger ("Knockdown");
    ai.enabled = false;

    actorCollider.SetColliderStance (false);
    yield return new WaitForSeconds (2.0f);
    actorCollider.SetColliderStance (true);

    baseAnim.SetTrigger ("GetUp");
    ai.enabled = true;
    knockdownRoutine = null;
  }
}

public enum RobotColor {
  Colorless = 0,
  Copper,
  Silver,
  Gold,
  Random 
}
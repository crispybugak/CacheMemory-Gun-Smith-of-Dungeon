using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{

   public IEnumerator TimeScaleCT(float duration, float timeScale)
   {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1f;
   }
   public void HitTimeScale()
   {
        StartCoroutine(TimeScaleCT(0.1f, 0.7f));
   }
   public void DeadTimeScale()
   {
        StartCoroutine(TimeScaleCT(3, 0.3f));
   }
}

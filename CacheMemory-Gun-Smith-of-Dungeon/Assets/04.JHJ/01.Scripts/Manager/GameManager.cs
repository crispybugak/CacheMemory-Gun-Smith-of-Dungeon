using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{

   public void HitTimeScaleCT()
    {
        StartCoroutine(TimeScaleCT(0.1f,0.7f));
    }
   public IEnumerator TimeScaleCT(float duration, float timeScale)
   {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1f;
   }
}

using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField]private GameObject doorN, doorE, doorS, doorW;

    public void Init(DungeonGen.RoomLinks links)
    {
        if(doorN) doorN.SetActive(links.N);
        if(doorE) doorE.SetActive(links.E);
        if(doorS) doorS.SetActive(links.S);
        if(doorW) doorW.SetActive(links.W);
    }
}

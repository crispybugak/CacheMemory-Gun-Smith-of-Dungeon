using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField]private GameObject doorN, doorE, doorS, doorW;
    [SerializeField]private GameObject cdoorN, cdoorE, cdoorS, cdoorW;
    
    private bool _linkN, _linkE, _linkS, _linkW;

    public void Init(DungeonGen.RoomLinks links)
    {
        if(doorN) doorN.SetActive(links.N);
        if(doorE) doorE.SetActive(links.E);
        if(doorS) doorS.SetActive(links.S);
        if(doorW) doorW.SetActive(links.W);
        
        ApplyLinkVisual(doorN, cdoorN,links.N);
        ApplyLinkVisual(doorE, cdoorE,links.E);
        ApplyLinkVisual(doorS, cdoorS,links.S);
        ApplyLinkVisual(doorW, cdoorW,links.W);
        
    }

    private void ApplyLinkVisual(GameObject openDoor, GameObject closeDoor, bool hasLink)
    {
        if(openDoor) openDoor.SetActive(hasLink);
        if(closeDoor) closeDoor.SetActive(!hasLink);
    }
    
    
}

using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum DoorDir{N,E,S,W }
    
    [SerializeField]private GameObject doorN, doorE, doorS, doorW;
    [SerializeField]private GameObject cdoorN, cdoorE, cdoorS, cdoorW;
    
    //방 기준 점
    [SerializeField] private Transform doorAnchorN;
    [SerializeField] private Transform doorAnchorE;
    [SerializeField] private Transform doorAnchorS;
    [SerializeField] private Transform doorAnchorW;

    
    private bool _linkN, _linkE, _linkS, _linkW;

    public void Init(DungeonGen.RoomLinks links)
    {
        _linkN = links.N;
        _linkE = links.E;
        _linkS = links.S;
        _linkW = links.W;

        ApplyLinkVisual(doorN, cdoorN,links.N);
        ApplyLinkVisual(doorE, cdoorE,links.E);
        ApplyLinkVisual(doorS, cdoorS,links.S);
        ApplyLinkVisual(doorW, cdoorW,links.W);
        
    }

    public void OpenDoor(DoorDir dir)
    {
        switch (dir)
        {
            case DoorDir.N:
                if(!_linkN) return;
                SwitchDoor(doorN,cdoorN,true);
                break;
            case DoorDir.E:
                if(!_linkE) return;
                SwitchDoor(doorE,cdoorE,true);
                break;
            case DoorDir.S:
                if(!_linkS) return;
                SwitchDoor(doorS,cdoorS,true);
                break;
            case DoorDir.W:
                if(!_linkW) return;
                SwitchDoor(doorW,cdoorW,true);
                break;
        }
    }

    public void CloseDoor(DoorDir dir)
    {
        switch (dir)
        {
            case DoorDir.N:
                if(!_linkN) return;
                SwitchDoor(doorN,cdoorN,false);
                break;
            case DoorDir.E:
                if(!_linkE) return;
                SwitchDoor(doorE,cdoorE,false);
                break;
            case DoorDir.S:
                if(!_linkS) return;
                SwitchDoor(doorS,cdoorS,false);
                break;
            case DoorDir.W:
                if(!_linkW) return;
                SwitchDoor(doorW,cdoorW,false);
                break;
        }
    }
    public void OpenAllLinkedDoors()
    {
        if (_linkN) SwitchDoor(doorN, cdoorN, true);
        if (_linkE) SwitchDoor(doorE, cdoorE, true);
        if (_linkS) SwitchDoor(doorS, cdoorS, true);
        if (_linkW) SwitchDoor(doorW, cdoorW, true);
    }

    public void CloseAllLinkedDoors()
    {
        if (_linkN) SwitchDoor(doorN, cdoorN, false);
        if (_linkE) SwitchDoor(doorE, cdoorE, false);
        if (_linkS) SwitchDoor(doorS, cdoorS, false);
        if (_linkW) SwitchDoor(doorW, cdoorW, false);
    }

    private void ApplyLinkVisual(GameObject openDoor, GameObject closeDoor, bool hasLink)
    {
        if(openDoor) openDoor.SetActive(hasLink);
        if(closeDoor) closeDoor.SetActive(!hasLink);
    }

    private void SwitchDoor(GameObject openDoor, GameObject closeDoor, bool open)
    {
        if(openDoor) openDoor.SetActive(open);
        if(closeDoor) closeDoor.SetActive(!open);
    }
    public Vector3 GetDoorWorldPos(DoorDir dir)
    {
        GameObject doorParent = null;
        switch (dir)
        {
            case DoorDir.N: doorParent = doorN; break;
            case DoorDir.E: doorParent = doorE; break;
            case DoorDir.S: doorParent = doorS; break;
            case DoorDir.W: doorParent = doorW; break;
        }

        if (doorParent == null)
            return transform.position;  
        
        Transform anchor;

        if (doorParent.transform.childCount > 0)
            anchor = doorParent.transform.GetChild(0); // 첫번째 자식
        else
            anchor = doorParent.transform;             // 자식 없으면 부모

        return anchor.position;
    }
    
}

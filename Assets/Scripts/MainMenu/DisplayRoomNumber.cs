using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayRoomNumber : MonoBehaviour
{
    [SerializeField] private string msgNoRoom, msgRoomCreated;
    // Start is called before the first frame update
    void Start()
    {
        NoRoomCreated();
    }

    public void NoRoomCreated()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = msgNoRoom;
    }
    public void RoomCreated(string roomNumber)
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = msgRoomCreated +  "\n" + roomNumber;
    }


}

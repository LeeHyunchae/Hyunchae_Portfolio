using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//크리에이트 플레이어, 플로어 각각의 컨트롤러가 하도록 넘기기
//루프문에서 랜덤액세스 3회이상이면 캐싱해주기
//스프라이트 래핑모드는 리핏으로 바꿔주기

public class InGameSceneController : MonoBehaviour
{
    private const string PLAYERPATH = "Prefabs/Player";
    private const string FLOORPATH = "Prefabs/Floor";
    private const int FLOORCOUNT = 10;

    private PlayerController playerCtrl;
    private FloorController floorCtrl;

    private void Awake()
    {
        CreatePlayer();
        CreateFloor();
    }

    private void Update()
    {
        playerCtrl.Update();
        floorCtrl.Update();

        if(floorCtrl.GetFrontFloor() != null)
        {
            playerCtrl.SetCurFloor(floorCtrl.GetFrontFloor());
        }

    }

    private void CreatePlayer()
    {
        playerCtrl = new PlayerController();
        playerCtrl.SetPlayer(Instantiate<GameObject>((GameObject)Resources.Load(PLAYERPATH), Vector2.zero, Quaternion.identity));
    }

    private void CreateFloor()
    {
        GameObject originFloor = (GameObject)Resources.Load(FLOORPATH);
        GameObject[] floors = new GameObject[FLOORCOUNT];


        for(int i = 0; i<FLOORCOUNT;i++)
        {
            floors[i] = Instantiate<GameObject>(originFloor, Vector2.zero, Quaternion.identity);
        }

        floorCtrl = new FloorController(floors);
    }
}

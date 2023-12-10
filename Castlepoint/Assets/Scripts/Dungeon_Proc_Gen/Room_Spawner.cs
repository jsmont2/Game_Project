using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
/*
* Room offset needs to be increments of 20
*/
public class Room_Spawner : MonoBehaviour, IDataPersistence
{
	public void LoadData(GameData data)
	{
		if (data.randSeed != 0)
		{
			randSeed = data.randSeed;
			UnityEngine.Random.seed = randSeed;
			cappingRooms = false;
			spawnedKey = false;
			roomsCreated = new List<Room>();
			tempRoom = new Room();
			SpawnDungeon(rooms, originRooms, roomsCreated);
			for (int i = 0; i < roomsCreated.Count; i++)
			{
				/*if (roomsCreated[i].box != null)
				{
					roomsCreated[i].box.transform.position = data.boxPos[i];
				}*/
				
				roomsCreated[i].roomActive = data.roomsActive[i];
				if (roomsCreated[i].roomActive) { roomsCreated[i].gameObject.SetActive(true); }
				else { roomsCreated[i].gameObject.SetActive(false); }
			}
		}

	}
	public void SaveData(GameData data)
	{
		data.randSeed = randSeed;
		data.roomNumbers = new List<int>();
		data.roomsActive = new List<bool>();
		for (int i = 0; i < roomsCreated.Count; i++)
		{
			data.roomNumbers.Add(i);
			data.roomsActive.Add(roomsCreated[i].roomActive);
			/*if (roomsCreated[i].box != null)
			{
				data.boxPos.Add(roomsCreated[i].box.transform.position);
			}*/
		}
	}
	[SerializeField]
	private List<Room> originRooms;
	[SerializeField]
	private List<Room> rooms;
	private List<Room> roomsCreated;
	[SerializeField]
	private List<Room> roomCaps;
	[SerializeField]
	private List<Room> bossRoomsList;
	[SerializeField]
	Room tempRoom;
	private bool bossRoomCreated;
	[SerializeField]
	private int sizeOfDungeon;
	private Vector3 upOffset = new Vector3(0, 60, 0);
	private Vector3 downOffset = new Vector3(0, -60, 0);
	private Vector3 leftOffset = new Vector3(-60, 0, 0);
	private Vector3 rightOffset = new Vector3(60, 0, 0);
	private Vector3 originRoomPos;
	private Vector3 checkRoomPos;
	private int currentSizeOfDungeon;

	private bool isRoomAbove;
	private bool isRoomBelow;
	private bool isRoomLeft;
	private bool isRoomRight;
	private bool needsConnectionBelow;
	private bool needsConnectionAbove;
	private bool needsConnectionRight;
	private bool needsConnectionLeft;
	private bool cappingRooms;
	private bool spawnedKey;
	public int randSeed;
	public int roomsMade;
	private void Start()
	{
		if (randSeed == 0)
		{
			randSeed = UnityEngine.Random.Range(0, 10000);
			UnityEngine.Random.seed = randSeed;
			cappingRooms = false;
			spawnedKey = false;
			roomsCreated = new List<Room>();
			tempRoom = new Room();
			SpawnDungeon(rooms, originRooms, roomsCreated);
		}
	}
	private void SpawnDungeon(List<Room> roomList, List<Room> originList, List<Room> dungeonRooms)
	{
		tempRoom = RandomizeRoom(originList);//Randomize which room is selected from origin list
		tempRoom.roomNum = roomsMade;
		roomsMade++;
		originRoomPos = new Vector3(0, -12.5f, 0);//This is the position of the origin room
		Room copy = Instantiate(tempRoom, originRoomPos, Quaternion.identity);//create the room in game
		copy.SetRoomPosition(originRoomPos);
		copy.SetOrigin();
		dungeonRooms.Add(copy);//add room to created list, this will be used to check position of created rooms
		tempRoom.roomActive = true;
		currentSizeOfDungeon++;
		FinishRoom(roomList, dungeonRooms, copy);//Begins recursion, spawns rooms to close all connections
		cappingRooms = true;
		FindMaxDistanceFromOrigin(dungeonRooms);
		CapRoomOpenings(roomList, dungeonRooms);
		SpawnKeyForBoss(dungeonRooms);
		SetAllRoomsActiveOff(dungeonRooms);
	}
	private void SpawnKeyForBoss(List<Room> dungeonRooms)
	{
		int rand = UnityEngine.Random.Range(1, dungeonRooms.Count);
		while (!spawnedKey)
		{
			for (int i = 0; i < dungeonRooms[rand].chestList.transform.childCount; i++)
			{
				if (dungeonRooms[rand] != bossRoomsList[i])
				{
					if (dungeonRooms[rand].chestList.transform.GetChild(i).gameObject.active)
					{
						dungeonRooms[rand].chestList.transform.GetChild(i).GetComponent<Chest>().SetHasKey();
						spawnedKey = true;
						dungeonRooms[rand].hasKey = true;
					}
					else { rand = UnityEngine.Random.Range(1, dungeonRooms.Count); }
				}
			}
		}

	}
	private void SetAllRoomsActiveOff(List<Room> dungeonRooms)
	{
		for (int i = 1; i < dungeonRooms.Count; i++)
		{
			dungeonRooms[i].roomActive = false;
			dungeonRooms[i].gameObject.SetActive(false);
		}
	}
	public void FindMaxDistanceFromOrigin(List<Room> dungeonRooms)
	{
		for (int i = 1; i < dungeonRooms.Count; i++)
		{
			dungeonRooms[i].FindDistanceFromOrigin();
		}
	}
	private Room RandomizeRoom(List<Room> roomList)
	{
		Room room = new Room();
		room = roomList[UnityEngine.Random.Range(0, roomList.Count)];//Randomize which room is selected from origin list
		return room;
	}
	public void MakeTheRoom(List<Room> roomList, List<Room> dungeonRooms, Room copy, Room temp, Room previousRoom, Room checkCons, Vector3 offset)
	{
		copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
		copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);//Set position of the room
		copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
		copy.roomNum = roomsMade;
		roomsMade++;
		dungeonRooms.Add(copy);
		currentSizeOfDungeon++;
		FinishRoom(roomList, dungeonRooms, copy);
	}
	private void SpawnRoom(List<Room> roomList, List<Room> dungeonRooms, Room previousRoom, Vector3 offset)
	{
		isRoomAbove = false;
		isRoomBelow = false;
		isRoomLeft = false;
		isRoomRight = false;
		needsConnectionBelow = false;
		needsConnectionAbove = false;
		needsConnectionRight = false;
		needsConnectionLeft = false;
		Room copy = new Room();
		Room temp = new Room();
		Room checkCons = new Room();
		checkCons.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
		CheckForRooms(dungeonRooms, checkCons);
		if (cappingRooms)
		{
			isRoomAbove = true;
			isRoomBelow = true;
			isRoomLeft = true;
			isRoomRight = true;
		}
		switch ((isRoomAbove, isRoomBelow, isRoomLeft, isRoomRight))//go through each case of a room existing around the room we want to make
		{
			case (true, false, false, false)://there is a room above
				if (needsConnectionAbove)//if the room above needs a connection made below it
				{
					while (temp == null || !temp.HasTopCon()) { temp = RandomizeRoom(roomList); }
					MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
				}
				break;
			case (false, true, false, false)://there is a room below
				if (needsConnectionBelow)
				{
					while (temp == null || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
					MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);

				}
				break;
			case (false, false, true, false)://there is a room to the left
				if (needsConnectionLeft)
				{
					while (temp == null || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
					MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
				}
				break;
			case (false, false, false, true)://there is a room to the right
				if (needsConnectionRight)
				{
					while (temp == null || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
					MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
				}
				break;
			case (true, true, false, false)://there is a room above and below
				switch ((needsConnectionAbove, needsConnectionBelow))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, false, true, false):// there is a room above and to the left
				switch ((needsConnectionAbove, needsConnectionLeft))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, false, false, true):// there is a room above and to the right
				switch ((needsConnectionAbove, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (false, true, true, false)://there is a room below and to the left
				switch ((needsConnectionBelow, needsConnectionLeft))
				{
					case (true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (false, true, false, true)://there is a room below and to the right
				switch ((needsConnectionBelow, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (false, false, true, true)://there is a room to the left and right
				switch ((needsConnectionLeft, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true):
						while (temp == null || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, true, true, false)://there is a room above,below and to the left
				switch ((needsConnectionAbove, needsConnectionBelow, needsConnectionLeft))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, true, false, true)://there is a room above,below and to the right
				switch ((needsConnectionAbove, needsConnectionBelow, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, false, true, true)://there is a room above,to the left and to the right
				switch ((needsConnectionAbove, needsConnectionLeft, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (false, true, true, true)://there is a room below,to the left and to the right
				switch ((needsConnectionBelow, needsConnectionLeft, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true):
						while (temp == null || temp.HasBottomCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
			case (true, true, true, true)://there is a room in every direction
				switch ((needsConnectionBelow, needsConnectionAbove, needsConnectionRight, needsConnectionLeft))
				{
					case (true, false, false, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true, false):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, false, true):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, false, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, false, true, true):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, false, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, false, true, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (false, true, true, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
					case (true, true, true, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						MakeTheRoom(roomList, dungeonRooms, copy, temp, previousRoom, checkCons, offset);
						break;
				}
				break;
		}
	}
	private void FinishRoom(List<Room> roomList, List<Room> dungeonRooms, Room thisRoom)
	{
		if (currentSizeOfDungeon >= sizeOfDungeon && cappingRooms == false) { return; }
		if (thisRoom.HasTopCon() && (thisRoom.adjacentRooms.GetConnectionAbove() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, upOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon && cappingRooms == false) { return; }
		if (thisRoom.HasBottomCon() && (thisRoom.adjacentRooms.GetConnectionBelow() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, downOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon && cappingRooms == false) { return; }
		if (thisRoom.HasLeftCon() && (thisRoom.adjacentRooms.GetConnectionLeft() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, leftOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon && cappingRooms == false) { return; }
		if (thisRoom.HasRightCon() && (thisRoom.adjacentRooms.GetConnectionRight() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, rightOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon && cappingRooms == false) { return; }
		else { thisRoom.SetAllConnected(); }
	}
	private void CheckForRooms(List<Room> dungeonRooms, Room tempRoom)
	{
		//check if a room exist in all directions
		isRoomAbove = CheckUpForRoom(dungeonRooms, tempRoom);
		isRoomBelow = CheckDownForRoom(dungeonRooms, tempRoom);
		isRoomLeft = CheckLeftForRoom(dungeonRooms, tempRoom);
		isRoomRight = CheckRightForRoom(dungeonRooms, tempRoom);
		//if a room exist check if that room needs a connection
		if (isRoomAbove) { needsConnectionAbove = CheckUpForRoomConnection(dungeonRooms, tempRoom); }
		if (isRoomBelow) { needsConnectionBelow = CheckDownForRoomConnection(dungeonRooms, tempRoom); }
		if (isRoomLeft) { needsConnectionLeft = CheckLeftForRoomConnection(dungeonRooms, tempRoom); }
		if (isRoomRight) { needsConnectionRight = CheckRightForRoomConnection(dungeonRooms, tempRoom); }
	}


	private bool CheckUpForRoom(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + upOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				room.adjacentRooms.SetRoomAbove(roomList[i]);
				roomList[i].adjacentRooms.SetRoomBelow(room);
				return true;
			}
		}
		return false;
	}
	private bool CheckDownForRoom(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + downOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				room.adjacentRooms.SetRoomBelow(roomList[i]);
				roomList[i].adjacentRooms.SetRoomAbove(room);
				return true;
			}
		}
		return false;
	}
	private bool CheckLeftForRoom(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + leftOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				room.adjacentRooms.SetRoomLeft(roomList[i]);
				roomList[i].adjacentRooms.SetRoomRight(room);
				return true;
			}
		}
		return false;
	}
	private bool CheckRightForRoom(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + rightOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				room.adjacentRooms.SetRoomRight(roomList[i]);
				roomList[i].adjacentRooms.SetRoomLeft(room);
				return true;
			}
		}
		return false;
	}
	private bool CheckUpForRoomConnection(List<Room> roomList, Room room)
	{
		if (room.adjacentRooms.GetRoomAbove().HasBottomCon())//if room needs a connection
		{
			room.adjacentRooms.GetRoomAbove().adjacentRooms.SetConnectionBelow(room.adjacentRooms.GetRoomAbove(), room);
			room.adjacentRooms.SetConnectionAbove(room, room.adjacentRooms.GetRoomAbove());
			return true;
		}
		return false;
	}
	private bool CheckDownForRoomConnection(List<Room> roomList, Room room)
	{
		if (room.adjacentRooms.GetRoomBelow().HasTopCon())//if room needs a connection
		{
			room.adjacentRooms.GetRoomBelow().adjacentRooms.SetConnectionAbove(room.adjacentRooms.GetRoomBelow(), room);
			room.adjacentRooms.SetConnectionBelow(room, room.adjacentRooms.GetRoomBelow());
			return true;
		}
		return false;
	}
	private bool CheckLeftForRoomConnection(List<Room> roomList, Room room)
	{
		if (room.adjacentRooms.GetRoomLeft().HasRightCon())//if room needs a connection
		{
			room.adjacentRooms.GetRoomLeft().adjacentRooms.SetConnectionRight(room.adjacentRooms.GetRoomLeft(), room);
			room.adjacentRooms.SetConnectionLeft(room, room.adjacentRooms.GetRoomLeft());
			return true;
		}
		return false;
	}
	private bool CheckRightForRoomConnection(List<Room> roomList, Room room)
	{
		if (room.adjacentRooms.GetRoomRight().HasLeftCon())//if room needs a connection
		{
			room.adjacentRooms.GetRoomRight().adjacentRooms.SetConnectionLeft(room.adjacentRooms.GetRoomRight(), room);
			room.adjacentRooms.SetConnectionRight(room, room.adjacentRooms.GetRoomRight());
			return true;
		}
		return false;
	}

	private void CapRoomOpenings(List<Room> roomList, List<Room> dungeonRoomList)
	{
		Room temp = FindFurthestRoom(dungeonRoomList);

		if (temp.HasTopCon() && temp.adjacentRooms.GetConnectionAbove() == null)
		{
			SpawnRoom(bossRoomsList, dungeonRoomList, temp, upOffset);
		}
		else if (temp.HasBottomCon() && temp.adjacentRooms.GetConnectionBelow() == null)
		{
			SpawnRoom(bossRoomsList, dungeonRoomList, temp, downOffset);
		}
		else if (temp.HasLeftCon() && temp.adjacentRooms.GetConnectionLeft() == null)
		{
			SpawnRoom(bossRoomsList, dungeonRoomList, temp, leftOffset);
		}
		else if (temp.HasRightCon() && temp.adjacentRooms.GetConnectionRight() == null)
		{
			SpawnRoom(bossRoomsList, dungeonRoomList, temp, rightOffset);
		}
		for (int i = 0; i < sizeOfDungeon; i++)
		{
			if (!dungeonRoomList[i].AllConnected())//if there is a room with open connections
			{
				if (dungeonRoomList[i].HasTopCon() && dungeonRoomList[i].adjacentRooms.GetConnectionAbove() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], upOffset);
				}
				if (dungeonRoomList[i].HasBottomCon() && dungeonRoomList[i].adjacentRooms.GetConnectionBelow() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], downOffset);
				}
				if (dungeonRoomList[i].HasLeftCon() && dungeonRoomList[i].adjacentRooms.GetConnectionLeft() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], leftOffset);
				}
				if (dungeonRoomList[i].HasRightCon() && dungeonRoomList[i].adjacentRooms.GetConnectionRight() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], rightOffset);
				}
			}
		}
		for (int i = 0; i < dungeonRoomList.Count; i++)
		{
			if (!dungeonRoomList[i].AllConnected())//if there is a room with open connections
			{
				if (dungeonRoomList[i].HasTopCon() && dungeonRoomList[i].adjacentRooms.GetConnectionAbove() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], upOffset);
				}
				if (dungeonRoomList[i].HasBottomCon() && dungeonRoomList[i].adjacentRooms.GetConnectionBelow() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], downOffset);
				}
				if (dungeonRoomList[i].HasLeftCon() && dungeonRoomList[i].adjacentRooms.GetConnectionLeft() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], leftOffset);
				}
				if (dungeonRoomList[i].HasRightCon() && dungeonRoomList[i].adjacentRooms.GetConnectionRight() == null)
				{
					SpawnRoom(roomList, dungeonRoomList, dungeonRoomList[i], rightOffset);
				}
			}
		}
	}
	private Room FindFurthestRoom(List<Room> dungeonRoomList)
	{
		Room temp = new Room();
		for (int i = 0; i < dungeonRoomList.Count; i++)
		{
			if (dungeonRoomList[i].ReturnDistanceFromOrigin() > temp.ReturnDistanceFromOrigin() && !dungeonRoomList[i].AllConnected())
			{
				temp = dungeonRoomList[i];
			}
		}
		return temp;
	}
}

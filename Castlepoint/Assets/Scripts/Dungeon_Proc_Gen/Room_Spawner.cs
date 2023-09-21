using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
/*
* Room offset needs to be increments of 20
*/
public class Room_Spawner : MonoBehaviour
{
	[SerializeField]
	private List<Room> originRooms;
	[SerializeField]
	private List<Room> rooms;
	private List<Room> roomsCreated;
	[SerializeField]
	private List<Room> roomCaps;
	[SerializeField]
	Room tempRoom;
	private bool bossRoomCreated;
	[SerializeField]
	private int sizeOfDungeon;
	private Vector3 upOffset = new Vector3(0, 20, 0);
	private Vector3 downOffset = new Vector3(0, -20, 0);
	private Vector3 leftOffset = new Vector3(-20, 0, 0);
	private Vector3 rightOffset = new Vector3(20, 0, 0);
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
	private void Awake()
	{
		cappingRooms = false;
		roomsCreated = new List<Room>();
		tempRoom = new Room();
		SpawnDungeon(rooms, originRooms);
	}
	private void SpawnDungeon(List<Room> roomList, List<Room> originList)
	{
		tempRoom = RandomizeRoom(originList);//Randomize which room is selected from origin list
		originRoomPos = new Vector3(-8f, 0, 0);//This is the position of the origin room
		Room copy = Instantiate(tempRoom, originRoomPos, Quaternion.identity);//create the room in game
		copy.SetRoomPosition(originRoomPos);
		roomsCreated.Add(copy);//add room to created list, this will be used to check position of created rooms
		currentSizeOfDungeon++;
		FinishRoom(roomList, roomsCreated, copy);//Begins recursion, spawns rooms to close all connections
		Debug.Log(roomsCreated.Count);
		cappingRooms = true;
		CapRoomOpenings(roomList, roomsCreated);
		Debug.Log(roomsCreated.Count);
	}

	private Room RandomizeRoom(List<Room> roomList)
	{
		Room room = new Room();
		room = roomList[UnityEngine.Random.Range(0, roomList.Count)];//Randomize which room is selected from origin list
		return room;
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
		if(cappingRooms)
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
					copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);//Set position of the room
					copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
					dungeonRooms.Add(copy);
					currentSizeOfDungeon++;
					FinishRoom(roomList, dungeonRooms, copy);
				}
				break;
			case (false, true, false, false)://there is a room below
				if (needsConnectionBelow)
				{
					while (temp == null || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
					copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);//Set position of the room
					copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
					dungeonRooms.Add(copy);
					currentSizeOfDungeon++;
					FinishRoom(roomList, dungeonRooms, copy);

				}
				break;
			case (false, false, true, false)://there is a room to the left
				if (needsConnectionLeft)
				{
					while (temp == null || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
					copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);//Set position of the room
					copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
					dungeonRooms.Add(copy);
					currentSizeOfDungeon++;
					FinishRoom(roomList, dungeonRooms, copy);
				}
				break;
			case (false, false, false, true)://there is a room to the right
				if (needsConnectionRight)
				{
					while (temp == null || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
					copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);//Set position of the room
					copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
					dungeonRooms.Add(copy);
					currentSizeOfDungeon++;
					FinishRoom(roomList, dungeonRooms, copy);
				}
				break;
			case (true, true, false, false)://there is a room above and below
				switch ((needsConnectionAbove, needsConnectionBelow))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, false, true, false):// there is a room above and to the left
				switch ((needsConnectionAbove, needsConnectionLeft))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, false, false, true):// there is a room above and to the right
				switch ((needsConnectionAbove, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasTopCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasTopCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasTopCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (false, true, true, false)://there is a room below and to the left
				switch ((needsConnectionBelow, needsConnectionLeft))
				{
					case (true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (false, true, false, true)://there is a room below and to the right
				switch ((needsConnectionBelow, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (false, false, true, true)://there is a room to the left and right
				switch ((needsConnectionLeft, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, true, true, false)://there is a room above,below and to the left
				switch ((needsConnectionAbove, needsConnectionBelow, needsConnectionLeft))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, true, false, true)://there is a room above,below and to the right
				switch ((needsConnectionAbove, needsConnectionBelow, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasBottomCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, false, true, true)://there is a room above,to the left and to the right
				switch ((needsConnectionAbove, needsConnectionLeft, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || temp.HasTopCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasTopCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || temp.HasTopCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasTopCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (false, true, true, true)://there is a room below,to the left and to the right
				switch ((needsConnectionBelow, needsConnectionLeft, needsConnectionRight))
				{
					case (true, false, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || temp.HasBottomCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon() || temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasLeftCon() || !temp.HasRightCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (true, true, true, true)://there is a room in every direction
				switch ((needsConnectionBelow, needsConnectionAbove, needsConnectionRight, needsConnectionLeft))
				{
					case (true, false, false, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true, false):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, false, true):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true, false):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, false, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true, false):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true, true):
						while (temp == null || temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true, false):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true, true):
						while (temp == null || !temp.HasBottomCon() || temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true, true):
						while (temp == null || temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true, true):
						while (temp == null || !temp.HasBottomCon() || !temp.HasTopCon() || !temp.HasRightCon() || !temp.HasLeftCon()) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.adjacentRooms.CopyRoomConnections(checkCons, copy);
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;

		}
	}
	private void FinishRoom(List<Room> roomList, List<Room> dungeonRooms, Room thisRoom)
	{
		if (currentSizeOfDungeon >= sizeOfDungeon) { return; }
		if (thisRoom.HasTopCon() && (thisRoom.adjacentRooms.GetConnectionAbove() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, upOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon) { return; }
		if (thisRoom.HasBottomCon() && (thisRoom.adjacentRooms.GetConnectionBelow() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, downOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon) { return; }
		if (thisRoom.HasLeftCon() && (thisRoom.adjacentRooms.GetConnectionLeft() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, leftOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon) { return; }
		if (thisRoom.HasRightCon() && (thisRoom.adjacentRooms.GetConnectionRight() == null)) { SpawnRoom(roomList, dungeonRooms, thisRoom, rightOffset); }
		if (currentSizeOfDungeon >= sizeOfDungeon) { return; }
		else { thisRoom.SetAllConnected(); }
	}
	private void CheckForRooms(List<Room> dungeonRooms, Room tempRoom)
	{
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

		for (int i = 0; i < sizeOfDungeon; i++)
		{
			if (!dungeonRoomList[i].AllConnected())//if there is a room with open connections
			{
				if (dungeonRoomList[i].HasTopCon() && dungeonRoomList[i].adjacentRooms.GetConnectionAbove() == null)
				{
					SpawnRoom(roomList,dungeonRoomList,dungeonRoomList[i], upOffset);
				}
				if (dungeonRoomList[i].HasBottomCon() && dungeonRoomList[i].adjacentRooms.GetConnectionBelow() == null)
				{
					SpawnRoom(roomList,dungeonRoomList,dungeonRoomList[i], downOffset);
				}
				if (dungeonRoomList[i].HasLeftCon() && dungeonRoomList[i].adjacentRooms.GetConnectionLeft() == null)
				{
					SpawnRoom(roomList,dungeonRoomList,dungeonRoomList[i], leftOffset);
				}
				if (dungeonRoomList[i].HasRightCon() && dungeonRoomList[i].adjacentRooms.GetConnectionRight() == null)
				{
					SpawnRoom(roomList,dungeonRoomList,dungeonRoomList[i], rightOffset);
				}
			}
		}
		/*for (int i = 0; i < dungeonRoomList.Count; i++)
		{	Room temp = new Room();
			Room copy = new Room();
			if (dungeonRoomList[i].HasTopCon() && dungeonRoomList[i].adjacentRooms.GetConnectionAbove() == null)
				{
					while(!temp.HasBottomCon()){temp = RandomizeRoom(roomCaps);}
					copy = Instantiate(temp, dungeonRoomList[i].GetRoomPosition() + upOffset, quaternion.identity);
				}
				if (dungeonRoomList[i].HasBottomCon() && dungeonRoomList[i].adjacentRooms.GetConnectionBelow() == null)
				{
					while(!temp.HasTopCon()){temp = RandomizeRoom(roomCaps);}
					copy = Instantiate(temp, dungeonRoomList[i].GetRoomPosition() + downOffset, quaternion.identity);
				}
				if (dungeonRoomList[i].HasLeftCon() && dungeonRoomList[i].adjacentRooms.GetConnectionLeft() == null)
				{
					while(!temp.HasRightCon()){temp = RandomizeRoom(roomCaps);}
					copy = Instantiate(temp, dungeonRoomList[i].GetRoomPosition() + leftOffset, quaternion.identity);
				}
				if (dungeonRoomList[i].HasRightCon() && dungeonRoomList[i].adjacentRooms.GetConnectionRight() == null)
				{
					while(!temp.HasLeftCon()){temp = RandomizeRoom(roomCaps);}
					copy = Instantiate(temp, dungeonRoomList[i].GetRoomPosition() + rightOffset, quaternion.identity);
				}
		}*/
	}
}

using System.Collections;
using System.Collections.Generic;
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
	private void Awake()
	{

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
		copy.ConBottom();
		copy.MakeOrigin();
		roomsCreated.Add(copy);//add room to created list, this will be used to check position of created rooms
		currentSizeOfDungeon++;
		FinishRoom(roomList, roomsCreated, copy);//Recurse spawning rooms until all connections are satisfied
		//CapRoomOpenings(roomsCreated);
	}

	private Room RandomizeRoom(List<Room> roomList)
	{
		Room room = new Room();
		room = roomList[Random.Range(0, roomList.Count)];//Randomize which room is selected from origin list
		return room;
	}

	private void SpawnRoom(List<Room> roomList, List<Room> dungeonRooms, Room previousRoom, Vector3 offset)
	{
		if (currentSizeOfDungeon >= sizeOfDungeon)
		{
			return;
		}
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
		temp.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
		CheckForRooms(dungeonRooms, temp);
		switch ((isRoomAbove, isRoomBelow, isRoomLeft, isRoomRight))//go through each case of a room existing around the room we want to make
		{
			case (true, false, false, false)://there is a room above
				if (needsConnectionAbove)//if the room above needs a connection made below it
				{
					while (temp == null || !temp.HasTopCon()) { temp = RandomizeRoom(roomList); }
					copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
					copy.ConTop();
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
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
					copy.ConBottom();
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
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
					copy.ConLeft();
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
					copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
					copy.ConRight();
					dungeonRooms.Add(copy);
					currentSizeOfDungeon++;
					FinishRoom(roomList, dungeonRooms, copy);
				}
				break;
			case (true, true, false, false)://there is a room above and below
				switch ((needsConnectionAbove, needsConnectionBelow))
				{
					case (true, false):
						while (temp == null || (!temp.HasTopCon() && temp.HasBottomCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasBottomCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || (!temp.HasTopCon() && !temp.HasBottomCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
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
						while (temp == null || (!temp.HasTopCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || (!temp.HasTopCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
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
						while (temp == null || (!temp.HasTopCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || ((!temp.HasTopCon() && !temp.HasRightCon()))) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConRight();
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
						while (temp == null || (!temp.HasBottomCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || (!temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
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
						while (temp == null || (!temp.HasBottomCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || (!temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConRight();
						dungeonRooms.Add(copy); FinishRoom(roomList, dungeonRooms, copy);
						break;
				}
				break;
			case (false, false, true, true)://there is a room to the left and right
				switch ((needsConnectionLeft, needsConnectionRight))
				{
					case (true, false):
						while (temp == null || (!temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true):
						while (temp == null || (temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true)://both rooms above and below need a connection
						while (temp == null || (!temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						copy.ConRight();
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
						while (temp == null || (!temp.HasTopCon() && temp.HasBottomCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || (temp.HasTopCon() && !temp.HasBottomCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || (temp.HasTopCon() && temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || (!temp.HasTopCon() && !temp.HasBottomCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || (!temp.HasTopCon() && temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || (!temp.HasTopCon() && !temp.HasBottomCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						copy.ConLeft();
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
						while (temp == null || (!temp.HasTopCon() && temp.HasBottomCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || (temp.HasTopCon() && !temp.HasBottomCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || (temp.HasTopCon() && temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || (!temp.HasTopCon() && !temp.HasBottomCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || (!temp.HasTopCon() && temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || (!temp.HasTopCon() && !temp.HasBottomCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						copy.ConRight();
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
						while (temp == null || (!temp.HasTopCon() && temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || (temp.HasTopCon() && !temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || (temp.HasTopCon() && temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || (!temp.HasTopCon() && !temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || (!temp.HasTopCon() && temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || (temp.HasTopCon() && !temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || (!temp.HasTopCon() && !temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
						copy.ConRight();
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
						while (temp == null || (!temp.HasBottomCon() && temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false):
						while (temp == null || (temp.HasBottomCon() && !temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true):
						while (temp == null || (temp.HasBottomCon() && temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasLeftCon() && temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true):
						while (temp == null || (!temp.HasBottomCon() && temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true):
						while (temp == null || (temp.HasBottomCon() && !temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasLeftCon() && !temp.HasRightCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
						copy.ConRight();
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
						while (temp == null || (!temp.HasBottomCon() && temp.HasTopCon() && temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false, false):
						while (temp == null || (temp.HasBottomCon() && !temp.HasTopCon() && temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true, false):
						while (temp == null || (temp.HasBottomCon() && temp.HasTopCon() && !temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, false, true):
						while (temp == null || (temp.HasBottomCon() && temp.HasTopCon() && temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false, false):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasTopCon() && temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true, false):
						while (temp == null || (!temp.HasBottomCon() && temp.HasTopCon() && !temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, false, true):
						while (temp == null || (!temp.HasBottomCon() && temp.HasTopCon() && temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true, false):
						while (temp == null || (temp.HasBottomCon() && !temp.HasTopCon() && !temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, false, true):
						while (temp == null || (temp.HasBottomCon() && !temp.HasTopCon() && temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, false, true, true):
						while (temp == null || (temp.HasBottomCon() && temp.HasTopCon() && !temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConLeft();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true, false):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasTopCon() && !temp.HasRightCon() && temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, false, true):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasTopCon() && temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						copy.ConLeft();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, false, true, true):
						while (temp == null || (!temp.HasBottomCon() && temp.HasTopCon() && !temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConBottom();
						copy.ConLeft();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (false, true, true, true):
						while (temp == null || (temp.HasBottomCon() && !temp.HasTopCon() && !temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConLeft();
						copy.ConRight();
						dungeonRooms.Add(copy);
						currentSizeOfDungeon++;
						FinishRoom(roomList, dungeonRooms, copy);
						break;
					case (true, true, true, true):
						while (temp == null || (!temp.HasBottomCon() && !temp.HasTopCon() && !temp.HasRightCon() && !temp.HasLeftCon())) { temp = RandomizeRoom(roomList); }
						copy = Instantiate(temp, previousRoom.GetRoomPosition() + offset, Quaternion.identity);//create the room in game below previous room
						copy.SetRoomPosition(previousRoom.GetRoomPosition() + offset);
						copy.ConTop();
						copy.ConBottom();
						copy.ConLeft();
						copy.ConRight();
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
		if (currentSizeOfDungeon >= sizeOfDungeon)
		{
			return;
		}
		if (thisRoom.HasTopCon() && !thisRoom.TopIsCon()) { SpawnRoom(roomList, dungeonRooms, thisRoom, upOffset); }
		if (thisRoom.HasBottomCon() && !thisRoom.BottomIsCon()) { SpawnRoom(roomList, dungeonRooms, thisRoom, downOffset); }
		if (thisRoom.HasLeftCon() && !thisRoom.LeftIsCon()) { SpawnRoom(roomList, dungeonRooms, thisRoom, leftOffset); }
		if (thisRoom.HasRightCon() && !thisRoom.RightIsCon()) { SpawnRoom(roomList, dungeonRooms, thisRoom, rightOffset); }
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

	private bool CheckUpForRoomConnection(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + upOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				if (roomList[i].HasBottomCon() & !roomList[i].BottomIsCon())//if room needs a connection
				{
					roomList[i].ConBottom();//we connect the room here because we will make the room when funtion returns
					return true;
				}
			}
		}
		return false;
	}
	private bool CheckDownForRoomConnection(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + downOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				if (roomList[i].HasTopCon() & !roomList[i].TopIsCon())//if room needs a connection
				{
					roomList[i].ConTop();
					return true;
				}
			}
		}
		return false;
	}
	private bool CheckLeftForRoomConnection(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + leftOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				if (roomList[i].HasRightCon() & !roomList[i].RightIsCon())//if room needs a connection
				{
					roomList[i].ConRight();
					return true;
				}
			}
		}
		return false;
	}
	private bool CheckRightForRoomConnection(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + rightOffset == roomList[i].GetRoomPosition())//if there is a room
			{
				if (roomList[i].HasLeftCon() & !roomList[i].LeftIsCon())//if room needs a connection
				{
					roomList[i].ConLeft();
					return true;
				}
			}
		}
		return false;
	}
	private bool CheckUpForRoom(List<Room> roomList, Room room)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			if (room.GetRoomPosition() + upOffset == roomList[i].GetRoomPosition())//if there is a room
			{
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
				return true;
			}
		}
		return false;
	}
	private void CapRoomOpenings(List<Room> dungeonRoomList)
	{
		for (int i = 0; i < dungeonRoomList.Count; i++)
		{
			if(!dungeonRoomList[i].AllConnected())//if there is a room with open connections
			{
				switch(dungeonRoomList[i].HasTopCon(),dungeonRoomList[i].HasBottomCon(),dungeonRoomList[i].HasLeftCon(),dungeonRoomList[i].HasRightCon())
				{
					case(true, false, false,false)://has top connection
						
					break;
				}
			}
		}
	}
}

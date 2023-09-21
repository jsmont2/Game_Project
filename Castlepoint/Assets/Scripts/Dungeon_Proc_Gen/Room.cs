using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Room : MonoBehaviour
{
	/// <summary>
	/// Stores rooms connected to the room
	/// </summary>
	public class AdjacentRooms
	{
		public class RoomConnection
		{
			public RoomConnection() { }
			public RoomConnection(Room origin, Room destination)//constructor
			{
				this.origin = origin;//sets origin = to variable passed in as origin
				this.destination = destination;//sets destination = to variable passed in as destination
			}
			Room origin;//Room that is at the center
			Room destination;//Room that is adjacent to the origin room
		}
		/// <summary>
		/// Connection to room above origin
		/// </summary>
		private RoomConnection connectionAbove;
		/// <summary>
		/// Connection to room below origin
		/// </summary>
		private RoomConnection connectionBelow;
		/// <summary>
		/// Connection to room left of origin
		/// </summary>
		private RoomConnection connectionLeft;
		/// <summary>
		/// Connection to room right of origin
		/// </summary>
		private RoomConnection connectionRight;
		/// <summary>
		/// Room located above the current room
		/// </summary>
		private Room roomAbove;
		/// <summary>
		/// Room located below the current room
		/// </summary>
		private Room roomBelow;
		/// <summary>
		/// Room located left of the current room
		/// </summary>
		private Room roomLeft;
		/// <summary>
		/// Room located right of the current room
		/// </summary>
		private Room roomRight;
		/// <summary>
		/// Stores the room as above the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomAbove(Room room) { roomAbove = room; }
		/// <summary>
		/// Stores the room as below the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomBelow(Room room) { roomBelow = room; }
		/// <summary>
		/// Stores the room as left of the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomLeft(Room room) { roomLeft = room; }
		/// <summary>
		/// Stores the room as right of the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomRight(Room room) { roomRight = room; }
		public Room GetRoomAbove() { return roomAbove; }
		public Room GetRoomBelow() { return roomBelow; }
		public Room GetRoomLeft() { return roomLeft; }
		public Room GetRoomRight() { return roomRight; }
		public void SetConnectionAbove(Room origin, Room destination)
		{ connectionAbove = new RoomConnection(origin, destination); }
		public void SetConnectionBelow(Room origin, Room destination)
		{ connectionBelow = new RoomConnection(origin, destination); }
		public void SetConnectionLeft(Room origin, Room destination)
		{ connectionLeft = new RoomConnection(origin, destination); }
		public void SetConnectionRight(Room origin, Room destination)
		{ connectionRight = new RoomConnection(origin, destination); }
		public RoomConnection GetConnectionAbove() { return connectionAbove; }
		public RoomConnection GetConnectionBelow() { return connectionBelow; }
		public RoomConnection GetConnectionLeft() { return connectionLeft; }
		public RoomConnection GetConnectionRight() { return connectionRight; }
		public void CopyRoomConnections(Room copyRoom, Room thisRoom)
		{
			if (copyRoom.adjacentRooms.GetRoomAbove() != null)
			{
				roomAbove = copyRoom.adjacentRooms.GetRoomAbove();//Reference room above into placeholder
				roomAbove.adjacentRooms.SetRoomBelow(thisRoom);
				if (copyRoom.adjacentRooms.GetConnectionAbove() != null)
				{
					thisRoom.adjacentRooms.SetConnectionAbove(thisRoom, roomAbove);
					roomAbove.adjacentRooms.SetConnectionBelow(roomAbove, thisRoom);//adjust room connection for room above
				}
			}
			if (copyRoom.adjacentRooms.GetRoomBelow() != null)
			{
				roomBelow = copyRoom.adjacentRooms.GetRoomBelow();//Reference room above into placeholder
				roomBelow.adjacentRooms.SetRoomAbove(thisRoom);
				if (copyRoom.adjacentRooms.GetConnectionBelow() != null)
				{
					thisRoom.adjacentRooms.SetConnectionBelow(thisRoom, roomBelow);
					roomBelow.adjacentRooms.SetConnectionAbove(roomBelow, thisRoom);//adjust room connection for room below
				}
			}
			if (copyRoom.adjacentRooms.GetRoomLeft() != null)
			{
				roomLeft = copyRoom.adjacentRooms.GetRoomLeft();//Reference room above into placeholder
				roomLeft.adjacentRooms.SetRoomRight(thisRoom);
				if (copyRoom.adjacentRooms.GetConnectionLeft() != null)
				{
					thisRoom.adjacentRooms.SetConnectionLeft(thisRoom, roomLeft);
					roomLeft.adjacentRooms.SetConnectionRight(roomLeft, thisRoom);//adjust room connection for room left
				}
			}
			if (copyRoom.adjacentRooms.GetRoomRight() != null)
			{
				roomRight = copyRoom.adjacentRooms.GetRoomRight();//Reference room above into placeholder
				roomRight.adjacentRooms.SetRoomLeft(thisRoom);
				if (copyRoom.adjacentRooms.GetConnectionRight() != null)
				{
					thisRoom.adjacentRooms.SetConnectionRight(thisRoom, roomRight);
					roomRight.adjacentRooms.SetConnectionLeft(roomRight, thisRoom);//adjust room connection for room right
				}
			}
		}
	}

	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the top
	/// </summary>
	[SerializeField]
	private bool hasTopConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the bottom
	/// </summary>
	[SerializeField]
	private bool hasBottomConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the left
	/// </summary>
	[SerializeField]
	private bool hasLeftConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the right
	/// </summary>
	[SerializeField]
	private bool hasRightConnection;
	/// <summary>
	/// BOOLEAN: Determines if all the connections in the room have been satisfied
	/// </summary>
	private bool allConnected;
	/// <summary>
	/// BOOLEAN: Determines if the room has stairs leading upwards
	/// </summary>
	private bool hasStairsUp;
	/// <summary>
	/// BOOLEAN: Determines if the room has stairs leading downwards
	/// </summary>
	private bool hasStairsDown;
	/// <summary>
	/// INT: Determines what floor the room is on;
	/// </summary>
	private int floorLevel;
	private Vector3 roomPosition;
	public AdjacentRooms adjacentRooms = new AdjacentRooms();






	/// <summary>
	/// Returns true if the room has a top connection
	/// </summary>
	/// <returns>hasTopConnection</returns>
	public bool HasTopCon() { return hasTopConnection; }
	/// <summary>
	/// Returns true if the room has a bottom connection
	/// </summary>
	/// <returns>hasBottomConnection</returns>
	public bool HasBottomCon() { return hasBottomConnection; }
	/// <summary>
	/// Returns true if the room has a left connection
	/// </summary>
	/// <returns>hasLeftConnection</returns>
	public bool HasLeftCon() { return hasLeftConnection; }
	/// <summary>
	/// Returns true if the room has a right connection
	/// </summary>
	/// <returns>hasRightConnection</returns>
	public bool HasRightCon() { return hasRightConnection; }
	/// <summary>
	/// Returns true if all the connections in the room are satisfied
	/// </summary>
	/// <returns>allConnected</returns>
	public bool AllConnected() { return allConnected; }
	public void SetAllConnected() { allConnected = true; }
	/// <summary>
	/// Returns true if the room has stairs leading upwards
	/// </summary>
	/// <returns>hasStairsUp</returns>
	public bool HasStairsup() { return hasStairsUp; }
	/// <summary>
	/// Returns true if the room has stairs leading downwards
	/// </summary>
	/// <returns>hasStairsDown</returns>
	public bool HasStairsdown() { return hasStairsDown; }
	/// <summary>
	/// Returns the floor level the room is on
	/// </summary>
	/// <returns>floorLevel</returns>
	public int GetFloorLevel() { return floorLevel; }
	public void SetRoomPosition(Vector3 position) { roomPosition = position; }
	/// <summary>
	/// Returns the position of the room
	/// </summary>
	/// <returns>roomPosition</returns>
	public Vector3 GetRoomPosition() { return roomPosition; }


}
